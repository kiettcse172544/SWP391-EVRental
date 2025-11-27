using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.IService
{
    public interface IRenterProfileService
    {
        Task<UploadRPResponseModel> CreateOrUpdateProfileAsync(UploadRPRequestModel request);
        Task<bool> ApproveProfileAsync(int profileId);
        Task<bool> RejectProfileAsync(int profileId);
        Task<UploadRPResponseModel> GetProfileByUserIdAsync(int userId);
    }
}
