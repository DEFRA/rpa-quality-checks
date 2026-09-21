using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.Models
{
    [ExcludeFromCodeCoverage]

    public class CheckResult
    {
        [Key]
        public int CheckResultId { get; set; }

        public int CheckId { get; set; }

        public int ResultId { get; set; }

        public bool Active { get; set; }

        public virtual Check Check { get; set; }

        public virtual Result Result { get; set; }

    }
}