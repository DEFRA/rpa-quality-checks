using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RPA.QualityPortal.Models
{
    [ExcludeFromCodeCoverage]
    public class WorkerServiceLog
    {
        [Key]
        public int Id { get; set; }

        public DateTime LastChange { get; set; }

        public int Changes { get; set; }
    }
}