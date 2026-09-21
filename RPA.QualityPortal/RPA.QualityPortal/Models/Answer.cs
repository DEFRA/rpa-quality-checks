using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;


namespace RPA.QualityPortal.Models
{
    [ExcludeFromCodeCoverage]
    public class Answer
    {
        [Key]
        public int AnswerId { get; set; }

        public string Text { get; set; }

        public int QuestionId { get; set; }

        public int CheckId { get; set; }

        public virtual Question Question { get; set; }

    }
}