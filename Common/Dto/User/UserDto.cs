namespace Common.Dto.User;

public class UserDto : IDto
{
    public uint? Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
}