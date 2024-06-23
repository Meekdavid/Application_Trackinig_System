using ATS.Helpers.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace ATS.Models
{
    public class ExperienceViewModel
    {
        [Required(ErrorMessage = "Company is required.")]
        [MaxLength(50, ErrorMessage = "Company name cannot exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Company name can only contain letters, numbers, and spaces.")]
        public string Company { get; set; }

        [Required(ErrorMessage = "Position is required.")]
        [MaxLength(50, ErrorMessage = "Position cannot exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Position can only contain letters, numbers, and spaces.")]
        public string Position { get; set; }

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,!?]*$", ErrorMessage = "Description can only contain letters, numbers, spaces, and basic punctuation.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        [DataType(DataType.Date)]
        [GreaterThan(nameof(StartDate), ErrorMessage = "End Date must be greater than Start Date.")]
        public DateTime EndDate { get; set; }
    }
}
