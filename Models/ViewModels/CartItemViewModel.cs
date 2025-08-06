namespace KhielsSkincare.Models.ViewModels
{
    public class CartItemViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public decimal GrandTotal { get; set; }

        public string Email { get; set; }
        public string UserName { get; set; }

    }
}
