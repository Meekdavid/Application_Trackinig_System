namespace ATS.Models
{
    public class R2Response
    {
        public int Id { get; set; }
        public int? ApplicationId { get; set; } = 1;// Foreign key to Application
        public string? Question { get; set; } = " .";
        public string? Response { get; set; } = " .";

        // Navigation property
        public Application Application { get; set; } = new Application();

        // Foreign key to JobPost
        public int? JobPostId { get; set; } = 1;
        public JobPost JobPost { get; set; } = new JobPost();
    }
}
