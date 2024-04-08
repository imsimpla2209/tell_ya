using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryManagement.Application.Dtos
{
    public class WriterDto
    {
        [Required(ErrorMessage = "The Writer name field is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "The Writer name is invalid")]
        public string WriterName { get; set; }
    }
}
