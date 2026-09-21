using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.ViewModels.OutboundCorrespondenceVM.MI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.ViewModels.OutboundCorrespondenceVM.MI
{
    [ExcludeFromCodeCoverage]
    public class OutboundCorrespondenceMI
    {
        public OutboundCorrespondenceMIStats OutboundCorrespondenceMIStats { get; set; }

        public string SearchType { get; set; }

        public string ResultHeader { get; set; }
        public List<OutboundCorrespondence> OutboundCorrespondenceList { get; set; }

        public List<OutboundCorrespondenceReCheck> OutboundCorrespondenceReCheckList { get; set; }

        [Display(Name = "From: ")]
        public DateTime? DateFrom { get; set; }

        [Display(Name = "To: ")]
        public DateTime? DateTo { get; set; }

        public List<OutboundCorrespondenceMIPersonStats> OutboundCorrespondenceMIPersonStatsList { get; set; }
    }
}