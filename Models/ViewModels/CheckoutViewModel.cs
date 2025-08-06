namespace KhielsSkincare.Models.ViewModels
{
    public class CheckoutViewModel
    {
        public string Email { get; set; }
        public string Uname { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string PaymentMethod { get; set; }

        // Các thuộc tính liên quan đến giỏ hàng
        public decimal ProvisionalAmount { get; set; } // Tạm tính  
        public decimal ShippingFee { get; set; } // Phí ship        
        public decimal? DiscountValue { get; set; } // Giảm giá      
        public decimal GrandTotal { get; set; } // Tổng cộng 

        public List<CartItem> CartItems { get; set; }
        public decimal CartTotal { get; set; }  // Thêm thuộc tính CartTotal

        public decimal ProvisionalTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public virtual Product Product { get; set; }
        public virtual ProductVariant ProductVariant { get; set; }
    }
}