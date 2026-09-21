namespace RPA.QualityPortal
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class PeopleContext : DbContext, IPeopleContext
    {
        public PeopleContext()
            : base("name=PeopleContext")
        {
        }

        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Manager> Managers { get; set; }
        public virtual DbSet<Person> People { get; set; }


    }
}
