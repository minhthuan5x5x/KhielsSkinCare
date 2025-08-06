namespace KhielsSkincare.Models.ViewModels
{
    public class CartViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal CartTotal { get; set; }  // Thêm thuộc tính CartTotal

        public decimal ProvisionalTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public virtual Product Product { get; set; }
        public virtual ProductVariant ProductVariant { get; set; }
    }
}
