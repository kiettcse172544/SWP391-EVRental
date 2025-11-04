using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Data.Entities
{
    public class HandoverAndReturn : BaseEntity
    {
        public int BookingID { get; set; }
        public int StaffID { get; set; }
        public int VehicleID { get; set; }
        public int StationID { get; set; }
        public int Type { get; set; }
        public DateTime CheckDate { get; set; }
        public string Exterior { get; set; }
        public string Interior { get; set; }
        public string Technical { get; set; }
        public string Accessories { get; set; }
        public string? Decription { get; set; }
        public int Status { get; set; }

        public virtual User User { get; set; }
        public virtual Booking Booking { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public virtual Station Station { get; set; }
    }
}
