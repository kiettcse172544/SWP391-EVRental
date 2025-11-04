using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Data.Entities
{
    public class Vehicle : BaseEntity
    {
        public string PlateNumber { get; set; }
        public int ModelID { get; set; }
        public int StationID { get; set; }
        public int BatteryLevel { get; set; }
        public int Odometer { get; set; }
        public int Status { get; set; } //1: Available, 2: Reserved, 3: InUse, Maintenance
        public virtual Station Station { get; set; }
        public virtual Model Model { get; set; }
        public virtual ICollection<HandoverAndReturn> HandoverAndReturns { get; set; }
        public virtual ICollection<CarItem> CarItems { get; set; }
    }
}
