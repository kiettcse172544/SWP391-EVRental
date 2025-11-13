using EVRenter_CM.Enums;
using EVRenter_Data.Entities;
using EVRenter_Repository.UnitOfWork;
using EVRenter_Repository.Utils;
using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EVRenter_Service.Service
{
    public interface IAuthService
    {
        Task<LoginResponseModel> LoginAsync(LoginRequestModel request);
    }

    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
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
    }
}
