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
using EVRenter_Service.IService;
using EVRenter_Repository.Repositories.Auth;

namespace EVRenter_Service.Service
{
    

    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IAuthRepository _authRepository;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration, IEmailService emailService, IAuthRepository authRepository)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _emailService = emailService;
            _authRepository = authRepository;
        }

        public async Task<LoginResponseModel> LoginAsync(LoginRequestModel request)
        {
            
            var user = await _authRepository.GetUserByEmailAsync(request.Email);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            
            if (!user.IsEmailVerified)
                throw new UnauthorizedAccessException("Email not verified.");

            
            if (!user.IsActive)
                throw new UnauthorizedAccessException("Account is inactive.");

            
            if (!PasswordTools.VerifyPassword(request.Password.Trim(), user.Password))
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
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim("address", user.Address),
                    new Claim("stationId", isStaff && user.StationId.HasValue
                        ? user.StationId.Value.ToString()
                        : string.Empty),
                    new Claim("verifiedStatus", user.IsVerified.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
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
            bool emailExists = await _authRepository.CheckEmail(request.Email);
            if (emailExists)
                throw new Exception("Email đã được đăng ký.");

            
            bool phoneExists = await _authRepository.CheckPhoneAsync(request.Phone);
            if (phoneExists)
                throw new Exception("Số điện thoại đã được đăng ký.");

            
            string hashedPassword = PasswordTools.HashPassword(request.Password.Trim());

            
            string token = Guid.NewGuid().ToString();

           
            var user = new User
            {
                FullName = request.FullName.Trim(),
                Email = request.Email.Trim(),
                Phone = request.Phone.Trim(),
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

            await _authRepository.AddUserAsync(user);

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
            

            var user = await _authRepository.GetUserByToken(token);

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

            _authRepository.UpdateUserAsync(user);

            return new VerifyEmailResponseModel
            {
                Success = true,
                Message = "Xác thực tài khoản thành công."
            };
        }

        public async Task<ChangePasswordResponseModel> ChangePasswordAsync(ChangePasswordRequestModelV2 request)
        {
            var userRepo = _unitOfWork.Repository<User>();

            
            var user = await _authRepository.GetUserById(request.UserId);

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
            await _authRepository.UpdateUserAsync(user);

            return new ChangePasswordResponseModel
            {
                Success = true,
                Message = "Đổi mật khẩu thành công."
            };
        }

    }
}
