using RPA.QualityPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.ViewModels
{

    [ExcludeFromCodeCoverage]
    public class QuestionAnswer
    {
        public CheckQuestion CheckQuestion { get; set; }

        [Required(ErrorMessage = "Please Select an Answer")]
        public string Answer { get; set; }

        public string Comment { get; set; }
    }
}