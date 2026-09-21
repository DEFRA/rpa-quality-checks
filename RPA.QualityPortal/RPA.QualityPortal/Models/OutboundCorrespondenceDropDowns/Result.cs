using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.Models
{
    [ExcludeFromCodeCoverage]

    public class Result
    {
        [Key]
        public int ResultId { get; set; }

        [Display(Name = "QC Result")]
        public string Text { get; set; }

    }
}