using System.ComponentModel.DataAnnotations;

namespace Hotel.Validation
{
    public class DOBValidation:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (Convert.ToDateTime(value) >= DateTime.Now)
            {
                return new ValidationResult("DOB should be less than curent date");
            }
            return ValidationResult.Success;
        }
    }
}
