using EVRenter_Data;
using EVRenter_Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVRenter_Service
{
    public class FeeTypeService
    {
        private readonly AppDbContext _context;

        public FeeTypeService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<FeeType> CreateAsync(FeeType feeType)
        {
            _context.FeeType.Add(feeType);
            await _context.SaveChangesAsync();
            return feeType;
        }


        public async Task<List<FeeType>> GetAllAsync()
        {
            return await _context.FeeType
                .Include(f => f.ExtraFee)
                .ToListAsync();
        }


        public async Task<FeeType?> GetByIdAsync(int id)
        {
            return await _context.FeeType
                .Include(f => f.ExtraFee)
                .FirstOrDefaultAsync(f => f.Id == id);
        }


        public async Task<bool> UpdateAsync(FeeType feeType)
        {
            var existing = await _context.FeeType.FindAsync(feeType.Id);
            if (existing == null)
                return false;

            _context.Entry(existing).CurrentValues.SetValues(feeType);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.FeeType.FindAsync(id);
            if (existing == null)
                return false;

            _context.FeeType.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<List<FeeType>> GetByExtraFeeIdAsync(int extraFeeId)
        {
            return await _context.FeeType
                .Where(f => f.ExtraFeeID == extraFeeId)
                .ToListAsync();
        }
    }
}
