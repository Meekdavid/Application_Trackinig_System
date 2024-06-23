using ATS.Database;
using ATS.Models;
using ATS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ATS.Controllers
{
    [Authorize(Roles = "Employer,Coordinator")]
    public class EmployerController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public EmployerController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Employer/Profile
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "An Error Occured";
                return RedirectToAction("Login", "Account");
            }

            var model = new EmployerProfileViewModel
            {
                ApplicationUser = user,
                JobPosts = await _context.JobPosts
                    .Where(jp => jp.CreatedBy.Id == user.Id)
                    .ToListAsync()
            };

            return View(model);
        }

        // GET: Employer/Dashboard
        [HttpGet]
        public async Task<IActionResult> ShortlistedDashboard()
        {
            var shortlistedCandidates = _context.Applications
                                       .Where(a => a.IsShortlisted)
                                       .Include(a => a.JobPost)
                                       .Include(a => a.Candidate)
                                       .ToList();
            return View("Dashboard", shortlistedCandidates);
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard(string id)
        {
            var shortlistedCandidates = _context.Applications
                                        .Where(a => a.IsShortlisted)
                                       .Include(a => a.JobPost).Where(a => a.JobPost.CreatedBy.Id == id)
                                       .Include(a => a.Candidate)
                                       .ToList();
            return View("Dashboard", shortlistedCandidates);
        }

        [Authorize(Roles = "Employer")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Create(JobPostViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var jobPost = new JobPost
                {
                    JobPostId = new Random().Next(000000, 99999).ToString(),
                    JobTitle = model.JobTitle,
                    Location = model.Location,
                    Salary = model.Salary,
                    Responsibilities = model.Responsibilities,
                    Description = model.Description,
                    Requirements = model.Requirements,
                    EmploymentType = model.EmploymentType,
                    CompanyName = model.CompanyName,
                    Industry = model.Industry,
                    ExperienceLevel = model.ExperienceLevel,
                    EducationLevel = model.EducationLevel,
                    DateCreated = DateTime.Now,
                    IsApproved = false,
                    CreatedById = user.Id,

                    //R1 Questions
                    R1CheckQuestion1 = model.R1CheckQuestion1,
                    R1CheckAnswer1 = model.R1CheckAnswer1,
                    R1CheckQuestion2 = model.R1CheckQuestion2,
                    R1CheckAnswer2 = model.R1CheckAnswer2,
                    R1CheckQuestion3 = model.R1CheckQuestion3,
                    R1CheckAnswer3 = model.R1CheckAnswer3,
                    R1CheckQuestion4 = model.R1CheckQuestion4,
                    R1CheckAnswer4 = model.R1CheckAnswer4,
                    R1CheckQuestion5 = model.R1CheckQuestion5,
                    R1CheckAnswer5 = model.R1CheckAnswer5,
                };

                TempData["Success"] = "Job Post Created Successfully";
                _context.JobPosts.Add(jobPost);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = "Unable to create Job, Contact Support";
            return View(model);
        }

        public IActionResult ViewCandidateDetails(string id)
        {
            var application = _context.Applications
                                      .Where(a => a.CandidateId == id)
                                      .Include(a => a.JobPost)
                                      .Include(a => a.Candidate)
                                      .FirstOrDefault();

            if (application == null)
            {
                return View();
            }

            var viewModel = new CandidateDetailsViewModel
            {
                Candidate = application,
                JobPost = application.JobPost,
                Application = application
            };

            return View(viewModel);
        }

        // GET: Employer/EmployerProfile/{employerId}
        [HttpGet]
        public async Task<IActionResult> EmployerProfile(string employerId)
        {
            var employer = await _userManager.Users
                .Include(u => u.JobPosts)
                .FirstOrDefaultAsync(u => u.Id == employerId);

            if (employer == null)
            {
                return View();
            }

            var model = new EmployerProfileViewModel
            {
                ApplicationUser = employer,
                JobPosts = employer.JobPosts.ToList()
            };

            return View("Profile", model);
        }
    }
}
