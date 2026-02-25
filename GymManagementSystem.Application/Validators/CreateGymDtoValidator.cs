using FluentValidation;
using GymManagementSystem.Application.DTOs;

namespace GymManagementSystem.Application.Validators
{
    public class CreateGymDtoValidator : AbstractValidator<CreateGymDto>
    {
        public CreateGymDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Gym name is required")
                .MaximumLength(100).WithMessage("Gym name cannot exceed 100 characters");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required")
                .MaximumLength(200).WithMessage("Address cannot exceed 200 characters");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .MinimumLength(10).WithMessage("Phone number must be at least 10 digits")
                .MaximumLength(15).WithMessage("Phone number cannot exceed 15 digits")
                .Matches(@"^\+?[0-9]+$").WithMessage("Phone number can only contain digits and optional + prefix");


            RuleFor(x => x.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be greater than 0")
                .LessThanOrEqualTo(10000).WithMessage("Capacity cannot exceed 10,000");
        }
    }
}