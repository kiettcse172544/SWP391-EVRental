using EVRenter_Data;
using EVRenter_Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Repository.Amenities
{
    public class AmenitiesRepository
    {
        private readonly AppDbContext _context;

        public AmenitiesRepository(AppDbContext context)
        {
            _context = context;
        }


    }
}
