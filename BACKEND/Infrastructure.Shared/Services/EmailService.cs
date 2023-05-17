using Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Infrastructure.Shared.Services
{
    public class EmailService : IEmailService
    {
        public MailSettings MailSettings { get; }
        public ILogger<EmailService> Logger { get; }

        public EmailService(IOptions<MailSettings> mailSettings,ILogger<EmailService> logger)
        {
            MailSettings = mailSettings.Value;
            Logger = logger;
        }

        public async Task SendAsync(EmailRequest request)
        {
            try
            {
                // create message
                var email = new MailMessage();
                email.To.Add(request.To);
                email.From = new MailAddress(MailSettings.EmailFrom);
                email.Subject = request.Subject;
                email.Body = request.Body;
                email.IsBodyHtml = true;
                using var smtp = new SmtpClient(MailSettings.SmtpHost, MailSettings.SmtpPort);
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential(MailSettings.SmtpUser, MailSettings.SmtpPass);
                smtp.Send(email);
            }
            catch (System.Exception ex)
            {
                Logger.LogError(ex.Message, ex);
            }
        }
    }
}
