using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.Api.Controllers
{
    [Route("api/gyms")]
    [ApiController]
    //[Authorize]
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
        // 1. PUT: api/Gyms/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateGymDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _gymService.UpdateGymAsync(id, dto);
                return Ok(new { Message = "Gym updated successfully! ✅" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // 2. DELETE: api/Gyms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _gymService.DeleteGymAsync(id);
                return Ok(new { Message = "Gym deleted successfully! 🗑️" });
            }
            catch (Exception ex)
            {
                // رسالة توضيحية لو المسح فشل بسبب وجود بيانات مرتبطة
                return BadRequest(new { Error = "Cannot delete this Gym. It likely has associated Members or Plans. ⛔", Message = ex.Message });
            }
        }
    }
}