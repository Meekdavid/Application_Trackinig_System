using Microsoft.AspNetCore.Identity;

namespace ATS.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = "";
        public string ProfilePictureBase64 { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string PreferredLanguage { get; set; } = "";
        public string Country { get; set; } = "";
        public string Bio { get; set; } = "";
        public string Address { get; set; } = "";
        public string Education { get; set; } = "";
        public string Experience { get; set; } = "";
        public bool IsActive { get; set; }
        public string Role { get; set; }
        public string CompanyName { get; set; } = "";
        public string Industry { get; set; } = "";
        public string AgencyName { get; set; } = "";
        public DateTime RegistrationDate { get; set; }
        public List<Skill> Skills { get; set; } = new List<Skill>();
        // Navigation properties
        public ICollection<JobPost> JobPosts { get; set; }
        public ICollection<Education> Educations { get; set; }
        public ICollection<Experience> Experiences { get; set; }
    }

}
