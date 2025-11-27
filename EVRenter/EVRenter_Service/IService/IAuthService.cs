using EVRenter_Service.RequestModel.register;
using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel.register;
using EVRenter_Service.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.IService
{
    public interface IAuthService
    {
        Task<LoginResponseModel> LoginAsync(LoginRequestModel request);
        Task<SignupResponseModel> RegisterAsync(SignupRequestModel request);
        Task<VerifyEmailResponseModel> VerifyEmailAsync(string token);
        Task<ChangePasswordResponseModel> ChangePasswordAsync(ChangePasswordRequestModelV2 request);

    }
}
