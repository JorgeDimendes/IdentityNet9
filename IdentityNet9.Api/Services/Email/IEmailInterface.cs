namespace IdentityNet9.Api.Services.Email
{
    public interface IEmailInterface
    {
        Task<bool> EnviarEmailAsync(string destinatario, string assunto, string mensagem);
    }
}