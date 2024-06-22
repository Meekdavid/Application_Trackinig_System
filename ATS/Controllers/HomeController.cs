﻿using ATS.Database;
using ATS.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ATS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            TempData["Error"] = null;
            TempData["Success"] = null;
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

        public IActionResult Privacy()
        {
            return View();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}

        [Route("Error")]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            ViewBag.ErrorMessage = exceptionHandlerPathFeature?.Error.Message;
            ViewBag.StackTrace = exceptionHandlerPathFeature?.Error.StackTrace;

            return View();
        }
    }
}