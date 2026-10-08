namespace IdentityNet9.Api.Dtos
{
    public class AtualizarUserRoleDto
    {
        public string UserId { get; set; }
        public List<string> Roles { get; set; }
    }
}