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

    public class CheckQuestion
    {
        [Key, Column(Order = 0)]
        public int CheckTypeId { get; set; }

        [Key, Column(Order = 1)]
        public int QuestionId { get; set; }

        public int Order { get; set; }

        public virtual CheckType CheckType { get; set; }

        public virtual Question Question { get; set; }
    }
}