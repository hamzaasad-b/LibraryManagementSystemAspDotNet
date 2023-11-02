using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class UserRepository : GenericRepository<User>
{
    public UserRepository(DbContext context) : base(context)
    {
    }
}