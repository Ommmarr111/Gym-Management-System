using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.Api.Controllers
{
    [Route("api/attendance")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpPost]
        public async Task<IActionResult> CheckIn([FromBody] CheckInDto dto)
        {
            var result = await _attendanceService.CheckInAsync(dto);
            return Ok(result);
        }

        [HttpGet("{memberId}")]
        public async Task<IActionResult> GetHistory(int memberId)
        {
            var history = await _attendanceService.GetMemberAttendanceHistoryAsync(memberId);
            return Ok(history);
        }

        [HttpGet("gym/{gymId}")]
        public async Task<IActionResult> GetByGymId(int gymId)
        {
            var history = await _attendanceService.GetGymAttendanceAsync(gymId);
            return Ok(history);
        }
    }
}