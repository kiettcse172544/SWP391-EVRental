using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Data.Entities
{
    public class Amenities : BaseEntity
    {
        public string AmenityName { get; set; }
        public int ModelID { get; set; }
        public virtual Model Model { get; set; }
    }
}
