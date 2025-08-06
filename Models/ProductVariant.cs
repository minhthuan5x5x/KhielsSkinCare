using MessagePack;
using System.ComponentModel.DataAnnotations;
using KhielsSkincare.Repository.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhielsSkincare.Models
{
    public class ProductVariant
    {
        public int ProductVariantId { get; set; }
        public int ProductId { get; set; }
        public string Size { get; set; } // Dung tích
        
        public decimal Price { get; set; } // Giá tiền
        public int Quantity { get; set; }
        public int Sold { get; set; }

        public virtual Product Product { get; set; }
    }

}