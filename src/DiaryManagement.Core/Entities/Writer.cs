using System.ComponentModel.DataAnnotations;

namespace DiaryManagement.Core.Entities
{
    public class Writer
    {
        [Key]
        public string WriterId { get; set; }
        [Required(ErrorMessage = "The Writer name field is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "The Writer name is invalid")]
        public string WriterName { get; set; }
        //
        public virtual ICollection<WriterDiary>? WriterDiarys { get; set; }
    }
}
