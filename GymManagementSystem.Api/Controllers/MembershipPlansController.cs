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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdPlanDto = await _service.CreatePlanAsync(request);

                return Ok(new { Message = "Plan created!", Plan = createdPlanDto });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
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
            if (plan == null)
                return NotFound("Plan not found");

            return Ok(plan);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateMembershipPlanDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.UpdatePlanAsync(id, request);

            if (!updated)
                return NotFound($"Plan with ID {id} not found.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeletePlanAsync(id);

            if (!deleted)
                return NotFound($"Plan with ID {id} not found.");

            return NoContent();
        }
    }
}