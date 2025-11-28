using EVRenter_Data.Entities;
using EVRenter_Repository.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Repository.Repositories.Payment
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddPaymentAsync(EVRenter_Data.Entities.Payment payment)
        {
            await _unitOfWork.Repository<EVRenter_Data.Entities.Payment>().InsertAsync(payment);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Booking> GetBookingByIdAsync(int id)
        {
            return await _unitOfWork.Repository<Booking>()
                .AsQueryable()
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDelete);
        }

        public async Task<EVRenter_Data.Entities.Payment> GetPaymentById(int id)
        {
            return await _unitOfWork.Repository<EVRenter_Data.Entities.Payment>()
                .AsQueryable()
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDelete);
        }

        public async Task<EVRenter_Data.Entities.Payment> GetPaymentByRefCode(string refCode)
        {
            return await _unitOfWork.Repository<EVRenter_Data.Entities.Payment>()
                .AsQueryable()
                .FirstOrDefaultAsync(p => p.ReferenceCode == refCode);
        }

        public async Task<List<EVRenter_Data.Entities.Payment>> GetPaymentByUserAsync(int id)
        {
            return await _unitOfWork.Repository<EVRenter_Data.Entities.Payment>()
                .AsQueryable()
                .Include(p => p.User)
                .Where(p => p.UserID == id && !p.IsDelete)
                .ToListAsync();
        }

        public async Task<List<EVRenter_Data.Entities.Payment>> GetPayments()
        {
            return await _unitOfWork.Repository<EVRenter_Data.Entities.Payment>()
                .AsQueryable()
                .Include(p => p.User)
                .Include(p => p.Booking)
                .Where(p => !p.IsDelete)
                .ToListAsync();
        }

        public async Task UpdateBookingAsync(Booking booking)
        {
            await _unitOfWork.Repository<Booking>().UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdatePaymentAsync(EVRenter_Data.Entities.Payment payment)
        {
            await _unitOfWork.Repository<EVRenter_Data.Entities.Payment>().UpdateAsync(payment);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
