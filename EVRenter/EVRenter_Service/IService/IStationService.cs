using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.IService
{
    public interface IStationService
    {
        Task<IEnumerable<StationResponseModel>> GetAllStation();
        Task<StationResponseModel?> GetStationByIdAsync(int id);
        Task<StationResponseModel> CreateStationAsync(StationRequestModel request);
        Task<StationResponseModel?> UpdateStationAsync(int id, StationUpdateRequest request);
        Task RebootStationQuantitiesAsync();
        Task<bool> DeleteStationAsync(int id);
    }
}
