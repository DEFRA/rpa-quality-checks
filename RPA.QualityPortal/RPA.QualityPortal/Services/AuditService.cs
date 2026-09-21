using RPA.QualityPortal.DAL;
using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.CheckTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.Services
{
    public class AuditService : IAuditService
    {
        IQualityContext db;

        public AuditService(IQualityContext context)
        {
            this.db = context;
        }

        public void SaveOutboundCorrespondenceAmendmentAudit(OutboundCorrespondence outboundCorrespondence, CheckAmendmentReason amendmentReason)
        {
            Audit audit = new Audit
            {
                AmendmentReason = amendmentReason.AmendmentReason,
                Check = db.Checks.Where(x => x.CheckId == outboundCorrespondence.CheckId).FirstOrDefault(),
                CheckId = outboundCorrespondence.CheckId,
                Comments = amendmentReason.AmendmentReasonComments 
            };

            db.Audits.Add(audit);

            db.SaveChanges();
        }

        public void SaveOutboundCorrespondenceReCheckAmendmentAudit(OutboundCorrespondenceReCheck outboundCorrespondenceReCheck, CheckAmendmentReason amendmentReason)
        {
            Audit audit = new Audit
            {
                AmendmentReason = amendmentReason.AmendmentReason,
                Check = db.Checks.Where(x => x.CheckId == outboundCorrespondenceReCheck.CheckId).FirstOrDefault(),
                CheckId = outboundCorrespondenceReCheck.CheckId,
                Comments = amendmentReason.AmendmentReasonComments
            };

            db.Audits.Add(audit);

            db.SaveChanges();
        }

        public void SaveBankAccountAmendmentAudit(BankAccountCheck bankAccount, CheckAmendmentReason amendmentReason)
        {
            Audit audit = new Audit
            {
                AmendmentReason = amendmentReason.AmendmentReason,
                Check = db.Checks.Where(x => x.CheckId == bankAccount.CheckId).FirstOrDefault(),
                CheckId = bankAccount.CheckId,
                Comments = amendmentReason.AmendmentReasonComments
            };

            db.Audits.Add(audit);

            db.SaveChanges();
        }

        public void SaveBankAccountReCheckAmendmentAudit(BankAccountReCheck bankAccountReCheck, CheckAmendmentReason amendmentReason)
        {
            Audit audit = new Audit
            {
                AmendmentReason = amendmentReason.AmendmentReason,
                Check = db.Checks.Where(x => x.CheckId == bankAccountReCheck.CheckId).FirstOrDefault(),
                CheckId = bankAccountReCheck.CheckId,
                Comments = amendmentReason.AmendmentReasonComments
            };

            db.Audits.Add(audit);

            db.SaveChanges();
        }
    }
}