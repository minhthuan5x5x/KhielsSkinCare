using KhielsSkincare.Models;
using KhielsSkincare.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace KhielsSkincare.Controllers
{
    public class HomeController : Controller
    {
        private readonly KhielsContext _khielsContext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, KhielsContext context)
        {
            _logger = logger;
            _khielsContext = context;
        }     
        public IActionResult ProductsSlide()
        {
            return View();
        }
        public async Task<IActionResult> Index()
        {
            var products = await _khielsContext.Products.Include(p => p.ProductVariants).ToListAsync();
            return View(products);
        }       
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statuscode)
        {
            if(statuscode == 404)
            {
                return View("NotFound");
            }
            else
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
}