namespace KhielsSkincare.Models
{
    public class Discount
    {
        public int DiscountId { get; set; }
        public int ProductId { get; set; } // Khóa ngoại để liên kết với sản phẩm
        public decimal DiscountPercentage { get; set; } // Phần trăm giảm giá
        public DateTime StartDate { get; set; } // Ngày bắt đầu giảm giá
        public DateTime EndDate { get; set; }   // Ngày kết thúc giảm giá
        public string Description { get; set; } // Mô tả chương trình giảm giá
        public string Code { get; set; } //Mã giảm giá

        public virtual Product Product { get; set; } // Liên kết đến sản phẩm
    }

}