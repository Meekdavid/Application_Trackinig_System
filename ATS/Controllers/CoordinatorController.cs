using ATS.Database;
using ATS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ATS.Controllers
{
    [Authorize(Roles = "Coordinator")]
    public class CoordinatorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CoordinatorController> _logger;

        public CoordinatorController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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

        public async Task<IActionResult> ShortlistedCandidates()
        {
            var shortlistedCandidates = _context.Applications
                                       .Where(a => a.IsShortlisted)
                                       .Include(a => a.JobPost)
                                       .Include(a => a.Candidate)
                                       .ToList();
            return View(shortlistedCandidates);
        }

        public async Task<IActionResult> Dashboard()
        {
            var jobs = await _context.JobPosts.Include(j => j.CreatedBy).Where(j => !j.IsApproved).ToListAsync();
            string approvedCount = _context.JobPosts.Include(j => j.CreatedBy).Where(j => j.IsApproved).Count().ToString();
            TempData["Approved"] = approvedCount;
           
            return View(jobs);
        }

        public async Task<IActionResult> ViewAllJobs()
        {
            var jobs = await _context.JobPosts.Include(j => j.CreatedBy).ToListAsync();
            return View(jobs);
        }

        [HttpGet]
        public async Task<IActionResult> ApproveJob(string id)
        {
            var job = await _context.JobPosts.FindAsync(id);
            var existingAssignment = await _context.JobPostRecruiters
                                .FirstOrDefaultAsync(jr => jr.JobPostId == id);

            if(existingAssignment == null)
            {
                TempData["Error"] = "A recruiter must be assigned";
                return RedirectToAction(nameof(Dashboard));
            }

            if (job != null)
            {
                TempData["Success"] = "Job Sucessfully Approved";
                job.IsApproved = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Dashboard));
        }

        public async Task<IActionResult> AssignRecruiter(string id)
        {
            var job = await _context.JobPosts.FindAsync(id);
            if (job != null)
            {
                var recruiters = await _userManager.GetUsersInRoleAsync("Recruiter");
                var viewModel = new AssignRecruiterViewModel
                {
                    JobPostId = id,
                    Recruiters = new MultiSelectList(recruiters, "Id", "FullName")
                };
                TempData["Success"] = "Recruiter Assigned Sucessfully";
                return View(viewModel);
            }
            TempData["Error"] = "No Jobs Found";
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> AssignRecruiter(AssignRecruiterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var job = await _context.JobPosts.FindAsync(model.JobPostId);
                if (job != null)
                {
                    foreach (var recruiterId in model.SelectedRecruiterIds)
                    {
                        var recruiter = await _userManager.FindByIdAsync(recruiterId);
                        if (recruiter != null)
                        {
                            var existingAssignment = await _context.JobPostRecruiters
                                .FirstOrDefaultAsync(jr => jr.JobPostId == model.JobPostId && jr.RecruiterId == recruiterId);

                            if (existingAssignment == null)
                            {
                                var jobPostRecruiter = new JobPostRecruiter
                                {
                                    JobPostId = model.JobPostId,
                                    RecruiterId = recruiterId
                                };
                                _context.JobPostRecruiters.Add(jobPostRecruiter);
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Recruiter assigned successfully.";
                    return RedirectToAction(nameof(Dashboard));
                }
                ModelState.AddModelError("", "Job post not found.");
            }

            // Re-populate the recruiters list in case of an error
            TempData["Error"] = "Some input fields are not valid.";
            var recruiters = await _userManager.GetUsersInRoleAsync("Recruiter");
            model.Recruiters = new MultiSelectList(recruiters, "Id", "FullName");

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRecruitersAndR2Questions(AssignRecruiterViewModel model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            var job = await _context.JobPosts.FindAsync(model.JobPostId);
            if (job != null)
            {
                // Assign Recruiters
                foreach (var recruiterId in model.SelectedRecruiterIds)
                {
                    var recruiter = await _userManager.FindByIdAsync(recruiterId);
                    
                    if (recruiter != null)
                    {
                        var existingAssignment = await _context.JobPostRecruiters
                            .FirstOrDefaultAsync(jr => jr.JobPostId == model.JobPostId && jr.RecruiterId == recruiterId);

                        if (existingAssignment == null)
                        {
                            var jobPostRecruiter = new JobPostRecruiter
                            {
                                JobPostRecruiterId = new Random().Next(00000, 99999).ToString(),
                                JobPostId = model.JobPostId,
                                RecruiterId = recruiterId
                            };
                            await _context.JobPostRecruiters.AddAsync(jobPostRecruiter);
                        }
                    }
                }

                foreach (R2Response r2Model in model.R2Questions)
                {
                    var r2Responses = new MainR2Questions
                    {
                        JobPostId = model.JobPostId,
                        Question = r2Model.Question,
                    };
                    await _context.JobPostQuestions.AddAsync(r2Responses);
                }                

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Recruiters assigned and R2 questions added successfully.";
                return RedirectToAction(nameof(Dashboard));
            }
            else
            {
                TempData["Error"] = "Job not Found";
                ModelState.AddModelError(string.Empty, "Job not found.");
            }

            // If we reach here, something went wrong, so return to the view with errors
            // Populate recruiters list again to display in case of validation errors
            var recruiters = await _userManager.GetUsersInRoleAsync("Recruiter");
            model.Recruiters = new MultiSelectList(recruiters.Select(r => new SelectListItem
            {
                Value = r.Id,
                Text = r.UserName
            }), "Value", "Text");

            return View(model);
        }

    }
}
