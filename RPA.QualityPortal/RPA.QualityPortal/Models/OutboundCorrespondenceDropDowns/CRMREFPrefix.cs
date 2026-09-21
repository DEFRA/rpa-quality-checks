using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns
{
    [ExcludeFromCodeCoverage]
    public class CRMRefPrefix
    {
        public int CRMRefPrefixId { get; set; }

        public string Text { get; set; }

        public bool Active { get; set; }
    }
}