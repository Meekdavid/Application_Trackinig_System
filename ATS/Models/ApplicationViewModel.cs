using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ATS.Models
{
    public class ApplicationViewModel
    {
        public string JobPostId { get; set; }

        [MaxLength(100, ErrorMessage = "Job title cannot exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Job title can only contain letters, numbers, and spaces.")]
        public string? JobTitle { get; set; } = ".";

        [Required(ErrorMessage = "Resume is required.")]
        public IFormFile? Resume { get; set; }

        public List<JobPost>? MatchingJobs { get; set; } = new List<JobPost>();

        public JobPost? thisJob { get; set; } = new JobPost();

        [Required(ErrorMessage = "Response 1 is required.")]
        [MaxLength(10, ErrorMessage = "Response cannot exceed 200 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Response can only contain letters, numbers, and spaces.")]
        public string R1Response1 { get; set; }

        [Required(ErrorMessage = "Response 2 is required.")]
        [MaxLength(10, ErrorMessage = "Response cannot exceed 200 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Response can only contain letters, numbers, and spaces.")]
        public string R1Response2 { get; set; }

        [Required(ErrorMessage = "Response 3 is required.")]
        [MaxLength(10, ErrorMessage = "Response cannot exceed 200 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Response can only contain letters, numbers, and spaces.")]
        public string R1Response3 { get; set; }

        [Required(ErrorMessage = "Response 4 is required.")]
        [MaxLength(10, ErrorMessage = "Response cannot exceed 200 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Response can only contain letters, numbers, and spaces.")]
        public string R1Response4 { get; set; }

        [Required(ErrorMessage = "Response 5 is required.")]
        [MaxLength(10, ErrorMessage = "Response cannot exceed 200 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Response can only contain letters, numbers, and spaces.")]
        public string R1Response5 { get; set; }

        public List<string> R1CheckQuestions { get; set; } = new List<string>();

        [MaxLength(15, ErrorMessage = "Skill cannot exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Skill can only contain letters, numbers, and spaces.")]
        public List<string> Skills { get; set; } = new List<string>();
    }
}
