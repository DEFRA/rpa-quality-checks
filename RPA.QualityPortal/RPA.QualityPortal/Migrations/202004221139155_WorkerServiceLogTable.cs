namespace RPA.QualityPortal.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class WorkerServiceLogTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WorkerServiceLog",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LastChange = c.DateTime(nullable: false),
                        Changes = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WorkerServiceLog");
        }
    }
}
