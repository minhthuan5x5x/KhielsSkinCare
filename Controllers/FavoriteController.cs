using KhielsSkincare.Models;
using KhielsSkincare.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KhielsSkincare.Controllers
{
    [Authorize]
    public class FavoriteController : Controller
    {
        private readonly KhielsContext _context;
        private readonly UserManager<AppUser> _userManager;

        public FavoriteController(KhielsContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy ID của người dùng hiện tại từ Identity
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Lấy thông tin người dùng từ UserManager
            var user = await _userManager.FindByIdAsync(userId);

            // Kiểm tra nếu người dùng không tồn tại (phòng trường hợp lỗi)
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy danh sách sản phẩm yêu thích của người dùng
            var favoriteProducts = await _context.FavoriteProducts
                .Include(fp => fp.Product)
                .Include(fp => fp.ProductVariant)
                .Where(fp => fp.UserId == userId)
                .ToListAsync();

            // Truyền thông tin người dùng vào ViewData để dùng trong _NavigationProfile
            ViewData["CurrentUser"] = user;

            return View(favoriteProducts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToFavorite([FromBody] FavoriteProductViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thêm vào danh sách yêu thích" });
            }

            // Kiểm tra xem sản phẩm đã có trong danh sách yêu thích chưa
            var existingFavorite = _context.FavoriteProducts
                .FirstOrDefault(fp => fp.UserId == userId && fp.ProductId == model.ProductId && fp.ProductVariantId == model.ProductVariantId);

            if (existingFavorite == null)
            {
                var favoriteProduct = new FavoriteProduct
                {
                    UserId = userId,
                    ProductId = model.ProductId,
                    ProductVariantId = model.ProductVariantId
                };

                _context.FavoriteProducts.Add(favoriteProduct);
                _context.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Sản phẩm đã có trong danh sách yêu thích" });
        }

        // ViewModel cho yêu cầu thêm vào danh sách yêu thích
        public class FavoriteProductViewModel
        {
            public int ProductId { get; set; }
            public int ProductVariantId { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromFavorites(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var favoriteProduct = await _context.FavoriteProducts
                .FirstOrDefaultAsync(fp => fp.UserId == userId && fp.ProductId == productId);

            if (favoriteProduct != null)
            {
                _context.FavoriteProducts.Remove(favoriteProduct);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
