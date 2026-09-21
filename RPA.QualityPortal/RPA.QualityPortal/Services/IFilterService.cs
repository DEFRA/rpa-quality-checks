using RPA.QualityPortal.Models.CheckTypes;
using System.Collections.Generic;
using System.Linq;

namespace RPA.QualityPortal.Services
{
    public interface IFilterService
    {
        IQueryable<OutboundCorrespondence> OutboundCorrespondenceFilterChecks(string searchString, string pageName);

        IQueryable<BankAccountCheck> BankAccountFilterChecks(string searchString, string dateFrom, string dateTo, string pageName, bool? coachingPoint);

        List<OutboundCorrespondence> MIfilterChecks();

        List<int> GetValidCheckIDs();
    }
}
