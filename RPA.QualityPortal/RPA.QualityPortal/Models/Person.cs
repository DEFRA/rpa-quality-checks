namespace RPA.QualityPortal
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Diagnostics.CodeAnalysis;

    [Table("OLA.People")]
    [ExcludeFromCodeCoverage]
    public partial class Person
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Person()
        {
            Managers = new HashSet<Manager>();
        }

        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [StringLength(255)]
        [Column("name")]
        public string Name { get; set; }

        [StringLength(255)]
        [Column("email")]
        public string Email { get; set; }

        [Column("locationId")]
        public Guid LocationId { get; set; }

        [Required]
        [StringLength(10)]
        [Column("staffNumber")]
        public string StaffNumber { get; set; }

        [StringLength(80)]
        [Column("personType")]
        public string PersonType { get; set; }

        [StringLength(60)]
        [Column("grade")]
        public string Grade { get; set; }

        [StringLength(240)]
        [Column("directorate")]
        public string Directorate { get; set; }

        [StringLength(60)]
        [Column("jobTitle")]
        public string JobTitle { get; set; }

        [StringLength(60)]
        [Column("workExtension")]
        public string WorkExtension { get; set; }

        [StringLength(60)]
        [Column("workExternal")]
        public string workExternal { get; set; }

        [StringLength(12)]
        [Column("costCentre")]
        public string CostCentre { get; set; }

        [StringLength(255)]
        [Column("costCentreName")]
        public string CostCentreName { get; set; }

        [StringLength(10)]
        [Column("fte")]
        public string FTE { get; set; }

        [StringLength(30)]
        [Column("supervisor")]
        public string Supervisor { get; set; }

        [Column("active")]
        public bool? Active { get; set; }

        [Column("allowUpdate")]
        public bool? AllowUpdate { get; set; }

        [StringLength(20)]
        [Column("organisation")]
        public string Organisation { get; set; }

        [StringLength(50)]
        [Column("updatedBy")]
        public string Updatedby { get; set; }

        [Column("updated")]
        public DateTime? Updated { get; set; }

        public virtual Location Location { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Manager> Managers { get; set; }
    }
}
