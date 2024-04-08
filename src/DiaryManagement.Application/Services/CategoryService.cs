using DiaryManagement.Application.Dtos;
using DiaryManagement.Core.Entities;
using DiaryManagement.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;



namespace DiaryManagement.Application.Services
{

    public interface ICategoryService
    {
        public Task<IEnumerable<Category>> GetAllCategoriesAsync();
        public Task<Category> GetCategoryByIdAsync(string categoryId);
        public Task<Result> AddCategoryAsync(CategoryDto form);
        public Task<Result> UpdateCategoryAsync(Category category);
        public Task<Result> RemoveCategoryAsync(string categoryId);
    }


    public class CategoryService : ICategoryService
    {
        private readonly ILogger<CategoryService> _logger;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ILogger<CategoryService> logger, ICategoryRepository categoryRepository)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return categories;
        }

        public async Task<Category> GetCategoryByIdAsync(string categoryId)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            return category;
        }

        public async Task<Result> RemoveCategoryAsync(string categoryId)
        {
            Category category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (category == null) return new Result(false, "The category does not exist");
            var result = await _categoryRepository.RemoveCategoryAsync(category);
            return new Result("The category is removed");
        }

        public async Task<Result> AddCategoryAsync(CategoryDto form)
        {
            Category newCategory = new Category()
            {
                CategoryId = Guid.NewGuid().ToString(),
                CategoryName = form.CategoryName
            };
            var result = await _categoryRepository.AddCategoryAsync(newCategory);
            if (!result) return new Result(false, "The category name already exist!");
            return new Result("The category is added");
        }

        public async Task<Result> UpdateCategoryAsync(Category category)
        {
            Category categoryExist = await _categoryRepository.GetCategoryByIdAsync(category.CategoryId);
            if (categoryExist == null) return new Result(false, "The category does not exist");
            var checkDuplicateResult = await _categoryRepository.CheckDuplicateCategoryAsync(categoryExist.CategoryId, category.CategoryName);
            if (!checkDuplicateResult) return new Result(false, "The category name already exist");
            var result = await _categoryRepository.UpdateCategoryAsync(categoryExist, category.CategoryName);
            if (!result) return new Result(false, "Failed to update the category name");
            return new Result("Update category successfully");
        }
    }
}
