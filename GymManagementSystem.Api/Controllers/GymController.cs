using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.Api.Controllers
{
    [Route("api/gyms")]
    [ApiController]
    public class GymsController : ControllerBase
    {
        private readonly IGymService _gymService;

        public GymsController(IGymService gymService)
        {
            _gymService = gymService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateGymDto dto)
        {
            var gym = await _gymService.CreateGymAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = gym.Id }, gym);
        }

        [HttpGet("{id}")]
        [Authorize]                                            // any authenticated staff

        public async Task<IActionResult> GetById(int id)
        {
            var gym = await _gymService.GetGymByIdAsync(id);
            return Ok(gym);
        }

        [HttpGet]
        [Authorize]                                            // any authenticated staff

        public async Task<IActionResult> GetAllGyms()
        {
            var gyms = await _gymService.GetAllGymsAsync();
            return Ok(gyms);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Update(int id, [FromBody] UpdateGymDto dto)
        {
            await _gymService.UpdateGymAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int id)
        {
            await _gymService.DeleteGymAsync(id);
            return NoContent();
        }
    }
}