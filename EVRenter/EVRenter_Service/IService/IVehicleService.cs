using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.IService
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleResponseModel>> GetAllVehicle();
        Task<VehicleResponseModel?> GetVehicleByIdAsync(int id);
        Task<IEnumerable<VehicleResponseModel>> GetAllVehicleByStation(int stationID);
        Task<VehicleResponseModel> CreateVehicleAsync(VehicleRequestModel request);
        Task<VehicleResponseModel?> UpdateVehicleAsync(int id, VehicleUpdateRequest request);
        Task<VehicleResponseModel?> UpdateVehicleStatusAsync(int vehicleId);
        Task<bool> UpdateCarItemsByVehicleAsync(UpdateCarItemsRequest request);
        Task<bool> DeleteVehicleAsync(int id);
        Task<VehicleResponseModel?> StaffRefusingAsync(int vehicleId);
    }
}
