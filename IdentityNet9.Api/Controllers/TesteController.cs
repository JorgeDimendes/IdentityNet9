using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityNet9.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TesteController : ControllerBase
    {
        [HttpGet("publico")]
        public IActionResult Publico()
        {
            return Ok("Publico");
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            return Ok("Admin");
        }

        [HttpGet("user")]
        [Authorize(Roles = "User")]
        public IActionResult User()
        {
            return Ok("User");
        }

        [HttpGet("userAdmin")]
        [Authorize(Roles = "User, Admin")]
        public IActionResult UserAdmin()
        {
            return Ok("User e Admin");
        }
    }
}
