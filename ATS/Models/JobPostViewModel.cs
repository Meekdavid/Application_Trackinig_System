using System.ComponentModel.DataAnnotations;

namespace ATS.Models
{
    public class JobPostViewModel
    {
        public int JobPostId { get; set; }

        [Required(ErrorMessage = "Job Title is required.")]
        [MaxLength(40, ErrorMessage = "Job Title cannot exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Job Title can only contain letters, numbers, and spaces.")]
        public string JobTitle { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [MaxLength(40, ErrorMessage = "Location cannot exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Location can only contain letters, numbers, and spaces.")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Salary is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive number.")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Responsibilities are required.")]
        [MaxLength(1000, ErrorMessage = "Responsibilities cannot exceed 1000 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,!?]*$", ErrorMessage = "Responsibilities can only contain letters, numbers, spaces, and basic punctuation.")]
        public string Responsibilities { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(5000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,!?]*$", ErrorMessage = "Description can only contain letters, numbers, spaces, and basic punctuation.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Requirements are required.")]
        [MaxLength(1000, ErrorMessage = "Requirements cannot exceed 1000 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,!?]*$", ErrorMessage = "Requirements can only contain letters, numbers, spaces, and basic punctuation.")]
        public string Requirements { get; set; }

        [Required(ErrorMessage = "Employment Type is required.")]
        [MaxLength(40, ErrorMessage = "Employment Type cannot exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Employment Type can only contain letters and spaces.")]
        public string EmploymentType { get; set; }

        [Required(ErrorMessage = "Company Name is required.")]
        [MaxLength(40, ErrorMessage = "Company Name cannot exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Company Name can only contain letters, numbers, and spaces.")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Industry is required.")]
        [MaxLength(40, ErrorMessage = "Industry cannot exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Industry can only contain letters and spaces.")]
        public string Industry { get; set; }

        [Required(ErrorMessage = "Experience Level is required.")]
        [MaxLength(40, ErrorMessage = "Experience Level cannot exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Experience Level can only contain letters and spaces.")]
        public string ExperienceLevel { get; set; }

        [Required(ErrorMessage = "Education Level is required.")]
        [MaxLength(40, ErrorMessage = "Education Level cannot exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Education Level can only contain letters and spaces.")]
        public string EducationLevel { get; set; }

        [MaxLength(30, ErrorMessage = "R1 Check Question cannot exceed 200 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,!?]*$", ErrorMessage = "R1 Check Question can only contain letters, numbers, spaces, and basic punctuation.")]
        public string R1CheckQuestion1 { get; set; }
        public bool R1CheckAnswer1 { get; set; }

        [MaxLength(30, ErrorMessage = "R1 Check Question cannot exceed 200 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,!?]*$", ErrorMessage = "R1 Check Question can only contain letters, numbers, spaces, and basic punctuation.")]
        public string R1CheckQuestion2 { get; set; }
        public bool R1CheckAnswer2 { get; set; }

        [MaxLength(30, ErrorMessage = "R1 Check Question cannot exceed 200 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,!?]*$", ErrorMessage = "R1 Check Question can only contain letters, numbers, spaces, and basic punctuation.")]
        public string R1CheckQuestion3 { get; set; }
        public bool R1CheckAnswer3 { get; set; }

        [MaxLength(30, ErrorMessage = "R1 Check Question cannot exceed 200 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,!?]*$", ErrorMessage = "R1 Check Question can only contain letters, numbers, spaces, and basic punctuation.")]
        public string R1CheckQuestion4 { get; set; }
        public bool R1CheckAnswer4 { get; set; }

        [MaxLength(30, ErrorMessage = "R1 Check Question cannot exceed 200 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,!?]*$", ErrorMessage = "R1 Check Question can only contain letters, numbers, spaces, and basic punctuation.")]
        public string R1CheckQuestion5 { get; set; }
        public bool R1CheckAnswer5 { get; set; }
    }
}
