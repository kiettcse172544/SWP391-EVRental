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
        private readonly IRenterProfileService _profileService;
        private readonly ILogger<RenterProfileController> _logger;

        public RenterProfileController(
            IRenterProfileService profileService,
            ILogger<RenterProfileController> logger)
        {
            _profileService = profileService;
            _logger = logger;
        }

        
        [HttpPost("upload-profile")]
        public async Task<IActionResult> UploadRenterProfile([FromForm] UploadRenterProfileForm form)
        {
            try
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
                    IDCardFrontImage = idFrontBytes,
                    IDCardBackImage = idBackBytes,
                    DriverLicenseFrontImage = dlFrontBytes,
                    DriverLicenseBackImage = dlBackBytes
                };

                var result = await _profileService.CreateOrUpdateProfileAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading renter profile");
                return StatusCode(500, new
                {
                    message = ex.Message,
                    inner = ex.InnerException?.Message,
                    stack = ex.StackTrace
                });
            }
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
                var result = await _profileService.GetProfileByUserIdAsync(userId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving profile");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }


        
        [HttpPut("approve/{renterId}")]
        public async Task<IActionResult> ApproveProfile(int renterId)
        {
            try
            {
                await _profileService.ApproveProfileAsync(renterId);

                return Ok(new
                {
                    message = "Profile approved successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving profile");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }


        
        [HttpPut("reject/{renterId}")]
        public async Task<IActionResult> RejectProfile(int renterId)
        {
            try
            {
                await _profileService.RejectProfileAsync(renterId);

                return Ok(new
                {
                    message = "Profile rejected successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting profile");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
