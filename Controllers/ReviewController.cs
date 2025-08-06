using KhielsSkincare.Models;
using KhielsSkincare.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhielsSkincare.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly KhielsContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ReviewController(KhielsContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] Review review)
        {
            if (ModelState.IsValid)
            {
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();
                return Ok(); // Trả về HTTP 200 OK
            }
            return BadRequest(ModelState); // Trả về HTTP 400 Bad Request nếu có lỗi
        }

        // Optionally, add a method to retrieve reviews for a product
        //public async Task<IActionResult> GetReviews(string productId)
        //{
        //    var reviews = await _context.Reviews
        //        .Where(r => r.ProductId == productId)
        //        .ToListAsync();
        //    return Json(reviews); // Trả về JSON cho yêu cầu AJAX
        //}

        //[HttpPost]
        //public async Task<IActionResult> CreateReview(string productId, string comment, int rating, string nametype, string nickname, [FromForm] Review model)
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null) return Unauthorized();

        //    var review = new Review
        //    {
        //        ProductId = productId,
        //        UserId = user.Id,
        //        Comment = comment,
        //        Rating = rating,
        //        CreatedAt = DateTime.Now
        //    };

        //    // Xử lý tên hiển thị
        //    if (nametype == "nickname" && !string.IsNullOrEmpty(nickname))
        //    {
        //        review.Comment += $" - {nickname}"; // Thêm nickname vào nội dung
        //    }
        //    else if (nametype == "fullname")
        //    {
        //        review.Comment += $" - {user.UserName}"; // Thêm tên thật
        //    }
        //    // Nếu là ẩn danh thì không làm gì
        //    Console.WriteLine($"Rating: {model.Rating}, Comment: {model.Comment}, NameType: {model.Name}, Nickname: {model.Name}");

        //    _context.Reviews.Add(review);
        //    await _context.SaveChangesAsync();

        //    // Trả về JSON cho client để cập nhật đánh giá mà không cần tải lại trang
        //    return Json(new
        //    {
        //        success = true,
        //        reviewId = review.ReviewId,
        //        rating = rating,
        //        comment = review.Comment,
        //        createdAt = review.CreatedAt
        //    });
        //}
    }
}
