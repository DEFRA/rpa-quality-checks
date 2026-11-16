using System.Net;
using System.Net.Mail;

namespace RPA.QualityPortal.Services
{
    public class SendService : ISendService
    {
        private readonly IConfiguration configuration;

        public SendService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // Mail settings replace <system.net><mailSettings> from Web.config. Azure Container Apps containers
        // have no reachable UNC pickup directory, so Mail:Host must always resolve to an authenticated SMTP
        // relay/managed email API (e.g., SendGrid, Exchange Online) - there is no local pickup-directory
        // fallback (AppCAT Local.0002/SMTP.0001).
        public void sendEmail(MailMessage mail)
        {
            var host = configuration["Mail:Host"];
            if (string.IsNullOrWhiteSpace(host))
                throw new InvalidOperationException("Required external configuration 'Mail:Host' is missing. " +
                    "Configure an authenticated SMTP relay; pickup-directory delivery is not supported on Azure Container Apps.");

            using (SmtpClient client = new SmtpClient(host, configuration.GetValue("Mail:Port", 587)))
            {
                client.EnableSsl = configuration.GetValue("Mail:EnableSsl", true);

                var username = configuration["Mail:Username"];
                if (!string.IsNullOrEmpty(username))
                    client.Credentials = new NetworkCredential(username, configuration["Mail:Password"]);

                client.Send(mail);
            }
        }
    }
}
