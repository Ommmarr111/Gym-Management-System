using FluentValidation;
using GymManagementSystem.Application.DTOs;

namespace GymManagementSystem.Application.Validators
{
    public class CheckInDtoValidator : AbstractValidator<CheckInDto>
    {
        public CheckInDtoValidator()
        {
            RuleFor(x => x.MemberId)
                .GreaterThan(0).WithMessage("Valid member must be selected");

            RuleFor(x => x.GymId)
                .GreaterThan(0).WithMessage("Valid gym must be selected");
        }
    }
}