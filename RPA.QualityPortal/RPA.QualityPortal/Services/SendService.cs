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

        // Mail settings replace <system.net><mailSettings> from Web.config:
        // Mail:Host delivers over SMTP, otherwise messages are written to Mail:PickupDirectory.
        public void sendEmail(MailMessage mail)
        {
            using (SmtpClient client = new SmtpClient())
            {
                var host = configuration["Mail:Host"];

                if (!string.IsNullOrEmpty(host))
                {
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Host = host;
                    client.Port = configuration.GetValue("Mail:Port", 25);
                }
                else
                {
                    client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    client.PickupDirectoryLocation = configuration["Mail:PickupDirectory"];
                }

                client.Send(mail);
            }
        }
    }
}
