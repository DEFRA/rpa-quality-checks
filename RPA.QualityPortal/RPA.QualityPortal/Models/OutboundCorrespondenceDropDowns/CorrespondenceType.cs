using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns
{
    [ExcludeFromCodeCoverage]
    public class CorrespondenceType
    {
        public int CorrespondenceTypeId { get; set; }

        [Display(Name = "Correspondence Type")]
        public string Text { get; set; }

        public bool Active { get; set; }
    }
}