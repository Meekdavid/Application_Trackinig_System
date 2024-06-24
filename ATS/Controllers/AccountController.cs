using ATS.Helpers;
using ATS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ATS.Controllers
{
    //[Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        FullName = model.FullName,
                        PhoneNumber = model.PhoneNumber,
                        PreferredLanguage = model.PreferredLanguage,
                        Country = model.Country,
                        Bio = model.Bio,
                        RegistrationDate = DateTime.Now,
                        IsActive = true,
                        Skills = model.Skills,
                        Role = model.Role,
                        AgencyName = model.AgencyName ?? "N/A",
                        CompanyName = model.CompanyName ?? "N/A",
                        Industry = model.Industry ?? "N/A",
                        Address = model.Address ?? "N/A"

                    };

                    if (model.ProfilePicture != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await model.ProfilePicture.CopyToAsync(memoryStream);
                            user.ProfilePictureBase64 = Convert.ToBase64String(memoryStream.ToArray());
                        }
                    }

                    // Add Educations
                    user.Educations = model.Educations.Select(e => new Education
                    {
                        Institution = e.Institution,
                        Degree = e.Degree,
                        FieldOfStudy = e.FieldOfStudy,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate
                    }).ToList();

                    // Add Experiences
                    user.Experiences = model.Experiences.Select(e => new Experience
                    {
                        Company = e.Company,
                        Position = e.Position,
                        Description = e.Description,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate
                    }).ToList();

                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        ViewData["Success"] = "Your Profiled has been Registered Successfully";
                        ViewBag.login = 1;
                        await _userManager.AddToRoleAsync(user, model.Role);
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                    foreach (var error in result.Errors)
                    {
                        ViewData["Error"] = ViewData["Error"] + "," + error.Description;
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                TempData["Error"] = "Some input fields are not valid.";
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An Error Occured.";
                @ViewBag.ErrorMessage = ex.Message;
                @ViewBag.StackTrace = ex.StackTrace;
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        ViewBag.login = 1;
                        TempData["Success"] = "Login Successful";
                        return RedirectToAction("Index", "Home");
                    }
                    ViewBag.login = 1;
                    TempData["Error"] = "Username or Password Incorrect";
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
                else
                {
                    TempData["Error"] = "Some input fields are not valid.";
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An Error Occured.";
                @ViewBag.ErrorMessage = ex.Message;
                @ViewBag.StackTrace = ex.StackTrace;
                return RedirectToAction("Error", "Home");
            }

        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
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

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Logout()
        //{
        //    await _signInManager.SignOutAsync();
        //    return RedirectToAction("Index", "Home");
        //}

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                var model = new ProfileViewModel
                {
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    PreferredLanguage = user.PreferredLanguage,
                    Country = user.Country,
                    Bio = user.Bio,
                    ProfilePictureBase64 = user.ProfilePictureBase64,
                    Skills = user.Skills,
                    Educations = user.Educations.Select(e => new EducationViewModel
                    {
                        Institution = e.Institution,
                        Degree = e.Degree,
                        FieldOfStudy = e.FieldOfStudy,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate
                    }).ToList(),
                    Experiences = user.Experiences.Select(e => new ExperienceViewModel
                    {
                        Company = e.Company,
                        Position = e.Position,
                        Description = e.Description,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate
                    }).ToList()
                };

                return View(model);
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
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        return RedirectToAction("Login");
                    }

                    user.FullName = model.FullName;
                    user.PhoneNumber = model.PhoneNumber;
                    user.PreferredLanguage = model.PreferredLanguage;
                    user.Country = model.Country;
                    user.Bio = model.Bio;
                    user.Skills = model.Skills;

                    // Update Educations
                    user.Educations.Clear();
                    user.Educations = model.Educations.Select(e => new Education
                    {
                        Institution = e.Institution,
                        Degree = e.Degree,
                        FieldOfStudy = e.FieldOfStudy,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate
                    }).ToList();

                    // Update Experiences
                    user.Experiences.Clear();
                    user.Experiences = model.Experiences.Select(e => new Experience
                    {
                        Company = e.Company,
                        Position = e.Position,
                        Description = e.Description,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate
                    }).ToList();

                    if (model.ProfilePicture != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await model.ProfilePicture.CopyToAsync(memoryStream);
                            user.ProfilePictureBase64 = Convert.ToBase64String(memoryStream.ToArray());
                        }
                    }

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Profile");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                TempData["Error"] = "Some input fields are not valid.";
                return View(model);
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