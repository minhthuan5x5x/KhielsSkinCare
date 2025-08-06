using KhielsSkincare.Areas.Admin.Repository;
using KhielsSkincare.Extensions;
using KhielsSkincare.Models;
using KhielsSkincare.Models.ViewModels;
using KhielsSkincare.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Net.payOS;
using Net.payOS.Types;
using NuGet.Protocol.Plugins;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace KhielsSkincare.Controllers
{
    [Authorize]
    public class CheckOutController : Controller
    {
        private readonly KhielsContext _khielsContext;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly PayOS _payOS;
        private readonly ILogger<CheckOutController> _logger;

        public CheckOutController(PayOS payOS, KhielsContext khielsContext, IEmailSender sender, IWebHostEnvironment webHostEnvironment, ILogger<CheckOutController> logger)
        {
            _payOS = payOS;
            _khielsContext = khielsContext;
            _emailSender = sender;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;

        }
        [HttpPost]
        public async Task<IActionResult> ProcessCheckout(CheckoutViewModel model, string returnUrl = null)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { returnUrl });
            }

            if (ModelState.IsValid)
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var orderCode = Guid.NewGuid().ToString();

                // Tạo mã đơn hàng duy nhất và lấy giỏ hàng từ session
                List<CartItem> cartItems = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();
                // Lấy giá trị mã giảm giá từ model, nếu có
                decimal discountValue = 0;

                var discountValueString = HttpContext.Session.GetString("discountValue");
                if (!string.IsNullOrEmpty(discountValueString))
                {
                    discountValue = decimal.Parse(discountValueString);
                }

                decimal provisionalAmount = cartItems.Sum(c => c.Price * c.Quantity);
                decimal totalAmount = provisionalAmount - discountValue;

                // Lưu thông tin giao hàng
                var shipping = new Shipping
                {
                    LastName = model.LastName,
                    FirstName = model.FirstName,
                    PhoneNumber = model.PhoneNumber,
                    AddressLine = model.AddressLine,
                    City = model.City,
                    OrderCode = orderCode
                };
                _khielsContext.Shippings.Add(shipping);

                // Lưu thông tin đơn hàng
                var order = new Order
                {
                    OrderCode = orderCode,
                    UserName = User.Identity.Name,
                    Address = model.AddressLine + ", " + model.City,
                    PhoneNumber = model.PhoneNumber,
                    OrderDate = DateTime.Now,
                    Status = 1, // Pending status
                    TotalAmount = totalAmount
                };
                _khielsContext.Orders.Add(order);

                foreach (var item in cartItems)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderCode = orderCode,
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        Size = item.Size
                    };
                    _khielsContext.OrderDetails.Add(orderDetail);

                    var productVariant = _khielsContext.ProductVariants.FirstOrDefault(v => v.ProductVariantId == item.ProductVariantId);
                    if (productVariant != null)
                    {
                        productVariant.Quantity -= item.Quantity;
                        productVariant.Sold += item.Quantity;
                    }
                }

                await _khielsContext.SaveChangesAsync();

                var paymentMethod = model.PaymentMethod;
                if (model.PaymentMethod == "cash")
                {
                    // Lưu thông tin vào bảng Payment cho phương thức thanh toán cash
                    var payment = new Payment
                    {
                        PaymentDate = DateTime.Now,
                        Amount = order.TotalAmount,
                        PaymentMethod = "Cash",
                        Status = "Wait",
                        OrderId = order.OrderId,
                    };

                    _khielsContext.Payments.Add(payment);
                    await _khielsContext.SaveChangesAsync();
                    HttpContext.Session.Remove("Cart");

                    await SendOrderConfirmationEmail(userEmail, User.Identity.Name, (int)order.OrderId, order.TotalAmount);

                    return Json(new { success = true });
                }

                else if (model.PaymentMethod == "payOS")
                {
                    // Xử lý thanh toán qua PayOS
                    PayOS payOS = new PayOS("b9f848b7-9f41-4818-8d2a-3700094a09bb", "102d48bf-ac35-4a6a-8938-c815fb194f7a", "505b4a3c487cbdf3d1d6003a9e57e816fa40909c40e2d782c7910e511b1d9917");

                    List<ItemData> items = cartItems.Select(item => new ItemData(item.ProductName, item.Quantity, (int)item.Price)).ToList();

                    PaymentData paymentData = new PaymentData(
                        (long)order.OrderId,
                        (int)Math.Round(order.TotalAmount),
                        "Thanh toan don hang",
                        items,
                        "https://localhost:7220",
                        "https://localhost:7220/checkOut/ThankYou"
                    );

                    CreatePaymentResult createPayment = await payOS.createPaymentLink(paymentData);

                    var payment = new Payment
                    {
                        PaymentDate = DateTime.Now,
                        Amount = order.TotalAmount,
                        PaymentMethod = "PayOS",
                        Status = "Completed",
                        OrderId = order.OrderId,
                    };

                    _khielsContext.Payments.Add(payment);
                    await _khielsContext.SaveChangesAsync();
                    HttpContext.Session.Remove("Cart");
                    await SendOrderConfirmationEmail(userEmail, User.Identity.Name, (int)order.OrderId, order.TotalAmount);

                    return Json(new { success = true, redirectUrl = createPayment.checkoutUrl});
                }
            }
            return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
        }
        public async Task<IActionResult> ThankYou()
        {                      
            return View();
        }

        // Hàm gửi email xác nhận đơn hàng
        private async Task SendOrderConfirmationEmail(string receiver, string userName, int orderId, decimal totalAmount)
        {
            var templatePath = Path.Combine(_webHostEnvironment.WebRootPath, "emailTemplates", "CheckOutSuccess.html");
            var emailContent = await System.IO.File.ReadAllTextAsync(templatePath);

            // Thay thế các biến trong template
            emailContent = emailContent.Replace("{{UserName}}", userName);
            emailContent = emailContent.Replace("{{OrderCode}}", orderId.ToString());
            emailContent = emailContent.Replace("{{TotalAmount}}", totalAmount.ToVnd());

            // Gửi email
            var subject = "Xác nhận đơn hàng";
            await _emailSender.SendEmailAsync(receiver, subject, emailContent);
        }
      
    }
}
