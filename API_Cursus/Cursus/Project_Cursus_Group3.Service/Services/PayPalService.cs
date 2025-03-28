using Azure;
using Google.Apis.Storage.v1.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using PayPal.Api;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalCheckoutSdk.Payments;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model.VNPAY;
using Project_Cursus_Group3.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Requests.BatchRequest;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Order = Project_Cursus_Group3.Data.Entities.Order;

namespace Project_Cursus_Group3.Service.Services
{
    public class PayPalService : IPayPalService
    {
        private readonly PayPalHttpClient _payPalHttpClient;
        private readonly ILogger<PayPalService> _logger;
        private readonly IConfiguration _IConfiguration;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IEmailSender _emailSender;
        private readonly CursusDbContext _cursusDbContext;
        private readonly string _baseUrl;
        private readonly string _returnUrl;
        private readonly string _cancelUrl;

        public PayPalService(ILogger<PayPalService> logger,
        IConfiguration iConfiguration, IOrderDetailRepository orderDetailRepository,
        IOrderRepository orderRepository, IPaymentRepository paymentRepository,
        ITransactionRepository transactionRepository, IEmailSender emailSender,
        CursusDbContext cursusDbContext)
        {
            _logger = logger;
            var clientId = iConfiguration["PayPal:ClientId"];
            var clientSecret = iConfiguration["PayPal:ClientSecret"];
            var isSandbox = iConfiguration["Paypal:Mode"].Equals("Sandbox", StringComparison.OrdinalIgnoreCase);
            _IConfiguration = iConfiguration;
            _baseUrl = _IConfiguration["PayPal:BaseUrl"];
            _returnUrl = _IConfiguration["PayPal:ReturnUrl"];
            _cancelUrl = _IConfiguration["PayPal:CancelUrl"];
            _orderDetailRepository = orderDetailRepository;
            _orderRepository = orderRepository;
            _paymentRepository = paymentRepository;
            _transactionRepository = transactionRepository;
            _emailSender = emailSender;
            _cursusDbContext = cursusDbContext;
            PayPalEnvironment environment = isSandbox
                ? new SandboxEnvironment(clientId, clientSecret)
                : new LiveEnvironment(clientId, clientSecret);

            _payPalHttpClient = new PayPalHttpClient(environment);
            //xác định xem môi trường live or sanbox.dùng sandbox..
        }
        public async Task<double> GetUSDtoVND()
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync($"{_IConfiguration["ExchangeRates:ApiUrl"]}?access_key={_IConfiguration["ExchangeRates:ApiKey"]}&symbols=USD,VND");
                JObject jsonData = JObject.Parse(response);
                decimal rate = (decimal)jsonData["rates"]["VND"];
                Console.WriteLine($"Current USD to VND exchange rate: {rate}");
                return (double)rate;
            }
        }
        public async Task<(string Status, string OrderId)> CapturePaymentAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException("token");
            }
            try
            {
                //check đơn
                var orderStatus = await CheckOrderStatusAsync(token);
                if (orderStatus == "COMPLETED")
                {
                    _logger.LogInformation("Order already captured for token: {token}", token);
                    return ("COMPLETED", token);
                }
                // Thực hiện capture nếu đơn hàng chưa hoàn tất
                var request = new OrdersCaptureRequest(token);
                request.RequestBody(new OrderActionRequest());
                var response = await _payPalHttpClient.Execute(request);
                var result = response.Result<PayPalCheckoutSdk.Orders.Order>();

                return (result.Status, result.Id);
            }
            catch (PayPalHttp.HttpException ex)
            {
                _logger.LogError(ex, "Error capturing PayPal payment for token: {token}", token);
                throw;
            }
        }

        public async Task<string> CreatePaymentAsync(PaymentInformationModel model, HttpContext context)
        {
            var username = GetUsernameFromToken(context);
            if (string.IsNullOrEmpty(username))
            {
                throw new Exception("UserName is null or empty, cannot process payment");
            }

            double amount = await CalculateAmountAsync(model, username);
            if (username == null)
            {
                throw new ArgumentNullException("Unauthorize");
            }
            if (model.OrderType == "wallet")
            {
                if (amount <= 0)
                {
                    throw new Exception("Invalid amount for wallet recharge");
                }
                var orderPayPal = new OrderRequest
                {
                    CheckoutPaymentIntent = "CAPTURE",
                    PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest
                    {
                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            CurrencyCode = "USD",
                            Value = amount.ToString("0.00")
                        },
                        ReferenceId = username
                    }
                },
                    ApplicationContext = new ApplicationContext
                    {
                        ReturnUrl = $"{_baseUrl.TrimEnd('/')}{_returnUrl}?username={username}&orderType={model.OrderType}&Amount={amount}",
                        CancelUrl = $"{_baseUrl.TrimEnd('/')}{_cancelUrl}?username={username}&orderType={model.OrderType}&Amount={amount}",
                        UserAction = "PAY_NOW",
                        ShippingPreference = "NO_SHIPPING"
                    }
                };

                var request = new OrdersCreateRequest();
                request.Prefer("return=representation");
                request.RequestBody(orderPayPal);

                var respond = await _payPalHttpClient.Execute(request);
                var result = respond.Result<PayPalCheckoutSdk.Orders.Order>();

                var approveUrl = result.Links?.FirstOrDefault(x => x.Rel.Equals("approve", StringComparison.OrdinalIgnoreCase))?.Href;
                if (string.IsNullOrEmpty(approveUrl))
                {
                    throw new Exception("PayPal approve URL not found in response.");
                }

                return approveUrl;


            }

            else if (model.OrderType == "paynow")

            {
                var order = await _cursusDbContext.Order.SingleOrDefaultAsync(o => o.OrderId == model.OrderId);
                if(order.Status == "Success" || order.Status == "Refunded")
                {
                    throw new Exception("Order has been processced.Unable to pay !! ");
                }
                var wallet = await _cursusDbContext.Wallet.SingleOrDefaultAsync(w => w.UserName == username);
                if (order == null)
                {
                    throw new Exception("Order not found");
                }

                if (wallet == null)
                {
                    throw new Exception("Wallet not found for user: " + username);
                }
                double balanceOfUser = (double)wallet.Balance;
                double orderPrice = (double)order.OrderPrice;
                if (order.OrderPrice <= balanceOfUser)
                {
                    wallet.Balance = balanceOfUser - orderPrice;
                    order.Status = "Success";
                    var transaction = new Transactions
                    {
                        walletId = wallet.WalletId,
                        PaymentCode = order.OrderId.ToString(),
                        CreatedDate = DateTime.UtcNow,
                        Amount = -orderPrice,
                        Description = "The order has been paid successfully with your wallet",
                        Status = "Complete"
                    };

                   await _transactionRepository.AddPaymentAsync(transaction);
                    return "ORDER_PROCESSED";
                }
                else
                {
                    amount = orderPrice - balanceOfUser;
                    wallet.Balance = 0;

                    var transaction = new Transactions
                    {
                        walletId = wallet.WalletId,
                        PaymentCode = order.OrderId.ToString(),
                        CreatedDate = DateTime.UtcNow,
                        Amount = -balanceOfUser,
                        Description = "Successfully paid part of the order price using wallet",
                        Status = "Complete"
                    };
                     await _transactionRepository.AddPaymentAsync(transaction);

                    if (string.IsNullOrEmpty(_baseUrl) || string.IsNullOrEmpty(_returnUrl) || string.IsNullOrEmpty(_cancelUrl))
                    {
                        throw new InvalidOperationException("Base URL or return/cancel URL configuration is missing.");
                    }
                    var fullReturnUrl = $"{_baseUrl.TrimEnd('/')}{_returnUrl}?orderId={order.OrderId}&username={username}&orderType={model.OrderType}&Amount={amount}";
                    var fullCancelUrl = $"{_baseUrl.TrimEnd('/')}{_cancelUrl}?orderId={order.OrderId}&username={username}&orderType={model.OrderType}&Amount={amount}";

                    var orderPayPal = new OrderRequest
                    {
                        CheckoutPaymentIntent = "CAPTURE",
                        PurchaseUnits = new List<PurchaseUnitRequest>
                    {
                        new PurchaseUnitRequest {
                            AmountWithBreakdown = new AmountWithBreakdown {
                                CurrencyCode = "USD",
                                Value = amount.ToString("0.00", CultureInfo.InvariantCulture)

                            },
                            ReferenceId = order.OrderId.ToString(),
                        }
                    },
                        ApplicationContext = new ApplicationContext
                        {
                            ReturnUrl = fullReturnUrl,
                            CancelUrl = fullCancelUrl,
                            UserAction = "PAY_NOW",
                            ShippingPreference = "NO_SHIPPING"
                        }
                    };
                    var request = new OrdersCreateRequest();
                    request.Prefer("return=representation");
                    request.RequestBody(orderPayPal);

                    var respond = await _payPalHttpClient.Execute(request);
                    var result = respond.Result<PayPalCheckoutSdk.Orders.Order>();

                    var approveUrl = result.Links?.FirstOrDefault(x => x.Rel.Equals("approve", StringComparison.OrdinalIgnoreCase))?.Href;
                    if (string.IsNullOrEmpty(approveUrl))
                    {
                        throw new System.Exception("PayPal approve URL not found in response.");
                    }
                    await _cursusDbContext.SaveChangesAsync();
                    return approveUrl;
                }
            }
            else
            {
                return "Invalid OrderType";
            }
        }

        private async Task<string> CheckOrderStatusAsync(string token)
        {
            var request = new OrdersGetRequest(token);
            var respond = await _payPalHttpClient.Execute(request);
            var result = respond.Result<PayPalCheckoutSdk.Orders.Order>();
            return result.Status;
        }
        private string GetUsernameFromToken(HttpContext context)
        {
            try
            {
                string token = context.Request.Cookies["authToken"];

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("No auth token found in cookies");
                    return null;
                }
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_IConfiguration["Jwt:Key"]);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _IConfiguration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _IConfiguration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                };

                ClaimsPrincipal principal;
                try
                {
                    principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                }
                catch (SecurityTokenException ex)
                {
                    _logger.LogError(ex, "Token validation failed");
                    return null;
                }
                var username = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(username))
                {
                    _logger.LogWarning("Username claim not found in token");
                    return null;
                }

                return username;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting username from token");
                return null;
            }
        }
        private async Task<double> CalculateAmountAsync(PaymentInformationModel model, string username)
        {
            if (model.OrderType == "wallet")
            {
                return model.Amount;
            }
            else if (model.OrderType == "paynow")
            {
                var order = _cursusDbContext.Order.SingleOrDefault(o => o.OrderId == model.OrderId);
                var wallet = _cursusDbContext.Wallet.SingleOrDefault(w => w.UserName == username);

                if (order == null)
                {
                    throw new Exception("Order not found");
                }

                if (wallet == null)
                {
                    throw new Exception("Wallet not found for user: " + username);
                }
                double rate = await GetUSDtoVND();
                double balance = (double)wallet.Balance / rate;


                return (double)order.OrderPrice - balance;
            }

            throw new Exception("Invalid OrderType");
        }

        public PaypalReturnModel ProcessPaymentCompleteAsync(PaymentInformationPaypal response, string username, string transactionId)
        {
            if (response.OrderType == "paynow")
            {
                return ProcessOrderPayment(response, username, transactionId).Result;
            }
            return ProcessWalletPayment(response, username, transactionId);
        }

        private PaypalReturnModel ProcessWalletPayment(PaymentInformationPaypal response, string username, string transactionId)
        {
            var wallet = _cursusDbContext.Wallet.SingleOrDefault(w => w.UserName == username);
            double fixAmount = (double)response.Amount;
            wallet.Balance += fixAmount;
            wallet.TransactionTime = DateTime.Now;

            //tạo transaction
            var transaction = new Transactions
            {
                walletId = wallet.WalletId,
                PaymentCode = transactionId,
                CreatedDate = DateTime.UtcNow,
                Amount = (double?)response.Amount,
                Description = "Wallet completed successfully",
                Status = "Complete"
            };

            _transactionRepository.AddPaymentAsync(transaction);

            return new PaypalReturnModel
            {
                description = "Wallet to up successfully",
                Amount = fixAmount,
                paymentMethod = "Paypal",
                OrderType = response.OrderType,
                username = username,
                ResponseDate = DateTime.Now,
                PaymentId = transactionId
            };
        }

        private async Task<PaypalReturnModel> ProcessOrderPayment(PaymentInformationPaypal response, string username, string transactionId)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException($"'{nameof(username)}' cannot be null or empty.", nameof(username));
            }

            var orderId = response.OrderId;
            var order = await _cursusDbContext.Order.FirstOrDefaultAsync(o => o.OrderId == orderId);
            double fixAmount = (double)response.Amount;
            if (order == null)
            {
                return new PaypalReturnModel
                {
                    description = "Pay for order cancel",
                    Amount = fixAmount,
                    paymentMethod = "Paypal",
                    OrderType = response.OrderType,
                    username = username,
                    ResponseDate = DateTime.Now
                };
            }

            order.Status = "Success";
            order.OrderCode = transactionId;

            var payment = new Data.Entities.Payment
            {
                OrderId = orderId,
                PaymentCode = transactionId,
                CreatedDate = DateTime.UtcNow,
                Amount = fixAmount,
                Description = "Payment completed successfully",
                Status = "Complete"
            };

            await _paymentRepository.AddPaymentAsync(payment);

            var rateString = _IConfiguration["RatePrice:rate"];
            if (!double.TryParse(rateString, NumberStyles.Any, CultureInfo.InvariantCulture, out double rate))
            {
                rate = 1.0;
            }

            var orderDetails = await _cursusDbContext.OrderDetail.Where(od => od.OrderId == orderId).ToListAsync();
            foreach (var detail in orderDetails)
            {
                var course = await _cursusDbContext.Course.FindAsync(detail.CourseId);
                if (course != null)
                {
                    course.TotalEnrollment += 1;

                    var instructorWallet = await _cursusDbContext.Wallet.FirstOrDefaultAsync(w => w.UserName == course.Username);
                    var walletOfAdmin = await _cursusDbContext.Wallet.FirstOrDefaultAsync(w => w.User.RoleId == 3);

                    if (instructorWallet != null && walletOfAdmin != null)
                    {
                        double amountofCourseIns = detail.Price * (1 - rate);
                        double amountOfCourseAd = detail.Price * rate;

                        instructorWallet.Balance += amountofCourseIns;
                        instructorWallet.TransactionTime = DateTime.Now;

                        walletOfAdmin.Balance += amountOfCourseAd;
                        walletOfAdmin.TransactionTime = DateTime.Now;

                        var instructorTransaction = new Transactions
                        {
                            walletId = instructorWallet.WalletId,
                            PaymentCode = transactionId,
                            CreatedDate = DateTime.UtcNow,
                            Amount = amountofCourseIns,
                            Description = $"Earnings for course {course.CourseCode} from order {order.OrderCode}",
                            Status = "Complete"
                        };
                        await _transactionRepository.AddPaymentAsync(instructorTransaction);

                        var adminTransaction = new Transactions
                        {
                            walletId = walletOfAdmin.WalletId,
                            PaymentCode = transactionId,
                            CreatedDate = DateTime.UtcNow,
                            Amount = amountOfCourseAd,
                            Description = $"Admin earnings for course {course.CourseCode} from order {order.OrderCode}",
                            Status = "Complete"
                        };
                        await _transactionRepository.AddPaymentAsync(adminTransaction);
                    }
                }
            }

            await _cursusDbContext.SaveChangesAsync();

            return new PaypalReturnModel
            {
                description = "Pay for order successfully",
                Amount = fixAmount,
                paymentMethod = "Paypal",
                OrderType = response.OrderType,
                username = username,
                ResponseDate = DateTime.Now
            };
        }

        private async Task SendOrderConfirmationEmail(string email, Data.Entities.Order order, List<OrderDetail> orderDetails)
        {
            string subject = "Order Information";
            string message = GenerateOrderConfirmationEmailBody(order, orderDetails);

            await _emailSender.EmailSendAsync(email, subject, message);
        }
        private string GenerateOrderConfirmationEmailBody(Data.Entities.Order order, List<OrderDetail> orderDetails)
        {
            StringBuilder body = new StringBuilder();
            body.AppendLine("<!DOCTYPE html>");
            body.AppendLine("<html lang=\"en\">");
            body.AppendLine("<head>");
            body.AppendLine("    <meta charset=\"UTF-8\">");
            body.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            body.AppendLine("    <title>Electronic Invoice</title>");
            body.AppendLine("    <style>");
            body.AppendLine("        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; max-width: 1200px; margin: 0 auto; padding: 20px; background-color: #f0f0f0; }");
            body.AppendLine("        .invoice-container { background-color: #ffffff; border-radius: 8px; box-shadow: 0 0 20px rgba(0, 0, 0, 0.1); padding: 30px; margin-bottom: 30px; }");
            body.AppendLine("        .header { text-align: center; margin-bottom: 30px; }");
            body.AppendLine("        .logo { max-width: 150px; margin-bottom: 20px; }");
            body.AppendLine("        h1 { color: #2c3e50; margin: 0; font-size: 28px; }");
            body.AppendLine("        .invoice-details { display: flex; justify-content: space-between; margin-bottom: 30px; }");
            body.AppendLine("        .invoice-details > div { flex-basis: 48%; }");
            body.AppendLine("        .customer-details, .order-details { background-color: #f9f9f9; border-radius: 5px; padding: 15px; }");
            body.AppendLine("        h2 { color: #2980b9; border-bottom: 2px solid #3498db; padding-bottom: 10px; font-size: 20px; }");
            body.AppendLine("        table { width: 100%; border-collapse: collapse; margin-top: 20px; }");
            body.AppendLine("        th, td { padding: 12px; text-align: left; border-bottom: 1px solid #e0e0e0; }");
            body.AppendLine("        thead { background-color: #3498db; color: white; }");
            body.AppendLine("        .total-row { font-weight: bold; background-color: #ecf0f1; }");
            body.AppendLine("        .thank-you { text-align: center; margin-top: 40px; font-size: 18px; color: #27ae60; }");
            body.AppendLine("        .footer { text-align: center; margin-top: 30px; font-size: 14px; color: #7f8c8d; }");
            body.AppendLine("    </style>");
            body.AppendLine("</head>");
            body.AppendLine("<body>");
            body.AppendLine("    <div class=\"invoice-container\">");
            body.AppendLine("        <div class=\"header\">");
            body.AppendLine("            <img src=\"/api/placeholder/150/50\" alt=\"Company Logo\" class=\"logo\">");
            body.AppendLine("            <h1>Electronic Invoice</h1>");
            body.AppendLine("        </div>");
            body.AppendLine("        <div class=\"invoice-details\">");
            body.AppendLine("            <div class=\"customer-details\">");
            body.AppendLine("                <h2>Customer Information</h2>");
            body.AppendLine($"                <p><strong>Customer Name:</strong> {order.User.UserName}</p>");
            body.AppendLine($"                <p><strong>Address:</strong> {order.User.Address}</p>");
            body.AppendLine($"                <p><strong>Email:</strong> {order.User.Email}</p>");
            body.AppendLine($"                <p><strong>Phone:</strong> {order.User.PhoneNumber}</p>");
            body.AppendLine("            </div>");
            body.AppendLine("            <div class=\"order-details\">");
            body.AppendLine("                <h2>Order Details</h2>");
            body.AppendLine($"                <p><strong>Order Code:</strong> {order.OrderCode}</p>");
            body.AppendLine($"                <p><strong>Order Date:</strong> {order.OrderDate}</p>");
            body.AppendLine("            </div>");
            body.AppendLine("        </div>");
            body.AppendLine("        <h2>Ordered Courses</h2>");
            body.AppendLine("        <table>");
            body.AppendLine("            <thead>");
            body.AppendLine("                <tr>");
            body.AppendLine("                    <th>Course Code</th>");
            body.AppendLine("                    <th>Course Title</th>");
            body.AppendLine("                    <th>Discount</th>");
            body.AppendLine("                    <th>Price (USD)</th>");
            body.AppendLine("                </tr>");
            body.AppendLine("            </thead>");
            body.AppendLine("            <tbody>");

            foreach (var detail in orderDetails)
            {
                var course = _cursusDbContext.Course.Find(detail.CourseId);
                body.AppendLine("                <tr>");
                body.AppendLine($"                    <td>{course.CourseCode}</td>");
                body.AppendLine($"                    <td>{course.CourseTitle}</td>");
                body.AppendLine($"                    <td>{course.Discount}%</td>");
                body.AppendLine($"                    <td>${detail.Price}</td>");
                body.AppendLine("                </tr>");
            }

            body.AppendLine("                <tr class=\"total-row\">");
            body.AppendLine("                    <td colspan=\"3\">Total</td>");
            body.AppendLine($"                    <td>${order.OrderPrice}</td>");
            body.AppendLine("                </tr>");
            body.AppendLine("            </tbody>");
            body.AppendLine("        </table>");
            body.AppendLine("        <div class=\"thank-you\">");
            body.AppendLine("            <p>Thank you for purchasing our courses!</p>");
            body.AppendLine("        </div>");
            body.AppendLine("    </div>");
            body.AppendLine("    <div class=\"footer\">");
            body.AppendLine("        <p>If you have any questions, please contact our support team.</p>");
            body.AppendLine("        <p>Email: support@example.com | Phone: (123) 456-7890</p>");
            body.AppendLine("    </div>");
            body.AppendLine("</body>");
            body.AppendLine("</html>");

            return body.ToString();
        }

        public async Task<RefundResponseModel> RequestRefundAsync(RefundRequestModel model)
        {
            var orderRf = await _cursusDbContext.Order.Include(u => u.User).FirstOrDefaultAsync(c => c.OrderId == model.OrderId);
            if (orderRf == null)
            {
                throw new Exception($"Don't have order with orderId : {model.OrderId} ");
            }

            if (orderRf.Status != "Success")
            {
                throw new InvalidOperationException("Order cannot be refunded - invalid status");
            }
            var payment = await _cursusDbContext.Payment
                .FirstOrDefaultAsync(p => p.OrderId == model.OrderId && p.Status == "Complete");

            if (payment == null)
            {
                throw new InvalidOperationException("No completed payment found for this order");
            }
            // Create pending refund record
            var refund = new Refunds
            {
                OrderId = orderRf.OrderId,
                Amount = (decimal)payment.Amount,
                RefundDate = DateTime.UtcNow,
                Reason = model.Reason,
                Status = "Pending"
            };

            _cursusDbContext.Refunds.Add(refund);
            await _cursusDbContext.SaveChangesAsync();

            return new RefundResponseModel
            {
                Success = true,
                Status = "Pending",
                Message = "Refund request submitted and pending admin approval",
                RefundDate = DateTime.UtcNow
            };
        }
        public async Task<RefundResponseModel> ApproveRefundAsync(AdminRefundApprovalModel model, string adminUserName)
        {
            // Verify admin
            var admin = await _cursusDbContext.User.FirstOrDefaultAsync(u => u.UserName == adminUserName && u.RoleId == 3);
            if (admin == null)
            {
                throw new UnauthorizedAccessException("Only administrators can approve refunds");
            }

            // Get refund request
            var refund = await _cursusDbContext.Refunds.FirstOrDefaultAsync(r => r.RefundId == model.RefundId);
            if (refund == null)
            {
                throw new InvalidOperationException("Refund request not found");
            }

            if (refund.Status != "Pending")
            {
                throw new InvalidOperationException("Refund request is not in pending state");
            }

            // If rejected
            if (!model.IsApproved)
            {
                refund.Status = "Rejected";
                await _cursusDbContext.SaveChangesAsync();
                return new RefundResponseModel
                {
                    Success = false,
                    Status = "Rejected",
                    Message = "Refund request rejected by admin",
                    RefundDate = DateTime.UtcNow
                };
            }

            // If approved, process the refund
            return await ProcessRefundAsync(refund);
        }
        private async Task<RefundResponseModel> ProcessRefundAsync(Refunds refund)
        {
            var order = await _cursusDbContext.Order
                .Include(o => o.Payments)
                .Include(o => o.User)
                .ThenInclude(w => w.Wallet)
                .FirstOrDefaultAsync(o => o.OrderId == refund.OrderId);

            if (order == null)
            {
                throw new InvalidOperationException("Order not found");
            }


            decimal orderPrice = (decimal)order.OrderPrice;
            decimal amount = (decimal)refund.Amount;

            // Process wallet refund if needed
            var wallet = await _cursusDbContext.Wallet.FirstOrDefaultAsync(w => w.UserName == order.UserName);
            if (wallet == null)
            {
                throw new Exception("Wallet not found");
            }

            if (orderPrice > amount)
            {
                double amountRfWallet = (double)(orderPrice - amount);
                wallet.Balance += amountRfWallet;
                var balanceRefund = new Transactions
                {
                    walletId = order.User.Wallet.WalletId,
                    PaymentCode = $"REFUND_{order.OrderCode}",
                    CreatedDate = DateTime.UtcNow,
                    Amount = (double?)amountRfWallet,
                    Description = $"Refund reversal for course",
                    Status = "Complete"
                };
                await _transactionRepository.AddPaymentAsync(balanceRefund);
            }

            // Process PayPal refund
            var getOrderRequest = new OrdersGetRequest(order.OrderCode);
            var orderResponse = await _payPalHttpClient.Execute(getOrderRequest);
            var paypalOrder = orderResponse.Result<PayPalCheckoutSdk.Orders.Order>();

            var captureId = paypalOrder.PurchaseUnits[0]
                .Payments.Captures[0].Id;

            var refundRequest = new CapturesRefundRequest(captureId);
            var refundBody = new PayPalCheckoutSdk.Payments.RefundRequest()
            {
                Amount = new PayPalCheckoutSdk.Payments.Money
                {
                    CurrencyCode = "USD",
                    Value = amount.ToString("0.00")
                },
                NoteToPayer = refund.Reason
            };

            refundRequest.RequestBody(refundBody);
            var response = await _payPalHttpClient.Execute(refundRequest);
            var refundResult = response.Result<PayPalCheckoutSdk.Payments.Refund>();

            if (refundResult.Status == "COMPLETED")
            {
                order.Status = "Refunded";
                refund.Status = "Complete";
                refund.TransactionId = refundResult.Id;

                await ReverseEarningsDistributionAsync(order, (decimal)order.OrderPrice);
                await _cursusDbContext.SaveChangesAsync();

                string emailOfUser = order.User.Email;
                if (emailOfUser != null)
                {
                    await SendRefundNotificationEmailAsync(emailOfUser, order, refund);
                }

                return new RefundResponseModel
                {
                    Success = true,
                    RefundId = refundResult.Id,
                    Status = "COMPLETED",
                    Message = "Refund processed successfully",
                    RefundDate = DateTime.UtcNow
                };
            }

            refund.Status = "Failed";
            await _cursusDbContext.SaveChangesAsync();

            return new RefundResponseModel
            {
                Success = false,
                Status = refundResult.Status,
                Message = "Refund could not be completed",
                RefundDate = DateTime.UtcNow
            };
        }
        //======================================================================
        public async Task<RefundResponseModel> RefundOrderAsync(RefundRequestModel model)
        {
            //var orderRf = await _cursusDbContext.Order.Include(u => u.User).FirstOrDefaultAsync(c => c.OrderId == model.OrderId);
            //if (orderRf == null)
            //{
            //    throw new Exception($"Don't have order with orderId : {model.OrderId} ");
            //}
            //string transactionId = orderRf.OrderCode;
            //var order = await _cursusDbContext.Order
            //    .Include(o => o.Payments)
            //     .Include(o => o.User)
            //     .ThenInclude(w => w.Wallet)
            //    .FirstOrDefaultAsync(o => o.OrderCode == transactionId);
            //if (order == null)
            //{
            //    throw new InvalidOperationException("Order not found");
            //}

            //if (order.Status != "Success")
            //{
            //    throw new InvalidOperationException("Order cannot be refunded - invalid status");
            //}

            //var payment = await _cursusDbContext.Payment
            //    .FirstOrDefaultAsync(p => p.OrderId == order.OrderId && p.Status == "Complete");

            //if (payment == null)
            //{
            //    throw new InvalidOperationException("No completed payment found for this order");
            //}
            //decimal orderPrice = (decimal)order.OrderPrice;
            //decimal amount = (decimal)payment.Amount;
            //var wallet = await _cursusDbContext.Wallet.FirstOrDefaultAsync(w => w.UserName == order.UserName);
            //if (wallet == null)
            //{
            //    throw new Exception("wrong hihi");
            //}
            //if (orderPrice > amount)
            //{
            //    double amountRfWallet = (double)(orderPrice - amount);
            //    wallet.Balance += amountRfWallet;
            //    var balanceRefund = new Transactions
            //    {
            //        walletId = order.User.Wallet.WalletId,
            //        PaymentCode = $"REFUND_{order.OrderCode}",
            //        CreatedDate = DateTime.UtcNow,
            //        Amount = (double?)amountRfWallet,
            //        Description = $"Refund reversal for course",
            //        Status = "Complete"
            //    };
            //    await _transactionRepository.AddPaymentAsync(balanceRefund);

            //}
            //var getOrderRequest = new OrdersGetRequest(payment.PaymentCode);
            //var orderResponse = await _payPalHttpClient.Execute(getOrderRequest);
            //var paypalOrder = orderResponse.Result<PayPalCheckoutSdk.Orders.Order>();

            //var captureId = paypalOrder.PurchaseUnits[0]
            //    .Payments.Captures[0].Id;

            //var refundRequest = new CapturesRefundRequest(captureId);
            //var refundBody = new PayPalCheckoutSdk.Payments.RefundRequest()
            //{
            //    Amount = new PayPalCheckoutSdk.Payments.Money
            //    {
            //        CurrencyCode = "USD",
            //        Value = amount.ToString("0.00")
            //    },
            //    NoteToPayer = model.Reason
            //};

            //refundRequest.RequestBody(refundBody);

            //var response = await _payPalHttpClient.Execute(refundRequest);
            //var refundResult = response.Result<PayPalCheckoutSdk.Payments.Refund>();

            //if (refundResult.Status == "COMPLETED")
            //{
            //    order.Status = "Refunded";

            //    var refund = new Refunds
            //    {
            //        OrderId = orderRf.OrderId,
            //        TransactionId = refundResult.Id,
            //        Amount = (decimal)order.OrderPrice,
            //        RefundDate = DateTime.UtcNow,
            //        Reason = model.Reason,
            //        Status = "Complete"
            //    };

            //    _cursusDbContext.Refunds.Add(refund);

            //    await ReverseEarningsDistributionAsync(order, (decimal)order.OrderPrice);

            //    await _cursusDbContext.SaveChangesAsync();

            //    string emailOfUser = order.User.Email;
            //    if (emailOfUser != null)
            //    {
            //        await SendRefundNotificationEmailAsync(emailOfUser, order, refund);
            //    }
            //    return new RefundResponseModel
            //    {
            //        Success = true,
            //        RefundId = refundResult.Id,
            //        Status = "COMPLETED",
            //        Message = "Refund processed successfully",
            //        RefundDate = DateTime.UtcNow
            //    };
            //}

            //return new RefundResponseModel
            //{
            //    Success = false,
            //    Status = refundResult.Status,
            //    Message = "Refund could not be completed",
            //    RefundDate = DateTime.UtcNow
            //};
            return null;
        }

        private async Task ReverseEarningsDistributionAsync(Order order, decimal refundAmount)
        {
            var rate = double.Parse(_IConfiguration["RatePrice:rate"]);
            var orderDetails = await _cursusDbContext.OrderDetail
                .Where(od => od.OrderId == order.OrderId)
                .ToListAsync();

            foreach (var detail in orderDetails)
            {
                var course = await _cursusDbContext.Course.FindAsync(detail.CourseId);
                if (course != null)
                {
                    course.TotalEnrollment -= 1;
                    var instructorWallet = await _cursusDbContext.Wallet
                        .FirstOrDefaultAsync(w => w.UserName == course.Username);
                    if (instructorWallet != null)
                    {
                        instructorWallet.Balance -= detail.Price * (1 - rate);

                        var instructorTransaction = new Transactions
                        {
                            walletId = instructorWallet.WalletId,
                            PaymentCode = $"REFUND_{order.OrderCode}",
                            CreatedDate = DateTime.UtcNow,
                            Amount = -(detail.Price * (1 - rate)),
                            Description = $"Refund reversal for course {course.CourseCode}",
                            Status = "Complete"
                        };
                        await _transactionRepository.AddPaymentAsync(instructorTransaction);
                    }
                    var adminWallet = await _cursusDbContext.Wallet
                        .FirstOrDefaultAsync(w => w.User.RoleId == 3);
                    if (adminWallet != null)
                    {
                        adminWallet.Balance -= (detail.Price * rate);

                        var adminTransaction = new Transactions
                        {
                            walletId = adminWallet.WalletId,
                            PaymentCode = $"REFUND_{order.OrderCode}",
                            CreatedDate = DateTime.UtcNow,
                            Amount = -(detail.Price * rate),
                            Description = $"Refund reversal for course {course.CourseCode}",
                            Status = "Complete"
                        };
                        await _transactionRepository.AddPaymentAsync(adminTransaction);
                    }
                }
            }
        }

        private async Task SendRefundNotificationEmailAsync(string email, Order order, Refunds refund)
        {
            string subject = "Refund Processed";
            string message = GenerateRefundEmailBody(order, refund);
            await _emailSender.EmailSendAsync(email, subject, message);
        }

        private string GenerateRefundEmailBody(Order order, Refunds refund)
        {
            StringBuilder body = new StringBuilder();
            body.AppendLine("<!DOCTYPE html>");
            body.AppendLine("<html>");
            body.AppendLine("<head>");
            body.AppendLine("    <style>");
            body.AppendLine("        body { font-family: Arial, sans-serif; }");
            body.AppendLine("        .container { max-width: 600px; margin: auto; padding: 20px; }");
            body.AppendLine("        .header { text-align: center; color: #333; }");
            body.AppendLine("        .details { margin: 20px 0; }");
            body.AppendLine("        .footer { text-align: center; color: #666; font-size: 12px; }");
            body.AppendLine("    </style>");
            body.AppendLine("</head>");
            body.AppendLine("<body>");
            body.AppendLine("    <div class='container'>");
            body.AppendLine("        <div class='header'>");
            body.AppendLine("            <h2>Refund Confirmation</h2>");
            body.AppendLine("        </div>");
            body.AppendLine("        <div class='details'>");
            body.AppendLine($"            <p>Order Number: {order.OrderCode}</p>");
            body.AppendLine($"            <p>Refund Amount: ${refund.Amount}</p>");
            body.AppendLine($"            <p>Refund Date: {refund.RefundDate}</p>");
            body.AppendLine($"            <p>Reason: {refund.Reason}</p>");
            body.AppendLine("        </div>");
            body.AppendLine("        <div class='footer'>");
            body.AppendLine("            <p>If you have any questions, please contact our support team.</p>");
            body.AppendLine("        </div>");
            body.AppendLine("    </div>");
            body.AppendLine("</body>");
            body.AppendLine("</html>");

            return body.ToString();
        }
    }
}
