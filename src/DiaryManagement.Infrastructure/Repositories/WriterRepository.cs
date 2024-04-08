using DiaryManagement.Core.Entities;
using DiaryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DiaryManagement.Infrastructure.Repositories
{
    public interface IWriterRepository
    {

        public Task<IEnumerable<Writer>> GetAllWritersAsync();
        public Task<Writer> GetWriterByIdAsync(string WriterId);
        public Task<bool> AddWriterAsync(Writer newWriter);
        public Task<bool> UpdateWriterAsync(Writer WriterExist, string newWriterName);
        public Task<bool> RemoveWriterAsync(Writer Writer);
        public Task<bool> CheckDuplicateWriterAsync(string currentWriterId, string newWriterName);


    }
    public class WriterRepository : IWriterRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public WriterRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddWriterAsync(Writer newWriter)
        {
            try
            {
                string upperWriterName = newWriter.WriterName.ToUpper();
                Writer WriterExist = await _dbContext.Writers.FirstOrDefaultAsync(a => a.WriterName.ToUpper().Equals(upperWriterName));
                if (WriterExist != null) return false;
                await _dbContext.AddAsync(newWriter);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Writer>> GetAllWritersAsync()
        {
            var Writers = await _dbContext.Writers.OrderBy(a => a.WriterName).AsNoTracking().ToListAsync();
            return Writers;
        }

        public async Task<Writer> GetWriterByIdAsync(string WriterId)
        {
            Writer Writer = await _dbContext.Writers.FindAsync(WriterId);
            return Writer;
        }

        public async Task<bool> RemoveWriterAsync(Writer Writer)
        {
            try
            {
                Writer?.WriterDiarys?.Clear();
                _dbContext.Remove(Writer);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }

        }

        public async Task<bool> UpdateWriterAsync(Writer WriterExist, string newWriterName)
        {
            try
            {
                WriterExist.WriterName = newWriterName;
                _dbContext.Update(WriterExist);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CheckDuplicateWriterAsync(string currentWriterId, string newWriterName)
        {
            var checkNameExist = await _dbContext.Writers.AnyAsync(a =>
                    a.WriterName.ToUpper().Equals(newWriterName.ToUpper()) &&
                    !a.WriterId.Equals(currentWriterId));
            if (checkNameExist) return false;
            return true;
        }
    }
}
