using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Data.Entities
{
    public class CarItem : BaseEntity
    {
        public int VehicleID { get; set; }
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public virtual ItemCategory Category { get; set; }
        public virtual Vehicle Vehicle { get; set; }
    }
}
