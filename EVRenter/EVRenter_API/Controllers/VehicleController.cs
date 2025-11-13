using EVRenter_Data.Entities;
using EVRenter_Service.RequestModel;
using EVRenter_Service.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVRenter_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVehicles()
        {
            var response = await _vehicleService.GetAllVehicle();
            return Ok(response);
        }

        [HttpGet("GetAllVehiclesByStation/{stationId}")]
        public async Task<IActionResult> GetAllVehiclesByStationID(int stationID)
        {
            var response = await _vehicleService.GetAllVehicleByStation(stationID);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicleById(int id)
        {


            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (vehicle == null)
            {
                return NotFound("Vehicle not found.");
            }

            return Ok(vehicle);
        }

        [HttpPost]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateVehicle([FromForm] VehicleRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var vehicle = await _vehicleService.CreateVehicleAsync(request);
            return CreatedAtAction(nameof(GetVehicleById), new { id = vehicle.Id }, vehicle);
        }

        [HttpPut("{id}")]
        //[Authorize]
        public async Task<IActionResult> UpdateVehicle(int id, [FromForm] VehicleUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedVehicle = await _vehicleService.UpdateVehicleAsync(id, request);
            if (updatedVehicle == null)
            {
                return NotFound("Vehicle not found.");
            }

            return Ok(updatedVehicle);
        }

        [HttpPut("UpdateCarItems/{vehicleId}")]
        public async Task<IActionResult> UpdateCarItemsByVehicle(int vehicleId,[FromBody] UpdateCarItemsRequest request)
        {
            if (vehicleId != request.VehicleID)
                return BadRequest("Vehicle ID mismatch.");

            try
            {
                var result = await _vehicleService.UpdateCarItemsByVehicleAsync(request);
                if (!result)
                    return BadRequest("Update failed.");

                return Ok("Car items updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




        [HttpPut("AutpUpdateStatus/{vehicleId}")]
        public async Task<IActionResult> UpdateVehicleStatus(int vehicleId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedVehicle = await _vehicleService.UpdateVehicleStatusAsync(vehicleId);
            if (updatedVehicle == null)
            {
                return NotFound("Vehicle not found.");
            }

            return Ok(updatedVehicle);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModel(int id)
        {
            var result = await _vehicleService.DeleteVehicleAsync(id);
            if (!result)
            {
                return NotFound("Vehicle not found.");
            }

            return NoContent();
        }

        [HttpPut("StaffRefusedStatus/{vehicleId}")]
        public async Task<IActionResult> StaffRefusedStatus(int vehicleId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedVehicle = await _vehicleService.StaffRefusingAsync(vehicleId);
            if (updatedVehicle == null)
            {
                return NotFound("Vehicle not found.");
            }

            return Ok(updatedVehicle);
        }
    }
}
