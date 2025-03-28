using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model.VNPAY;
using Project_Cursus_Group3.Service.Config;
using Project_Cursus_Group3.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Service.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        private readonly CursusDbContext _cursusDbContext;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IEmailSender _emailSender;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger _logger;


        public VnPayService(IConfiguration configuration, CursusDbContext cursusDbContext, IPaymentRepository paymentRepository, IEmailSender emailSender, ITransactionRepository transactionRepository)
        {
            _configuration = configuration;
            _cursusDbContext = cursusDbContext;
            _paymentRepository = paymentRepository;
            _emailSender = emailSender;
            _transactionRepository = transactionRepository;
        }
        public async Task<double> GetUSDtoVND()
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync($"{_configuration["ExchangeRates:ApiUrl"]}?access_key={_configuration["ExchangeRates:ApiKey"]}&symbols=USD,VND");
                JObject jsonData = JObject.Parse(response);
                decimal rate = (decimal)jsonData["rates"]["VND"];
                Console.WriteLine($"Current USD to VND exchange rate: {rate}");
                return (double)rate;
            }
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
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
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
        public async Task<string> CreatePaymentUrlAsync(PaymentInformationModel model, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();

            var pay = new VnpayConfig();
            var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];

            var username = GetUsernameFromToken(context);
            if (username == null)
            {
                throw new Exception("Unauthorized access");
            }

            if (model.OrderType == "paynow")
            {
                var order = await _cursusDbContext.Order.SingleOrDefaultAsync(o => o.OrderId == model.OrderId);
                var wallet = await _cursusDbContext.Wallet.SingleOrDefaultAsync(w => w.UserName == username);
                if (order == null)
                {
                    throw new Exception("Order not found");
                }

                if (wallet == null)
                {
                    throw new Exception("Wallet not found for user: " + username);
                }
                double balance = (double)wallet.Balance;
                var rate = await GetUSDtoVND();
                double orderPriceInVND = (double)order.OrderPrice * rate;
                if (balance >= orderPriceInVND)
                {
                    wallet.Balance = balance - orderPriceInVND;
                    order.Status = "Success";
                    _cursusDbContext.SaveChanges();
                    return "ORDER_PROCESSED";
                }
            }
            double amount = await CalculateAmountAsync(model, username);

            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((long)(amount * 100)).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", GetOrderInfo(model, username));
            pay.AddRequestData("vnp_OrderType", model.OrderType);
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", tick);

            var paymentUrl = pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

            return paymentUrl;
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

                double balance = (double)wallet.Balance;
                var ratee = await GetUSDtoVND();

                    return (double)order.OrderPrice * ratee - balance;
            }

            throw new Exception("Invalid OrderType");
        }

        private string GetOrderInfo(PaymentInformationModel model,string username)
        {
            return model.OrderType == "paynow"
                ? $"{model.OrderType}:{model.OrderId}"
                : $"{username}";
        }

        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnpayConfig();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);
            var orderDescriptionParts = response.OrderDescription.Split(':');

            if (!response.Success || response.VnPayResponseCode != "00")
            {
                return new PaymentResponseModel
                {
                    Success = false,
                    OrderDescription = response.OrderDescription,
                    VnPayResponseCode = response.VnPayResponseCode,
                    OrderType = response.OrderType,
                    Amount = response.Amount,
                    ResponseDate = DateTime.Now
                };
            }

            if (orderDescriptionParts[0] == "paynow")
            {
                return ProcessOrderPayment(response).Result;
            }
            return ProcessWalletPayment(response);
        }

        private PaymentResponseModel ProcessWalletPayment(PaymentResponseModel response)
        {
            if (response.Amount <= 0)
            {
                return new PaymentResponseModel
                {
                    Success = false,
                    OrderDescription = "Invalid Amount",
                    VnPayResponseCode = response.VnPayResponseCode,
                    OrderType = response.OrderType,
                    Amount = response.Amount,
                    ResponseDate = DateTime.Now
                };
            }

            var wallet = _cursusDbContext.Wallet.SingleOrDefault(w => w.UserName == response.OrderDescription.Trim());
            if (wallet == null)
            {
                wallet = new Wallet
                {
                    UserName = response.OrderDescription.Trim(),
                    Balance = 0,
                    TransactionTime = DateTime.Now
                };
                _cursusDbContext.Wallet.Add(wallet);
            }
            double fixAmount = response.Amount / 100;
            wallet.Balance += fixAmount;
            wallet.TransactionTime = DateTime.Now;

            //tạo transaction
            var transaction = new Transactions
            {
                walletId = wallet.WalletId,
                PaymentCode = response.TransactionId,
                CreatedDate = DateTime.UtcNow,
                Amount = response.Amount / 100,
                Description = "Wallet completed successfully",
                Status = "Complete"
            };

            _transactionRepository.AddPaymentAsync(transaction);

            return new PaymentResponseModel
            {
                Success = true,
                OrderDescription = "Wallet top-up successful",
                TransactionId = response.TransactionId,
                VnPayResponseCode = response.VnPayResponseCode,
                PaymentMethod = "VnPay",
                OrderType = "Wallet",
                OrderId = response.OrderDescription.Trim(),
                Amount = response.Amount,
                ResponseDate = DateTime.Now
            };
        }

        private async Task<PaymentResponseModel> ProcessOrderPayment(PaymentResponseModel response)
        {
            var orderDescriptionParts = response.OrderDescription.Split(':');
            string orderIdDes = orderDescriptionParts[1];
            if (!int.TryParse(orderIdDes, out int orderId))
            {
                return new PaymentResponseModel
                {
                    Success = false,
                    OrderDescription = "Invalid OrderId format",
                    VnPayResponseCode = response.VnPayResponseCode,
                    OrderType = response.OrderType,
                    Amount = response.Amount,
                    ResponseDate = DateTime.Now
                };
            }

            var order = await _cursusDbContext.Order
                .Include(o => o.User)
                .SingleOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
            {
                return new PaymentResponseModel
                {
                    Success = false,
                    OrderDescription = "Order not found",
                    VnPayResponseCode = response.VnPayResponseCode,
                    PaymentMethod = "VNPAY",
                    Amount = response.Amount
                };
            }

            var payment = new Payment
            {
                OrderId = orderId,
                PaymentCode = response.TransactionId,
                CreatedDate = DateTime.UtcNow,
                Amount = response.Amount / 100,
                Description = "Payment completed successfully",
                Status = "Complete"
            };

            await _paymentRepository.AddPaymentAsync(payment);
            order.Status = "Success";
            var rateString = _configuration["RatePrice:rate"];
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
                        instructorWallet.Balance += detail.Price * (1-rate);
                        instructorWallet.TransactionTime = DateTime.Now;
                        var instructorTransaction = new Transactions
                        {
                            walletId = instructorWallet.WalletId,
                            PaymentCode = "Instructor earnings",
                            CreatedDate = DateTime.UtcNow,
                            Amount = instructorWallet.Balance,
                            Description = $"Earnings for course {course.CourseCode} from order {order.OrderCode}",
                            Status = "Complete"
                        };
                        // Update admin's wallet and create transaction
                        walletOfAdmin.Balance += (detail.Price * rate);
                        walletOfAdmin.TransactionTime = DateTime.Now;
                        var adminTransaction = new Transactions
                        {
                            walletId = walletOfAdmin.WalletId,
                            PaymentCode = "Admin earnings",
                            CreatedDate = DateTime.UtcNow,
                            Amount = walletOfAdmin.Balance,
                            Description = $"Admin earnings for course {course.CourseCode} from order {order.OrderCode}",
                            Status = "Complete"
                        };
                    }
                }
            }
            await _cursusDbContext.SaveChangesAsync();

            //send email
            if (order.User != null && !string.IsNullOrEmpty(order.User.Email))
            {
                await SendOrderConfirmationEmail(order.User.Email, order, orderDetails);
            }
            return new PaymentResponseModel
            {
                Success = true,
                OrderDescription = "Payment completed successfully",
                TransactionId = response.TransactionId,
                OrderId = orderDescriptionParts[1],
                PaymentMethod = "VNPAY",
                VnPayResponseCode = response.VnPayResponseCode,
                OrderType = orderDescriptionParts[0],
                Amount = response.Amount,
                ResponseDate = DateTime.Now
            };
        }
        private async Task SendOrderConfirmationEmail(string email, Order order, List<OrderDetail> orderDetails)
        {
            string subject = "Order Information";
            string message = GenerateOrderConfirmationEmailBody(order, orderDetails);

            await _emailSender.EmailSendAsync(email, subject, message);
        }

        private string GenerateOrderConfirmationEmailBody(Order order, List<OrderDetail> orderDetails)
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
    }
}