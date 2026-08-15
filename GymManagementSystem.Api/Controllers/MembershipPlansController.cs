using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.Api.Controllers
{
    [Route("api/plans")]
    [ApiController]
    public class MembershipPlansController : ControllerBase
    {
        private readonly IMembershipPlanService _service;

        public MembershipPlansController(IMembershipPlanService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreatePlan([FromBody] CreateMembershipPlanDto request)
        {
            var createdPlanDto = await _service.CreatePlanAsync(request);
            return Ok(new { Message = "Plan created!", Plan = createdPlanDto });
        }

        [HttpGet]
        [Authorize] // any authenticated staff
        public async Task<IActionResult> GetAllPlans()
        {
            var plans = await _service.GetAllPlansAsync();
            return Ok(plans);
        }

        [HttpGet("{id}")]

        [Authorize] // any authenticated staff
        public async Task<IActionResult> GetPlanById(int id)
        {
            var plan = await _service.GetPlanByIdAsync(id);
            return Ok(plan);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMembershipPlanDto dto)
        {
            await _service.UpdatePlanAsync(id, dto);
            return Ok(new { Message = "Plan updated successfully! ✅" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeletePlanAsync(id);
            return Ok(new { Message = "Plan deleted successfully! 🗑️" });
        }

        [HttpGet("gym/{gymId}")]
        [Authorize] // any authenticated staff
        public async Task<IActionResult> GetByGymId(int gymId)
        {
            var plans = await _service.GetPlansByGymIdAsync(gymId);
            return Ok(plans);
        }
    }
}