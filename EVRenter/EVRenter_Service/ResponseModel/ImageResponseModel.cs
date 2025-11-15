using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.ResponseModel
{
    public class UploadRPResponseModel
    {
        public string IDNumber { get; set; }
        public string DriverLicenseNo { get; set; }

        public int IDFrontImageId { get; set; }
        public int IDBackImageId { get; set; }

        public int DriverLicenseFrontImageId { get; set; }
        public int DriverLicenseBackImageId { get; set; }
    }


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

        public string ImageContentType { get; set; }

        public byte[] ImageData { get; set; }
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
