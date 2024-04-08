using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryManagement.Core.Entities
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "The first name is invalid")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "The last name is invalid")]
        public string LastName { get; set; }
        [Required]
        [AllowNull]
        public bool Gender { get; set; }
        [AllowNull]
        public DateTime Dob { get; set; }
        public string? Avatar { get; set; }
        public string? Address { get; set; }
        [Required]
        public bool IsActivated { get; set; }
        [Required]
        public DateTime CreationDate { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
    }
}
