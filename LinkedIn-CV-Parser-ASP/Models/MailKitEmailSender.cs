

using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Threading.Tasks;

namespace LinkedIn_CV_Parser_ASP.Models {
    public class MailKitEmailSender : IEmailSender {
        public MailKitEmailSender(IOptions<MailKitEmailSenderOptions> options) {
            this.Options = options.Value;
        }

        public MailKitEmailSenderOptions Options { get; set; }

        public Task SendEmailAsync(string email, string subject, string message) {
            return Execute(email, subject, message);
        }

        public async Task Execute(string to, string subject, string message) {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(Options.Sender_EMail);
            if (!string.IsNullOrEmpty(Options.Sender_Name))
                email.Sender.Name = Options.Sender_Name;
            email.From.Add(email.Sender);
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart("html") { Text = message };

            // send email
            using (var smtp = new SmtpClient()) {
                await smtp.ConnectAsync(Options.Host_Address, Options.Host_Port, SecureSocketOptions.SslOnConnect);
                await smtp.AuthenticateAsync(Options.Host_Username, Options.Host_Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
