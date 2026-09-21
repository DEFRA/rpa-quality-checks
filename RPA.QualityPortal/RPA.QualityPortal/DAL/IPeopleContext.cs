using System.Data.Entity;

namespace RPA.QualityPortal
{
    public interface IPeopleContext
    {
        DbSet<Location> Locations { get; set; }
        DbSet<Manager> Managers { get; set; }
        DbSet<Person> People { get; set; }
    }
}