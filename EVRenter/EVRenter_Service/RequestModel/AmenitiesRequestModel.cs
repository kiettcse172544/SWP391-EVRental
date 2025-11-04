using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.RequestModel
{
    public class AmenitiesRequestModel
    {
        public int ModelID { get; set; }
        public IEnumerable<EachAmenityModel> Amenities { get; set; }
    }

    public class EachAmenityModel
    {
        public string Name { get; set; }
    }
}
