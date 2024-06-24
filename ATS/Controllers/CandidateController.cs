using ATS.Database;
using ATS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ATS.Helpers.Attributes;

namespace ATS.Controllers
{
    public class CandidateController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public CandidateController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        //[Authorize(Roles = "Candidate")]
        [CustomAuthorize("Candidate")]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                // Retrieve current user
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return View();
                }

                // Retrieve profile picture from ApplicationUser's base64 stored image
                var profilePictureBase64 = user.ProfilePictureBase64;
                var profilePictureUrl = $"data:image/jpeg;base64,{profilePictureBase64}"; // Construct data URL
                user.ProfilePictureBase64 = profilePictureUrl;
                user.FullName = await GetSubstringBeforeFirstSpace(user.FullName);

                // Get available jobs
                var availableJobs = await _context.JobPosts.Include(j => j.CreatedBy).Where(j => j.IsApproved).ToListAsync();

                // Prepare view model
                var viewModel = new CandidateDashboardViewModel
                {
                    AvailableJobs = availableJobs,
                    Profile = user
                };

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

        [CustomAuthorize("Candidate")]
        public async Task<IActionResult> Apply(string jobPostId)
        {
            try
            {
                var jobPost = _context.JobPosts.Find(jobPostId);
                if (jobPost == null)
                {
                    return View();
                }

                var viewModel = new ApplicationViewModel
                {
                    JobPostId = jobPost.JobPostId,
                    JobTitle = jobPost.JobTitle,
                    R1CheckQuestions = new List<string>
                {
                    jobPost.R1CheckQuestion1,
                    jobPost.R1CheckQuestion2,
                    jobPost.R1CheckQuestion3,
                    jobPost.R1CheckQuestion4,
                    jobPost.R1CheckQuestion5
                }
                };

                viewModel.MatchingJobs = await _context.JobPosts.Where(j => j.JobTitle == jobPost.JobTitle
                || j.Industry == jobPost.Industry || j.CompanyName == jobPost.CompanyName).ToListAsync();

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

        public async Task<IActionResult> JobDetails(string id)
        {
            try
            {
                var jobPost = _context.JobPosts.Find(id);
                if (jobPost == null)
                {
                    TempData["Error"] = "No Job Posts Found";
                    return View();
                }


                var viewModel = new ApplicationViewModel
                {
                    thisJob = jobPost,
                    JobPostId = jobPost.JobPostId,
                    JobTitle = jobPost.JobTitle,
                    R1CheckQuestions = new List<string>
                {
                    jobPost.R1CheckQuestion1,
                    jobPost.R1CheckQuestion2,
                    jobPost.R1CheckQuestion3,
                    jobPost.R1CheckQuestion4,
                    jobPost.R1CheckQuestion5
                }
                };

                viewModel.MatchingJobs = await _context.JobPosts.Where(j => j.JobTitle == jobPost.JobTitle
                || j.Industry == jobPost.Industry || j.CompanyName == jobPost.CompanyName).ToListAsync();

                return View(jobPost);
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
        [CustomAuthorize("Candidate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(ApplicationViewModel model)
        {
            try
            {
                // Retrieve Job Details
                var jobPost = _context.JobPosts.Find(model.JobPostId);
                if (jobPost == null)
                {
                    TempData["Error"] = "Job not found.";
                    return RedirectToAction("Dashboard", "Candidate");
                }

                var user = await _userManager.GetUserAsync(User);

                // Check if the candidate has already applied for this job
                bool alreadyApplied = _context.Applications
                                              .Any(a => a.JobPostId == model.JobPostId && a.CandidateId == user.Id);

                if (alreadyApplied)
                {
                    TempData["Error"] = "You have already applied for this job.";
                    return RedirectToAction("Index", "Home");
                    //return RedirectToAction("JobDetails", "Candidate");

                }

                // Convert the uploaded file to a base64 string
                string resumeBase64 = null;
                if (model.Resume != null && model.Resume.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await model.Resume.CopyToAsync(ms);
                        resumeBase64 = Convert.ToBase64String(ms.ToArray());
                    }
                }

                var application = new Application
                {
                    JobPostId = model.JobPostId,
                    CandidateId = user.Id,
                    ResumeBase64 = resumeBase64,
                    R1Response1 = Request.Form["R1Response1"].FirstOrDefault(),
                    R1Response2 = Request.Form["R1Response2"].FirstOrDefault(),
                    R1Response3 = Request.Form["R1Response3"].FirstOrDefault(),
                    R1Response4 = Request.Form["R1Response4"].FirstOrDefault(),
                    R1Response5 = Request.Form["R1Response5"].FirstOrDefault(),
                    IsShortlisted = false
                };

                // Implement R1 Check to screen candidates
                bool goodCandidate = await CompareR1Responses(application, jobPost);
                if (!goodCandidate)
                {
                    TempData["Error"] = "You are not qualified for this position.";
                    application.IsScreenedOut = true;
                    _context.Applications.Add(application);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Apply", new { jobPostId = model.JobPostId });
                }

                TempData["Success"] = "Application submitted successfully.";
                application.IsScreenedOut = !goodCandidate;
                _context.Applications.Add(application);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An Error Occured.";
                @ViewBag.ErrorMessage = ex.Message;
                @ViewBag.StackTrace = ex.StackTrace;
                return RedirectToAction("Error", "Home");
            }
            
        }



        public async Task<bool> CompareR1Responses(Application candidateInfo, JobPost jobDetails)
        {
            double matchingRate = double.Parse(_configuration["AppSettings:MatchingRate"]);
            if (candidateInfo.R1Response1 == null || candidateInfo.R1Response2 == null || candidateInfo.R1Response3 == null
                || candidateInfo.R1Response4 == null || candidateInfo.R1Response5 == null) return false;

            int matchCount = 0;
            if (bool.Parse(candidateInfo.R1Response1) == jobDetails.R1CheckAnswer1) matchCount++;
            if (bool.Parse(candidateInfo.R1Response2) == jobDetails.R1CheckAnswer2) matchCount++;
            if (bool.Parse(candidateInfo.R1Response3) == jobDetails.R1CheckAnswer3) matchCount++;
            if (bool.Parse(candidateInfo.R1Response4) == jobDetails.R1CheckAnswer4) matchCount++;
            if (bool.Parse(candidateInfo.R1Response5) == jobDetails.R1CheckAnswer5) matchCount++;

            double actualMatchingRate = (double)matchCount / 5 * 100;
            return actualMatchingRate >= matchingRate;
        }

        public async Task<string> GetSubstringBeforeFirstSpace(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            int spaceIndex = input.IndexOf(' ');

            if (spaceIndex == -1)
            {
                // If there is no space, return the entire string
                return input;
            }

            return input.Substring(0, spaceIndex).Replace(",", "");
        }

    }
}
