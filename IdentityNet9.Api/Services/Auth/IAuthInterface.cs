using IdentityNet9.Api.Dtos;
using IdentityNet9.Api.Models;

namespace IdentityNet9.Api.Services.Auth
{
    public interface IAuthInterface
    {
        Task<ResponseModel<string>> Register(RegisterDto registerDto);
        Task<ResponseModel<string>> Login(LoginDto loginDto);
    }
}