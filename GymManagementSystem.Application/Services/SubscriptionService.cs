using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Domain.Entities;

namespace GymManagementSystem.Application.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepo;
        private readonly IMembershipPlanRepository _planRepo;
        private readonly IMemberRepository _memberRepo;

        public SubscriptionService(
            ISubscriptionRepository subscriptionRepo,
            IMembershipPlanRepository planRepo,
            IMemberRepository memberRepo)
        {
            _subscriptionRepo = subscriptionRepo;
            _planRepo = planRepo;
            _memberRepo = memberRepo;
        }

        public async Task<SubscriptionDto> CreateSubscriptionAsync(CreateSubscriptionDto dto)
        {
            var member = await _memberRepo.GetByIdAsync(dto.MemberId);
            if (member == null)
                throw new Exception("Member not found!");

            var plan = await _planRepo.GetByIdAsync(dto.MembershipPlanId);
            if (plan == null)
                throw new Exception("Membership Plan not found!");

            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(plan.DurationInDays);

            var subscription = new Subscription
            {
                MemberId = dto.MemberId,
                MembershipPlanId = dto.MembershipPlanId,
                StartDate = startDate,
                EndDate = endDate,
                AmountPaid = plan.Price,
                Status = "Active"
            };

            await _subscriptionRepo.AddAsync(subscription);

            return new SubscriptionDto
            {
                Id = subscription.Id,
                MemberId = subscription.MemberId,
                MemberName = $"{member.FirstName} {member.LastName}",
                MembershipPlanId = subscription.MembershipPlanId,
                PlanName = plan.Name,
                PricePaid = subscription.AmountPaid,
                StartDate = subscription.StartDate,
                EndDate = subscription.EndDate,
                Status = subscription.Status
            };
        }

        public async Task<List<SubscriptionDto>> GetAllSubscriptionsAsync()
        {
            var subs = await _subscriptionRepo.GetAllAsync();

            return subs.Select(s => new SubscriptionDto
            {
                Id = s.Id,
                MemberId = s.MemberId,
                MemberName = s.Member != null ? $"{s.Member.FirstName} {s.Member.LastName}" : "Unknown",
                MembershipPlanId = s.MembershipPlanId,
                PlanName = s.MembershipPlan != null ? s.MembershipPlan.Name : "Unknown",
                PricePaid = s.AmountPaid,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                Status = s.Status
            }).ToList();
        }

        public async Task<SubscriptionDto?> GetSubscriptionByIdAsync(int id)
        {
            var s = await _subscriptionRepo.GetByIdAsync(id);
            if (s == null) return null;

            return new SubscriptionDto
            {
                Id = s.Id,
                MemberId = s.MemberId,
                MemberName = s.Member != null ? $"{s.Member.FirstName} {s.Member.LastName}" : "Unknown",
                MembershipPlanId = s.MembershipPlanId,
                PlanName = s.MembershipPlan != null ? s.MembershipPlan.Name : "Unknown",
                PricePaid = s.AmountPaid,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                Status = s.Status
            };
        }
    }
}