using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Domain.Entities;
using GymManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.Infrastructure.Repositories
{
    public class GymRepository : IGymRepository
    {
        private readonly ApplicationDbContext _context;

        public GymRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Gym>> GetAllAsync()
        {
            return await _context.Gyms.ToListAsync();
        }

        public async Task<Gym?> GetByIdAsync(int id)
        {
            return await _context.Gyms.FindAsync(id);

        }

        public async Task<int> AddAsync(Gym gym)
        {
            await _context.Gyms.AddAsync(gym);
            await _context.SaveChangesAsync();
            return gym.Id;
        }
    }
}