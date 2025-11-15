using AutoMapper;
using AutoMapper.QueryableExtensions;
using EVRenter_CM.Enums;
using EVRenter_Data.Entities;
using EVRenter_Repository.UnitOfWork;
using EVRenter_Repository.Utils;
using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

        Task<IEnumerable<RenterResponseModel>> GetAllRentersAsync();
        Task<RenterResponseModel?> GetRentalByIdAsync(int id);
        Task<RenterResponseModel> InitializeRenterProfileAsync(RenterProfileRequest request);
        Task<RenterResponseModel?> UpdateRenterAsync(int id, RenterUpdateRequest request);

        Task<bool> DeleteUserAsync(int id);

        Task<bool> UpdateVerifiedStatus(int id, int newStatus);
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
        public async Task<IEnumerable<RenterResponseModel>> GetAllRentersAsync()
        {
            return await _unitOfWork.Repository<User>().AsQueryable()
                .Where(u => !u.IsDelete && u.RoleID == RoleType.Renter) // error
                .ProjectTo<RenterResponseModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        // Lấy người dùng theo ID
        public async Task<UserResponseModel?> GetUserByIdAsync(int id)
        {
            // Get the user with basic information
            var user = await _unitOfWork.Repository<User>().AsQueryable()
                .Where(u => !u.IsDelete && u.Id == id)
                .ProjectTo<UserResponseModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return user;
        }

        // Lấy người thuê theo ID
        public async Task<RenterResponseModel?> GetRentalByIdAsync(int id)
        {
            // Get the user with basic information
            var user = await _unitOfWork.Repository<User>().AsQueryable()
                .Where(u => !u.IsDelete && u.Id == id && u.RoleID == RoleType.Renter)
                .ProjectTo<RenterResponseModel>(_mapper.ConfigurationProvider)
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

        // Tạo hồ sơ người thuê mới
        public async Task<RenterResponseModel> InitializeRenterProfileAsync(RenterProfileRequest request)
        {

            var existingUser = await _unitOfWork.Repository<User>().FindAsync(u => u.Id == request.UserID);
            if (existingUser == null)
            {
                throw new InvalidOperationException("User is not found.");
            }

            var checkId = await _unitOfWork.Repository<RenterProfile>().FindAsync(u => u.IDNumber == request.IDNumber ||
            u.DriverLicenseNo == request.DriverLicenseNo);
            if (checkId != null)
            {
                throw new InvalidOperationException("IDNumber or DriverLicenseNo is already exist.");
            }

            var renter = _mapper.Map<RenterProfile>(request);

            await _unitOfWork.Repository<RenterProfile>().InsertAsync(renter);
            await _unitOfWork.SaveChangesAsync();

            var createdUser = await _unitOfWork.Repository<User>()
                .AsQueryable()
                .Where(u => u.Id == renter.UserID)
                .ProjectTo<RenterResponseModel>(_mapper.ConfigurationProvider)
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

        public async Task<RenterResponseModel?> UpdateRenterAsync(int id, RenterUpdateRequest request)
        {
            var existingUser = await _unitOfWork.Repository<User>()
                .AsQueryable()
                .Where(u => u.Id == id && !u.IsDelete)
                .FirstOrDefaultAsync();
            if (existingUser == null) return null;

            var existingRenter = await _unitOfWork.Repository<RenterProfile>()
                .AsQueryable()
                .Where(u => u.UserID == id && !u.IsDelete)
                .FirstOrDefaultAsync();
            if (existingRenter == null) return null;

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

            //if (!string.IsNullOrEmpty(request.IDNumber))
            //{
            //    existingRenter.IDNumber = request.IDNumber;
            //    hasUpdates = true;
            //}

            //if (!string.IsNullOrEmpty(request.DriverLicenseNo))
            //{
            //    existingRenter.DriverLicenseNo = request.DriverLicenseNo;
            //    hasUpdates = true;
            //}

            if (request.Type.HasValue)
            {
                existingRenter.Type = request.Type.Value;
                hasUpdates = true;
            }

            if (request.IsEmailVerified.HasValue)
            {
                existingUser.IsEmailVerified = request.IsEmailVerified.Value;
                hasUpdates = true;
            }

            if (hasUpdates)
            {
                await _unitOfWork.Repository<User>().Update(existingUser, id);
                await _unitOfWork.Repository<RenterProfile>().Update(existingRenter, id);
                await _unitOfWork.SaveChangesAsync();
            }

            return _mapper.Map<RenterResponseModel>(existingUser);

        }


        // Xóa người dùng
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _unitOfWork.Repository<User>().GetById(id);
            if (user == null) return false;

            user.IsDelete = true;

            var renterProfile = await _unitOfWork.Repository<RenterProfile>()
                .AsQueryable()
                .Where(u => u.UserID == id && !u.IsDelete)
                .FirstOrDefaultAsync();
            if (renterProfile == null)
            {
                renterProfile.IsDelete = true ;
            }
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        
        public async Task<bool> UpdateVerifiedStatus(int id, int newStatus)
        {
            var user = await _unitOfWork.Repository<User>()
                            .AsQueryable()
                            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDelete);

            if(user == null)
            {
                throw new Exception("User not found");
            }

            user.IsVerified = newStatus;

            await _unitOfWork.Repository<User>().UpdateAsync(user);

            int check = await _unitOfWork.SaveChangesAsync();

            return check > 0;
        }
    }
}
