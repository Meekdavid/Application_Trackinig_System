namespace ATS.Models
{
    public class JobPostRecruiter
    {
        public string JobPostRecruiterId { get; set; }
        public string JobPostId { get; set; }
        public string RecruiterId { get; set; }

        public JobPost JobPost { get; set; }
        public ApplicationUser Recruiter { get; set; }
    }

}
