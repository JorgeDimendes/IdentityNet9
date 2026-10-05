namespace IdentityNet9.Api.Models
{
    public class EmailSettings
    {
        public string SmtServer { get; set; } = string.Empty;
        public int SmtPort { get; set; }
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string SenderPassword { get; set; } = string.Empty;
        public bool UseSsl { get; set; }
    }
}