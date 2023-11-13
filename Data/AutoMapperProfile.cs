using AutoMapper;
using Common.Dto.Book;
using Common.Dto.User;
using Data.Entities;

namespace Data;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<BookDto, Book>()
            .ForMember(des => des.Id, opt => opt.Condition(src => src.Id != null));
        CreateMap<Book, BookDto>();
        CreateMap<UserDto, User>()
            .ForMember(des => des.Id, opt => opt.Condition(src => src.Id != null));
        CreateMap<User, UserDto>();
    }
}