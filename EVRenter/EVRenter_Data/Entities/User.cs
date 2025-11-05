using EVRenter_CM.Enums;
using System;
using System.Collections.Generic;

namespace EVRenter_Data.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }

        // Mỗi staff có một station riêng 
        public int? StationId { get; set; }

        // Vai trò (Admin, Staff, Renter)
        public RoleType RoleID { get; set; } = RoleType.Renter;

        // Kích hoạt tài khoản
        public bool IsActive { get; set; } = false;

        // Xác thực email
        public bool IsEmailVerified { get; set; } = false;

        // Token gửi qua email để xác thực tài khoản
        public string? EmailVerificationToken { get; set; }

        // Token hết hạn sau thời gian nhất định
        public DateTime? EmailVerificationTokenExpiresAt { get; set; }

        // Dành cho quên mật khẩu
        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordTokenExpiresAt { get; set; }

        
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<HandoverAndReturn> HandoverAndReturns { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<ExtraFee> ExtraFees { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual StaffProfile StaffProfile { get; set; }
        public virtual RenterProfile RenterProfile { get; set; }
    }
}
