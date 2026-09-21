using System.Net.Mail;

namespace RPA.QualityPortal.Services
{
    public interface ISendService
    {
        void sendEmail(MailMessage mail);
    }
}