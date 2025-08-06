namespace KhielsSkincare.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; } // Địa chỉ giao hàng
        public string PhoneNumber { get; set; } // Số điện thoại
        public DateTime OrderDate { get; set; }
        public int Status { get; set; }
        public decimal TotalAmount { get; set; }
        public ICollection<Payment> Payments { get; set; } // Thêm thuộc tính Payments nếu bạn muốn theo dõi nhiều thanh toán
    }
}
