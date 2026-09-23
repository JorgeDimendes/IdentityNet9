using IdentityNet9.Api.Dtos;
using IdentityNet9.Api.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace IdentityNet9.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthInterface _authInterface;

        public AuthController(IAuthInterface authInterface)
        {
            _authInterface = authInterface;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Registrar(RegisterDto registerDto)
        {
            var resultado = await _authInterface.Register(registerDto);
            if(!resultado.Status) return BadRequest(resultado);

            return Ok(resultado);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var resultado = await _authInterface.Login(loginDto);
            if (!resultado.Status) return BadRequest(resultado);

            return Ok(resultado);
        }
    }
}