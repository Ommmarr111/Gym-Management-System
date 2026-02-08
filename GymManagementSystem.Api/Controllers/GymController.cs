using GymManagementSystem.Api.DTOs;
using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.Api.Controllers
{
    [Route("api/gyms")]
    [ApiController]
    [Authorize]
    public class GymsController : ControllerBase
    {
        private readonly IGymService _gymService;
        public GymsController(IGymService gymService)
        {
            _gymService = gymService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> CreateGym(CreateGymDto request)
        {
            var gym = new Gym
            {
                Name = request.Name,
                Address = request.Address,
                PhoneNumber = request.PhoneNumber
            };

            var gymId = await _gymService.CreateGymAsync(gym);

            return Ok(new { Message = "Gym created successfully!", GymId = gymId });
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllGyms()
        {
            var gyms = await _gymService.GetAllGymsAsync();
            var result = gyms.Select(g => new GymDto
            {
                Id = g.Id,
                Name = g.Name,
                Address = g.Address,
                PhoneNumber = g.PhoneNumber
            });

            return Ok(result);
        }
    }
}