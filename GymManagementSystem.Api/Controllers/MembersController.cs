using Asp.Versioning;
using GymManagementSystem.Application.BackgroundJobs.Interfaces;
using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.DTOs.Members;
using GymManagementSystem.Application.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/members")]
    public class MembersController : ControllerBase
    {
        private readonly IMemberService _service;

        public MembersController(IMemberService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] MemberRequestParams parameters)
        {
            var pagedMembers = await _service.GetAllMembersAsync(parameters);
            return Ok(pagedMembers);
        }

        [HttpGet("{id}")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var member = await _service.GetMemberByIdAsync(id);
            return Ok(member);
        }

        [HttpGet("{id}")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> GetByIdV2(int id)
        {
            var member = await _service.GetMemberByIdWithSubscriptionStatusAsync(id);
            return Ok(member);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateMemberDto dto)
        {
            var createdMember = await _service.CreateMemberAsync(dto);
            // Enqueue a background job to send a welcome email to the newly created member
            BackgroundJob.Enqueue<IEmailServiceJob>(x => x.SendWelcomeEmailAsync(createdMember.Id));
            return CreatedAtAction(nameof(GetById), new { id = createdMember.Id }, createdMember);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateMemberDto dto)
        {
            await _service.UpdateMemberAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteMemberAsync(id);
            return NoContent();
        }
    }
}