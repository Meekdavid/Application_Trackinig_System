using System.ComponentModel.DataAnnotations;

namespace ATS.Models
{
    public class MainR2Questions
    {
        [Key]
        public int Id { get; set; }
        public string? Question { get; set; }
        public string? JobPostId { get; set; }
        public JobPost JobPost { get; set; }
    }
}
