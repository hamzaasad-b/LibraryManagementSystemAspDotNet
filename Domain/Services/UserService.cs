using Common.Dto.User;
using Data.Entities;
using Data.Repositories;

namespace Domain.Services;

public class UserService : BaseService<User, UserDto>
{
    public UserService(UserRepository repository) : base(repository)
    {
    }
}