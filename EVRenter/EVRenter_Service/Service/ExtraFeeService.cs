using EVRenter_Data;
using EVRenter_Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVRenter_Service
{
    public class ExtraFeeService
    {
        private readonly AppDbContext _context;

        public ExtraFeeService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<ExtraFee> CreateAsync(ExtraFee extraFee)
        {
            _context.ExtraFee.Add(extraFee);
            await _context.SaveChangesAsync();
            return extraFee;
        }


        public async Task<List<ExtraFee>> GetAllAsync()
        {
            return await _context.ExtraFee
                .Include(e => e.FeeTypes)
                .Include(e => e.Booking)
                .Include(e => e.User)
                .ToListAsync();
        }


        public async Task<ExtraFee?> GetByIdAsync(int id)
        {
            return await _context.ExtraFee
                .Include(e => e.FeeTypes)
                .Include(e => e.Booking)
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == id);
        }


        public async Task<bool> UpdateAsync(ExtraFee extraFee)
        {
            var existing = await _context.ExtraFee.FindAsync(extraFee.Id);
            if (existing == null)
                return false;

            _context.Entry(existing).CurrentValues.SetValues(extraFee);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.ExtraFee
                .Include(e => e.FeeTypes)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (existing == null)
                return false;


            if (existing.FeeTypes != null && existing.FeeTypes.Any())
                _context.FeeType.RemoveRange(existing.FeeTypes);

            _context.ExtraFee.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
