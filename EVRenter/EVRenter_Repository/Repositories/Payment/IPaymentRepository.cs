using EVRenter_Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Repository.Repositories.Payment
{
    public interface IPaymentRepository
    {
        Task<Booking> GetBookingByIdAsync(int id);
        Task UpdateBookingAsync(Booking booking);

        Task AddPaymentAsync(EVRenter_Data.Entities.Payment payment);
        Task<EVRenter_Data.Entities.Payment> GetPaymentByRefCode(string refCode);
        Task UpdatePaymentAsync(EVRenter_Data.Entities.Payment payment);

        Task<List<EVRenter_Data.Entities.Payment>> GetPaymentByUserAsync(int id);
        Task<List<EVRenter_Data.Entities.Payment>> GetPayments();
        Task<EVRenter_Data.Entities.Payment> GetPaymentById(int id);
    }
}
