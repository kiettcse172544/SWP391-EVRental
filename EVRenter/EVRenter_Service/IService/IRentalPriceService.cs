using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.IService
{
    public interface IRentalPriceService
    {
        Task<IEnumerable<RentalPriceResponse>> GetAllRentalPrice();
        Task<RentalPriceResponse?> GetPriceByIdAsync(int id);
        Task<RentalPriceResponse?> GetPriceByModelAsync(int modelId);
        Task<RentalPriceResponse> CreatePriceAsync(PriceRequestModel request);
        Task<RentalPriceResponse> UpdatePriceByModelAsync(PriceUpdateRequest request);
        Task<bool> DeletePriceAsync(int id);

    }
}
