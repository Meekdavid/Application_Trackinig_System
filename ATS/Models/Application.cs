using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ATS.Models
{
    public class Application
    {
        [Key]
        public int ApplicationId { get; set; }

        [Required]
        public int? JobPostId { get; set; }
        public JobPost JobPost { get; set; }

        [Required]
        public string? CandidateId { get; set; } = " .";
        public ApplicationUser Candidate { get; set; }

        [Required]
        public string? ResumeBase64 { get; set; }
        public string? R1Response1 { get; set; }
        public string? R1Response2 { get; set; }
        public string? R1Response3 { get; set; }
        public string? R1Response4 { get; set; }
        public string? R1Response5 { get; set; }

        public bool IsShortlisted { get; set; }
        public bool IsScreenedOut { get; set; }

        // Additional applicant information
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Education { get; set; }
        public string? Experience { get; set; }
        //public List<Skill> Skills { get; set; } = new List<Skill>();
        public List<R2Response> R2Responses { get; set; }
        //public List<string> R2Responses { get; set; }

        public Application()
        {
            R2Responses = new List<R2Response>();
        }

    }
}
