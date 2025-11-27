using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.IService
{
    public interface IExtraFeeService
    {
        Task<ExtraFeeResponseModel> CreateExtraFeeAsync(ExtraFeeCreateRequest request);
        Task<IEnumerable<ExtraFeeResponseModel>> GetAllExtraFeesAsync();
        Task<IEnumerable<ExtraFeeResponseModel>> GetExtraFeesByBookingAsync(int bookingId);
    }
}
