using System.ComponentModel.DataAnnotations;

namespace ATS.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [Display(Name = "Preferred Language")]
        public string PreferredLanguage { get; set; }
        public string Country { get; set; }
        public string Bio { get; set; }
        public string? Address { get; set; } = " .";
        public string? AgencyName { get; set; } = " .";
        public List<EducationViewModel>? Educations { get; set; } = new List<EducationViewModel>();
        public List<ExperienceViewModel>? Experiences { get; set; } = new List<ExperienceViewModel>();
        [Display(Name = "Profile Picture")]
        public IFormFile ProfilePicture { get; set; }
        // List of Skills
        public List<Skill>? Skills { get; set; } = new List<Skill>();

        [Required]
        [Display(Name = "Register As")]
        public string? Role { get; set; }

        // Fields for Employer role
        public string? CompanyName { get; set; } = " .";

        public string? Industry { get; set; } = " .";
    }

}
