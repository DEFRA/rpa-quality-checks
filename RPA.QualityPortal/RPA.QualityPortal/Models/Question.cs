using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.Models
{
    [ExcludeFromCodeCoverage]

    public class Question
    {
        [Key]
        public int QuestionId { get; set; }

        [Display(Name = "Question")]
        public string Text { get; set; }
    }
}