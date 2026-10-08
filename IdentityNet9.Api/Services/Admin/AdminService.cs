using IdentityNet9.Api.Dtos;
using IdentityNet9.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityNet9.Api.Services.Admin
{
    public class AdminService : IAdminInterface
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ResponseModel<string>> AdicionarRoles(AtualizarUserRoleDto atualizarUserRoleDto)
        {
            ResponseModel<string> response = new ResponseModel<string>();

            try
            {
                var user = await _userManager.FindByIdAsync(atualizarUserRoleDto.UserId);
                if(user == null)
                {
                    response.Mensagem = "Usuario não localizado";
                    response.Status = false;
                    return response;
                }

                if(!await ValidarRoles(atualizarUserRoleDto.Roles))
                {
                    response.Mensagem = "Um ou mais perfis não existem!";
                    response.Status = false;
                    return response;
                }

                var resultado = await _userManager.AddToRolesAsync(user, atualizarUserRoleDto.Roles);
                if (!resultado.Succeeded)
                {
                    response.Mensagem = string.Join(", ", resultado.Errors.Select(a => a.Description));
                    response.Status = false;
                    return response;
                }

                response.Mensagem = "Perfis adicionados com sucesso!";

                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                return response;
            }
        }

        public async Task<ResponseModel<List<ListagemUserRoleDto>>> GetUsuariosComRoles()
        {
            ResponseModel<List<ListagemUserRoleDto>> response = new ResponseModel<List<ListagemUserRoleDto>>();
            try
            {
                var users = _userManager.Users.ToList();
                var usuariosComRoles = new List<ListagemUserRoleDto>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    usuariosComRoles.Add(new ListagemUserRoleDto
                    {
                        Id = user.Id,
                        Usuario = user.UserName,
                        Email = user.Email,
                        Roles = roles.ToList()
                    });
                }

                response.Mensagem = "Usuarios recuperados!";
                response.Dados = usuariosComRoles;
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                return response;
            }
        }

        public async Task<ResponseModel<string>> RemoverRoles(AtualizarUserRoleDto atualizarUserRoleDto)
        {
            ResponseModel<string> response = new ResponseModel<string>();

            try
            {
                var user = await _userManager.FindByIdAsync(atualizarUserRoleDto.UserId);
                if (user == null)
                {
                    response.Mensagem = "Usuario não localizado";
                    response.Status = false;
                    return response;
                }

                var resultado = await _userManager.RemoveFromRolesAsync(user, atualizarUserRoleDto.Roles);

                if (!resultado.Succeeded)
                {
                    response.Mensagem = string.Join(", ", resultado.Errors.Select(a => a.Description));
                    response.Status = false;
                    return response;
                }

                response.Mensagem = "Perfis removidos com sucesso!";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                return response;
            }
        }

        private async Task<bool> ValidarRoles(List<string> roles)
        {
            foreach(var role in roles)
            {
                if(!await _roleManager.RoleExistsAsync(role))
                {
                    return false;
                }
            }

            return true;
        }
    }
}