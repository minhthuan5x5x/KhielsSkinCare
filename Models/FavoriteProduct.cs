namespace KhielsSkincare.Models
{
    public class FavoriteProduct
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int ProductVariantId { get; set; }
        public virtual AppUser User { get; set; }
        public virtual Product Product { get; set; }
        public virtual ProductVariant ProductVariant { get; set; }
    }
}
