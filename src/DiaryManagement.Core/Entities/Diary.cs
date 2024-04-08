using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DiaryManagement.Core.Entities
{
    public class Diary
    {
        [Key]
        public string DiaryId { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "The Diary name is invalid")]
        public string DiaryName { get; set; }
        public string? Image { get; set; }
        [Required]
        public int ViewCount { get; set; } = 0;
        [Required]
        public string CategoryId { get; set; }
        [AllowNull]
        public string Content { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        [Required]
        public DateTime LatestUpdate { get; set; } = DateTime.Now;
        public virtual Category? Category { get; set; }
        public virtual ICollection<WriterDiary>? WriterDiarys { get; set; }

    }
}
