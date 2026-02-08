using GymManagementSystem.Api.DTOs;
using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Domain.Entities;
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
        public async Task<IActionResult> CreatePlan(CreateMembershipPlanDto request)
        {
            var plan = new MembershipPlan
            {
                Name = request.Name,
                Price = request.Price,
                Description = request.Description,
                DurationInDays = request.DurationInDays,
                GymId = request.GymId
            };
            try
            {
                var planId = await _service.CreatePlanAsync(plan);
                return Ok(new { Message = "Plan created!", PlanId = planId });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = ex.Message,
                    InnerError = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllPlans()
        {
            var plans = await _service.GetAllPlansAsync();
            var result = plans.Select(p => new
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                DurationInDays = p.DurationInDays,
                GymId = p.GymId,
                GymName = p.Gym != null ? p.Gym.Name : "Unknown Gym"
            });
            return Ok(result);
        }
    }
}