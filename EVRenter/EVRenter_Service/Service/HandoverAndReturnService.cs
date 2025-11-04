using EVRenter_Data;
using EVRenter_Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVRenter_Service
{
    public class HandoverAndReturnService
    {
        private readonly AppDbContext _context;

        public HandoverAndReturnService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<HandoverAndReturn> CreateAsync(HandoverAndReturn entity)
        {
            _context.HandoverAndReturn.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }


        public async Task<List<HandoverAndReturn>> GetAllAsync()
        {
            return await _context.HandoverAndReturn
                .Include(h => h.Booking)
                .Include(h => h.Vehicle)
                .Include(h => h.Station)
                .Include(h => h.User)
                .ToListAsync();
        }


        public async Task<HandoverAndReturn?> GetByIdAsync(int id)
        {
            return await _context.HandoverAndReturn
                .Include(h => h.Booking)
                .Include(h => h.Vehicle)
                .Include(h => h.Station)
                .Include(h => h.User)
                .FirstOrDefaultAsync(h => h.Id == id);
        }


        public async Task<List<HandoverAndReturn>> GetByBookingIdAsync(int bookingId)
        {
            return await _context.HandoverAndReturn
                .Include(h => h.Vehicle)
                .Include(h => h.Station)
                .Include(h => h.User)
                .Where(h => h.BookingID == bookingId)
                .ToListAsync();
        }


        public async Task<bool> UpdateAsync(HandoverAndReturn entity)
        {
            var existing = await _context.HandoverAndReturn.FindAsync(entity.Id);
            if (existing == null)
                return false;

            _context.Entry(existing).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.HandoverAndReturn.FindAsync(id);
            if (existing == null)
                return false;

            _context.HandoverAndReturn.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
