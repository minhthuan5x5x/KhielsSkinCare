namespace KhielsSkincare.Models
{
    public class Shipping
    {
        public int Id { get; set; } // ID duy nhất của thanh toán
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string PhoneNumber { get; set; } // Số điện thoại
        public string AddressLine { get; set; } // Địa chỉ chi tiết
        public string City { get; set; } // Thành phố
        public string OrderCode { get; set; }
    }
}
