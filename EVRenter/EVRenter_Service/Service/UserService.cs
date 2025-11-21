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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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
        Task<StaffResponseModel> CreateUserAsync(UserCreateRequest request);
        Task<UserResponseModel?> UpdateUserAsync(int id, UserUpdateRequest request);

        Task<IEnumerable<RenterResponseModel>> GetAllRentersAsync();
        Task<RenterResponseModel?> GetRentalByIdAsync(int id);
        Task<RenterResponseModel> InitializeRenterProfileAsync(RenterProfileRequest request);
        Task<RenterResponseModel?> UpdateRenterAsync(int id, RenterUpdateRequest request);

        Task<StaffResponseModel> InitializeStaffProfileAsync(StaffProfileRequest request);
        Task<StaffResponseModel?> UpdateStaffAsync(int id, StaffUpdateRequest request);

        Task<bool> DeleteUserAsync(int id);

        Task<bool> UpdateVerifiedStatus(int id, int newStatus);

        Task<bool> RebootRenterType();
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
            var users = await _unitOfWork.Repository<User>().AsQueryable()
                .Where(u => !u.IsDelete)
                .Include(u => u.StaffProfile)
                    .ThenInclude(s => s.Station)
                .Include(u => u.RenterProfile)
                    .ThenInclude(p => p.IDImages)
                        .ThenInclude(i => i.Image)
                .Include(u => u.RenterProfile)
                    .ThenInclude(p => p.DriverLicenseImages)
                        .ThenInclude(i => i.Image)
                .Include(u => u.Bookings)
                .ProjectTo<UserResponseModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

            foreach (var user in users)
            {
                if (user.RoleID != 3)
                {
                    user.Renter = null;
                }
                if (user.RoleID != 2)
                {
                    user.Staff = null;
                }
            }

            return users;
        }

        // Lấy tất cả người thuê
        public async Task<IEnumerable<RenterResponseModel>> GetAllRentersAsync()
        {
            var renters = await _unitOfWork.Repository<User>().AsQueryable()
                .Where(u => !u.IsDelete && u.RoleID == RoleType.Renter)
                .Include(u => u.RenterProfile)
                    .ThenInclude(p => p.IDImages)
                        .ThenInclude(i => i.Image)
                .Include(u => u.RenterProfile)
                    .ThenInclude(p => p.DriverLicenseImages)
                        .ThenInclude(i => i.Image)
                .Include(u => u.Bookings)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RenterResponseModel>>(renters);
        }

        public async Task<bool> RebootRenterType()
        {
            var renters = await _unitOfWork.Repository<RenterProfile>().AsQueryable()
                .Where(u => !u.IsDelete)
                .ToListAsync();

            foreach (var renter in renters)
            {
                renter.Type = 1;
                await _unitOfWork.Repository<RenterProfile>().UpdateAsync(renter);
            }

            return true;
        }


        // Lấy người dùng theo ID
        public async Task<UserResponseModel?> GetUserByIdAsync(int id)
        {
            // Get the user with basic information
            var user = await _unitOfWork.Repository<User>().AsQueryable()
                .Where(u => !u.IsDelete && u.Id == id)
                .ProjectTo<UserResponseModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (user.RoleID != 3)
            {
                user.Renter = null;
            }
            if (user.RoleID != 2)
            {
                user.Staff = null;
            }

            return user;
        }

        // Lấy người thuê theo ID
        public async Task<RenterResponseModel?> GetRentalByIdAsync(int id)
        {
            var renter = await _unitOfWork.Repository<User>().AsQueryable()
                .Where(u => !u.IsDelete && u.Id == id && u.RoleID == RoleType.Renter)
                .Include(u => u.RenterProfile)
                    .ThenInclude(p => p.IDImages)
                        .ThenInclude(i => i.Image)
                .Include(u => u.RenterProfile)
                    .ThenInclude(p => p.DriverLicenseImages)
                        .ThenInclude(i => i.Image)
                .Include(u => u.Bookings)
                .FirstOrDefaultAsync();

            return _mapper.Map<RenterResponseModel>(renter);
        }


        // Tạo người dùng mới
        public async Task<StaffResponseModel> CreateUserAsync(UserCreateRequest request)
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

            if (request.StationID.HasValue)
            {
                var station = await _unitOfWork.Repository<Station>().FindAsync(u => u.Id == request.StationID.Value);
                if(station == null)
                {
                    throw new InvalidOperationException("Station not found.");
                }
            }

            var user = _mapper.Map<User>(request);
            user.Password = PasswordTools.HashPassword(user.Password);
            user.RoleID = RoleType.Staff;

            await _unitOfWork.Repository<User>().InsertAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var createdUser = await _unitOfWork.Repository<User>()
                .AsQueryable()
                .Where(u => u.Id == user.Id)
                .Include(u => u.StaffProfile)
                    .ThenInclude(s => s.Station)
                .ProjectTo<StaffResponseModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (createdUser == null)
            {
                throw new Exception("Failed to retrieve created user.");
            }

            if (request.StationID.HasValue)
            {
                var stationPro = new StaffProfileRequest
                {
                    UserID = createdUser.Id,
                    StationID = request.StationID.Value
                };

                createdUser = await InitializeStaffProfileAsync(stationPro);
            }

            return createdUser;
        }

        public async Task<StaffResponseModel> InitializeStaffProfileAsync(StaffProfileRequest request)
        {
            var existingUser = await _unitOfWork.Repository<User>().FindAsync(u => u.Id == request.UserID);
            if (existingUser == null)
            {
                throw new InvalidOperationException("User is not found.");
            }

            var staff = _mapper.Map<StaffProfile>(request);

            await _unitOfWork.Repository<StaffProfile>().InsertAsync(staff);
            await _unitOfWork.SaveChangesAsync();

            //gọi lại

            var createdUser = await _unitOfWork.Repository<User>()
                .AsQueryable()
                .Where(u => u.Id == staff.UserID)
                .Include(u => u.StaffProfile)
                    .ThenInclude(s => s.Station)
                .ProjectTo<StaffResponseModel>(_mapper.ConfigurationProvider)
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
                .Where(u => u.Id == id && !u.IsDelete && u.IsVerified == 3)
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


        public async Task<StaffResponseModel?> UpdateStaffAsync(int id, StaffUpdateRequest request)
        {
            var existingUser = await _unitOfWork.Repository<User>()
                .AsQueryable()
                .Where(u => u.Id == id && !u.IsDelete)
                .FirstOrDefaultAsync();
            if (existingUser == null) return null;

            var existingStaff = await _unitOfWork.Repository<StaffProfile>()
                .AsQueryable()
                .Where(u => u.UserID == id && !u.IsDelete)
                .FirstOrDefaultAsync();
            if (existingStaff == null) return null;

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

            if (request.StationID.HasValue)
            {
                existingStaff.StationID = request.StationID.Value;
                hasUpdates = true;
            }

            if (hasUpdates)
            {
                await _unitOfWork.Repository<User>().Update(existingUser, id);
                await _unitOfWork.Repository<StaffProfile>().UpdateAsync(existingStaff);
                await _unitOfWork.SaveChangesAsync();
            }

            var updatedUser = await _unitOfWork.Repository<User>().AsQueryable()
                .Where(u => u.Id == id)
                .Include(u => u.StaffProfile)
                    .ThenInclude(s => s.Station)
                .ProjectTo<StaffResponseModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (updatedUser == null)
            {
                throw new Exception("Failed to retrieve updated user.");
            }

            return updatedUser;

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
                renterProfile.IsDelete = true;
            }
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateVerifiedStatus(int id, int newStatus)
        {
            var user = await _unitOfWork.Repository<User>()
                            .AsQueryable()
                            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDelete);

            if (user == null)
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
