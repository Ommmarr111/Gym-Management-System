using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.DTOs.Members;
using GymManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.Api.Controllers
{
    [Route("api/members")]
    [ApiController]
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
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var member = await _service.GetMemberByIdAsync(id);
            return Ok(member);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateMemberDto dto)
        {
            var createdMember = await _service.CreateMemberAsync(dto);

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