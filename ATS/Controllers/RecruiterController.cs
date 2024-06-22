using ATS.Database;
using ATS.Helpers;
using ATS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ATS.Controllers
{
    [Authorize(Roles = "Recruiter")]
    public class RecruiterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecruiterController(ApplicationDbContext context)
        {
            _context = context;
        }

        //public IActionResult Dashboard()
        //{
        //    var application = _context.Applications
        //                    .Include(a => a.JobPost)
        //                    .Include(a => a.Candidate);

        //    TempData["Total"] = application.Where(a => !a.IsShortlisted && !a.IsScreenedOut).Count().ToString();
        //    TempData["Shortlisted"] = application.Where(a => a.IsShortlisted && !a.IsScreenedOut).Count().ToString();
        //    TempData["Pending"] = application.Where(a => !a.IsShortlisted && !a.IsScreenedOut).Count().ToString();
        //    TempData["ScreenedOut"] = application.Where(a => a.IsScreenedOut).Count().ToString();

        //    return View(application
        //                    .Where(a => !a.IsShortlisted && !a.IsScreenedOut)
        //                    .FirstOrDefault());
        //}

        public async Task<IActionResult> Dashboard()
        {
            // Fetch the logged-in recruiter's user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get the job post IDs assigned to this recruiter
            var assignedJobPostIds = await _context.JobPostRecruiters
                                                   .Where(jr => jr.RecruiterId == userId)
                                                   .Select(jr => jr.JobPostId)
                                                   .ToListAsync();

            // Filter the applications based on the job posts assigned to the recruiter
            var applications = await _context.Applications
                                             .Where(a => assignedJobPostIds.Contains((int)a.JobPostId))
                                             .Include(a => a.JobPost)
                                             .Include(a => a.Candidate)
                                             .ToListAsync();

            // Calculate statistics
            TempData["Total"] = applications.Where(a => !a.IsShortlisted && !a.IsScreenedOut).Count().ToString();
            TempData["Shortlisted"] = applications.Where(a => a.IsShortlisted && !a.IsScreenedOut).Count().ToString();
            TempData["Pending"] = applications.Where(a => !a.IsShortlisted && !a.IsScreenedOut).Count().ToString();
            TempData["ScreenedOut"] = applications.Where(a => a.IsScreenedOut).Count().ToString();

            return View(applications.Where(a => !a.IsShortlisted && !a.IsScreenedOut).FirstOrDefault());
        }


        public IActionResult ViewResume(int id)
        {
            var application = _context.Applications.Find(id);
            if (application == null)
            {
                return View();
            }

            var fileBytes = FileHelpers.ConvertFromBase64(application.ResumeBase64);
            return File(fileBytes, "application/pdf");
        }

        public IActionResult ReviewApplication(int id)
        {
            var application = _context.Applications
                                       .Include(a => a.JobPost)
                                       .Include(a => a.Candidate)
                                       .FirstOrDefault(a => a.ApplicationId == id);

            if (application == null)
            {
                return View();
            }

            var r2Questions = application.JobPost.R2Questions;

            var viewModel = new ReviewApplicationViewModel
            {
                ApplicationId = application.ApplicationId,
                R2Questions = r2Questions,
                R2Responses = r2Questions.Select(q => new R2Response
                {
                    ApplicationId = application.ApplicationId,
                    Question = q.Question,
                    Response = string.Empty // Initialize with empty response
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReviewApplication(ReviewApplicationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var application = await _context.Applications
                                                .Include(a => a.R2Responses)
                                                .FirstOrDefaultAsync(a => a.ApplicationId == model.ApplicationId);

                if (application == null)
                {
                    return View();
                }

                foreach (var r2Response in model.R2Responses)
                {
                    var existingResponse = application.R2Responses
                                                      .FirstOrDefault(r => r.Id == r2Response.Id);

                    if (existingResponse != null)
                    {
                        existingResponse.Response = r2Response.Response;
                    }
                    else
                    {
                        application.R2Responses.Add(new R2Response
                        {
                            ApplicationId = r2Response.ApplicationId,
                            Question = r2Response.Question,
                            Response = r2Response.Response
                        });
                    }
                }

                application.IsShortlisted = true;

                TempData["SuccessMessage"] = "Application reviewed successfully!";
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Dashboard));
            }

            // If ModelState is not valid, return the view with errors
            TempData["Error"] = "Some Input Fields are not Valid!";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Shortlist(int id)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application != null)
            {
                application.IsShortlisted = true;
                await _context.SaveChangesAsync();
            }
            TempData["SuccessMessage"] = "Candidate Shortlisted successfully!";
            return RedirectToAction(nameof(Dashboard));
        }
    }
}
