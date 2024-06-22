using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ATS.Helpers.Attributes
{
    public class NoSpecialCharactersAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var input = value.ToString();
                if (Regex.IsMatch(input, @"[^a-zA-Z0-9\s]"))
                {
                    return new ValidationResult("Input contains invalid characters.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
