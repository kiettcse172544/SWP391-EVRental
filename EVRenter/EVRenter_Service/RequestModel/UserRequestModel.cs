using EVRenter_Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.RequestModel
{

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
        public string? Address { get; set; } = string.Empty;
        public int? StationID { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class StaffProfileRequest
    {
        [Required(ErrorMessage = "User ID is required.")]
        public int UserID { get; set; }
        [Required(ErrorMessage = "Station ID is required.")]
        public int StationID { get; set; }
        //public string? StaffCode { get; set; }
    }

    public class StaffUpdateRequest
    {
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string? FullName { get; set; } = null;

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [RegularExpression(@"^\+?[0-9]{10,15}$", ErrorMessage = "Phone number must be between 10 and 15 digits and may start with a '+' sign.")]
        public string? Phone { get; set; } = null;
        public string? Address { get; set; } = null;
        public int? StationID { get; set; }
    }

    public class RenterProfileRequest
    {
        [Required(ErrorMessage = "Renter ID is required.")]
        public int UserID { get; set; }
        [Required(ErrorMessage = "ID Card Number is required.")]
        public string IDNumber { get; set; }
        [Required(ErrorMessage = "Driver License No is required.")]
        public string DriverLicenseNo { get; set; }
    }

    public class UserUpdateRequest
    {
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string? FullName { get; set; } = null;

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [RegularExpression(@"^\+?[0-9]{10,15}$", ErrorMessage = "Phone number must be between 10 and 15 digits and may start with a '+' sign.")]
        public string? Phone { get; set; } = null;
        public string? Address { get; set; } = null;
    }

    public class RenterUpdateRequest
    {
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string? FullName { get; set; } = null;

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [RegularExpression(@"^\+?[0-9]{10,15}$", ErrorMessage = "Phone number must be between 10 and 15 digits and may start with a '+' sign.")]
        public string? Phone { get; set; } = null;
        public string? Address { get; set; } = null;
        //public string? IDNumber { get; set; }
        //public string? DriverLicenseNo { get; set; }
        public int? Type { get; set; } = null;
        public bool? IsEmailVerified { get; set; } = null;
    }

    public class ChangePasswordRequestModel
    {
        public int UserId { get; set; }

        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }

}
