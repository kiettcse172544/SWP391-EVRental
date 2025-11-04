using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Data.Entities
{
    public class StaffProfile
    {
        public int UserID { get; set; }
        public int StationID { get; set; }
        public string StaffCode { get; set; }
        public int IsDelete { get; set; }
        public virtual Station Station { get; set; }
        public virtual User User { get; set; }
    }
}
