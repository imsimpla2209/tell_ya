using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DiaryManagement.Application.Dtos
{
    public class DiaryDto
    {
        public string? DiaryId { get; set; }
        [Required(ErrorMessage = "The Diary name field is required")]

        [StringLength(500, MinimumLength = 2, ErrorMessage = "The Diary name is invalid")]
        public string DiaryName { get; set; }
        public IFormFile? Image { get; set; }
        [Required(ErrorMessage = "The category field is required")]
        public string CategoryId { get; set; }
        [Required(ErrorMessage = "The Writer field is required")]
        public List<string> WriterId { get; set; }
        public string? Content { get; set; }
        [Range(0, 1000, ErrorMessage = "The quantity must be between 0 and 1000")]
        public int Quantity { get; set; }
    }
}
