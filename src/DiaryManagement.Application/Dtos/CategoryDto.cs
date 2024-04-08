using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryManagement.Application.Dtos
{
    public class CategoryDto
    {
        [Required(ErrorMessage = "The category name field is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "The category name is invalid")]
        public string CategoryName { get; set; }
    }
}
