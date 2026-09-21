using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace RPA.QualityPortal.Services
{
    public class SendService : ISendService
    {
        public void sendEmail(MailMessage mail)
        {

            SmtpClient client = new SmtpClient();
            client.Send(mail);              

        }
    }
}