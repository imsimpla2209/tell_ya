using DiaryManagement.Core.Entities;
using DiaryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DiaryManagement.Infrastructure.Repositories
{
    public interface IDiaryRepository
    {

        public Task<IEnumerable<Diary>> GetAllDiaryAsync();
        public Task<IEnumerable<Diary>> GetAllDiaryAsync(int order, int skip, int take);
        public Task<Diary> GetDiaryByIdAsync(string DiaryId);
        public Task<bool> AddDiaryAsync(Diary newDiary);
        public Task<bool> AddWriterDiaryAsync(string DiaryId, string WriterId);
        public Task<bool> UpdateDiaryAsync(Diary DiaryEdited);
        public Task<bool> RemoveDiaryAsync(Diary Diary);
    }
    public class DiaryRepository : IDiaryRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public DiaryRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Diary>> GetAllDiaryAsync()
        {
            var Diarys = await _dbContext.Diarys
                .Include(b => b.Category)
                .Include(b => b.WriterDiarys)
                .ThenInclude(ab => ab.Writer)
                .OrderBy(b => b.DiaryName)
                .ToListAsync();
            return Diarys;
        }
        public async Task<IEnumerable<Diary>> GetAllDiaryAsync(int order, int skip, int take)
        {
            var Diarys = await _dbContext.Diarys
                .Include(b => b.Category)
                .Include(b => b.WriterDiarys)
                .ThenInclude(ab => ab.Writer)
                .Skip(skip).Take(take)
                .OrderBy(b => b.CreationDate)
                .ToListAsync();
            if (order == 1) Diarys.Reverse();
            return Diarys;
        }
        public async Task<Diary> GetDiaryByIdAsync(string DiaryId)
        {
            Diary Diary = await _dbContext.Diarys
                .Include(b => b.Category)
                .Include(b => b.WriterDiarys)
                .ThenInclude(ab => ab.Writer)
                .FirstOrDefaultAsync(b => b.DiaryId.Equals(DiaryId));
            return Diary;
        }
        public async Task<bool> AddDiaryAsync(Diary newDiary)
        {
            try
            {
                await _dbContext.AddAsync(newDiary);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> AddWriterDiaryAsync(string DiaryId, string WriterId)
        {
            try
            {
                WriterDiary newWriterDiary = new WriterDiary()
                {
                    DiaryId = DiaryId,
                    WriterId = WriterId
                };
                await _dbContext.AddAsync(newWriterDiary);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> RemoveDiaryAsync(Diary Diary)
        {
            try
            {
                _dbContext.Remove(Diary);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> UpdateDiaryAsync(Diary DiaryEdited)
        {
            try
            {
                _dbContext.Update(DiaryEdited);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

}

