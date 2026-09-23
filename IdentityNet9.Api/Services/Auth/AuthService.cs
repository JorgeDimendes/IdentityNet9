using IdentityNet9.Api.Dtos;
using IdentityNet9.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityNet9.Api.Services.Auth
{
    public class AuthService : IAuthInterface
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, 
                           RoleManager<IdentityRole> roleManager, 
                           IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<ResponseModel<string>> Register(RegisterDto registerDto)
        {
            ResponseModel<string> response = new ResponseModel<string>();

            try
			{
                var user = new ApplicationUser
                {
                    Email = registerDto.Email,
                    NomeCompleto = registerDto.NomeCompleto,
                    UserName = registerDto.Usuario
                };

                var result = await _userManager.CreateAsync(user, registerDto.Senha);

                if (!result.Succeeded)
                {
                    response.Dados = string.Join(", ", result.Errors.Select(e => e.Description));
                    response.Status = false;
                    return response;
                }

                //Roles
                var rolesInvalidas = new List<string>();
                foreach (var role in registerDto.Roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        rolesInvalidas.Add(role);
                    }
                }

                if (rolesInvalidas.Any())
                {
                    response.Dados = $"As seguintes roles não existem: {string.Join(", ", rolesInvalidas)}";
                    response.Status = false;
                    return response;
                }

                foreach (var role in registerDto.Roles)
                {
                    await _userManager.AddToRoleAsync(user, role);
                }

                response.Mensagem = "Usuario cadastrado com sucesso";
                return response;
            }
			catch (Exception ex)
			{
                response.Mensagem = ex.Message;
                response.Status = false;

                return response;
			}
        }

        public async Task<ResponseModel<string>> Login(LoginDto loginDto)
        {
            ResponseModel<string> response = new ResponseModel<string>();
            try
            {
                //Verificar se existe usuario e se está correto
                var user = await _userManager.FindByNameAsync(loginDto.Usuario);
                if(user == null)
                {
                    response.Mensagem = "Credenciais invalidas";
                    response.Status = false;
                    return response;
                }

                //Verificar se a senha está correto
                var validPassword = await _userManager.CheckPasswordAsync(user, loginDto.Senha);
                if (!validPassword)
                {
                    response.Mensagem = "Credenciais invalidas";
                    response.Status = false;
                    return response;
                }

                //Claims
                var authClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("nome", user.NomeCompleto),
                    new Claim("usuario", user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                };

                //Claims de roles
                var userRoles = await _userManager.GetRolesAsync(user);
                foreach(var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                //Jwt
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
                var token = new JwtSecurityToken(
                        issuer: _configuration["Jwt:Issuer"],
                        audience: _configuration["Jwt:Audience"],
                        claims: authClaims,
                        expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"])),
                        signingCredentials: creds
                    );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                response.Dados = tokenString;
                response.Mensagem = "Usuario logado com sucesso";

                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                return response;
            }
        }
    }
}