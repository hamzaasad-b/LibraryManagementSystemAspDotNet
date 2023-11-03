using Api.Dto;
using Data.Entities;

namespace Api.Helpers;

public static class DtoHelpers
{
    public static UserResponseDto UserToUserResponseDto(User user)
    {
        return new UserResponseDto()
            { FullName = user.FullName, Email = user.Email!, Id = user.Id };
    }

    public static IEnumerable<UserResponseDto> UserToUserResponseDto(IEnumerable<User> users)
    {
        List<UserResponseDto> userResponseDtos = new List<UserResponseDto>();
        foreach (var user in users)
        {
            userResponseDtos.Add(new UserResponseDto()
                { FullName = user.FullName, Email = user.Email!, Id = user.Id });
        }

        return userResponseDtos;
    }
}