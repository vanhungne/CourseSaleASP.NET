using AutoMapper;
using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model.CartModel;
using Project_Cursus_Group3.Data.ViewModels.CartDTO;
using Project_Cursus_Group3.Data.ViewModels.OrderDTO;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Text;

namespace Project_Cursus_Group3.Data.Repository
{
    public class CartRepository : ICartRepository
    {
        private static readonly ConcurrentDictionary<string, List<CartItem>> Carts = new();
        private readonly ICourseRepository courseRepository;
        private readonly IOrderRepository orderRepository;
        private readonly IOrderDetailRepository orderDetailRepository;
        private readonly CursusDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IPurchasedCourseRepository _puchasedCourseRepository;

        public CartRepository(ICourseRepository courseRepository, IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository, IMapper mapper, CursusDbContext cursusDbContext, IConfiguration configuration, IEmailSender emailSender, ITransactionRepository transactionRepository, IPurchasedCourseRepository puchasedCourseRepository)
        {
            this.courseRepository = courseRepository;
            this.orderRepository = orderRepository;
            this.orderDetailRepository = orderDetailRepository;
            _mapper = mapper;
            _context = cursusDbContext;
            _configuration = configuration;
            _emailSender = emailSender;
            _transactionRepository = transactionRepository;
            _puchasedCourseRepository = puchasedCourseRepository;
        }

        public async Task<CartViewModel> AddToCartAsync(string userName, int courseId)
        {

            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("User ID cannot be null or empty");
            }

            var purchasedCourse = await _context.PurchasedCourse
                                       .FirstOrDefaultAsync(pc => pc.UserName == userName && pc.CourseId == courseId);

            if (purchasedCourse != null)
            {
                throw new InvalidOperationException($"You already own the course with ID {courseId}.");
            }

            if (!Carts.ContainsKey(userName))
            {
                Carts[userName] = new List<CartItem>();
            }

            var cart = Carts[userName];
            var existingItem = cart.Find(item => item.CourseId == courseId);
            var course = await courseRepository.GetByIdAsync(courseId);

            if (course == null)
            {
                throw new InvalidOperationException($"Course with ID {courseId} not found.");
            }

            var cartViewModel = new CartViewModel
            {
                CourseId = course.CourseId,
                CourseTitle = course.CourseTitle,
                CourseCode = course.CourseCode,
                Description = course.Description,
                Discount = (double)course.Discount,
                Price = course.Price,
                ShortDescription = course.ShortDescription,
                AddedDate = DateTime.Now.AddHours(7)
            };

            if (existingItem == null)
            {
                cart.Add(new CartItem
                {
                    CourseId = courseId,
                    AddedDate = DateTime.Now.AddHours(7)
                });

                cartViewModel.Message = $"Course '{course.CourseTitle}' added to cart successfully.";
            }
            else
            {

                cartViewModel.Message = $"Course '{course.CourseTitle}' is already in the cart.";
            }

            return cartViewModel;
        }

        public CartSummaryViewModel GetCart(string userName)
        {
            var cartSummary = new CartSummaryViewModel();

            if (Carts.ContainsKey(userName))
            {
                var cartItems = Carts[userName];
                int totalItems = 0;
                double totalPrice = 0;

                foreach (var item in cartItems)
                {
                    var course = courseRepository.GetByIdAsync(item.CourseId).Result;
                    if (course != null)
                    {
                        totalItems++;
                        totalPrice += course.Price;

                        cartSummary.CartItems.Add(new CartViewModel
                        {
                            CourseId = course.CourseId,
                            CourseTitle = course.CourseTitle,
                            CourseCode = course.CourseCode,
                            Description = course.Description,
                            Discount = (double)course.Discount,
                            Price = course.Price,
                            ShortDescription = course.ShortDescription,
                            AddedDate = item.AddedDate,
                            Message = $"Course '{course.CourseTitle}' is in your cart."
                        });
                    }
                }

                cartSummary.TotalItems = totalItems;
                cartSummary.TotalPrice = totalPrice;
            }

            return cartSummary; 
        }



        public void ClearAllCart(string userName)
        {
            if (Carts.ContainsKey(userName))
            {
                Carts[userName].Clear();
            }
        }

        public void RemoveItemsInCart(string userName, int courseId)
        {

            if (Carts.ContainsKey(userName))
            {
                var cart = Carts[userName];
                var existingItem = cart.Find(item => item.CourseId == courseId);

                if (existingItem == null)
                {
                    throw new InvalidOperationException($"Course with ID {courseId} does not exist in the cart.");
                }

                cart.RemoveAll(item => item.CourseId == courseId);
            }
            else
            {
                throw new InvalidOperationException("User's cart does not exist.");
            }
        }



        public async Task<OrderViewModel> CheckoutAsync(string userName)
        {
            if (!Carts.TryGetValue(userName, out var cartItems) || cartItems.Count == 0)
            {
                throw new InvalidOperationException("The cart is empty.");
            }

            double balance = 0;
            double totalPrice = 0;
            foreach (var item in cartItems)
            {
                var course = await courseRepository.GetByIdAsync(item.CourseId);
                if (course != null)
                {
                    totalPrice += course.Price - (course.Price * (double)course.Discount / 100);
                }
            }
            var wallet = _context.Wallet.FirstOrDefault(w => w.UserName == userName);
            if (wallet != null)
            {
                balance = wallet.Balance ?? 0;
            }

            var order = new Order
            {
                UserName = userName,
                OrderCode = GenerateOrderCode(),
                OrderDate = DateTime.Now,
                OrderPrice = totalPrice,
                Status = "Pending"
            };
            var createdOrder = await orderRepository.CreateOrderAsync(order);

            if (totalPrice <= balance)
            {
                wallet.Balance = balance - totalPrice;
                wallet.TransactionTime = DateTime.Now;
                order.Status = "Success";

                var userTransaction = new Transactions
                {
                    walletId = wallet.WalletId,
                    PaymentCode = "User payment for order",
                    CreatedDate = DateTime.UtcNow,
                    Amount = -totalPrice,
                    Description = $"Payment for order {order.OrderCode}",
                    Status = "Complete"
                };
                await _transactionRepository.AddPaymentAsync(userTransaction);

                var orderDetails = new List<OrderDetail>();
                foreach (var item in cartItems)
                {
                    var course = await courseRepository.GetByIdAsync(item.CourseId);
                    if (course != null)
                    {
                        var coursePrice = course.Price - (course.Price * (double)course.Discount / 100);
                        var orderDetail = new OrderDetail
                        {
                            OrderId = createdOrder.OrderId,
                            CourseId = course.CourseId,
                            Price = (double)coursePrice
                        };

                        course.TotalEnrollment += 1;

                        var walletOfInstructor = _context.Wallet.FirstOrDefault(w => w.UserName == course.Username);
                        var walletOfAdmin = _context.Wallet.FirstOrDefault(w => w.User.RoleId == 3);

                        var rateString = _configuration["RatePrice:rate"];
                        if (!double.TryParse(rateString, NumberStyles.Any, CultureInfo.InvariantCulture, out double rate))
                        {
                            rate = 1.0;
                        }

                        double instructorAmount = coursePrice * (1 - rate);
                        double adminAmount = coursePrice * rate;

                        if (walletOfInstructor != null)
                        {
                            walletOfInstructor.Balance += instructorAmount;
                            walletOfInstructor.TransactionTime = DateTime.Now;

                            var instructorTransaction = new Transactions
                            {
                                walletId = walletOfInstructor.WalletId,
                                PaymentCode = "Instructor earnings",
                                CreatedDate = DateTime.UtcNow,
                                Amount = instructorAmount,
                                Description = $"Earnings for course {course.CourseCode} from order {order.OrderCode}",
                                Status = "Complete"
                            };
                            await _transactionRepository.AddPaymentAsync(instructorTransaction);
                        }

                        if (walletOfAdmin != null)
                        {
                            walletOfAdmin.Balance += adminAmount;
                            walletOfAdmin.TransactionTime = DateTime.Now;

                            var adminTransaction = new Transactions
                            {
                                walletId = walletOfAdmin.WalletId,
                                PaymentCode = "Admin earnings",
                                CreatedDate = DateTime.UtcNow,
                                Amount = adminAmount,
                                Description = $"Admin earnings for course {course.CourseCode} from order {order.OrderCode}",
                                Status = "Complete"
                            };
                            await _transactionRepository.AddPaymentAsync(adminTransaction);
                        }

                        orderDetails.Add(orderDetail);

                        var purchaseCourse = new PurchasedCourse
                        {
                            CourseId = course.CourseId,
                            UserName = userName,
                            Status = "Success"
                        };
                        await _puchasedCourseRepository.AddPurchaseCourse(purchaseCourse);
                    }
                }

                await orderDetailRepository.CreateOrderDetailsAsync(orderDetails);

                var user = await _context.User.FirstOrDefaultAsync(u => u.UserName == userName);
                if (user != null)
                {
                    await SendOrderConfirmationEmail(user.Email, order, orderDetails);
                }
            }
            else
            {
                var orderDetails = new List<OrderDetail>();
                foreach (var item in cartItems)
                {
                    var course = await courseRepository.GetByIdAsync(item.CourseId);
                    if (course != null)
                    {
                        var orderDetail = new OrderDetail
                        {
                            OrderId = createdOrder.OrderId,
                            CourseId = course.CourseId,
                            Price = (double)course.Price
                        };

                        orderDetails.Add(orderDetail);

                        var purchaseCourse = new PurchasedCourse
                        {
                            CourseId = course.CourseId,
                            UserName = userName,
                            Status = "Progress"
                        };
                        await _puchasedCourseRepository.AddPurchaseCourse(purchaseCourse);
                    }
                }

                await orderDetailRepository.CreateOrderDetailsAsync(orderDetails);
            }
            ClearAllCart(userName);
            return _mapper.Map<OrderViewModel>(createdOrder);
        }
        private string GenerateOrderCode()
        {
            return $"ORD-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N").Substring(0, 6)}";
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


        private async Task SendOrderConfirmationEmail(string email, Order order, List<OrderDetail> orderDetails)
        {
            string subject = "Order Confirmation";
            string message = GenerateOrderConfirmationEmailBody(order, orderDetails);

            await _emailSender.EmailSendAsync(email, subject, message);
        }

        //format
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
                var course = _context.Course.Find(detail.CourseId);
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
