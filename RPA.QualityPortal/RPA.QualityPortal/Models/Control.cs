using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.Models
{
    [ExcludeFromCodeCoverage]

    public class Control
    {
        [Key]
        public int ControlId { get; set; }

        [Required]
        public string Property { get; set; }

        public bool Active { get; set; }
    }
}