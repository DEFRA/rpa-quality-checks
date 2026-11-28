namespace RPA.QualityPortal
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class Manager
    {
        public Guid personId { get; set; }

        public Guid managerId { get; set; }

        public virtual Person Person { get; set; }
    }
}
