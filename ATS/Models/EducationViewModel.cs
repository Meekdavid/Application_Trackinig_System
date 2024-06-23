using ATS.Helpers.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace ATS.Models
{
    public class EducationViewModel
    {
        [Required(ErrorMessage = "Institution is required.")]
        [MaxLength(50, ErrorMessage = "Institution name cannot exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Institution name can only contain letters, numbers, and spaces.")]
        public string Institution { get; set; }

        [Required(ErrorMessage = "Degree is required.")]
        [MaxLength(50, ErrorMessage = "Degree name cannot exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Degree name can only contain letters, numbers, and spaces.")]
        public string Degree { get; set; }

        [Required(ErrorMessage = "Field of study is required.")]
        [MaxLength(50, ErrorMessage = "Field of study cannot exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Field of study can only contain letters, numbers, and spaces.")]
        public string FieldOfStudy { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        [DataType(DataType.Date)]
        [GreaterThan(nameof(StartDate), ErrorMessage = "End Date must be greater than Start Date.")]
        public DateTime EndDate { get; set; }
    }
}
