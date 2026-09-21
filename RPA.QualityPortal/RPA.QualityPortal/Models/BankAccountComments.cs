using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RPA.QualityPortal.Models
{
    [ExcludeFromCodeCoverage]
    public class BankAccountComments
    {
        [Key]
        public int BankAccountCommentId { get; set; }

        [Display(Name = "Answer Comment")]
        public string AnswerComment { get; set; }

        public int AnswerId { get; set; }

        public int CheckId { get; set; }

        public int QuestionId { get; set; }
    }
}