using FluentValidation;
using GymManagementSystem.Application.DTOs;

namespace GymManagementSystem.Application.Validators
{
    public class FreezeSubscriptionDtoValidator : AbstractValidator<FreezeSubscriptionDto>
    {
        public FreezeSubscriptionDtoValidator()
        {
            RuleFor(x => x.DurationDays)
                .GreaterThan(0).WithMessage("Freeze duration must be at least 1 day")
                .LessThanOrEqualTo(90).WithMessage("Freeze duration cannot exceed 90 days");

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Freeze reason is required")
                .MaximumLength(200).WithMessage("Reason cannot exceed 200 characters");
        }
    }
}