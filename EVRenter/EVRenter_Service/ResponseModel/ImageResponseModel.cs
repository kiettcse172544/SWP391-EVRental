using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.ResponseModel
{
    public class UploadRPResponseModel
    {
        public int RenterId { get; set; }

        public string IDNumber { get; set; }
        public string DriverLicenseNo { get; set; }

        
        public byte[] IDCardFrontImage { get; set; }
        public byte[] IDCardBackImage { get; set; }

        public byte[] DriverLicenseFrontImage { get; set; }
        public byte[] DriverLicenseBackImage { get; set; }

       
        public int VerificationStatus { get; set; }
        public string Message { get; set; }
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
