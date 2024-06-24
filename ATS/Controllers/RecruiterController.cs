using ATS.Database;
using ATS.Helpers;
using ATS.Helpers.Attributes;
using ATS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ATS.Controllers
{
    [CustomAuthorize("Coordinator,Recruiter")]
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
            try
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

                if (viewModel != null)
                {
                    var r2Questions = await _context.JobPostQuestions.Where(a => a.JobPostId == viewModel.JobPostId).ToListAsync();

                    // Use Select to create a new list with mapped values
                    viewModel.JobPost.R2Questions = viewModel.R2Responses = r2Questions.Select(q => new R2Response
                    {
                        ApplicationId = viewModel.ApplicationId,
                        JobPostId = viewModel.JobPostId,
                        Question = q.Question
                    }).ToList();
                }

                // Calculate statistics
                TempData["Total"] = applications.Where(a => !a.IsShortlisted && !a.IsScreenedOut).Count().ToString();
                TempData["Shortlisted"] = applications.Where(a => a.IsShortlisted && !a.IsScreenedOut).Count().ToString();
                TempData["Pending"] = applications.Where(a => !a.IsShortlisted && !a.IsScreenedOut).Count().ToString();
                TempData["ScreenedOut"] = applications.Where(a => a.IsScreenedOut).Count().ToString();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An Error Occured.";
                @ViewBag.ErrorMessage = ex.Message;
                @ViewBag.StackTrace = ex.StackTrace;
                return RedirectToAction("Error", "Home");
            }
            
        }


        public IActionResult ViewResume(int id)
        {
            try
            {
                var application = _context.Applications.Find(id);
                if (application == null)
                {
                    return View();
                }

                var fileBytes = FileHelpers.ConvertFromBase64(application.ResumeBase64);
                return File(fileBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An Error Occured.";
                @ViewBag.ErrorMessage = ex.Message;
                @ViewBag.StackTrace = ex.StackTrace;
                return RedirectToAction("Error", "Home");
            }
            
        }

        public async Task<IActionResult> ViewCandidateDetails(string id)
        {
            try
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
            catch (Exception ex)
            {
                TempData["Error"] = "An Error Occured.";
                @ViewBag.ErrorMessage = ex.Message;
                @ViewBag.StackTrace = ex.StackTrace;
                return RedirectToAction("Error", "Home");
            }
            
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
            try
            {
                var application = await _context.Applications
                                            .Include(a => a.R2Responses)
                                            .FirstOrDefaultAsync(a => a.ApplicationId == model.ApplicationId);


                if (application == null)
                {
                    TempData["Error"] = "Unable to retrieve application details";
                    return RedirectToAction(nameof(Dashboard));
                }

                // Ensure JobPostId and ApplicationId are correctly set
                var jobPost = await _context.JobPosts.FindAsync(model.JobPostId);

                if (jobPost == null)
                {
                    TempData["Error"] = "Job Post Not Found";
                    return RedirectToAction(nameof(Dashboard));
                }
                application.IsShortlisted = true;

                // Transform the collection using LINQ and then add to the context
                var responsesToAdd = model.R2Responses.Select(response => new R2Response
                {
                    ApplicationId = model.ApplicationId,
                    JobPostId = model.JobPostId,
                    Question = response.Question,
                    Response = response.Response,
                    Application = application,
                    JobPost = jobPost
                }).ToList();
                jobPost.R2Questions = responsesToAdd;
                application.R2Responses = responsesToAdd;
                application.JobPost = jobPost;

                _context.R2Details.AddRange(responsesToAdd);

                TempData["SuccessMessage"] = "Application shortlisted successfully!";
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Dashboard));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An Error Occured.";
                @ViewBag.ErrorMessage = ex.Message;
                @ViewBag.StackTrace = ex.StackTrace;
                return RedirectToAction("Error", "Home");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Shortlist(int id)
        {
            try
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
            catch (Exception ex)
            {
                TempData["Error"] = "An Error Occured.";
                @ViewBag.ErrorMessage = ex.Message;
                @ViewBag.StackTrace = ex.StackTrace;
                return RedirectToAction("Error", "Home");
            }
            
        }
    }
}
