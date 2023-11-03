using Data.Context;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class UserRepository : GenericRepository<User>
{
    public UserRepository(LmsDbContext context) : base(context)
    {
    }
}