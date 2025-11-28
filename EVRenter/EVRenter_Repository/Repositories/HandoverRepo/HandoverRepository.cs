using Azure.Core;
using EVRenter_CM.Enums;
using EVRenter_Data.Entities;
using EVRenter_Repository.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Repository.Repositories.HandoverRepo
{
    public class HandoverRepository : IHandoverRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public HandoverRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddHandover(HandoverAndReturn handoverAndReturn)
        {
            await _unitOfWork.Repository<HandoverAndReturn>().AddAsync(handoverAndReturn);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> CheckStaff(int id)
        {
            return await _unitOfWork.Repository<User>()
                            .AsQueryable()
                            .AnyAsync(stf => stf.Id == id
                            && !stf.IsDelete
                            && stf.RoleID == RoleType.Staff);
        }

        public async Task<List<HandoverAndReturn>> GetAllHandovers()
        {
            return await _unitOfWork.Repository<HandoverAndReturn>()
                .AsQueryable()
                .Include(h => h.Booking)
                .Include(h => h.Vehicle)
                .Include(h => h.Station)
                .Where(h => !h.IsDelete)
                .OrderByDescending(h => h.CheckDate)
                .ToListAsync();
        }

        public async Task<Booking> GetBookingById(int id)
        {
            return await _unitOfWork.Repository<Booking>()
                .AsQueryable()
                .Include(b => b.Vehicle)
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDelete);
        }

        public async Task<List<CarItem>> GetCarItems(int vehicleId)
        {
            return await _unitOfWork.Repository<CarItem>()
                .AsQueryable()
                .Include(c => c.Category)
                .Where(c => c.VehicleID == vehicleId && !c.IsDelete)
                .ToListAsync();
        }

        public async Task<HandoverAndReturn> GetHandover(int handoverId)
        {
            return await _unitOfWork.Repository<HandoverAndReturn>()
                .GetByIdAsync(handoverId);
        }

        public async Task<HandoverAndReturn> GetHandoverById(int id)
        {
            return await _unitOfWork.Repository<HandoverAndReturn>()
                .AsQueryable()
                .Include(h => h.Booking)
                .Include(h => h.Vehicle)
                .Include(h => h.Station)
                .FirstOrDefaultAsync(h => h.Id == id && !h.IsDelete);
        }

        public async Task<Vehicle> GetVehicle(int vehicleId)
        {
            return await _unitOfWork.Repository<Vehicle>()
                .AsQueryable()
                .FirstOrDefaultAsync(v => v.Id == vehicleId && !v.IsDelete);
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
