using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.IService
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
}
