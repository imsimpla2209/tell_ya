using DiaryManagement.Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using DiaryManagement.Core.Entities;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using DiaryManagement.Infrastructure.Constants;
using DiaryManagement.Application.Services;

namespace DiaryManagement.Presentation.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Manager")]
    public class AdminController : Controller
    {
        public readonly IUserService _userService;
        public readonly ICategoryService _categoryService;
        public readonly IDiaryService _DiaryService;
        public readonly IWriterService _WriterService;
        public readonly SignInManager<User> _signInManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IUserService userService, ICategoryService categoryService,
            IWriterService WriterService, SignInManager<User> signInManager,
            IDiaryService DiaryService, ILogger<AdminController> logger)
        {
            _userService = userService;
            _categoryService = categoryService;
            _WriterService = WriterService;
            _DiaryService = DiaryService;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            var Diarys = await _DiaryService.GetAllDiarysAsync();
            var Writers = await _WriterService.GetAllWritersAsync();
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewData["TotalUser"] = users != null ? users.Count() : 0;
            ViewData["TotalDiary"] = Diarys != null ? Diarys.Count() : 0;
            ViewData["TotalWriter"] = Writers != null ? Writers.Count() : 0;
            ViewData["TotalCategory"] = categories != null ? categories.Count() : 0;
            return View();
        }

        //User
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeUserStatus(string userId)
        {
            var result = await _userService.ChangeUserStatus(userId);
            return RedirectToAction(nameof(GetAllUsers));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpDto model)
        {

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> AddCategory()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(CategoryDto category)
        {

            if (ModelState.IsValid)
            {
                Result result = await _categoryService.AddCategoryAsync(category);
                if (result.IsSucceed) return RedirectToAction(nameof(GetAllCategories));
                ViewBag.AddCateMsg = result.Message;
                return View();

            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(string categoryId)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(Category category)
        {

            if (ModelState.IsValid)
            {
                var result = await _categoryService.UpdateCategoryAsync(category);
                if (result.IsSucceed) return RedirectToAction(nameof(GetAllCategories));
                ViewBag.EditCateMsg = result.Message;
                return View(category);
            }
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> RemoveCategory(string categoryId)
        {
            var result = await _categoryService.RemoveCategoryAsync(categoryId);
            return RedirectToAction(nameof(GetAllCategories)); ;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }
        [HttpGet]
        public async Task<IActionResult> AddWriter()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddWriter(WriterDto Writer)
        {

            if (ModelState.IsValid)
            {
                var result = await _WriterService.AddWriterAsync(Writer);
                if (result.IsSucceed) return RedirectToAction(nameof(GetAllWriters));
                ViewBag.AddAuthMsg = result.Message;
                return View();

            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditWriter(string WriterId)
        {
            var category = await _WriterService.GetWriterByIdAsync(WriterId);
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditWriter(Writer Writer)
        {

            if (ModelState.IsValid)
            {
                var result = await _WriterService.UpdateWriterAsync(Writer);
                if (result.IsSucceed) return RedirectToAction(nameof(GetAllWriters));
                ViewBag.EditAuthMsg = result.Message;
                return View(Writer);
            }
            return View(Writer);
        }

        [HttpGet]
        public async Task<IActionResult> RemoveWriter(string WriterId)
        {
            var result = await _WriterService.RemoveWriterAsync(WriterId);
            return RedirectToAction(nameof(GetAllWriters)); ;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWriters()
        {
            var Writers = await _WriterService.GetAllWritersAsync();
            return View(Writers);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllDiarys()
        {
            var Diarys = await _DiaryService.GetAllDiarysAsync();
            return View(Diarys);
        }

        [HttpGet]
        public async Task<IActionResult> AddDiary()
        {
            ViewData["Categories"] = new List<Category>();
            ViewData["Writers"] = new List<Writer>();
            var categories = await _categoryService.GetAllCategoriesAsync();
            if (categories.Any()) ViewData["Categories"] = categories;
            var Writers = await _WriterService.GetAllWritersAsync();
            if (Writers.Any()) ViewData["Writers"] = Writers;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDiary(DiaryDto Diary)
        {
            if (ModelState.IsValid)
            {
                var result = await _DiaryService.AddDiaryAsync(Diary);
                if (result.IsSucceed) return RedirectToAction(nameof(GetAllDiarys));
                ViewBag.AddDiaryMsg = result.Message;
            }
            ViewData["Categories"] = new List<Category>();
            ViewData["Writers"] = new List<Writer>();
            var categories = await _categoryService.GetAllCategoriesAsync();
            if (categories.Any()) ViewData["Categories"] = categories;
            var Writers = await _WriterService.GetAllWritersAsync();
            if (Writers.Any()) ViewData["Writers"] = Writers;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditDiary(string DiaryId)
        {
            ViewData["Categories"] = new List<Category>();
            ViewData["Writers"] = new List<Writer>();
            var categories = await _categoryService.GetAllCategoriesAsync();
            if (categories.Any()) ViewData["Categories"] = categories;
            var Writers = await _WriterService.GetAllWritersAsync();
            if (Writers.Any()) ViewData["Writers"] = Writers;
            var currentDiary = await _DiaryService.GetDiaryByIdAsync(DiaryId);
            ViewData["CurrentDiary"] = currentDiary;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDiary(DiaryDto Diary)
        {
            if (ModelState.IsValid)
            {
                var result = await _DiaryService.UpdateDiaryAsync(Diary);
                if (result.IsSucceed) return RedirectToAction(nameof(GetAllDiarys));
                ViewBag.EditDiaryMsg = result.Message;
            }
            ViewData["Categories"] = new List<Category>();
            ViewData["Writers"] = new List<Writer>();
            var categories = await _categoryService.GetAllCategoriesAsync();
            if (categories.Any()) ViewData["Categories"] = categories;
            var Writers = await _WriterService.GetAllWritersAsync();
            if (Writers.Any()) ViewData["Writers"] = Writers;
            var currentDiary = await _DiaryService.GetDiaryByIdAsync(Diary.DiaryId);
            ViewData["CurrentDiary"] = currentDiary;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> RemoveDiary(string DiaryId)
        {
            var result = await _DiaryService.RemoveDiaryAsync(DiaryId);
            return RedirectToAction(nameof(GetAllDiarys)); ;
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Authentication", new { area = "Authentication" });
        }
    }
}
