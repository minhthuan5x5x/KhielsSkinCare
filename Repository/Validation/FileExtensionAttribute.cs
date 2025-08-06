using System.ComponentModel.DataAnnotations;

namespace KhielsSkincare.Repository.Validation
{
    public class FileExtensionAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(value is FormFile file)
            {
                var extension=Path.GetExtension(file.FileName);
                string[] extensions = { "jpg", "pnj" };

                bool result = extensions.Any(x => extension.EndsWith(x));

                if (!result)
                {
                    return new ValidationResult("Allow extensions are jpg or pnj");
                }
            }
            return ValidationResult.Success;
        }
    }
}
