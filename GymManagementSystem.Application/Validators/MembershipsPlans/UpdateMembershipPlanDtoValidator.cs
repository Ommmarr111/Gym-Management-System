using FluentValidation;
using GymManagementSystem.Application.DTOs;

namespace GymManagementSystem.Application.Validators
{
    public class UpdateMembershipPlanDtoValidator : AbstractValidator<UpdateMembershipPlanDto>
    {
        public UpdateMembershipPlanDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Plan name is required")
                .MaximumLength(100).WithMessage("Plan name cannot exceed 100 characters");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0")
                .LessThanOrEqualTo(100000).WithMessage("Price cannot exceed 100,000");

            RuleFor(x => x.DurationInDays)
                .GreaterThan(0).WithMessage("Duration must be greater than 0")
                .LessThanOrEqualTo(3650).WithMessage("Duration cannot exceed 10 years (3650 days)");
        }
    }
}