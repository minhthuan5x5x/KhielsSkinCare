using KhielsSkincare.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhielsSkincare.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên sản phẩm không được vượt quá 100 ký tự.")]
        public string ProductName { get; set; }
        [Required(ErrorMessage = "Brand is required.")]
        [StringLength(100, ErrorMessage = "Brand can't be longer than 100 characters.")]
        public string Brand { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }
        public virtual ICollection<FavoriteProduct> FavoriteProducts { get; set; }
        public int CategoryId { get; set; }
        public string ImageUrl { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public string Image4 { get; set; }
        public string Image5 { get; set; }

        public string SkinType { get; set; }
        public string SkinCondition { get; set; } 

        public virtual Category Categories { get; set; }
        public virtual ICollection<ProductVariant> ProductVariants { get; set; }
        public ICollection<Review> Reviews { get; set; } 

        [NotMapped]
        [FileExtension]
        public IFormFile ImageUpload { get; set; } // Hình ảnh chính

        [NotMapped]
        [FileExtension]
        public IFormFile ImageUpload1 { get; set; }
        [NotMapped]
        [FileExtension]
        public IFormFile ImageUpload2 { get; set; }
        [NotMapped]
        [FileExtension]
        public IFormFile ImageUpload3 { get; set; }
        [NotMapped]
        [FileExtension]
        public IFormFile ImageUpload4 { get; set; }
        [NotMapped]
        [FileExtension]
        public IFormFile ImageUpload5 { get; set; }
    }

}
