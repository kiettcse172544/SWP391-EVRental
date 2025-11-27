using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.IService
{
    public interface IHandoverService
    {
        Task<HandoverResponseModel> CreateHandoverAsync(HandoverCreateRequest request);
        Task<bool> ConfirmHandoverAsync(int handoverId);
        Task<IEnumerable<HandoverResponseModel>> GetAllHandoversAsync();
        Task<HandoverResponseModel?> GetHandoverByIdAsync(int id);
    }
}
