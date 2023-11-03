using Api.Dto.Book;
using FluentValidation;

namespace Api.Validators;

public class IssueBookToUserDtoValidator : AbstractValidator<IssueBookToUserDto>
{
    public IssueBookToUserDtoValidator()
    {
        RuleFor(x => x.BookId).NotNull();
        RuleFor(x => x.UserId).NotNull();
    }
}