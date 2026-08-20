using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Domain.Entities;
using GymManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.Infrastructure.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly ApplicationDbContext _context;

        public MemberRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Member>> GetAllAsync()
        {
            return await _context.Members
                .Where(m => !m.IsDeleted)
                .Include(m => m.Gym)
                .ToListAsync();
        }

        public async Task<Member?> GetByIdAsync(int id)
        {
            return await _context.Members
                .Include(m => m.Gym)
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
        }

        public async Task<Member> AddAsync(Member member)
        {
            await _context.Members.AddAsync(member);
            return member;
        }

        public Task UpdateAsync(Member member)
        {
            _context.Members.Update(member);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member != null)
            {
                member.IsDeleted = true;
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Members
                .AnyAsync(m => m.Email == email && !m.IsDeleted);
        }

        public async Task<int> CountByGymIdAsync(int gymId)
        {
            return await _context.Members.CountAsync(m => m.GymId == gymId);
        }

        public IQueryable<Member> GetAllAsQueryable()
        {
            return _context.Members.Include(m => m.Gym);
        }
    }
}