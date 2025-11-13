using EVRenter_Service.RequestModel;
using EVRenter_Service.Service;
using Microsoft.AspNetCore.Mvc;

namespace EVRenter_API.Controllers
{
    [Route("api/images")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        
        [HttpPost("upload-model")]
        public async Task<IActionResult> UploadModelImage(
            [FromForm] int modelId,
            [FromForm] IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            var request = new UploadModelImageRequest
            {
                ModelID = modelId,
                ImageData = ms.ToArray(),
                ContentType = file.ContentType
            };

            var result = await _imageService.UploadModelImageAsync(request);
            return Ok(result);
        }


        
        [HttpPost("upload-vehicle")]
        public async Task<IActionResult> UploadVehicleImage(
            [FromForm] int vehicleId,
            [FromForm] IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            var request = new UploadVehicleImageRequest
            {
                VehicleID = vehicleId,
                ImageData = ms.ToArray(),
                ContentType = file.ContentType
            };

            var result = await _imageService.UploadVehicleImageAsync(request);
            return Ok(result);
        }


        
        [HttpPost("upload-id")]
        public async Task<IActionResult> UploadIDImage(
            [FromForm] int renterId,
            [FromForm] IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            var request = new UploadIDImageRequest
            {
                RenterID = renterId,
                ImageData = ms.ToArray(),
                ContentType = file.ContentType
            };

            var result = await _imageService.UploadIDImageAsync(request);
            return Ok(result);
        }


        
        [HttpPost("upload-driver-license")]
        public async Task<IActionResult> UploadDriverLicenseImage(
            [FromForm] int renterId,
            [FromForm] IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            var request = new UploadDriverLicenseImageRequest
            {
                RenterID = renterId,
                ImageData = ms.ToArray(),
                ContentType = file.ContentType
            };

            var result = await _imageService.UploadDriverLicenseImageAsync(request);
            return Ok(result);
        }
    }
}
