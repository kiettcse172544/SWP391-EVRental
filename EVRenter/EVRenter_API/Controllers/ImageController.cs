using EVRenter_API.FormModels;
using EVRenter_Service.RequestModel;
using EVRenter_Service.Service;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
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
        [Authorize]
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




        //[HttpPost("upload-id")]
        //public async Task<IActionResult> UploadIDImage([FromForm] IDImageUploadForm form)
        //{
        //    using var ms = new MemoryStream();
        //    await form.File.CopyToAsync(ms);

        //    var request = new UploadIDImageRequest
        //    {
        //        RenterID = form.RenterId,
        //        ImageData = ms.ToArray(),
        //        ContentType = form.File.ContentType
        //    };

        //    var result = await _imageService.UploadIDImageAsync(request);
        //    return Ok(result);
        //}



        //[HttpPost("upload-driver-license")]
        //public async Task<IActionResult> UploadDriverLicenseImage([FromForm] DriverLicenseImageUploadForm form)
        //{
        //    using var ms = new MemoryStream();
        //    await form.File.CopyToAsync(ms);

        //    var request = new UploadDriverLicenseImageRequest
        //    {
        //        RenterID = form.RenterId,
        //        ImageData = ms.ToArray(),
        //        ContentType = form.File.ContentType
        //    };

        //    var result = await _imageService.UploadDriverLicenseImageAsync(request);
        //    return Ok(result);
        //}

        [HttpGet("file/{imageId}")]
        [Authorize]
        public async Task<IActionResult> GetImageFile(int imageId)
        {
            var image = await _imageService.GetImageByIdAsync(imageId);
            if (image == null)
                return NotFound("Image not found.");

            return File(image.ImageData, image.ContentType);
        }

        [HttpGet("model/{modelId}")]
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> GetVehicleImages(int vehicleId)
        {
            var base64Images = await _imageService.GetImagesByVehicleIdAsync(vehicleId);

            if (!base64Images.Any())
                return NotFound("No images found for this vehicle.");

            return Ok(base64Images);  
        }



        [HttpGet("id/{renterId}")]
        [Authorize]
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
        [Authorize]
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

        [HttpPost("vehicle/delete-by-base64")]
        [Authorize]
        public async Task<IActionResult> DeleteByBase64([FromBody] DeleteVehicleImageRequest request)
        {
            try
            {
                var result = await _imageService.DeleteVehicleImageByBase64Async(
                    request.VehicleId,
                    request.Base64Image
                );

                return Ok(new { message = "Xóa ảnh thành công." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }
}
