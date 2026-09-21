using System.Collections.Generic;
using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.ViewModels.OutboundCorrespondenceVM;
using RPA.QualityPortal.ViewModels.OutboundCorrespondenceVM.MI;

namespace RPA.QualityPortal.Services
{
    public interface IMIService
    {
        OutboundCorrespondenceMI CollateMI(OutboundCorrespondenceMI outboundCorrespondenceMI);
        List<OutboundCorrespondenceReCheck> GetAllReChecks(List<OutboundCorrespondence> outboundCorrespondence);
        OutboundCorrespondenceMI OutboundCorrespondenceDateSearch(OutboundCorrespondenceMI outboundCorrespondenceMI, string dateFrom, string dateTo);
        OutboundCorrespondenceMI OutboundCorrespondenceMISearch(string searchString, string searchOption, string dateFrom, string dateTo, int? SchemeId, int? BusinessAreaId, int? CorrespondenceTypeId, OutboundCorrespondenceMI outboundCorrespondenceMI);
        List<OutboundCorrespondenceMIPersonStats> OutboundCorrespondencePersonStats(OutboundCorrespondenceMI outboundCorrespondenceMI, string searchType);
        OutboundCorrespondenceMIDetails CollateMIDetails(OutboundCorrespondenceMI outboundCorrespondenceMI, string personName, bool detailsClick);
        List<FailReasonCount> GetFailDetails(List<OutboundCorrespondence> outboundCorrespondenceFails);
        List<AnswerMI> AnswerMIList();
        List<AnswerMI> AnswerMIListSingleUser(string personName);
    }
}