namespace RPA.QualityPortal
{
    using Microsoft.EntityFrameworkCore;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class PeopleContext : DbContext, IPeopleContext
    {
        public PeopleContext(DbContextOptions<PeopleContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Manager> Managers { get; set; }
        public virtual DbSet<Person> People { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            // EF6 mapped DateTime to the legacy "datetime" type.
            configurationBuilder.Properties<DateTime>().HaveColumnType("datetime");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // The shared People database lives in the OLA schema; EF6 split "OLA.People" at the dot, EF Core does not.
            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("Locations", "OLA");
                entity.HasKey(x => x.id);
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.ToTable("People", "OLA");
                entity.HasKey(x => x.Id);
                entity.HasOne(x => x.Location).WithMany(x => x.People).HasForeignKey(x => x.LocationId);
            });

            modelBuilder.Entity<Manager>(entity =>
            {
                entity.ToTable("Managers", "OLA");
                entity.HasKey(x => new { x.personId, x.managerId });
                entity.HasOne(x => x.Person).WithMany(x => x.Managers).HasForeignKey(x => x.personId);
            });
        }
    }
}