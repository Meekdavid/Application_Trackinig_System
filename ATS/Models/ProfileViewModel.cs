using System.ComponentModel.DataAnnotations;

namespace ATS.Models
{
    public class ProfileViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string PreferredLanguage { get; set; }
        public string Country { get; set; }
        public string Bio { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile ProfilePicture { get; set; }

        public string ProfilePictureBase64 { get; set; }
        public List<EducationViewModel> Educations { get; set; } = new List<EducationViewModel>();
        public List<ExperienceViewModel> Experiences { get; set; } = new List<ExperienceViewModel>();
        // List of Skills
        public List<Skill> Skills { get; set; } = new List<Skill>();
        public string ProfilePictureUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(ProfilePictureBase64))
                {
                    return $"data:image/jpeg;base64,{ProfilePictureBase64}";
                }
                return null;
            }
        }
    }

}
