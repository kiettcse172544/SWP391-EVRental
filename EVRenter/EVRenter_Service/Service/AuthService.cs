using EVRenter_CM.Enums;
using EVRenter_Data.Entities;
using EVRenter_Repository.UnitOfWork;
using EVRenter_Repository.Utils;
using EVRenter_Service.RequestModel;
using EVRenter_Service.RequestModel.register;
using EVRenter_Service.ResponseModel;
using EVRenter_Service.ResponseModel.register;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace EVRenter_Service.Service
{
    public interface IAuthService
    {
        Task<LoginResponseModel> LoginAsync(LoginRequestModel request);
        Task<SignupResponseModel> RegisterAsync(SignupRequestModel request);
        Task<VerifyEmailResponseModel> VerifyEmailAsync(string token);
        Task<ChangePasswordResponseModel> ChangePasswordAsync(ChangePasswordRequestModelV2 request);

    }

    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration, IEmailService emailService )
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<LoginResponseModel> LoginAsync(LoginRequestModel request)
        {
            
            var user = await _unitOfWork.Repository<User>()
                .AsQueryable()
                .FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDelete);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            
            if (!user.IsEmailVerified)
                throw new UnauthorizedAccessException("Email not verified.");

            
            if (!user.IsActive)
                throw new UnauthorizedAccessException("Account is inactive.");

            
            if (!PasswordTools.VerifyPassword(request.Password, user.Password))
                throw new UnauthorizedAccessException("Invalid password.");

            
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var isStaff = ((RoleType)user.RoleID) == RoleType.Staff;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userId", user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, ((RoleType)user.RoleID).ToString()),
                    new Claim("phone", user.Phone ?? string.Empty),
                    new Claim("stationId", isStaff && user.StationId.HasValue
            ? user.StationId.Value.ToString()
            : string.Empty),
                    new Claim("verifiedStatus", user.IsVerified.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new LoginResponseModel
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = ((RoleType)user.RoleID).ToString(),
                Token = tokenHandler.WriteToken(token),
                Phone = user.Phone,
                StationId = isStaff ? user.StationId : null,
                Verified = user.IsEmailVerified ? "Verified" : "Pending",
                VerifiedStatus = user.IsVerified
            };
        }

    

        public async Task<SignupResponseModel> RegisterAsync(SignupRequestModel request)
        {
            var userRepo = _unitOfWork.Repository<User>();

            
            bool emailExists = await userRepo.AsQueryable()
                .AnyAsync(u => u.Email == request.Email && !u.IsDelete);

            if (emailExists)
                throw new Exception("Email đã được đăng ký.");

            
            bool phoneExists = await userRepo.AsQueryable()
                .AnyAsync(u => u.Phone == request.Phone && !u.IsDelete);

            if (phoneExists)
                throw new Exception("Số điện thoại đã được đăng ký.");

            
            string hashedPassword = PasswordTools.HashPassword(request.Password);

            
            string token = Guid.NewGuid().ToString();

           
            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                Password = hashedPassword,
                Address = "",

                StationId = null,
                RoleID = RoleType.Renter,

                IsActive = false,
                IsEmailVerified = false,
                IsDelete = false,

                IsVerified = 1,

                EmailVerificationToken = token,
                EmailVerificationTokenExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            await userRepo.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            string verifyUrl = $"https://swp-391-fawn.vercel.app/verify?token={token}";
            string body = $@"
        <h2>Chào {user.FullName},</h2>
        <p>Cảm ơn bạn đã đăng ký tài khoản EVRenter.</p>
        <p>Vui lòng nhấn vào liên kết dưới đây để xác thực email:</p>
        <a href='{verifyUrl}'>{verifyUrl}</a>
        <p>Link hết hạn sau 1 giờ.</p>
    ";

            await _emailService.SendEmailAsync(user.Email, "Xác thực tài khoản EVRenter", body);

            return new SignupResponseModel
            {
                UserId = user.Id,
                Email = user.Email,
                IsEmailSent = true,
                Message = "Đăng ký thành công. Vui lòng kiểm tra email để xác thực."
            };
        }



        public async Task<VerifyEmailResponseModel> VerifyEmailAsync(string token)
        {
            var userRepo = _unitOfWork.Repository<User>();

            var user = await userRepo.AsQueryable()
                .FirstOrDefaultAsync(u =>
                    u.EmailVerificationToken == token &&
                    !u.IsDelete);

            if (user == null)
            {
                return new VerifyEmailResponseModel
                {
                    Success = false,
                    Message = "Token không hợp lệ."
                };
            }

            if (user.EmailVerificationTokenExpiresAt < DateTime.UtcNow)
            {
                return new VerifyEmailResponseModel
                {
                    Success = false,
                    Message = "Token đã hết hạn."
                };
            }

            
            user.IsEmailVerified = true;
            user.IsActive = true;

            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpiresAt = null;

            userRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return new VerifyEmailResponseModel
            {
                Success = true,
                Message = "Xác thực tài khoản thành công."
            };
        }

        public async Task<ChangePasswordResponseModel> ChangePasswordAsync(ChangePasswordRequestModelV2 request)
        {
            var userRepo = _unitOfWork.Repository<User>();

            
            var user = await userRepo.AsQueryable()
                .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDelete);

            if (user == null)
            {
                return new ChangePasswordResponseModel
                {
                    Success = false,
                    Message = "Người dùng không tồn tại."
                };
            }

            
            bool isCurrentPasswordValid = PasswordTools.VerifyPassword(request.CurrentPassword, user.Password);

            if (!isCurrentPasswordValid)
            {
                return new ChangePasswordResponseModel
                {
                    Success = false,
                    Message = "Mật khẩu hiện tại không chính xác."
                };
            }

            
            if (request.NewPassword != request.ConfirmNewPassword)
            {
                return new ChangePasswordResponseModel
                {
                    Success = false,
                    Message = "Xác nhận mật khẩu mới không trùng khớp."
                };
            }

            
            if (PasswordTools.VerifyPassword(request.NewPassword, user.Password))
            {
                return new ChangePasswordResponseModel
                {
                    Success = false,
                    Message = "Mật khẩu mới không được trùng với mật khẩu hiện tại."
                };
            }

            
            string hashedNewPassword = PasswordTools.HashPassword(request.NewPassword);

            
            user.Password = hashedNewPassword;
            await userRepo.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new ChangePasswordResponseModel
            {
                Success = true,
                Message = "Đổi mật khẩu thành công."
            };
        }

    }
}
