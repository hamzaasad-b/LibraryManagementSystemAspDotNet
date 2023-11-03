using Data.Entities;
using Data.Repositories;

namespace Domain.Services;

public class UserService : BaseService<User>
{
    public UserService(UserRepository repository) : base(repository)
    {
    }
}