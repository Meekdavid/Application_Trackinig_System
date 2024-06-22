using ATS.Database;
using ATS.Helpers;
using ATS.Models;
using ATS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ATS.Controllers
{
    [Authorize]
    public class JobPostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public JobPostController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> Dashboard()
        {
            var jobs = await _context.JobPosts.Include(j => j.CreatedBy).Where(j => !j.IsApproved).ToListAsync();
            return View(jobs);
        }

        [HttpPost]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> ApproveJob(int id)
        {
            var job = await _context.JobPosts.FindAsync(id);
            if (job != null)
            {
                job.IsApproved = true;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Job approved successfully.";
            }
            return RedirectToAction(nameof(Dashboard));
        }

        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> AssignRecruiter(int jobId)
        {
            var job = await _context.JobPosts.FindAsync(jobId);
            if (job == null)
            {
                return RedirectToAction("");
            }

            var recruiters = await _userManager.GetUsersInRoleAsync("Recruiter");

            var recruiterList = recruiters.Select(r => new SelectListItem
            {
                Value = r.Id,
                Text = r.FullName
            }).ToList();

            var model = new AssignRecruiterViewModel
            {
                JobPostId = job.JobPostId,
                JobTitle = job.JobTitle,
                Recruiters = new MultiSelectList(recruiterList, "Value", "Text")
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Coordinator")]
        [ValidateAntiForgeryToken]
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

            var recruiters = await _userManager.GetUsersInRoleAsync("Recruiter");
            var recruiterList = recruiters.Select(r => new SelectListItem
            {
                Value = r.Id,
                Text = r.FullName
            }).ToList();

            model.Recruiters = new MultiSelectList(recruiterList, "Value", "Text");
            TempData["Error"] = "Unable to create Job, Contact Support";
            return View(model);
        }

        //[Authorize(Roles = "Candidate")]
        //public async Task<IActionResult> Apply(int jobId)
        //{
        //    var job = await _context.JobPosts.FindAsync(jobId);
        //    if (job != null)
        //    {
        //        var model = new ApplicationViewModel
        //        {
        //            JobPostId = job.JobPostId,
        //            JobTitle = job.JobTitle,
        //            R1CheckQuestions = job.R2Questions
        //        };
        //        return View(model);
        //    }
        //    return View();
        //}


        //[HttpPost]
        //[Authorize(Roles = "Candidate")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Apply(ApplicationViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userManager.GetUserAsync(User);
        //        var resumeBase64 = ConvertFileToBase64(model.Resume);
        //        var application = new Application
        //        {
        //            JobPostId = model.JobPostId,
        //            CandidateId = user.Id,
        //            ResumeBase64 = resumeBase64,
        //            R1Response1 = model.R1Response1,
        //            R1Response2 = model.R1Response2,
        //            R1Response3 = model.R1Response3,
        //            R1Response4 = model.R1Response4,
        //            R1Response5 = model.R1Response5,
        //            IsShortlisted = false
        //        };
        //        _context.Applications.Add(application);
        //        await _context.SaveChangesAsync();
        //        TempData["Success"] = "Application submitted successfully.";
        //        return RedirectToAction("Index", "Home");
        //    }
        //    return View(model);
        //}

        private string ConvertFileToBase64(IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileBytes = ms.ToArray();
                return Convert.ToBase64String(fileBytes);
            }
        }
    }
}
