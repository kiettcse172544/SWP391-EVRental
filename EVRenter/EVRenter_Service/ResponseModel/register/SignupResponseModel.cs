using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.ResponseModel.register
{
    public class SignupResponseModel
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public bool IsEmailSent { get; set; }
        public string Message { get; set; }
    }
}
