using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVRenter_API.FormModels
{
    public class VehicleImageUploadForm
    {
        [FromForm(Name = "vehicleId")]
        public int VehicleId { get; set; }

        [FromForm(Name = "file")]
        public IFormFile File { get; set; }
    }
}
