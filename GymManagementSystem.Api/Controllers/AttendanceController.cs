using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.DTOs.Attendance;
using GymManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin,Manager,Receptionist")]       // front desk checks members in
        public async Task<IActionResult> CheckIn([FromBody] CheckInDto dto)
        {
            var result = await _attendanceService.CheckInAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]                    // reporting, not front-desk use

        public async Task<IActionResult> GetHistory([FromQuery] AttendanceRequestParams parameters)
        {
            var result = await _attendanceService.GetAttendanceHistoryAsync(parameters);
            return Ok(result);
        }

        [HttpGet("gym/{gymId}")]
        [Authorize(Roles = "Admin,Manager")]                    // reporting, not front-desk use

        public async Task<IActionResult> GetByGymId(int gymId)
        {
            var history = await _attendanceService.GetGymAttendanceAsync(gymId);
            return Ok(history);
        }
    }
}