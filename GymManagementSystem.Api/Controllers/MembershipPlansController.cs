using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.Interfaces;
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

        [HttpPost("create")]
        public async Task<IActionResult> CreatePlan([FromBody] CreateMembershipPlanDto request)
        {
            var createdPlanDto = await _service.CreatePlanAsync(request);
            return Ok(new { Message = "Plan created!", Plan = createdPlanDto });
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllPlans()
        {
            var plans = await _service.GetAllPlansAsync();
            return Ok(plans);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlanById(int id)
        {
            var plan = await _service.GetPlanByIdAsync(id);
            return Ok(plan);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMembershipPlanDto dto)
        {
            await _service.UpdatePlanAsync(id, dto);
            return Ok(new { Message = "Plan updated successfully! ✅" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeletePlanAsync(id);
            return Ok(new { Message = "Plan deleted successfully! 🗑️" });
        }
    }
}