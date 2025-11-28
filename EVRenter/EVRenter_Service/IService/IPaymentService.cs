using EVRenter_Data.Entities;
using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.IService
{
    public interface IPaymentService
    {
        Task<IEnumerable<Payment>> GetAllPaymentsAsync();


        Task<PaymentResponseModel> CreatePaymentAsync(PaymentCreateRequest request, string ipAddr);


        Task<bool> HandleVnPayCallbackAsync(PaymentCallbackRequest callback);


        Task<IEnumerable<PaymentResponseModel>> GetPaymentsByUserAsync(int userId);

        Task Update(int bookingId, int paymentId);
    }
}
