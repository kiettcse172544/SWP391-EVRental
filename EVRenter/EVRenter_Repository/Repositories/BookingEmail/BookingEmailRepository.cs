using EVRenter_Data.Entities;
using EVRenter_Repository.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Repository.Repositories.BookingEmail
{
    public class BookingEmailRepository : IBookingEmail
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingEmailRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Booking> GetBookingById(int id)
        {
            return await _unitOfWork.Repository<Booking>()
                .AsQueryable()
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDelete);
        }

        public async Task<Booking> GetBookingByToken(string token)
        {
            return await _unitOfWork.Repository<Booking>()
                .AsQueryable()
                .FirstOrDefaultAsync(b => b.SignatureToken == token && !b.IsDelete);
        }

        public async Task<Vehicle> GetVehicleById(int id)
        {
            return await _unitOfWork.Repository<Vehicle>()
                            .AsQueryable()
                            .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task UpdateBooking(Booking booking)
        {
            await _unitOfWork.Repository<Booking>().UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateVehicle(Vehicle vehicle)
        {
            await _unitOfWork.Repository<Vehicle>().UpdateAsync(vehicle);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
