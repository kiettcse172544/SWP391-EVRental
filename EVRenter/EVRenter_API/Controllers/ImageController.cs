using EVRenter_API.FormModels;
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
        public async Task<IActionResult> UploadModelImage([FromForm] ModelImageUploadForm form)
        {
            using var ms = new MemoryStream();
            await form.File.CopyToAsync(ms);

            var request = new UploadModelImageRequest
            {
                ModelID = form.ModelId,
                ImageData = ms.ToArray(),
                ContentType = form.File.ContentType
            };

            var result = await _imageService.UploadModelImageAsync(request);
            return Ok(result);
        }



        [HttpPost("upload-vehicle")]
        public async Task<IActionResult> UploadVehicleImage([FromForm] VehicleImageUploadForm form)
        {
            using var ms = new MemoryStream();
            await form.File.CopyToAsync(ms);

            var request = new UploadVehicleImageRequest
            {
                VehicleID = form.VehicleId,
                ImageData = ms.ToArray(),
                ContentType = form.File.ContentType
            };

            var result = await _imageService.UploadVehicleImageAsync(request);
            return Ok(result);
        }



        [HttpPost("upload-id")]
        public async Task<IActionResult> UploadIDImage([FromForm] IDImageUploadForm form)
        {
            using var ms = new MemoryStream();
            await form.File.CopyToAsync(ms);

            var request = new UploadIDImageRequest
            {
                RenterID = form.RenterId,
                ImageData = ms.ToArray(),
                ContentType = form.File.ContentType
            };

            var result = await _imageService.UploadIDImageAsync(request);
            return Ok(result);
        }



        [HttpPost("upload-driver-license")]
        public async Task<IActionResult> UploadDriverLicenseImage([FromForm] DriverLicenseImageUploadForm form)
        {
            using var ms = new MemoryStream();
            await form.File.CopyToAsync(ms);

            var request = new UploadDriverLicenseImageRequest
            {
                RenterID = form.RenterId,
                ImageData = ms.ToArray(),
                ContentType = form.File.ContentType
            };

            var result = await _imageService.UploadDriverLicenseImageAsync(request);
            return Ok(result);
        }

        [HttpGet("file/{imageId}")]
        public async Task<IActionResult> GetImageFile(int imageId)
        {
            var image = await _imageService.GetImageByIdAsync(imageId);
            if (image == null)
                return NotFound("Image not found.");

            return File(image.ImageData, image.ContentType);
        }

        [HttpGet("model/{modelId}")]
        public async Task<IActionResult> GetModelImages(int modelId)
        {
            var images = await _imageService.GetImagesByModelIdAsync(modelId);

            
            var result = images.Select(i => new
            {
                imageId = i.ImageID,
                contentType = i.ContentType
            });

            return Ok(result);
        }

        [HttpGet("vehicle/{vehicleId}")]
        public async Task<IActionResult> GetVehicleImages(int vehicleId)
        {
            var images = await _imageService.GetImagesByVehicleIdAsync(vehicleId);

            var result = images.Select(i => new
            {
                imageId = i.ImageID,
                contentType = i.ContentType
            });

            return Ok(result);
        }

        [HttpGet("id/{renterId}")]
        public async Task<IActionResult> GetIDImages(int renterId)
        {
            var images = await _imageService.GetIDImagesByRenterIdAsync(renterId);

            var result = images.Select(i => new
            {
                imageId = i.ImageID,
                contentType = i.ContentType
            });

            return Ok(result);
        }

        [HttpGet("driver-license/{renterId}")]
        public async Task<IActionResult> GetDriverLicenseImages(int renterId)
        {
            var images = await _imageService.GetDriverLicenseImagesByRenterIdAsync(renterId);

            var result = images.Select(i => new
            {
                imageId = i.ImageID,
                contentType = i.ContentType
            });

            return Ok(result);
        }

    }
}
