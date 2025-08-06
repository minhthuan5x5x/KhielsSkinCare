namespace KhielsSkincare.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryType { get; set; }
        public string CategorySkin { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
