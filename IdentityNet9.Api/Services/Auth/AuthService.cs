using IdentityNet9.Api.Dtos;
using IdentityNet9.Api.Models;
using IdentityNet9.Api.Services.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace IdentityNet9.Api.Services.Auth
{
    public class AuthService : IAuthInterface
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailInterface _emailInterface;

        public AuthService(UserManager<ApplicationUser> userManager, 
                           RoleManager<IdentityRole> roleManager, 
                           IConfiguration configuration,
                           IEmailInterface emailInterface)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailInterface = emailInterface;
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

                //Confirmação de Email
                await ConfirmacaoEmail(user);

                response.Mensagem = "Usuario cadastrado com sucesso. Verifique seu e-mail para confirmar";
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

                //Deixar obrigatorio a confirmação do email
                if(!await _userManager.IsEmailConfirmedAsync(user))
                {
                    response.Mensagem = "E-mail não confirmado";
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

        //Confirmação de Email
        private async Task ConfirmacaoEmail(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var confirmUrl = $"https://localhost:7071/api/auth/confirmar-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";
            //var confirmUrl =
            //                $"https://localhost:7071/api/auth/confirmar-email" +
            //                $"?userId={Uri.EscapeDataString(user.Id)}" +
            //                $"&token={Uri.EscapeDataString(token)}";

            var mensagem = $"<h3>Confirme seu e-mail</h3><p>Clique no link para confirmar: <a href='{confirmUrl}'>Confirmar</a></p>";

            await _emailInterface.EnviarEmailAsync(user.Email, "Confirmação de E-mail", mensagem);
        }

        public async Task<ResponseModel<string>> ConfirmarEmail(string userId, string token)
        {
            ResponseModel<string> response = new ResponseModel<string>();

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if(user == null)
                {
                    response.Mensagem = "Usuario não localizado";
                    response.Status = false;
                    return response;
                }

                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (!result.Succeeded)
                {
                    response.Mensagem = "Erro ao confirmar o email!";
                    response.Status = false;
                    return response;
                }

                response.Mensagem = "email confirmado com sucesso";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status= false;
                return response;
            }
        }

        public async Task<ResponseModel<string>> EsqueciSenha(EsqueciSenhaDto esqueciSenhaDto)
        {
            ResponseModel<string> response = new ResponseModel<string>();

            try
            {
                var user = await _userManager.FindByNameAsync(esqueciSenhaDto.Usuario);
                if (user == null)
                {
                    response.Mensagem = "Usuario não localizado";
                    response.Status = false;
                    return response;
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var resetLink = $"https://localhost:7071/api/auth/resetar-senha?token={WebUtility.UrlEncode(token)}";

                var mensagem = $"Clique no link para redefinir sua senha: <a href='{resetLink}'>Redefinir Senha</a>";

                var emailEnviado = await _emailInterface.EnviarEmailAsync(user.Email, "Redefinição de Senha", mensagem);

                if (!emailEnviado)
                {
                    response.Mensagem = "Falha ao enviar email";
                    response.Status = false;
                    return response;
                }

                response.Mensagem = "Se o email estiver cadastrado, um link de redefinição será enviado!";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                return response;
            }
        }

        public async Task<ResponseModel<string>> ResetarSenha(ResetarSenhaDto resetarSenhaDto)
        {
            ResponseModel<string> response = new ResponseModel<string>();

            try
            {
                var user = await _userManager.FindByNameAsync(resetarSenhaDto.Usuario);
                if (user == null)
                {
                    response.Mensagem = "Usuario não localizado";
                    response.Status = false;
                    return response;
                }

                var result = await _userManager.ResetPasswordAsync(user, WebUtility.UrlDecode(resetarSenhaDto.Token), 
                             resetarSenhaDto.NovaSenha);

                if (!result.Succeeded)
                {
                    response.Status = false;
                    response.Mensagem = string.Join(", ", result.Errors.Select(e => e.Description));
                    return response;
                }

                response.Mensagem = "Senha redefinida com sucesso";
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