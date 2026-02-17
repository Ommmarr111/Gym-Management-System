using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.Api.Controllers
{
    [Route("api/subscriptions")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionService _service;

        public SubscriptionsController(ISubscriptionService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSubscriptionDto dto)
        {
            var subscription = await _service.CreateSubscriptionAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = subscription.Id }, subscription);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var subs = await _service.GetAllSubscriptionsAsync();
            return Ok(subs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sub = await _service.GetSubscriptionByIdAsync(id);
            return Ok(sub);
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            await _service.CancelSubscriptionAsync(id);
            return Ok(new { Message = "Subscription cancelled successfully." });
        }

        [HttpGet("member/{memberId}")]
        public async Task<IActionResult> GetByMemberId(int memberId)
        {
            var subs = await _service.GetSubscriptionsByMemberIdAsync(memberId);
            return Ok(subs);
        }
    }
}