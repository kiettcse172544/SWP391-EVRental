using AutoMapper;
using AutoMapper.QueryableExtensions;
using EVRenter_CM.Enums;
using EVRenter_Data.Entities;
using EVRenter_Repository.UnitOfWork;
using EVRenter_Repository.Utils;
using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EVRenter_Service.Service
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseModel>> GetAllUsersAsync();
        Task<UserResponseModel?> GetUserByIdAsync(int id);
        Task<UserResponseModel> CreateUserAsync(UserCreateRequest request);
        Task<UserResponseModel?> UpdateUserAsync(int id, UserUpdateRequest request);
        Task<bool> DeleteUserAsync(int id);
    }
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Lấy tất cả người dùng
        public async Task<IEnumerable<UserResponseModel>> GetAllUsersAsync()
        {
            return await _unitOfWork.Repository<User>().AsQueryable()
                .Where(u => !u.IsDelete)
                .ProjectTo<UserResponseModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        // Lấy tất cả người thuê
        public async Task<IEnumerable<UserResponseModel>> GetAllRentersAsync()
        {
            return await _unitOfWork.Repository<User>().AsQueryable()
                .Where(u => !u.IsDelete && u.RoleID == RoleType.Renter) // error
                .ProjectTo<UserResponseModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        // Lấy người dùng theo ID
        public async Task<UserResponseModel?> GetUserByIdAsync(int id)
        {
            // Get the user with basic information
            var user = await _unitOfWork.Repository<User>().AsQueryable()
                .Where(u => u.Id == id)
                .ProjectTo<UserResponseModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return user;
        }

        // Tạo người dùng mới
        public async Task<UserResponseModel> CreateUserAsync(UserCreateRequest request)
        {
            if (!Regex.IsMatch(request.Phone, "^\\+?[0-9]{10,15}$"))
            {
                throw new ArgumentException("Invalid phone number format.");
            }

            var existingUser = await _unitOfWork.Repository<User>().FindAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email already exists.");
            }

            var user = _mapper.Map<User>(request);
            user.Password = PasswordTools.HashPassword(user.Password);

            await _unitOfWork.Repository<User>().InsertAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var createdUser = await _unitOfWork.Repository<User>()
                .AsQueryable()
                .Where(u => u.Id == user.Id)
                .ProjectTo<UserResponseModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (createdUser == null)
            {
                throw new Exception("Failed to retrieve created user.");
            }

            return createdUser;
        }

        public async Task<UserResponseModel?> UpdateUserAsync(int id, UserUpdateRequest request)
        {
            var existingUser = await _unitOfWork.Repository<User>()
                .AsQueryable()
                .Where(u => u.Id == id && !u.IsDelete)
                .FirstOrDefaultAsync();

            if (existingUser == null) return null;

            // Kiểm tra xem có bất kỳ trường nào được cập nhật không
            bool hasUpdates = false;

            if (!string.IsNullOrEmpty(request.Email))
            {
                var emailUser = await _unitOfWork.Repository<User>()
                    .FindAsync(u => u.Email == request.Email && u.Id != id);

                if (emailUser != null)
                {
                    throw new InvalidOperationException("Email already exists.");
                }
                existingUser.Email = request.Email;
                hasUpdates = true;
            }

            // Cập nhật từng trường nếu có giá trị mới
            if (!string.IsNullOrEmpty(request.FullName))
            {
                existingUser.FullName = request.FullName;
                hasUpdates = true;
            }

            if (!string.IsNullOrEmpty(request.Phone))
            {
                existingUser.Phone = request.Phone;
                hasUpdates = true;
            }

            if (!string.IsNullOrEmpty(request.Address))
            {
                existingUser.Address = request.Address;
                hasUpdates = true;
            }

            if (request.IsEmailVerified.HasValue)
            {
                existingUser.IsEmailVerified = request.IsEmailVerified.Value;
                hasUpdates = true;
            }

            if (request.RoleID.HasValue)
            {
                existingUser.RoleID = (RoleType)request.RoleID.Value;
                hasUpdates = true;
            }

            if (hasUpdates)
            {
                await _unitOfWork.Repository<User>().Update(existingUser, id);
                await _unitOfWork.SaveChangesAsync();
            }

            return _mapper.Map<UserResponseModel>(existingUser);

        }


        // Xóa người dùng
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _unitOfWork.Repository<User>().GetById(id);
            if (user == null) return false;

            user.IsDelete = true;
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

    }
}
