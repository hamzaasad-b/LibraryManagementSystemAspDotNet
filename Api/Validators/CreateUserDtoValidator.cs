using Api.Dto;
using FluentValidation;

namespace Api.Validators;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Email).EmailAddress().NotEmpty();
    }
}