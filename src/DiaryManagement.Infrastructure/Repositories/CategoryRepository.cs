using DiaryManagement.Core.Entities;
using DiaryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DiaryManagement.Infrastructure.Repositories
{
    public interface ICategoryRepository
    {

        public Task<IEnumerable<Category>> GetAllCategoriesAsync();
        public Task<Category> GetCategoryByIdAsync(string categoryId);
        public Task<bool> AddCategoryAsync(Category newCategory);
        public Task<bool> UpdateCategoryAsync(Category categoryExist, string newCategoryName);
        public Task<bool> RemoveCategoryAsync(Category category);
        public Task<bool> CheckDuplicateCategoryAsync(string currentCategoryId, string newCategoryName);


    }
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddCategoryAsync(Category newCategory)
        {
            try
            {
                string upperCategoryName = newCategory.CategoryName.ToUpper();
                Category categoryExist = await _dbContext.Categories.FirstOrDefaultAsync(c => c.CategoryName.ToUpper().Equals(upperCategoryName));
                if (categoryExist != null) return false;
                await _dbContext.AddAsync(newCategory);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            var categories = await _dbContext.Categories.OrderBy(c => c.CategoryName).AsNoTracking().ToListAsync();
            return categories;
        }

        public async Task<Category> GetCategoryByIdAsync(string categoryId)
        {
            Category category = await _dbContext.Categories.FindAsync(categoryId);
            return category;
        }

        public async Task<bool> RemoveCategoryAsync(Category category)
        {
            try
            {
                category?.Diarys?.Clear();
                _dbContext.Remove(category);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }

        }

        public async Task<bool> UpdateCategoryAsync(Category categoryExist, string newCategoryName)
        {
            try
            {
                categoryExist.CategoryName = newCategoryName;
                _dbContext.Update(categoryExist);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CheckDuplicateCategoryAsync(string currentCategoryId, string newCategoryName)
        {
            var checkNameExist = await _dbContext.Categories.AnyAsync(c =>
                    c.CategoryName.ToUpper().Equals(newCategoryName.ToUpper()) &&
                    !c.CategoryId.Equals(currentCategoryId));
            if (checkNameExist) return false;
            return true;
        }
    }
}
