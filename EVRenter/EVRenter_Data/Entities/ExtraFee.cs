using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Data.Entities
{
    public class ExtraFee : BaseEntity
    {
        public int BookingID { get; set; }
        public int UserID { get; set; }
        public int HandoverAndReturnID { get; set; }
        [Precision(18, 2)]
        public decimal Amount { get; set; }
        [Precision(18, 2)]
        public decimal Deposit { get; set; }
        public int IsRefunded { get; set; }
        [Precision(18, 2)]
        public decimal Cost { get; set; }
        public virtual ICollection<FeeType> FeeTypes { get; set; }
        public virtual Booking Booking { get; set; }
        public virtual User User { get; set; }
    }
}
