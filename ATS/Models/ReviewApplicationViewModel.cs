using System.ComponentModel.DataAnnotations;

namespace ATS.Models
{
    public class ReviewApplicationViewModel
    {
        public int ApplicationId { get; set; }

        public List<R2Response> R2Questions { get; set; }

        [Required(ErrorMessage = "Please provide a response for each question.")]
        public List<R2Response> R2Responses { get; set; }
    }
}
