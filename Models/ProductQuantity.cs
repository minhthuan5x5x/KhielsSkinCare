using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KhielsSkincare.Models
{
    public class ProductQuantity
    {
        [Key]
        public int StockId { get; set; }

        [Required]
        public int ProductVariantId { get; set; } // Liên kết đến biến thể sản phẩm

        [Required(ErrorMessage = "Số lượng không thể để trống.")]
        public int Quantity { get; set; }
        public DateTime DateCreate { get; set; }

        [ForeignKey("ProductVariantId")]
        public virtual ProductVariant ProductVariant { get; set; }
    }
}
