using EVRenter_Service.RequestModel;
using EVRenter_Service.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EVRenter_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // Lấy tất cả người dùng (hiện tại chỉ hỗ trợ lấy tất cả, không có lọc và phân trang)
        // Giữ lại để tương thích ngược với code cũ
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var response = await _userService.GetAllUsersAsync();
            return Ok(response);
        }

        // Lấy người dùng theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            //var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            //if (userIdClaim == null)
            //{
            //    return Unauthorized("User ID not found in token claims");
            //}

            //// Chuyển đổi ID từ claim sang int
            //if (!int.TryParse(userIdClaim.Value, out int currentUserId))
            //{
            //    return BadRequest("Invalid user ID in token");
            //}

            //bool isManagerOrStaff = User.IsInRole("Manager") || User.IsInRole("Staff");

            //if (!isManagerOrStaff && currentUserId != id)
            //{
            //    return Forbid("You can only view your own user information");
            //}

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }

        // Tạo người dùng mới
        [HttpPost]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateUser([FromForm] UserCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.CreateUserAsync(request);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        // Cập nhật người dùng
        [HttpPut("{id}")]
        //[Authorize]
        public async Task<IActionResult> UpdateUser(int id, [FromForm] UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedUser = await _userService.UpdateUserAsync(id, request);
            if (updatedUser == null)
            {
                return NotFound("User not found.");
            }

            return Ok(updatedUser);
        }

        // Xóa người dùng
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
            {
                return NotFound("User not found.");
            }

            return NoContent();
        }
    }
}
