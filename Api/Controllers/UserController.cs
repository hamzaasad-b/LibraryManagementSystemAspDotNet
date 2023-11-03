using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Api.Dto;
using Api.Dto.Common;
using Api.Helpers;
using Data.Entities;
using Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;

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
        public async Task<ResponseDto<IEnumerable<UserResponseDto>>> Get()
        {
            var result = await _userService.GetAllEntities();
            if (!result.Success)
            {
                return Fail<IEnumerable<UserResponseDto>>(default);
            }

            return Success(DtoHelpers.UserToUserResponseDto(result.Data!));
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ResponseDto<UserResponseDto?>> Get(uint id)
        {
            var result = await _userService.GetById(id);
            if (result.Success)
            {
                return result.Data is not null
                    ? Success<UserResponseDto?>(DtoHelpers.UserToUserResponseDto(result.Data))
                    : NotFound<UserResponseDto?>();
            }

            return Fail<UserResponseDto?>(default);
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