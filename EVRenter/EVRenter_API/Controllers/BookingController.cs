using EVRenter_Service.RequestModel;
using EVRenter_Service.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVRenter_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllBookings()
        {
            var response = await _bookingService.GetAllBooking();
            return Ok(response);
        }

        [HttpGet("GetAllBookingsForStaff")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> GetAllBookingsForStaff()
        {
            var response = await _bookingService.GetAllBookingsForStaff();
            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetBookingById(int id)
        {


            var vehicle = await _bookingService.GetBookingByIdAsync(id);
            if (vehicle == null)
            {
                return NotFound("Booking not found.");
            }

            return Ok(vehicle);
        }

        [HttpGet("GetBookingByIdForStaff/{id}")]
        [Authorize]
        public async Task<IActionResult> GetBookingByIdForStaff(int id)
        {


            var vehicle = await _bookingService.GetBookingByIdForStaffAsync(id);
            if (vehicle == null)
            {
                return NotFound("Booking not found.");
            }

            return Ok(vehicle);
        }

        [HttpGet("StaffGetBookingByStation/{stationId}")]
        [Authorize]
        public async Task<IActionResult> GetStaffBookingByStation(int stationId)
        {
            var booking = await _bookingService.GetStaffBookingsByStattion(stationId);
            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            return Ok(booking);
        }

        [HttpGet("GetByCar/{vehicleID}")]
        [Authorize]
        public async Task<IActionResult> GetBookingByCar(int vehicleID)
        {


            var vehicle = await _bookingService.GetBookingByVehicleAsync(vehicleID);
            if (vehicle == null)
            {
                return NotFound("Booking not found.");
            }

            return Ok(vehicle);
        }

        [HttpGet("GetBookingsByRenter/{renterID}")]
        [Authorize]
        public async Task<IActionResult> GetBookingsByRenter(int renterID)
        {
            var response = await _bookingService.GetBookingByRenter(renterID);
            return Ok(response);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBooking([FromForm] BookingRequestModel request)
        { try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var booking = await _bookingService.CreateBookingAsync(request);
                return CreatedAtAction(nameof(GetBookingById), new { id = booking.Id }, booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, detail = ex.StackTrace });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromForm] BookingUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedBooking = await _bookingService.UpdateBookingStatsusAsync(id, request);
            if (updatedBooking == null)
            {
                return NotFound("User not found.");
            }

            return Ok(updatedBooking);
        }

        [HttpPut("AutoUpdateBookingStatus/{bookingId}")]
        //[Authorize]
        public async Task<IActionResult> AutoUpdateBookingStatus(int bookingId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedBooking = await _bookingService.AutoUpdateBookingStatusAsync(bookingId);
            if (updatedBooking == null)
            {
                return NotFound("User not found.");
            }

            return Ok(updatedBooking);
        }

        [HttpPut("RefuseBookingStatus/{bookingId}")]
        //[Authorize]
        public async Task<IActionResult> RefuseBookingStatus(int bookingId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedBooking = await _bookingService.StaffRefusingAsync(bookingId);
            if (updatedBooking == null)
            {
                return NotFound("User not found.");
            }

            return Ok(updatedBooking);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUnpaidBooking(int id)
        {
            var result = await _bookingService.DeleteUnpaidBookingAsync(id);
            if (!result)
            {
                return NotFound("Booking not found.");
            }

            return NoContent();
        }
    }
}