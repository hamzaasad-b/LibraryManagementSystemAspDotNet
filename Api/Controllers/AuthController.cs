using System.Security.Claims;
using Api.Dto.Auth;
using Api.Dto.Common;
using Api.Jwt;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly AuthenticationService _authService;
        private readonly JwtManager _jwtManager;
        private readonly IConfiguration _config;

        public AuthController(AuthenticationService authService, JwtManager jwtManager, IConfiguration config)
        {
            _authService = authService;
            _jwtManager = jwtManager;
            _config = config;
        }
        // token

        [HttpPost("token")]
        public async Task<ResponseDto<AuthResponseDto>> Token([FromBody] LoginDto dto)
        {
            var result = await _authService.VerifyUser(dto.Email, dto.Password);
            if (!result.Success || result.Data is null)
            {
                return Unauthorized<AuthResponseDto>();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.Data.Id.ToString()
                    , ClaimValueTypes.Integer),
                new Claim(ClaimTypes.Email, result.Data.Email!),
                new Claim("sub", result.Data.Id.ToString(), ClaimValueTypes.Integer)
            };
            var token = _jwtManager.GenerateJwtToken(_config["JwtSettings:Key"]!
                , _config["JwtSettings:Issuer"]!,
                _config["JwtSettings:Audience"]!, 60, claims);

            return ResponseDto.Successful(new AuthResponseDto() { Token = token });
        }

        [HttpPost("register")]
        public async Task<ResponseDto> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterUser(dto.Email, dto.Password);
            return result.Success
                ? Success()
                : BadRequest<ResponseDto>(errors: result.Errors);
        }
    }
}