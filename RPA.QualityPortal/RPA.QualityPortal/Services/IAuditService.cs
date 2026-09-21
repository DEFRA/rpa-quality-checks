using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.CheckTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.QualityPortal.Services
{
    public interface IAuditService
    {
        void SaveOutboundCorrespondenceAmendmentAudit(OutboundCorrespondence outboundCorrespondence, CheckAmendmentReason amendmentReason);

        void SaveOutboundCorrespondenceReCheckAmendmentAudit(OutboundCorrespondenceReCheck outboundCorrespondenceReCecek, CheckAmendmentReason amendmentReason);

        void SaveBankAccountAmendmentAudit(BankAccountCheck bankAccount, CheckAmendmentReason amendmentReason);

        void SaveBankAccountReCheckAmendmentAudit(BankAccountReCheck bankAccountReCheck, CheckAmendmentReason amendmentReason);
    }
}
