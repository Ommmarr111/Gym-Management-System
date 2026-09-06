using AutoMapper;
using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.DTOs.Members;
using GymManagementSystem.Application.Exceptions;
using GymManagementSystem.Application.Extensions;
using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Data;

namespace GymManagementSystem.Application.Services
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<MemberService> _logger;


        public MemberService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MemberService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResult<MemberDto>> GetAllMembersAsync(MemberRequestParams parameters)
        {
            // 1. Get the IQueryable from the repo
            var query = _unitOfWork.Members.GetAllAsQueryable();

            // 2. Apply Searching & Filtering
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var search = parameters.SearchTerm.ToLower();
                query = query.Where(m =>
                    m.FirstName.ToLower().Contains(search) ||
                    m.LastName.ToLower().Contains(search) ||
                    m.Email.ToLower().Contains(search) ||
                    m.PhoneNumber.Contains(search));
            }

            if (parameters.GymId.HasValue)
            {
                query = query.Where(m => m.GymId == parameters.GymId.Value);
            }

            // 3. Apply Sorting
            if (!string.IsNullOrWhiteSpace(parameters.SortBy))
            {
                query = parameters.SortBy.ToLower() switch
                {
                    "firstname" => parameters.IsDescending ? query.OrderByDescending(m => m.FirstName) : query.OrderBy(m => m.FirstName),
                    "lastname" => parameters.IsDescending ? query.OrderByDescending(m => m.LastName) : query.OrderBy(m => m.LastName),
                    "email" => parameters.IsDescending ? query.OrderByDescending(m => m.Email) : query.OrderBy(m => m.Email),
                    "joindate" => parameters.IsDescending ? query.OrderByDescending(m => m.JoinDate) : query.OrderBy(m => m.JoinDate),
                    _ => query.OrderBy(m => m.Id)
                };
            }
            else
            {
                query = query.OrderBy(m => m.Id); // Required fallback for EF Core Skip/Take
            }

            // 4. Execute the DB Query and get Paged Entities
            var pagedEntities = await query.ToPagedResultAsync(parameters.PageNumber, parameters.PageSize);

            // 5. Map the Data to DTOs
            var memberDtos = _mapper.Map<List<MemberDto>>(pagedEntities.Items);

            // 6. Return a new PagedResult wrapped around the DTOs
            return new PagedResult<MemberDto>(
                memberDtos,
                pagedEntities.TotalCount,
                pagedEntities.CurrentPage,
                pagedEntities.PageSize);
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

            var gym = await _unitOfWork.Gyms.GetByIdAsync(dto.GymId);
            if (gym == null)
                throw new NotFoundException($"Gym with id = {dto.GymId} not found");

            var newMember = _mapper.Map<Member>(dto);
            newMember.JoinDate = DateTime.UtcNow;

            Member createdMember;

            await using var transaction = await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                var currentMemberCount = await _unitOfWork.Members.CountByGymIdAsync(dto.GymId);

                if (currentMemberCount >= gym.Capacity)
                {
                    _logger.LogWarning("Gym {GymId} capacity check failed — registration rejected, current count {CurrentCount}, capacity {Capacity}",
                        dto.GymId, currentMemberCount, gym.Capacity);
                    throw new BusinessRuleException($"Gym '{gym.Name}' is at full capacity ({gym.Capacity} members)");
                }

                createdMember = await _unitOfWork.Members.AddAsync(newMember);
                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogWarning(ex, "Member creation transaction rolled back for gym {GymId} — possible concurrent capacity conflict",
                    dto.GymId);
                throw;
            }

            var memberWithGym = await _unitOfWork.Members.GetByIdAsync(createdMember.Id);
            return _mapper.Map<MemberDetailsDto>(memberWithGym);
        }

        public async Task UpdateMemberAsync(int id, CreateMemberDto dto)
        {
            var existingMember = await _unitOfWork.Members.GetByIdAsync(id);

            if (existingMember == null)
                throw new NotFoundException($"Member with id = {id} not found");

            var emailExists = await _unitOfWork.Members.EmailExistsAsync(dto.Email);

            if (emailExists && existingMember.Email != dto.Email)
                throw new BusinessRuleException($"Email {dto.Email} is already taken by another member");

            _mapper.Map(dto, existingMember);

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