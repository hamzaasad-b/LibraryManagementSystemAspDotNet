using Data.Entities;
using Data.Repositories;

namespace Domain.Services;

public class UserService : BaseService<User>
{
    protected UserService(UserRepository repository) : base(repository)
    {
    }
}