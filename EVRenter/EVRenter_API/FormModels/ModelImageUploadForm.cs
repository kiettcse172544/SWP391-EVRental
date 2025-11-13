using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVRenter_API.FormModels
{
    public class ModelImageUploadForm
    {
        [FromForm(Name = "modelId")]
        public int ModelId { get; set; }

        [FromForm(Name = "file")]
        public IFormFile File { get; set; }
    }
}
