namespace KhielsSkincare.Models
{
    public class CartItem
    {
        public int ProductId { get; set; } 
        public string ProductName { get; set; } 
        public int ProductVariantId { get; set; } 
        public string Size { get; set; } 
        public decimal Price { get; set; } 
        public int Quantity { get; set; } 
        public decimal Total { 
            get { return Quantity * Price; }
        }
        public string Image { get; set; }

        
        public CartItem() { }

        
        public CartItem(Product product, ProductVariant variant)
        {
            ProductId = product.ProductId;
            ProductName = product.ProductName;
            ProductVariantId = variant.ProductVariantId;
            Size = variant.Size;
            Price = variant.Price;
            Quantity = 1; 
            Image = product.ImageUrl; 
        }
      
    }
}
