namespace RPA.QualityPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBankCheckOverride : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BankAccount", "Overriden", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BankAccount", "Overriden");
        }
    }
}
