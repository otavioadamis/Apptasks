using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using WebApplication1.Interfaces;
using WebApplication1.Models.Email;
using WebApplication1.Repository;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace WebApplication1.Services.EmailServices
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings emailSettings;
        private readonly IUserService _userService;

        public EmailService(IOptions<EmailSettings> options, IUserService userService)
        {
            this.emailSettings = options.Value;
            this._userService = userService;
        }

        public async Task SendEmailAsync(HashSet<string> emails)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(emailSettings.Email);
            email.Subject = "A deadline is coming!";

            using var smtp = new SmtpClient();
            smtp.Connect(emailSettings.Host, emailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(emailSettings.Email, emailSettings.Password);

            foreach (string x in emails)
            {
                email.To.Add(MailboxAddress.Parse(x));
                var userName = _userService.GetUserByEmail(x);
                
                var builder = new BodyBuilder();
                builder.HtmlBody = "Hello, " + userName.Name + "! A task deadline is tomorrow! Please check your app!";
                
                email.Body = builder.ToMessageBody();
                await smtp.SendAsync(email);
            }

            smtp.Disconnect(true);
        }
    }
}
