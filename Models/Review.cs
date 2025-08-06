using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhielsSkincare.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }
        public int ProductId { get; set; } 
        public string UserId { get; set; } 
        public int Rating { get; set; } // Đánh giá sao (1-5)
        public string Comment { get; set; } // Nội dung đánh giá
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [ForeignKey("ProductId")]
        public Product Product { get; set; } // Điều hướng đến sản phẩm
        public virtual AppUser User { get; set; }
    }

}
