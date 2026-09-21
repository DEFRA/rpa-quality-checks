using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.Models
{
    [ExcludeFromCodeCoverage]
    public class LockCheck
    {
        public int LockCheckId { get; set; }

        public int CheckId { get; set; }

        public DateTime Timestamp { get; set; }

        [Display(Name = "Check Locked By")]
        public string CheckLockedBy { get; set; }

        public LockCheck()
        {
            Timestamp = DateTime.Now;
        }
    }
}