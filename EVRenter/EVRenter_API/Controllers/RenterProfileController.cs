using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using EVRenter_Service.Service;
using Microsoft.AspNetCore.Mvc;

namespace EVRenter_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RenterProfileController : ControllerBase
    {
        private readonly IImageService _profileService;
        private readonly ILogger<RenterProfileController> _logger;

        public RenterProfileController(
            IImageService profileService,
            ILogger<RenterProfileController> logger)
        {
            _profileService = profileService;
            _logger = logger;
        }


        [HttpPost("upload-profile")]
        public async Task<IActionResult> UploadRenterProfile([FromForm] UploadRenterProfileForm form)
        {
            byte[] idFrontBytes = await FileToBytes(form.IDFront);
            byte[] idBackBytes = await FileToBytes(form.IDBack);
            byte[] dlFrontBytes = await FileToBytes(form.DLFront);
            byte[] dlBackBytes = await FileToBytes(form.DLBack);

            var request = new UploadRPRequestModel
            {
                RenterId = form.RenterId,
                IDNumber = form.IDNumber,
                DriverLicenseNo = form.DriverLicenseNo,
                IDNumberImage1 = idFrontBytes,
                IDNumberImage2 = idBackBytes,
                DriverLicenseImage1 = dlFrontBytes,
                DriverLicenseImage2 = dlBackBytes
            };

            var result = await _profileService.CreateRenterProfileAsync(request);

            return Ok(result);
        }

        private async Task<byte[]> FileToBytes(IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            return ms.ToArray();
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetRenterProfile(int userId)
        {
            try
            {
                var result = await _profileService.GetRenterProfileAsync(userId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Profile not found");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting renter profile");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
