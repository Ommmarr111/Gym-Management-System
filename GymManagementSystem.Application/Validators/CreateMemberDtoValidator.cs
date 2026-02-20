using FluentValidation;
using GymManagementSystem.Application.DTOs;

namespace GymManagementSystem.Application.Validators
{
    public class CreateMemberDtoValidator : AbstractValidator<CreateMemberDto>
    {
        public CreateMemberDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Please provide a valid email address")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .MinimumLength(10).WithMessage("Phone number must be at least 10 digits")
                .MaximumLength(15).WithMessage("Phone number cannot exceed 15 digits")
                .Matches(@"^\+?[0-9]+$").WithMessage("Phone number can only contain digits and optional + prefix");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required")
                .LessThan(DateTime.Now.AddYears(-18)).WithMessage("Member must be at least 18 years old")
                .GreaterThan(DateTime.Now.AddYears(-120)).WithMessage("Please enter a valid date of birth");

            RuleFor(x => x.GymId)
                .GreaterThan(0).WithMessage("Valid gym must be selected");
        }
    }
}