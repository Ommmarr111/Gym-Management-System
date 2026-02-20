using FluentValidation;
using GymManagementSystem.Application.DTOs;

namespace GymManagementSystem.Application.Validators
{
    public class CreateSubscriptionDtoValidator : AbstractValidator<CreateSubscriptionDto>
    {
        public CreateSubscriptionDtoValidator()
        {
            RuleFor(x => x.MemberId)
                .GreaterThan(0).WithMessage("Valid member must be selected");

            RuleFor(x => x.MembershipPlanId)
                .GreaterThan(0).WithMessage("Valid membership plan must be selected");
        }
    }
}