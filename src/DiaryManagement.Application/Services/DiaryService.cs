using DiaryManagement.Application.Dtos;
using DiaryManagement.Core.Entities;
using DiaryManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;

namespace DiaryManagement.Application.Services
{
    public interface IDiaryService
    {
        public Task<IEnumerable<Diary>> GetAllDiarysAsync();
        public Task<IEnumerable<Diary>> GetAllDiaryAsync(int order, int skip, int take);
        public Task<IEnumerable<Diary>> GetAllDiaryAsync(string categoryId, int orderName, string keywords, int page);
        public Task<int> CountTotalPageAsync(IEnumerable<Diary> DiaryList);
        public Task<Diary> GetDiaryByIdAsync(string DiaryId);
        public Task<Result> AddDiaryAsync(DiaryDto form);
        public Task<Result> UpdateDiaryAsync(DiaryDto form);
        public Task<Result> RemoveDiaryAsync(string DiaryId);
    }

    public class DiaryService : IDiaryService
    {
        private readonly IDiaryRepository _DiaryRepository;

        private const string _basePath = "wwwroot";

        private const string _uploadFolder = "uploads";

        public static string UploadFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return "";
                }
                var directoryPath = Path.Combine(_basePath, _uploadFolder);
                Directory.CreateDirectory(directoryPath);
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(directoryPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                var relativePath = filePath.Replace("wwwroot", "").Replace("\\", "/");
                return relativePath;
            }
            catch
            {
                return "";
            }
        }

        public static void RemoveFile(string path)
        {
            string filePath = path.Insert(0, "wwwroot").Replace("/", "\\");
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

        public DiaryService(IDiaryRepository DiaryRepository)
        {
            _DiaryRepository = DiaryRepository;
        }

        public async Task<IEnumerable<Diary>> GetAllDiarysAsync()
        {
            var Diarys = await _DiaryRepository.GetAllDiaryAsync();
            return Diarys;
        }
        public async Task<IEnumerable<Diary>> GetAllDiaryAsync(int order, int skip, int take)
        {
            var Diarys = await _DiaryRepository.GetAllDiaryAsync(order, skip, take);
            return Diarys;
        }
        public async Task<IEnumerable<Diary>> GetAllDiaryAsync(string categoryId, int orderName, string keywords, int page)
        {
            var Diarys = await _DiaryRepository.GetAllDiaryAsync();
            if (categoryId != null && !categoryId.Equals("All")) Diarys = Diarys.Where(b => b.CategoryId.Equals(categoryId));
            if (keywords != null && !keywords.Equals(""))
            {
                keywords = keywords.ToUpper();
                Diarys = Diarys.Where(b => b.DiaryName.ToUpper().Contains(keywords) ||
                                         b.WriterDiarys.Any(ab => ab.Writer.WriterName.ToUpper().Contains(keywords)));
            }
            if (orderName == 1) Diarys = Diarys.OrderByDescending(b => b.DiaryName);
            Diarys = Diarys.Skip(5 * page).Take(5);
            return Diarys;
        }
        public async Task<int> CountTotalPageAsync(IEnumerable<Diary> DiaryList)
        {
            int total = DiaryList.Count();
            int totalPage = total / 5;
            if (total % 5 != 0) totalPage++;
            return totalPage;
        }

        public async Task<Diary> GetDiaryByIdAsync(string DiaryId)
        {
            Diary Diary = await _DiaryRepository.GetDiaryByIdAsync(DiaryId);
            Diary.ViewCount++;
            await _DiaryRepository.UpdateDiaryAsync(Diary);
            return Diary;

        }
        public async Task<Result> AddDiaryAsync(DiaryDto form)
        {

            Diary newDiary = new Diary()
            {
                DiaryId = Guid.NewGuid().ToString(),
                DiaryName = form.DiaryName,
                Image = "",
                CategoryId = form.CategoryId,
                Content = form.Content != null ? form.Content : "",
                CreationDate = DateTime.Now,
                LatestUpdate = DateTime.Now,
            };
            if (form.Image != null)
            {
                string imagePath = UploadFile(form.Image);
                newDiary.Image = imagePath;
            }
            var result = await _DiaryRepository.AddDiaryAsync(newDiary);
            if (!result) return new Result(false, "The Diary code already exist");
            foreach (var WriterId in form.WriterId)
            {
                bool addWriterResult = await _DiaryRepository.AddWriterDiaryAsync(newDiary.DiaryId, WriterId);
                if (!addWriterResult) return new Result(false, "Failed to add Writer Diary");
            }
            return new Result("The Diary is added");
        }

        public async Task<Result> UpdateDiaryAsync(DiaryDto form)
        {
            Diary DiaryExist = await _DiaryRepository.GetDiaryByIdAsync(form.DiaryId);
            if (DiaryExist == null) return new Result(false, "The Diary does not exist");

            DiaryExist.DiaryName = form.DiaryName;
            DiaryExist.CategoryId = form.CategoryId;
            DiaryExist.DiaryName = form.DiaryName;
            DiaryExist.Content = form.Content != null ? form.Content : DiaryExist.Content;
            DiaryExist.LatestUpdate = DateTime.Now;
            if (form.Image != null)
            {
                if (DiaryExist.Image != null) RemoveFile(DiaryExist.Image);
                var imagePath = UploadFile(form.Image);
                DiaryExist.Image = imagePath;
            }
            DiaryExist.WriterDiarys.Clear();
            var result = await _DiaryRepository.UpdateDiaryAsync(DiaryExist);
            foreach (var WriterId in form.WriterId)
            {
                bool addWriterResult = await _DiaryRepository.AddWriterDiaryAsync(DiaryExist.DiaryId, WriterId);
                if (!addWriterResult) return new Result(false, "Failed to add Writer Diary");
            }
            if (!result) return new Result(false, "Failed to update the Writer name");
            return new Result("Update category successfully");
        }

        public async Task<Result> RemoveDiaryAsync(string DiaryId)
        {
            Diary Diary = await _DiaryRepository.GetDiaryByIdAsync(DiaryId);
            if (Diary == null) return new Result(false, "The Diary does not exist");
            if (Diary.Image != null)
                RemoveFile(Diary.Image);
            Diary?.WriterDiarys?.Clear();
            var result = await _DiaryRepository.RemoveDiaryAsync(Diary);
            return new Result("The Diary is removed");
        }
    }
}
