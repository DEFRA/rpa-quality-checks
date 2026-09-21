namespace RPA.QualityPortal.Migrations
{
    using System.Diagnostics.CodeAnalysis;
    using System.Data.Entity.Migrations;
    
    [ExcludeFromCodeCoverage]
    public partial class bankReCheck : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Check", "BankAccountId", c => c.Int());
            AddColumn("dbo.Check", "ReCheckActive", c => c.Boolean());
            AddColumn("dbo.Check", "ReCheckComments", c => c.String());
            AddColumn("dbo.Check", "ReCheckCompletedBy", c => c.String());
            AddColumn("dbo.Check", "FurtherReCheckRequired", c => c.Boolean());
            AddColumn("dbo.Check", "Discriminator", c => c.String(maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Check", "Discriminator");
            DropColumn("dbo.Check", "FurtherReCheckRequired");
            DropColumn("dbo.Check", "ReCheckCompletedBy");
            DropColumn("dbo.Check", "ReCheckComments");
            DropColumn("dbo.Check", "ReCheckActive");
            DropColumn("dbo.Check", "BankAccountId");
        }
    }
}
