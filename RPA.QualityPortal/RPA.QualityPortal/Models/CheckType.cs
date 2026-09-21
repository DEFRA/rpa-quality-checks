using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.Models
{
    [ExcludeFromCodeCoverage]

    public class CheckType
    {
        [Key]
        public int CheckTypeId { get; set; }

        public string Name { get; set; }

        public int Order { get; set; }

    }
}