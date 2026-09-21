namespace RPA.QualityPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]

    public partial class LockReCheck : DbMigration
    {
        public override void Up()
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
            
        }
        
        public override void Down()
        {
            DropTable("dbo.LockReCheck");
        }
    }
}
