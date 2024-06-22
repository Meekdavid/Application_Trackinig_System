using System.ComponentModel.DataAnnotations;

namespace ATS.Models
{
    public class Skill
    {
        [Key]
        public int SkillId { get; set; }
        public string Name { get; set; }

        public int ApplicationId { get; set; }
        public Application Application { get; set; }
    }
}
