using FluentValidation;

namespace Api.Dto;

public class CreateUserDto 
{
    public string? FullName { get; set; }
    public string Email { get; set; }
}