using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _attendanceService.CheckInAsync(dto);
                return Ok(new { Message = "Welcome! Access Granted ✅", Data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Access Denied ⛔", Error = ex.Message });
            }
        }

        [HttpGet("history/{memberId}")]
        public async Task<IActionResult> GetHistory(int memberId)
        {
            var history = await _attendanceService.GetMemberAttendanceHistoryAsync(memberId);
            return Ok(history);
        }
    }
}