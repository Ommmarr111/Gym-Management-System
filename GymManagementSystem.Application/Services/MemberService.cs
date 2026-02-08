using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Domain.Entities;

namespace GymManagementSystem.Application.Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _repository;

        public MemberService(IMemberRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MemberDto>> GetAllMembersAsync()
        {
            var members = await _repository.GetAllAsync();

            return members.Select(m => new MemberDto
            {
                Id = m.Id,
                FirstName = m.FirstName,
                LastName = m.LastName,
                Email = m.Email,
                PhoneNumber = m.PhoneNumber,
                DateOfBirth = m.DateOfBirth,
                JoinDate = m.JoinDate,
                GymId = m.GymId,
                GymName = m.Gym != null ? m.Gym.Name : "No Gym Name Found"
            }).ToList();
        }
        public async Task<MemberDto?> GetMemberByIdAsync(int id)
        {
            var m = await _repository.GetByIdAsync(id);
            if (m == null) return null;

            return new MemberDto
            {
                Id = m.Id,
                FirstName = m.FirstName,
                LastName = m.LastName,
                Email = m.Email,
                PhoneNumber = m.PhoneNumber,
                DateOfBirth = m.DateOfBirth,
                JoinDate = m.JoinDate,
                GymId = m.GymId,
                GymName = m.Gym != null ? m.Gym.Name : "No Gym Name Found"
            };
        }

        public async Task<MemberDto> CreateMemberAsync(CreateMemberDto dto)
        {
            var newMember = new Member
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                DateOfBirth = dto.DateOfBirth,
                GymId = dto.GymId,
                JoinDate = DateTime.UtcNow
            };

            var createdMember = await _repository.AddAsync(newMember);

            return new MemberDto
            {
                Id = createdMember.Id,
                FirstName = createdMember.FirstName,
                LastName = createdMember.LastName,
                Email = createdMember.Email,
                PhoneNumber = createdMember.PhoneNumber,
                DateOfBirth = createdMember.DateOfBirth,
                JoinDate = createdMember.JoinDate,
                GymId = createdMember.GymId,
                GymName = "Newly Added"
            };
        }

        public async Task<bool> UpdateMemberAsync(int id, CreateMemberDto dto)
        {
            var existingMember = await _repository.GetByIdAsync(id);
            if (existingMember == null) return false;

            existingMember.FirstName = dto.FirstName;
            existingMember.LastName = dto.LastName;
            existingMember.Email = dto.Email;
            existingMember.PhoneNumber = dto.PhoneNumber;
            existingMember.DateOfBirth = dto.DateOfBirth;
            existingMember.GymId = dto.GymId;

            await _repository.UpdateAsync(existingMember);
            return true;
        }

        public async Task<bool> DeleteMemberAsync(int id)
        {
            var existingMember = await _repository.GetByIdAsync(id);
            if (existingMember == null) return false;

            await _repository.DeleteAsync(id);
            return true;
        }
    }
}