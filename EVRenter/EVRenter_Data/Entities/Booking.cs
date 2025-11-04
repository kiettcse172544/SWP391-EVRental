using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Data.Entities
{
    public class Booking : BaseEntity
    {
        public int VehicleID { get; set; }
        public int RenterID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RentalType { get; set; }
        [Precision(18, 2)]
        public decimal Deposit { get; set; }
        [Precision(18, 2)]
        public decimal RetalCost { get; set; }
        public int? VoucherID { get; set; }
        [Precision(18, 2)]
        public decimal BaseCost { get; set; }
        [Precision(18, 2)]
        public decimal? FinalCost { get; set; }
        public int Status { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<HandoverAndReturn> HandoverAndReturns { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<ExtraFee> ExtraFees { get; set; }
        public virtual Voucher Voucher { get; set; }
    }
}
