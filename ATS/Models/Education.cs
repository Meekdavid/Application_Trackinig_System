namespace ATS.Models
{
    public class Education
    {
        public int Id { get; set; }
        public string? Institution { get; set; }
        public string? Degree { get; set; }
        public string? FieldOfStudy { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Foreign key to ApplicationUser
        public string? UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
