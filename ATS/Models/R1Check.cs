namespace ATS.Models
{
    public class R1Check
    {
        public int R1CheckId { get; set; }
        public int JobPostId { get; set; }
        public List<Question> questionChecks { get; set; }
        public JobPost JobPost { get; set; }
    }

    public class Question
    {
        public string QuestionTitle { get; set; }
        public string CorrectAnswer { get; set; }
    }

}
