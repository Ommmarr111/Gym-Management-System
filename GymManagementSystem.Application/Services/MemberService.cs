using AutoMapper;
using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.Exceptions;
using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Domain.Entities;

namespace GymManagementSystem.Application.Services
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MemberService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<MemberDto>> GetAllMembersAsync()
        {
            var members = await _unitOfWork.Members.GetAllAsync();

            return _mapper.Map<List<MemberDto>>(members);
        }

        public async Task<MemberDetailsDto> GetMemberByIdAsync(int id)
        {
            var member = await _unitOfWork.Members.GetByIdAsync(id);

            if (member == null)
                throw new NotFoundException($"Member with id = {id} not found");

            return _mapper.Map<MemberDetailsDto>(member);
        }


        public async Task<MemberDetailsDto> CreateMemberAsync(CreateMemberDto dto)
        {
            var emailExists = await _unitOfWork.Members.EmailExistsAsync(dto.Email);

            if (emailExists)
                throw new BusinessRuleException($"A member with email {dto.Email} already exists");
            var newMember = _mapper.Map<Member>(dto);
            newMember.JoinDate = DateTime.UtcNow;

            var createdMember = await _unitOfWork.Members.AddAsync(newMember);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<MemberDetailsDto>(createdMember);
        }

        public async Task UpdateMemberAsync(int id, CreateMemberDto dto)
        {
            var existingMember = await _unitOfWork.Members.GetByIdAsync(id);

            if (existingMember == null)
                throw new NotFoundException($"Member with id = {id} not found");

            var emailExists = await _unitOfWork.Members.EmailExistsAsync(dto.Email);

            if (emailExists && existingMember.Email != dto.Email)
                throw new BusinessRuleException($"Email {dto.Email} is already taken by another member");

            _mapper.Map(dto, existingMember); // Update existing member with new values from DTO

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