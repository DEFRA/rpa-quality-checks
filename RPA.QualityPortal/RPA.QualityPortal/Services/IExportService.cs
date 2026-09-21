using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.Models;
using OfficeOpenXml;

namespace RPA.QualityPortal.Services
{
    public interface IExportService
    {
        string Build(IEnumerable<OutboundCorrespondence> outboundCorrespondences, IEnumerable<OutboundCorrespondenceReCheck> outboundCorrespondenceReChecks);

        string BuildBank(IEnumerable<BankAccountCheck> bankAccounts, IEnumerable<BankAccountReCheck> bankAccountReChecks);
    }
}
