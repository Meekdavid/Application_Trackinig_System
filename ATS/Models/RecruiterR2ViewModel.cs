using System.ComponentModel.DataAnnotations;

namespace ATS.Models
{
    public class RecruiterR2ViewModel
    {
        public int JobPostId { get; set; }

        public List<string> R2Questions { get; set; }

        [Required(ErrorMessage = "Please provide answers for all R2 questions.")]
        public List<bool?> R2Answers { get; set; }
    }
}
