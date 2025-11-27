using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.IService
{
    public interface IModelService
    {
        Task<IEnumerable<ModelResponseModel>> GetAllModel();
        Task<ModelResponseModel?> GetModelByIdAsync(int id);
        Task<IEnumerable<ModelResponseModel>> GetModelByStationAsync(int stationId);
        Task<IEnumerable<ModelResponseModel>> GetModelQuantityByStationIdAsync(int stationID);
        Task<IEnumerable<ModelResponseModel>> GetAllModelQuantityAsync();
        Task<ModelResponseModel> CreateModelAsync(ModelRequestModel request);
        Task<ModelResponseModel?> UpdateModelAsync(int id, ModelUpdateRequest request);
        Task<bool> DeleteModelAsync(int id);
        Task RebootModelQuantitiesAsync();
    }
}
