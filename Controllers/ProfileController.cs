using KhielsSkincare.Models;
using KhielsSkincare.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using KhielsSkincare.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace KhielsSkincare.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly KhielsContext _context;
        private readonly ILogger<HomeController> _logger;

        public ProfileController(ILogger<HomeController> logger, KhielsContext context, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> ProfileDetail()
        {
            ViewData["CurrentPage"] = "ProfileDetail";
            var user = await _userManager.GetUserAsync(User);

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(AppUser model, string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin người dùng
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;

            // Kiểm tra nếu người dùng muốn thay đổi mật khẩu
            if (!string.IsNullOrEmpty(CurrentPassword) && !string.IsNullOrEmpty(NewPassword))
            {
                // Kiểm tra mật khẩu hiện tại
                var passwordCheck = await _userManager.CheckPasswordAsync(user, CurrentPassword);
                if (!passwordCheck)
                {
                    ModelState.AddModelError(string.Empty, "Mật khẩu hiện tại không đúng.");
                    return View("ProfileDetail", user); // Trả về trang với thông báo lỗi
                }

                if (NewPassword != ConfirmPassword)
                {
                    ModelState.AddModelError(string.Empty, "Mật khẩu mới và xác nhận mật khẩu không khớp.");
                    return View("ProfileDetail", user);
                }

                // Đổi mật khẩu
                var result = await _userManager.ChangePasswordAsync(user, CurrentPassword, NewPassword);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi đổi mật khẩu.");
                    return View("ProfileDetail", user);
                }
            }

            // Cập nhật thông tin người dùng
            var updateResult = await _userManager.UpdateAsync(user);
            if (updateResult.Succeeded)
            {
                TempData["success"] = "Cập nhật thông tin thành công!";
            }
            else
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("ProfileDetail", user);
            }

            return RedirectToAction("ProfileDetail");
        }

        public async Task<IActionResult> Orders()
        {
            ViewData["CurrentPage"] = "Orders";
            var user = await _userManager.GetUserAsync(User);

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> ReviewList()
        {
            // Lấy ID của người dùng hiện tại từ Identity
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            // Kiểm tra nếu người dùng không tồn tại (phòng trường hợp lỗi)
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy danh sách đánh giá của người dùng từ cơ sở dữ liệu
            var reviews = await _context.Reviews
                .Where(r => r.UserId == userId)  // Lọc các đánh giá của người dùng hiện tại
                .Include(r => r.User)
                .Include(r => r.Product)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            if (!reviews.Any())
            {
                Console.WriteLine("Không có đánh giá nào cho người dùng này.");
            }

            ViewData["CurrentUser"] = user;

            return View(reviews);
        }
    }
}