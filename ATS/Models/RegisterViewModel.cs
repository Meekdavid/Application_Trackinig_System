using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ATS.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full Name is required.")]
        [Display(Name = "Full Name")]
        [StringLength(50, ErrorMessage = "Full Name must not exceed {1} characters.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(30, ErrorMessage = "Email address must not exceed {1} characters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be between {2} and {1} characters.")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(40, ErrorMessage = "Phone Number must not exceed {1} characters.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Preferred Language")]
        [StringLength(50, ErrorMessage = "Preferred Language must not exceed {1} characters.")]
        public string PreferredLanguage { get; set; }

        [StringLength(100, ErrorMessage = "Country must not exceed {1} characters.")]
        public string Country { get; set; }

        [StringLength(500, ErrorMessage = "Bio must not exceed {1} characters.")]
        public string Bio { get; set; }

        [StringLength(50, ErrorMessage = "Address must not exceed {1} characters.")]
        public string Address { get; set; } = " .";

        [StringLength(40, ErrorMessage = "Agency Name must not exceed {1} characters.")]
        public string AgencyName { get; set; } = " .";

        public List<EducationViewModel> Educations { get; set; } = new List<EducationViewModel>();

        public List<ExperienceViewModel> Experiences { get; set; } = new List<ExperienceViewModel>();
        public List<Skill> Skills { get; set; } = new List<Skill>();

        [Display(Name = "Profile Picture")]
        public IFormFile ProfilePicture { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        [Display(Name = "Register As")]
        public string Role { get; set; }

        [StringLength(40, ErrorMessage = "Company Name must not exceed {1} characters.")]
        public string CompanyName { get; set; } = " .";

        [StringLength(40, ErrorMessage = "Industry must not exceed {1} characters.")]
        public string Industry { get; set; } = " .";
    }
}
