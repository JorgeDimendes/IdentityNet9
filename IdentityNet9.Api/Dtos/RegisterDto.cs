namespace IdentityNet9.Api.Dtos
{
    public class RegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string NomeCompleto { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}