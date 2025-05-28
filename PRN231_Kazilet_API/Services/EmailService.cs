using Microsoft.Extensions.Options;
using MimeKit;

namespace PRN231_Kazilet_API.Services
{
    public class EmailService
    {
        private string _smtpServer;
        private int _smtpPort;
        private string _username;
        private string _password;
        private string _displayName;
        private readonly IConfiguration mailSettings;

        public EmailService(IConfiguration configuration)
        {
            mailSettings = configuration;
            _smtpServer = mailSettings["MailSettings:Host"];
            _smtpPort = int.Parse(mailSettings["MailSettings:Port"]);
            _username = mailSettings["MailSettings:Mail"];
            _password = mailSettings["MailSettings:Password"];
            _displayName = mailSettings["MailSettings:DisplayName"];
        }
        public async Task SendEmailAsync(string toEmail, string tag, string subject, string htmlText)
        {
            //var filePath = Path.Combine(_env.WebRootPath, "template", fileName);
            //var htmlContent = await File.ReadAllTextAsync(filePath);
            // htmlContent.replace("{new@Password!Here}", newPwd);

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_displayName, _username));
            email.To.Add(new MailboxAddress("", toEmail));
            email.Subject = !string.IsNullOrEmpty(tag) ? $"[{tag}] {subject}" : subject;

            var body = new TextPart("html")
            {
                Text = htmlText
            };

            email.Body = body;

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_username, _password);

                await client.SendAsync(email);
                await client.DisconnectAsync(true);
            }
        }
    }
}
