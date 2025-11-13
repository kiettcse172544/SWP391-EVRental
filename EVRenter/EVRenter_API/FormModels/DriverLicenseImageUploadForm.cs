using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVRenter_API.FormModels
{
    public class DriverLicenseImageUploadForm
    {
        [FromForm(Name = "renterId")]
        public int RenterId { get; set; }

        [FromForm(Name = "file")]
        public IFormFile File { get; set; }
    }
}
