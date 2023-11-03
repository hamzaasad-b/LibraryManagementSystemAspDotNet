using Api.Dto.Book;
using FluentValidation;

namespace Api.Validators;

public class CreateBookDtoValidator : AbstractValidator<CreateBookDto>
{
    public CreateBookDtoValidator()
    {
        RuleFor(x => x.Iban).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
    }
}