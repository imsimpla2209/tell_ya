using DiaryManagement.Application.Dtos;
using DiaryManagement.Core.Entities;
using DiaryManagement.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace DiaryManagement.Application.Services
{
    public interface IWriterService
    {
        public Task<IEnumerable<Writer>> GetAllWritersAsync();
        public Task<Writer> GetWriterByIdAsync(string categoryId);
        public Task<Result> AddWriterAsync(WriterDto form);
        public Task<Result> UpdateWriterAsync(Writer category);
        public Task<Result> RemoveWriterAsync(string categoryId);
    }

    public class WriterService : IWriterService
    {
        private readonly ILogger<CategoryService> _logger;
        private readonly IWriterRepository _WriterRepository;

        public WriterService(ILogger<CategoryService> logger, IWriterRepository WriterRepository)
        {
            _logger = logger;
            _WriterRepository = WriterRepository;
        }

        public async Task<IEnumerable<Writer>> GetAllWritersAsync()
        {
            var categories = await _WriterRepository.GetAllWritersAsync();
            return categories;
        }

        public async Task<Writer> GetWriterByIdAsync(string WriterId)
        {
            var Writer = await _WriterRepository.GetWriterByIdAsync(WriterId);
            return Writer;
        }

        public async Task<Result> RemoveWriterAsync(string WriterId)
        {
            Writer Writer = await _WriterRepository.GetWriterByIdAsync(WriterId);
            if (Writer == null) return new Result(false, "The Writer does not exist");
            await _WriterRepository.RemoveWriterAsync(Writer);
            return new Result("The Writer is removed");
        }

        public async Task<Result> AddWriterAsync(WriterDto form)
        {
            Writer newWriter = new Writer()
            {
                WriterId = Guid.NewGuid().ToString(),
                WriterName = form.WriterName
            };
            var result = await _WriterRepository.AddWriterAsync(newWriter);
            if (!result) return new Result(false, "The Writer already exist!");
            return new Result("The Writer is added");
        }

        public async Task<Result> UpdateWriterAsync(Writer Writer)
        {
            Writer WriterExist = await _WriterRepository.GetWriterByIdAsync(Writer.WriterId);
            if (WriterExist == null) return new Result(false, "The Writer does not exist");
            var checkDuplicateResult = await _WriterRepository.CheckDuplicateWriterAsync(WriterExist.WriterId, Writer.WriterName);
            if (!checkDuplicateResult) return new Result(false, "The Writer name already exist");
            var result = await _WriterRepository.UpdateWriterAsync(WriterExist, Writer.WriterName);
            if (!result) return new Result(false, "Failed to update the Writer name");
            return new Result("Update category successfully");
        }
    }
}
