 using System.ComponentModel.DataAnnotations.Schema;

namespace KhielsSkincare.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public string OrderCode { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}