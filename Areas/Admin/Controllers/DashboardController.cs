using KhielsSkincare.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhielsSkincare.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Dashboard")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly KhielsContext _khielsContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DashboardController (KhielsContext khiels, IWebHostEnvironment webHostEnvironment)
        {
            _khielsContext = khiels;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var count_product = _khielsContext.Products.Count();
            var count_order = _khielsContext.Orders.Count();
            var count_user = _khielsContext.Users.Count();

            ViewBag.CountProduct = count_product;
            ViewBag.CountOrder = count_order;
            ViewBag.CountUser = count_user;

            return View();
        }
    }
}
