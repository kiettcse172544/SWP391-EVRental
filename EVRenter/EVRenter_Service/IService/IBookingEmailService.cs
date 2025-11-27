using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.IService
{
    public interface IBookingEmailService
    {
        Task<bool> SendSignatureEmailAsync(int bookingId);
        Task<bool> ConfirmSignatureAsync(string token);
    }
}
