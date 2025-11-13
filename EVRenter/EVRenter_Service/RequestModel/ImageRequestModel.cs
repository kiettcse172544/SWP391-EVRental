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
        public string IDNumber {  get; set; }
        public string DriverLicenseNo { get; set; }

        public byte[] IDNumberImage1 { get; set; }
        public byte[] IDNumberImage2 { get; set; }

        public byte[] DriverLicenseImage1 { get; set; }
        public byte[] DriverLicenseImage2 { get;set; }
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


}
