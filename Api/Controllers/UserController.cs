using System.Net;
using Api.Dto;
using Api.Dto.Common;
using Common.Dto.User;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly AuthenticationService _authService;
        private readonly UserService _userService;

        public UserController(AuthenticationService authService, UserService userService)
        {
            _authService = authService;
            _userService = userService;
        }


        // GET: api/User
        [HttpGet]
        public async Task<ResponseDto<IEnumerable<UserDto>>> Get()
        {
            var result = await _userService.GetAllEntities();
            if (!result.Success)
            {
                return Fail<IEnumerable<UserDto>>(default);
            }

            return Success(result.Data!);
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ResponseDto<UserDto?>> Get(uint id)
        {
            var result = await _userService.GetById(id);
            if (result.Success)
            {
                return result.Data is not null
                    ? Success<UserDto?>(result.Data)
                    : NotFound<UserDto?>();
            }

            return Fail<UserDto?>(default);
        }

        // POST: api/User
        [HttpPost]
        public async Task<ResponseDto> Post([FromBody] CreateUserDto userDto)
        {
            var result = await _authService.RegisterUser(userDto.Email, null, userDto.FullName);
            return result.Success
                ? Success()
                : Fail();
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<ResponseDto> Put(uint id, [FromBody] CreateUserDto userDto)
        {
            var result = await _userService.GetById(id);
            if (!result.Success || result.Data is null)
            {
                return WithHttpStatus(HttpStatusCode.NotFound);
            }

            result.Data.Email = userDto.Email;
            result.Data.FullName = userDto.FullName;
            var res = await _userService.Update(id, result.Data);

            return res.Success
                ? Success()
                : Fail();
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(uint id)
        {
            var result = await _userService.DeleteTEntity(id);
            return result.Success
                ? Success()
                : Fail();
        }
    }
}