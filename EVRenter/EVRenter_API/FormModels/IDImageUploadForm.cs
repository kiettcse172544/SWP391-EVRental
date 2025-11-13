using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVRenter_API.FormModels
{
    public class IDImageUploadForm
    {
        [FromForm(Name = "renterId")]
        public int RenterId { get; set; }

        [FromForm(Name = "file")]
        public IFormFile File { get; set; }
    }
}
