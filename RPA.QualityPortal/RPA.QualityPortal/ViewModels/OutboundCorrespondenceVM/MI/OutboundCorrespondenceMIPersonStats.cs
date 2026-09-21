using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.ViewModels.OutboundCorrespondenceVM.MI
{
    [ExcludeFromCodeCoverage]
    public class OutboundCorrespondenceMIPersonStats
    {
        [Display(Name = "Name")]
        public string PersonName { get; set; }

        [Display(Name = "Line Manager Name")]
        public string LineManagerName { get; set; }

        [Display(Name = "HEO Name")]
        public string HEOName { get; set; }
       
        [Display(Name = "SEO Name")]
        public string  SEOName { get; set; }

        public OutboundCorrespondenceMIStats OutboundCorrespondenceMIStats { get; set; }

 
    }
}