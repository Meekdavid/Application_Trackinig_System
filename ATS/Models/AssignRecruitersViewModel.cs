using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ATS.Models
{
    public class AssignRecruiterViewModel
    {
        [Required(ErrorMessage = "Please select at least one recruiter.")]
        [Display(Name = "Recruiters")]
        public List<string> SelectedRecruiterIds { get; set; }

        [Required(ErrorMessage = "Please enter R2 questions.")]
        [Display(Name = "R2 Questions")]
        public List<R2Response> R2Questions { get; set; }

        public MultiSelectList? Recruiters { get; set; } = new MultiSelectList(new List<ApplicationUser>(), "Id", "FullName");

        [Required]
        public string JobPostId { get; set; }

        [Required(ErrorMessage = "Job title is required.")]
        [MaxLength(50, ErrorMessage = "Job title cannot exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Job title can only contain letters, numbers, and spaces.")]
        public string JobTitle { get; set; }

        public AssignRecruiterViewModel()
        {
            SelectedRecruiterIds = new List<string>();
            R2Questions = new List<R2Response>();
            JobTitle = ".";
        }
    }
}
