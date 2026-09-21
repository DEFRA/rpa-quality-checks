namespace RPA.QualityPortal.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class lockcheck : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LockCheck",
                c => new
                    {
                        LockCheckId = c.Int(nullable: false, identity: true),
                        CheckId = c.Int(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                        CheckLockedBy = c.String(),
                    })
                .PrimaryKey(t => t.LockCheckId);

            DropTable("dbo.LockReCheck");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.LockReCheck",
                c => new
                {
                    LockReCheckId = c.Int(nullable: false, identity: true),
                    CheckId = c.Int(nullable: false),
                    Timestamp = c.DateTime(nullable: false),
                    ReCheckLockedBy = c.String(),
                })
                .PrimaryKey(t => t.LockReCheckId);

            DropTable("dbo.LockCheck");
        }
    }
}
