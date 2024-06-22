namespace ATS.Models
{
    public class Experience
    {
        public int Id { get; set; }
        public string? Company { get; set; }
        public string? Position { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Foreign key to ApplicationUser
        public string? UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
