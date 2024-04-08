using DiaryManagement.Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using DiaryManagement.Core.Entities;
using DiaryManagement.Infrastructure.Constants;
using DiaryManagement.Application.Services;

namespace DiaryManagement.Presentation.Areas.Authentication.Controllers
{
    [Area("Authentication")]
    public class AuthenticationController : Controller
    {
        public readonly IUserService _userService;
        public readonly SignInManager<User> _signInManager;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IUserService userService, SignInManager<User> signInManager, ILogger<AuthenticationController> logger)
        {
            _userService = userService;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpDto model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.CreateAsync(model, RoleName.Member);
                if (!result.IsSucceed)
                {
                    ViewBag.SignUpError = result.Message;
                    return View();
                }
                return RedirectToAction(nameof(Login));
            }
            ViewBag.SignUpError = "Failed to create account";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.LoginAsync(model);
                if (!result.IsSucceed)
                {
                    _logger.LogWarning(result.Message);
                    ViewBag.LoginError = result.Message;
                    return View();
                }
                _logger.LogInformation("User logged in");
                if (result.Message.Equals(RoleName.Manager))
                {
                    return RedirectToAction("Index", "Admin", new { area = "Admin" });
                }
                else
                {
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _userService.LogoutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
