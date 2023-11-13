using AutoMapper;
using Common.Dto.User;
using Data.Context;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class UserRepository : GenericRepository<User, UserDto>
{
    public UserRepository(LmsDbContext context, IMapper mapper) : base(context, mapper)
    {
    }
}