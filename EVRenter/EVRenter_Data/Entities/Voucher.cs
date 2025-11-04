using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Data.Entities
{
    public class Voucher : BaseEntity
    {
        public int AppliedType {  get; set; }
        [Precision(18, 2)]
        public decimal SalePercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Status { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; } 

    }
}
