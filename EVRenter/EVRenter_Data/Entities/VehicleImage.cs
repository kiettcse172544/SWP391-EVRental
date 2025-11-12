using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Data.Entities
{
    public class VehicleImage
    {
        public int VehicleID { get; set; }      
        public int ImageID { get; set; }        

        public virtual Vehicle Vehicle { get; set; }
        public virtual Image Image { get; set; }
    }
}
