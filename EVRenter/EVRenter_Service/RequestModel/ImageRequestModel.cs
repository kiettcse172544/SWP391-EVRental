using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.RequestModel
{
    public class ImageRequestModel
    {
    }

    public class UploadRPRequestModel
    {
        public int RenterId { get; set; }

        public string IDNumber { get; set; }
        public string DriverLicenseNo { get; set; }

        
        public byte[] IDCardFrontImage { get; set; }
        public byte[] IDCardBackImage { get; set; }

        
        public byte[] DriverLicenseFrontImage { get; set; }
        public byte[] DriverLicenseBackImage { get; set; }
    }



    public class UploadModelImageRequest
    {
        public int ModelID { get; set; }
        public byte[] ImageData { get; set; }
        public string ContentType { get; set; }
    }


    public class UploadVehicleImageRequest
    {
        public int VehicleID { get; set; }
        public byte[] ImageData { get; set; }
        public string ContentType { get; set; }
    }


    public class UploadIDImageRequest
    {
        public int RenterID { get; set; }
        public byte[] ImageData { get; set; }
        public string ContentType { get; set; }
    }


    public class UploadDriverLicenseImageRequest
    {
        public int RenterID { get; set; }
        public byte[] ImageData { get; set; }
        public string ContentType { get; set; }
    }

    public class DeleteVehicleImageRequest
    {
        public int VehicleId { get; set; }
        public string Base64Image { get; set; }
    }

}
