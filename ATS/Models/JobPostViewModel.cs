using System.ComponentModel.DataAnnotations;

namespace ATS.Models
{
    public class JobPostViewModel
    {
        public int JobPostId { get; set; }

        [Required]
        public string JobTitle { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public decimal Salary { get; set; }

        [Required]
        public string Responsibilities { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Requirements { get; set; }

        [Required]
        public string EmploymentType { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string Industry { get; set; }

        [Required]
        public string ExperienceLevel { get; set; }

        [Required]
        public string EducationLevel { get; set; }

        // R1 Check Questions
        public string R1CheckQuestion1 { get; set; }
        public bool R1CheckAnswer1 { get; set; }

        public string R1CheckQuestion2 { get; set; }
        public bool R1CheckAnswer2 { get; set; }

        public string R1CheckQuestion3 { get; set; }
        public bool R1CheckAnswer3 { get; set; }

        public string R1CheckQuestion4 { get; set; }
        public bool R1CheckAnswer4 { get; set; }

        public string R1CheckQuestion5 { get; set; }
        public bool R1CheckAnswer5 { get; set; }
    }


}
