using DiaryManagement.Application.Dtos;
using DiaryManagement.Application.Services;
using DiaryManagement.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DiaryManagement.Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;
        private readonly IDiaryService _DiaryService;
        private readonly ICategoryService _categoryService;

        public HomeController(ILogger<HomeController> logger, IUserService userService,
            IDiaryService DiaryService, ICategoryService categoryService)
        {
            _logger = logger;
            _userService = userService;
            _DiaryService = DiaryService;
            _categoryService = categoryService;

        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var Diarys = await _DiaryService.GetAllDiaryAsync(1, 0, 5);
            return View(Diarys);
        }

        [HttpGet]
        public async Task<IActionResult> AllDiarys(string categoryId, string orderByName, string keyword, string page)
        {
            int currentPage = int.Parse(page);
            ViewData["Categories"] = new List<Category>();
            var categories = await _categoryService.GetAllCategoriesAsync();
            if (categories.Any()) ViewData["Categories"] = categories;
            int order = orderByName.StartsWith("A") ? 0 : 1;
            var Diarys = await _DiaryService.GetAllDiaryAsync(categoryId, order, keyword, currentPage);
            ViewData["CurrentPage"] = currentPage;
            ViewData["TotalPage"] = await _DiaryService.CountTotalPageAsync(Diarys);
            ViewData["CategoryId"] = categoryId;
            ViewData["OrderByName"] = orderByName;
            ViewData["Keyword"] = keyword == null ? "" : keyword;
            return View(Diarys);
        }

        [HttpGet]
        public async Task<IActionResult> GetDiaryDetail(string DiaryId)
        {
            Diary Diary = await _DiaryService.GetDiaryByIdAsync(DiaryId);
            return View(Diary);
        }

        [HttpGet]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> UserInfo()
        {
            User currentUser = await _userService.GetCurrentUserAsync();
            if (currentUser == null) return RedirectToAction(nameof(Index));
            ViewData["CurrentUser"] = currentUser;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> UserInfo(UpdateUserDto user)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.UpdateUserAsync(user);
                ViewBag.EditUserMsg = result.Message;
            }
            User currentUser = await _userService.GetCurrentUserAsync();
            if (currentUser == null) return RedirectToAction(nameof(Index));
            ViewData["CurrentUser"] = currentUser;
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> AboutUs()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ContactUs()
        {
            return View();
        }
    }
}