namespace IdentityNet9.Api.Dtos
{
    public class ListagemUserRoleDto
    {
        public string Id { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}