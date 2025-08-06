using KhielsSkincare.Models;
using KhielsSkincare.Models.ViewModels;
using KhielsSkincare.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhielsSkincare.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/User")]

    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly KhielsContext _khielsContext;
        public UserController(UserManager<AppUser> userManager, KhielsContext khielsContext)
        {
            _userManager = userManager;
            _khielsContext = khielsContext;
        }
        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var usersRole = await (from u in _khielsContext.Users
                                   join ur in _khielsContext.UserRoles on u.Id equals ur.UserId into userRoles // Group join
                                   from ur in userRoles.DefaultIfEmpty() // Join bên trái
                                   join r in _khielsContext.Roles on ur.RoleId equals r.Id into roles // Group join cho roles
                                   from r in roles.DefaultIfEmpty() // Join bên trái
                                   select new
                                   {
                                       UserId = u.Id,
                                       UserName = u.UserName,
                                       Email = u.Email,
                                       RoleName = r != null ? r.Name : "User" // Nếu không có role thì hiển thị "User"
                                   }).ToListAsync();
            return View(usersRole);
        }

        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            // Lấy thông tin người dùng dựa trên Id
            var user = await _khielsContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(); // Trả về lỗi 404 nếu không tìm thấy người dùng
            }

            return View(user); // Trả về view với thông tin người dùng
        }


        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _khielsContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(); // Trả về lỗi 404 nếu không tìm thấy người dùng
            }

            return View(user); // Trả về view với thông tin người dùng để xác nhận xóa
        }

        [HttpPost]
        [Route("Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _khielsContext.Users.FindAsync(id);
            if (user != null)
            {
                _khielsContext.Users.Remove(user);
                await _khielsContext.SaveChangesAsync();
            }
            return RedirectToAction("Index"); // Chuyển hướng về danh sách người dùng sau khi xóa
        }

        [Route("Payment")]
        public IActionResult Payment()
        {
            var payments = _khielsContext.Payments.Include(p => p.Order).ToList();
            return View(payments);
        }

        [Route("Shipping")]
        public IActionResult Shipping()
        {
            var shippings = _khielsContext.Shippings.ToList();  
            return View(shippings);
        }
    }
}
