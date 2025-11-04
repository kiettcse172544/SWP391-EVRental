using EVRenter_Data.Entities;
using EVRenter_Service.RequestModel;
using EVRenter_Service.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVRenter_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StationController : ControllerBase
    {
        private readonly IStationService _stationService;

        public StationController(IStationService stationService)
        {
            _stationService = stationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStations()
        {
            var response = await _stationService.GetAllStation();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStationById(int id)
        {


            var user = await _stationService.GetStationByIdAsync(id);
            if (user == null)
            {
                return NotFound("Station not found.");
            }

            return Ok(user);
        }

        [HttpPost]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateStation([FromForm] StationRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var station = await _stationService.CreateStationAsync(request);
            return CreatedAtAction(nameof(GetStationById), new { id = station.Id }, station);
        }

        [HttpPut("{id}")]
        //[Authorize]
        public async Task<IActionResult> UpdateStation(int id, [FromForm] StationUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedStation = await _stationService.UpdateStationAsync(id, request);
            if (updatedStation == null)
            {
                return NotFound("Station not found.");
            }

            return Ok(updatedStation);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStation(int id)
        {
            var result = await _stationService.DeleteStationAsync(id);
            if (!result)
            {
                return NotFound("Station not found.");
            }

            return NoContent();
        }
    }
}
