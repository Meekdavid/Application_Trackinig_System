namespace ATS.Models
{
    public class JobPostRecruiter
    {
        public int JobPostRecruiterId { get; set; }
        public int JobPostId { get; set; }
        public string RecruiterId { get; set; }

        public JobPost JobPost { get; set; }
        public ApplicationUser Recruiter { get; set; }
    }

}
