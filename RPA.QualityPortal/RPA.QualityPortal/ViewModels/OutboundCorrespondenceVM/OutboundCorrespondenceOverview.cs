using RPA.QualityPortal.Models.CheckTypes;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RPA.QualityPortal.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class OutboundCorrespondenceOverview
    {
        public OutboundCorrespondence OutboundCorrespondence { get; set; }

        [Display(Name = "QC Result")]
        public string QCResult { get; set; }

        [Display(Name = "QC Recheck Result")]
        public string ReCheckQCResult { get; set; }

        public OutboundCorrespondenceReCheck OutboundCorrespondenceReCheck { get; set; }
    }
}