using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace RPA.QualityPortal.Services
{
    public interface IOutboundCorrespondenceService
    {
        QuestionAnswerList QCResultCalculate(QuestionAnswerList qaList);

        void SaveOutboundCorrespondence(OutboundCorrespondence outboundCorrespondence, QuestionAnswerList qaList);

        void SaveEditOutboundCorrespondence(OutboundCorrespondence outboundCorrespondence, QuestionAnswerList qaList, string previousResult);

        bool CheckDuplicates(string UniqueId, int UniqueIndetififerPrefixId, string CrmRef, int CRMRefPrefixId, bool DuplicatedRecord = false);

        bool CheckEditDuplicates(string UniqueId, int UniqueIndetififerPrefixId, string CrmRef, int CRMRefPrefixId, int CheckId, bool DuplicatedRecord = false);

        void SaveOutboundCorrespondenceReCheck(OutboundCorrespondenceReCheck outboundCorrespondenceReCheck, QuestionAnswerList qaList);

        void SaveEditOutboundCorrespondenceReCheck(OutboundCorrespondenceReCheck outboundCorrespondenceReCheck, QuestionAnswerList qaList);

        IQueryable<OutboundCorrespondenceOverview> RetrieveOutboundCorrespondenceChecks(IQueryable<OutboundCorrespondence> outboundCorrespondenceChecks);

        IQueryable<OutboundCorrespondenceOverview> RetrieveOutboundCorrespondenceRechecks(IQueryable<OutboundCorrespondenceReCheck> outboundCorrespondenceRechecks);

        void DeleteCheck(int CheckId);
        void DeleteReCheck(int CheckId);
    }
}