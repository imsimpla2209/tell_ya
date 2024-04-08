using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryManagement.Core.Entities
{
    public class WriterDiary
    {
        [Required]
        public string WriterId { get; set; }
        [Required]
        public string DiaryId { get; set; }
        public virtual Writer? Writer { get; set; }
        public virtual Diary? Diary { get; set; }

    }
}
