using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.RequestModel
{
    internal class UserRequestModel
    {
    }

    public class UserCreateRequest
    {
        [Required(ErrorMessage = "FullName is required.")]
        [StringLength(50, ErrorMessage = "FullName cannot exceed 50 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\+?[0-9]{10,15}$", ErrorMessage = "Phone number must be between 10 and 15 digits and may start with a '+' sign.")]
        public string Phone { get; set; } = string.Empty;

        public int? RoleID { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UserUpdateRequest
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [RegularExpression(@"^\+?[0-9]{10,15}$", ErrorMessage = "Phone number must be between 10 and 15 digits and may start with a '+' sign.")]
        public string Phone { get; set; } = string.Empty;
        public int? RoleID { get; set; }
    }
}
