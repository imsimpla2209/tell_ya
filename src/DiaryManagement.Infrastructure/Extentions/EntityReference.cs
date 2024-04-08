using DiaryManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiaryManagement.Infrastructure.Extentions
{
    public static class EntityReference
    {
        public static void DuplicateIndex(ModelBuilder builder)
        {
            builder.Entity<WriterDiary>().HasKey(a => new { a.WriterId, a.DiaryId });
        }
    }
}
