using IdentityNet9.Api.Models;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

namespace IdentityNet9.Api.Services.Email
{
    public class EmailService : IEmailInterface
    {
        private readonly EmailSettings _emailSettings;
        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public async Task<bool> EnviarEmailAsync(string destinatario, string assunto, string mensagem)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                email.To.Add(MailboxAddress.Parse(destinatario));
                email.Subject = assunto;

                email.Body = new TextPart("html") { Text = mensagem };

                using var smtp = new SmtpClient();

                await smtp.ConnectAsync(_emailSettings.SmtServer,
                                        _emailSettings.SmtPort,
                                        SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.SenderEmail,
                                             _emailSettings.SenderPassword);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar email: {ex.Message}");
                return false;
            }
        }
    }
}