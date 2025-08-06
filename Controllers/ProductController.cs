using KhielsSkincare.Models;
using KhielsSkincare.Models.ViewModels;
using KhielsSkincare.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KhielsSkincare.Controllers
{
    public class ProductController : Controller
    {
        private readonly KhielsContext _khielsContext;

        public ProductController(ILogger<HomeController> logger, KhielsContext context)
        {            
            _khielsContext = context;
        }
        // GET: ProductController
        public ActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> ForMen()
        {
            var skinTypeForMen = "Nam"; // Hoặc "Male", tuỳ theo giá trị trong cột SkinType của bạn
            var productsForMen = await _khielsContext.Products
                .Include(p => p.ProductVariants)
                .Where(p => p.SkinType == skinTypeForMen)
                .ToListAsync();

            return View(productsForMen);
        }
        public async Task<IActionResult> Details(int Id)
        {
            if (Id == null) return RedirectToAction("Index");

            var product = await _khielsContext.Products
                .Include(p => p.ProductVariants) // Nạp biến thể sản phẩm
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.ProductId == Id);

            if (product == null) return NotFound(); // Kiểm tra nếu sản phẩm không tồn tại           

            var relatedProduct = await _khielsContext.Products
            .Include(p => p.ProductVariants)
            .Where(p => p.CategoryId == product.CategoryId && p.ProductId != product.ProductId)
            .Take(3)
            .ToListAsync();

            var reviews = await _khielsContext.Reviews
            .Where(r => r.ProductId == Id)
            .ToListAsync();

            var model = new ProductDetailViewModel
            {
                ProductDetail = product,
                RelatedProducts = relatedProduct,
                Reviews = reviews
            };

            return View(model);
        }
        [HttpPost]
        public JsonResult AddReview(Review review)
        {
            if (ModelState.IsValid)
            {
                review.CreatedAt = DateTime.Now;
                review.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Gán giá trị cho Name dựa trên lựa chọn của người dùng
                var nametype = Request.Form["nametype"]; // Lấy giá trị nametype từ form

                if (nametype == "fullname")
                {
                    // Lấy tên từ AppUser (giả sử bạn đã lưu tên thật trong Claim)
                    var userName = User.FindFirstValue(ClaimTypes.Name); // Cần phải kiểm tra nếu Claim này tồn tại
                    review.Name = userName ?? "Ẩn danh"; // Gán tên thật hoặc "Ẩn danh" nếu không có
                }
                else if (nametype == "nickname")
                {
                    review.Name = Request.Form["nickname"]; // Gán nickname
                }
                else
                {
                    review.Name = "Ẩn danh"; // Gán "Ẩn danh" nếu chọn ẩn danh
                }

                // Đảm bảo gán ProductId nếu chưa có
                if (review.ProductId == 0)
                {
                    return Json(new { success = false, error = "ProductId không hợp lệ." });
                }

                _khielsContext.Reviews.Add(review);
                _khielsContext.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false, error = "Dữ liệu không hợp lệ." });
        }

    }
}
