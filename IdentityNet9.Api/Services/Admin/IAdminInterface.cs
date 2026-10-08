using IdentityNet9.Api.Dtos;
using IdentityNet9.Api.Models;

namespace IdentityNet9.Api.Services.Admin
{
    public interface IAdminInterface
    {
        Task<ResponseModel<List<ListagemUserRoleDto>>> GetUsuariosComRoles();
        Task<ResponseModel<string>> AdicionarRoles(AtualizarUserRoleDto atualizarUserRoleDto);
        Task<ResponseModel<string>> RemoverRoles(AtualizarUserRoleDto atualizarUserRoleDto);
    }
}