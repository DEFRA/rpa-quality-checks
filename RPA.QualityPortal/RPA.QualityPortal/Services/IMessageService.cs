using RPA.QualityPortal.Models.CheckTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace RPA.QualityPortal.Services
{
    public interface IMessageService
    {
        MailMessage GenerateOutboundCorrespondenceEmail(OutboundCorrespondence outboundCorrespondence);

        MailMessage GenerateBankEmail(BankAccountCheck bankAccount);
    }
}