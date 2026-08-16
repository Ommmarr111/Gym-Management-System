using AutoMapper;
using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.DTOs.Subscriptions;
using GymManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.Api.Controllers
{
    [Route("api/subscriptions")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionService _service;
        private readonly IMapper _mapper;

        public SubscriptionsController(ISubscriptionService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        public async Task<IActionResult> Create([FromBody] CreateSubscriptionDto dto)
        {
            var subscription = await _service.CreateSubscriptionAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = subscription.Id }, subscription);
        }

        [HttpGet]
        [Authorize] // any authenticated staff
        public async Task<IActionResult> GetAll([FromQuery] SubscriptionRequestParams subscriptionRequestParams)
        {
            var subs = await _service.GetAllSubscriptionsAsync(subscriptionRequestParams);
            return Ok(subs);
        }

        [HttpGet("{id}")]
        [Authorize] // any authenticated staff
        public async Task<IActionResult> GetById(int id)
        {
            var sub = await _service.GetSubscriptionByIdAsync(id);
            return Ok(sub);
        }

        [HttpPost("{id}/cancel")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Cancel(int id)
        {
            var subscription = await _service.CancelSubscriptionAsync(id);
            return Ok(subscription);
        }

        [HttpPost("{id}/freeze")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Freeze(int id, [FromBody] FreezeSubscriptionDto dto)
        {
            var subscription = await _service.FreezeSubscriptionAsync(id, dto);
            return Ok(subscription);
        }

        [HttpPost("{id}/unfreeze")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Unfreeze(int id)
        {
            var subscription = await _service.UnfreezeSubscriptionAsync(id);
            return Ok(subscription);
        }
    }
}