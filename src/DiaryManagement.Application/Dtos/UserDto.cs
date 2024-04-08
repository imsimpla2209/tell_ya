using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DiaryManagement.Application.Dtos
{
    public class UserDto
    {
        [Required]
        public string Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The first name is invalid")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The last name is invalid")]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [AllowNull]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "The phone number is invalid")]
        public string? PhoneNumber { get; set; }
        [AllowNull]
        public bool Gender { get; set; }
        [AllowNull]
        public DateTime Dob { get; set; }
        public string? Avatar { get; set; }
        public string? Address { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        public bool IsActivated { get; set; }
        [Required]
        public DateTime CreationDate { get; set; } = DateTime.Now;
    }
}
