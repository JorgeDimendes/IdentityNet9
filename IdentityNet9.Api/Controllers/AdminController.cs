using IdentityNet9.Api.Dtos;
using IdentityNet9.Api.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityNet9.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminInterface _adminInterface;
        public AdminController(IAdminInterface adminInterface)
        {
            _adminInterface = adminInterface;
        }

        [HttpGet("usuarios")]
        public async Task<IActionResult> GetUsuariosComRoles()
        {
            var usuarios = await _adminInterface.GetUsuariosComRoles();

            if (!usuarios.Status) return BadRequest(usuarios);

            return Ok(usuarios);
        }

        [HttpPost("adicionar-roles")]
        public async Task<IActionResult> AdicionarRoles(AtualizarUserRoleDto atualizarUserRoleDto)
        {
            var restultado = await _adminInterface.AdicionarRoles(atualizarUserRoleDto);

            if (!restultado.Status) return BadRequest(restultado);

            return Ok(restultado);
        }

        [HttpPost("remover-roles")]
        public async Task<IActionResult> RemoverRoles(AtualizarUserRoleDto atualizarUserRoleDto)
        {
            var restultado = await _adminInterface.RemoverRoles(atualizarUserRoleDto);

            if (!restultado.Status) return BadRequest(restultado);

            return Ok(restultado);
        }
    }
}