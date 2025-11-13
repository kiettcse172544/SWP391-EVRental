using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.ResponseModel
{
    public class ImageResponseModel
    {
        public int ImageID { get; set; }
        public string ContentType { get; set; }
        public byte[] ImageData { get; set; }
    }

    public class ModelImageResponse
    {
        public int ModelID { get; set; }
        public int ImageID { get; set; }

        public string ImageContentType { get; set; }
        
    }

    public class VehicleImageResponse
    {
        public int VehicleID { get; set; }
        public int ImageID { get; set; }

        public string ImageContentType { get; set; }
        
    }

    public class IDImageResponse
    {
        public int RenterID { get; set; }
        public int ImageID { get; set; }

        public string ImageContentType { get; set; }
       
    }

    public class DriverLicenseImageResponse
    {
        public int RenterID { get; set; }
        public int ImageID { get; set; }

        public string ImageContentType { get; set; }
        
    }
}
