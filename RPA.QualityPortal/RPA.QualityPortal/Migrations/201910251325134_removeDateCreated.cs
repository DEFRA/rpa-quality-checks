namespace RPA.QualityPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class removeDateCreated : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Check", "DateCreated");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Check", "DateCreated", c => c.DateTime());
        }
    }
}
