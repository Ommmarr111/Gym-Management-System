using AutoMapper;
using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Application.Exceptions;
using GymManagementSystem.Application.Interfaces;
using GymManagementSystem.Domain.Entities;

namespace GymManagementSystem.Application.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubscriptionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SubscriptionDto> CreateSubscriptionAsync(CreateSubscriptionDto dto)
        {
            var member = await _unitOfWork.Members.GetByIdAsync(dto.MemberId);

            if (member == null)
                throw new NotFoundException($"Member with id = {dto.MemberId} not found");

            var plan = await _unitOfWork.MembershipPlans.GetByIdAsync(dto.MembershipPlanId);

            if (plan == null)
                throw new NotFoundException($"Membership plan with id = {dto.MembershipPlanId} not found");

            var activeSubscription = await _unitOfWork.Subscriptions
                .GetActiveSubscriptionAsync(dto.MemberId, dto.MembershipPlanId);

            if (activeSubscription != null)
                throw new BusinessRuleException(
                    $"Member with id = {dto.MemberId} already has an active subscription for this plan");

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

            await _unitOfWork.Subscriptions.AddAsync(subscription);
            await _unitOfWork.SaveChangesAsync();

            var payment = new Payment
            {
                SubscriptionId = subscription.Id,
                Amount = plan.Price,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = "Cash",
                Status = "Completed",
                TransactionReference = $"TXN-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}"
            };

            await _unitOfWork.Payments.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<SubscriptionDto>(subscription);
        }

        public async Task<List<SubscriptionDto>> GetAllSubscriptionsAsync()
        {
            var subs = await _unitOfWork.Subscriptions.GetAllAsync();
            return _mapper.Map<List<SubscriptionDto>>(subs);  // ✅ Use mapper
        }

        public async Task<SubscriptionDto> GetSubscriptionByIdAsync(int id)
        {
            var s = await _unitOfWork.Subscriptions.GetByIdAsync(id);

            if (s == null)
                throw new NotFoundException($"Subscription with id = {id} not found");

            return _mapper.Map<SubscriptionDto>(s);  // ✅ Use mapper
        }

        public async Task CancelSubscriptionAsync(int subscriptionId)
        {
            var sub = await _unitOfWork.Subscriptions.GetByIdAsync(subscriptionId);

            if (sub == null)
                throw new NotFoundException($"Subscription with id = {subscriptionId} not found");

            if (sub.Status == "Cancelled")
                throw new BusinessRuleException($"Subscription with id = {subscriptionId} is already cancelled");

            sub.Status = "Cancelled";  // ✅ Update directly
            await _unitOfWork.Subscriptions.UpdateAsync(sub);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<SubscriptionDto>> GetSubscriptionsByMemberIdAsync(int memberId)
        {
            var member = await _unitOfWork.Members.GetByIdAsync(memberId);

            if (member == null)
                throw new NotFoundException($"Member with id = {memberId} not found");

            var subs = await _unitOfWork.Subscriptions.GetByMemberIdAsync(memberId);

            return _mapper.Map<List<SubscriptionDto>>(subs);  // ✅ Use mapper
        }

        public async Task FreezeSubscriptionAsync(int subscriptionId, FreezeSubscriptionDto dto)
        {
            var subscription = await _unitOfWork.Subscriptions.GetByIdAsync(subscriptionId);

            if (subscription == null)
                throw new NotFoundException($"Subscription with id = {subscriptionId} not found");

            if (subscription.Status != "Active")
                throw new BusinessRuleException($"Only active subscriptions can be frozen. Current status: {subscription.Status}");

            if (dto.DurationDays <= 0 || dto.DurationDays > 90)
                throw new ValidationException("Freeze duration must be between 1 and 90 days");

            subscription.Status = "Frozen";
            subscription.FrozenDate = DateTime.UtcNow;
            subscription.FrozenDurationDays = dto.DurationDays;
            subscription.EndDate = subscription.EndDate.AddDays(dto.DurationDays);

            await _unitOfWork.Subscriptions.UpdateAsync(subscription);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UnfreezeSubscriptionAsync(int subscriptionId)
        {
            var subscription = await _unitOfWork.Subscriptions.GetByIdAsync(subscriptionId);

            if (subscription == null)
                throw new NotFoundException($"Subscription with id = {subscriptionId} not found");

            if (subscription.Status != "Frozen")
                throw new BusinessRuleException($"Only frozen subscriptions can be unfrozen. Current status: {subscription.Status}");

            subscription.Status = "Active";
            subscription.FrozenDate = null;
            subscription.FrozenDurationDays = null;

            await _unitOfWork.Subscriptions.UpdateAsync(subscription);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}