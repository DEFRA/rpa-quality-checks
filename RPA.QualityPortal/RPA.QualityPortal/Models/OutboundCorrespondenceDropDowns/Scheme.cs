using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns
{
    [ExcludeFromCodeCoverage]
    public class Scheme
    {
        public int SchemeId { get; set; }

        [Display(Name = "Scheme")]
        public string Text { get; set; }

        public bool Active { get; set; }
    }
}