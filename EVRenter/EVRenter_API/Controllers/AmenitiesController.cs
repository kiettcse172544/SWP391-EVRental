using EVRenter_Service.RequestModel;
using EVRenter_Service.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVRenter_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AmenitiesController : ControllerBase
    {
        private readonly IAmenitiesService _amenitiesService;

        public AmenitiesController(IAmenitiesService amenitiesService)
        {
            _amenitiesService = amenitiesService;
        }

        [HttpGet("{modelId}")]
        public async Task<IActionResult> GetAmenitiesByModel(int modelId)
        {
            var amenities = await _amenitiesService.GetAllAmenitiesByModel(modelId);
            return Ok(amenities);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAmenities([FromBody] AmenitiesRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var amenities = await _amenitiesService.CreateAmenities(request);
            return Ok(amenities);
            //return CreatedAtAction(nameof(GetAmenitiesByModel), new { id = amenities.M }, booking);
        }
    }
}
