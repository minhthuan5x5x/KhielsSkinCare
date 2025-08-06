using KhielsSkincare.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace KhielsSkincare.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly KhielsContext _khielsContext;
        public OrderController(KhielsContext khielsContext)
        {
            _khielsContext = khielsContext;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _khielsContext.Orders.OrderByDescending(p => p.OrderId).ToListAsync());
        }
        public async Task<IActionResult> ViewOrder(string orderCode)
        {
            var DetailsOrder = await _khielsContext.OrderDetails.Include(o => o.Product).Where(od => od.OrderCode == orderCode).ToListAsync();
            return View(DetailsOrder);
        }
    }
}
