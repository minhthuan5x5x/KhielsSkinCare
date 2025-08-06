namespace KhielsSkincare.Models.ViewModels
{
    public class ProductDetailViewModel
    {
        public Product ProductDetail { get; set; }
        public IEnumerable<Product> RelatedProducts { get; set; }
        public List<Review> Reviews { get; set; } // Danh sách đánh giá
        public Review NewReview { get; set; } = new Review(); // Đánh giá mới
    }
}
