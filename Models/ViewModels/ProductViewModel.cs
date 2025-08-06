using System.ComponentModel.DataAnnotations.Schema;

namespace KhielsSkincare.Models.ViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public List<ProductVariant> ProductVariants { get; set; }
        public IEnumerable<Category> Categories { get; set; } // Danh sách các danh mục để hiển thị trong dropdown

        [NotMapped]
        public List<IFormFile> ImageUpload { get; set; } // Thuộc tính để tải lên nhiều tệp hình ảnh
        public ProductViewModel()
        {
            ProductVariants = new List<ProductVariant>(); // Khởi tạo danh sách
        }
    }
}
