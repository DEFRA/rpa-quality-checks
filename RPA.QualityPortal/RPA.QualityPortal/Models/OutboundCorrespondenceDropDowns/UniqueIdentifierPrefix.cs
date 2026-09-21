using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns
{
    [ExcludeFromCodeCoverage]
    public class UniqueIdentifierPrefix
    {
        public int UniqueIdentifierPrefixId { get; set; }

        public string Text { get; set; }

        public bool Active { get; set; }
    }
}