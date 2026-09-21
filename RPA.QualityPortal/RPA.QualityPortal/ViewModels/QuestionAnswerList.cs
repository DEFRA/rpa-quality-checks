using RPA.QualityPortal.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.ViewModels
{

    [ExcludeFromCodeCoverage]
    public class QuestionAnswerList
    {
        public List<QuestionAnswer> QuestionAnswers { get; set; }

        public string QCResult { get; set; }

        public QuestionAnswerList()
        {
            QuestionAnswers = new List<QuestionAnswer>();
        }

        public bool? Challenge  { get; set; }
    }
}