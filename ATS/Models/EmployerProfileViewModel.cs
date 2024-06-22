using ATS.Models;
using System.Collections.Generic;

namespace ATS.ViewModels
{
    public class EmployerProfileViewModel
    {
        public ApplicationUser ApplicationUser { get; set; }
        public List<JobPost> JobPosts { get; set; }
    }
}
