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
                                             .Include(a => a.JobPost)
                                             .Include(a => a.Candidate)
                                             .Where(a => assignedJobPostIds.Contains((a.JobPostId))).ToListAsync();

            //Retreive the R2 Questions
            var viewModel = applications.Where(a => !a.IsShortlisted && !a.IsScreenedOut).FirstOrDefault();
            var r2Questions = await _context.JobPostQuestions.Where(a => a.JobPostId == viewModel.JobPostId).ToListAsync();

            // Use Select to create a new list with mapped values
            viewModel.JobPost.R2Questions = viewModel.R2Responses = r2Questions.Select(q => new R2Response
            {
                ApplicationId = viewModel.ApplicationId,
                JobPostId = viewModel.JobPostId,
                Question = q.Question
            }).ToList();

            //viewModel.R2Responses = r2Questions.Select(q => new R2Response
            //{
            //    ApplicationId = viewModel.ApplicationId,
            //    JobPostId = viewModel.JobPostId,
            //    Question = q.Question
            //}).ToList();

            // Calculate statistics
            TempData["Total"] = applications.Where(a => !a.IsShortlisted && !a.IsScreenedOut).Count().ToString();
            TempData["Shortlisted"] = applications.Where(a => a.IsShortlisted && !a.IsScreenedOut).Count().ToString();
            TempData["Pending"] = applications.Where(a => !a.IsShortlisted && !a.IsScreenedOut).Count().ToString();
            TempData["ScreenedOut"] = applications.Where(a => a.IsScreenedOut).Count().ToString();

            return View(viewModel);
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

        public async Task<IActionResult> ViewCandidateDetails(string id)
        {
            var application = await _context.Applications
                                      .Where(a => a.CandidateId == id)
                                      .Include(a => a.JobPost)
                                      .Include(a => a.Candidate)
                                      //.Include(a => a.Experience)
                                      .FirstOrDefaultAsync();

            application.Candidate.Educations = await _context.Educations
                .Where(a => a.UserId == id).ToListAsync();

            application.Candidate.Experiences = await _context.Experiences
                .Where(a => a.UserId == id).ToListAsync();

            if (application == null)
            {
                TempData["Error"] = "Unable to Retreive Details";
                return View();
            }

            var viewModel = new CandidateDetailsViewModel
            {
                Candidate = application,
                JobPost = application.JobPost,
                Application = application
            };

            return View(application);
        }

        //public async Task<IActionResult> ReviewApplication(int id)
        //{
        //    var application = _context.Applications
        //                               .Include(a => a.JobPost)
        //                               .Include(a => a.Candidate)
        //                               .FirstOrDefault(a => a.ApplicationId == id);

        //    if (application == null)
        //    {
        //        return View();
        //    }

        //    var r2Questions = await _context.JobPostQuestions
        //        .Where(a => a.JobPostId == application.JobPostId).ToListAsync();

        //    var viewModel = new ReviewApplicationViewModel
        //    {
        //        ApplicationId = application.ApplicationId,
        //        R2Questions = r2Questions,
        //        R2Responses = r2Questions.Select(q => new R2Response
        //        {
        //            ApplicationId = application.ApplicationId,
        //            Question = q.Question,
        //            Response = string.Empty // Initialize with empty response
        //        }).ToList()
        //    };

        //    return View(viewModel);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReviewApplication(Application model)
        {
            var application = await _context.Applications
                                            .Include(a => a.R2Responses)
                                            .FirstOrDefaultAsync(a => a.ApplicationId == model.ApplicationId);

            if (application == null)
            {
                TempData["Error"] = "Unable to retrieve application details";
                return RedirectToAction(nameof(Dashboard));
            }

            model.JobPost = await _context.JobPosts
                .Include(a => a.CreatedBy)
                .Include(a => a.JobPostRecruiters)
                .Include(a => a.R2Questions)
                .FirstOrDefaultAsync(j => j.JobPostId == model.JobPostId);

            // Ensure the number of responses matches the number of questions
            if (model.JobPost.R2Questions != null && model.R2Responses != null)
            {
                for (int i = 0; i < model.JobPost.R2Questions.Count; i++)
                {
                    if (i < model.R2Responses.Count)
                    {
                        model.R2Responses[i].Question = model.JobPost.R2Questions[i].Question;
                    }
                }
            }
            

            _context.R2Details.AddRange(model.R2Responses);
            _context.Applications.Update(model);

            application.IsShortlisted = true;

            TempData["SuccessMessage"] = "Application shortlisted successfully!";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
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
