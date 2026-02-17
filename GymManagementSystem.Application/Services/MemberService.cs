using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.Exceptions;
using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Domain.Entities;

namespace GymManagementSystem.Application.Services
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MemberService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<MemberDto>> GetAllMembersAsync()
        {
            var members = await _unitOfWork.Members.GetAllAsync();

            return members.Select(m => new MemberDto
            {
                Id = m.Id,
                FullName = $"{m.FirstName} {m.LastName}",
                Email = m.Email,
                GymName = m.Gym != null ? m.Gym.Name : "No Gym Assigned"
            }).ToList();
        }

        public async Task<MemberDetailsDto> GetMemberByIdAsync(int id)
        {
            var m = await _unitOfWork.Members.GetByIdAsync(id);

            if (m == null)
                throw new NotFoundException($"Member with id = {id} not found");

            return new MemberDetailsDto
            {
                Id = m.Id,
                FirstName = m.FirstName,
                LastName = m.LastName,
                Email = m.Email,
                PhoneNumber = m.PhoneNumber,
                DateOfBirth = m.DateOfBirth,
                JoinDate = m.JoinDate,
                GymId = m.GymId,
                GymName = m.Gym != null ? m.Gym.Name : "No Gym Assigned"
            };
        }

        public async Task<MemberDetailsDto> CreateMemberAsync(CreateMemberDto dto)
        {
            var emailExists = await _unitOfWork.Members.EmailExistsAsync(dto.Email);

            if (emailExists)
                throw new BusinessRuleException($"A member with email {dto.Email} already exists");

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

            var createdMember = await _unitOfWork.Members.AddAsync(newMember);
            await _unitOfWork.SaveChangesAsync();

            return new MemberDetailsDto
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

        public async Task UpdateMemberAsync(int id, CreateMemberDto dto)
        {
            var existingMember = await _unitOfWork.Members.GetByIdAsync(id);

            if (existingMember == null)
                throw new NotFoundException($"Member with id = {id} not found");

            var emailExists = await _unitOfWork.Members.EmailExistsAsync(dto.Email);

            if (emailExists && existingMember.Email != dto.Email)
                throw new BusinessRuleException($"Email {dto.Email} is already taken by another member");

            existingMember.FirstName = dto.FirstName;
            existingMember.LastName = dto.LastName;
            existingMember.Email = dto.Email;
            existingMember.PhoneNumber = dto.PhoneNumber;
            existingMember.DateOfBirth = dto.DateOfBirth;
            existingMember.GymId = dto.GymId;

            await _unitOfWork.Members.UpdateAsync(existingMember);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteMemberAsync(int id)
        {
            var existingMember = await _unitOfWork.Members.GetByIdAsync(id);

            if (existingMember == null)
                throw new NotFoundException($"Member with id = {id} not found");

            await _unitOfWork.Members.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}