namespace RPA.QualityPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class changeDropDownLinks : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OutboundCorrespondence", "BusinessArea_BusinessAreaId", "dbo.BusinessArea");
            DropForeignKey("dbo.OutboundCorrespondence", "CorrespondenceType_CorrespondenceTypeId", "dbo.CorrespondenceType");
            DropForeignKey("dbo.OutboundCorrespondence", "CRMRefPrefix_CRMRefPrefixId", "dbo.CRMRefPrefix");
            DropForeignKey("dbo.OutboundCorrespondence", "FailReason_FailReasonID", "dbo.FailReason");
            DropForeignKey("dbo.OutboundCorrespondence", "Scheme_SchemeId", "dbo.Scheme");
            DropForeignKey("dbo.OutboundCorrespondence", "UniqueIdentifierPrefix_UniqueIdentifierPrefixId", "dbo.UniqueIdentifierPrefix");
            DropIndex("dbo.OutboundCorrespondence", new[] { "BusinessArea_BusinessAreaId" });
            DropIndex("dbo.OutboundCorrespondence", new[] { "CorrespondenceType_CorrespondenceTypeId" });
            DropIndex("dbo.OutboundCorrespondence", new[] { "CRMRefPrefix_CRMRefPrefixId" });
            DropIndex("dbo.OutboundCorrespondence", new[] { "FailReason_FailReasonID" });
            DropIndex("dbo.OutboundCorrespondence", new[] { "Scheme_SchemeId" });
            DropIndex("dbo.OutboundCorrespondence", new[] { "UniqueIdentifierPrefix_UniqueIdentifierPrefixId" });
            RenameColumn(table: "dbo.OutboundCorrespondence", name: "BusinessArea_BusinessAreaId", newName: "BusinessAreaId");
            RenameColumn(table: "dbo.OutboundCorrespondence", name: "CorrespondenceType_CorrespondenceTypeId", newName: "CorrespondenceTypeId");
            RenameColumn(table: "dbo.OutboundCorrespondence", name: "CRMRefPrefix_CRMRefPrefixId", newName: "CRMRefPrefixId");
            RenameColumn(table: "dbo.OutboundCorrespondence", name: "FailReason_FailReasonID", newName: "FailReasonId");
            RenameColumn(table: "dbo.OutboundCorrespondence", name: "Scheme_SchemeId", newName: "SchemeId");
            RenameColumn(table: "dbo.OutboundCorrespondence", name: "UniqueIdentifierPrefix_UniqueIdentifierPrefixId", newName: "UniqueIdentifierPrefixId");
            AlterColumn("dbo.OutboundCorrespondence", "BusinessAreaId", c => c.Int(nullable: false));
            AlterColumn("dbo.OutboundCorrespondence", "CorrespondenceTypeId", c => c.Int(nullable: false));
            AlterColumn("dbo.OutboundCorrespondence", "CRMRefPrefixId", c => c.Int(nullable: false));
            AlterColumn("dbo.OutboundCorrespondence", "FailReasonId", c => c.Int(nullable: false));
            AlterColumn("dbo.OutboundCorrespondence", "SchemeId", c => c.Int(nullable: false));
            AlterColumn("dbo.OutboundCorrespondence", "UniqueIdentifierPrefixId", c => c.Int(nullable: false));
            CreateIndex("dbo.OutboundCorrespondence", "UniqueIdentifierPrefixId");
            CreateIndex("dbo.OutboundCorrespondence", "CRMRefPrefixId");
            CreateIndex("dbo.OutboundCorrespondence", "CorrespondenceTypeId");
            CreateIndex("dbo.OutboundCorrespondence", "SchemeId");
            CreateIndex("dbo.OutboundCorrespondence", "BusinessAreaId");
            CreateIndex("dbo.OutboundCorrespondence", "FailReasonId");
            AddForeignKey("dbo.OutboundCorrespondence", "BusinessAreaId", "dbo.BusinessArea", "BusinessAreaId", cascadeDelete: true);
            AddForeignKey("dbo.OutboundCorrespondence", "CorrespondenceTypeId", "dbo.CorrespondenceType", "CorrespondenceTypeId", cascadeDelete: true);
            AddForeignKey("dbo.OutboundCorrespondence", "CRMRefPrefixId", "dbo.CRMRefPrefix", "CRMRefPrefixId", cascadeDelete: true);
            AddForeignKey("dbo.OutboundCorrespondence", "FailReasonId", "dbo.FailReason", "FailReasonID", cascadeDelete: true);
            AddForeignKey("dbo.OutboundCorrespondence", "SchemeId", "dbo.Scheme", "SchemeId", cascadeDelete: true);
            AddForeignKey("dbo.OutboundCorrespondence", "UniqueIdentifierPrefixId", "dbo.UniqueIdentifierPrefix", "UniqueIdentifierPrefixId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OutboundCorrespondence", "UniqueIdentifierPrefixId", "dbo.UniqueIdentifierPrefix");
            DropForeignKey("dbo.OutboundCorrespondence", "SchemeId", "dbo.Scheme");
            DropForeignKey("dbo.OutboundCorrespondence", "FailReasonId", "dbo.FailReason");
            DropForeignKey("dbo.OutboundCorrespondence", "CRMRefPrefixId", "dbo.CRMRefPrefix");
            DropForeignKey("dbo.OutboundCorrespondence", "CorrespondenceTypeId", "dbo.CorrespondenceType");
            DropForeignKey("dbo.OutboundCorrespondence", "BusinessAreaId", "dbo.BusinessArea");
            DropIndex("dbo.OutboundCorrespondence", new[] { "FailReasonId" });
            DropIndex("dbo.OutboundCorrespondence", new[] { "BusinessAreaId" });
            DropIndex("dbo.OutboundCorrespondence", new[] { "SchemeId" });
            DropIndex("dbo.OutboundCorrespondence", new[] { "CorrespondenceTypeId" });
            DropIndex("dbo.OutboundCorrespondence", new[] { "CRMRefPrefixId" });
            DropIndex("dbo.OutboundCorrespondence", new[] { "UniqueIdentifierPrefixId" });
            AlterColumn("dbo.OutboundCorrespondence", "UniqueIdentifierPrefixId", c => c.Int());
            AlterColumn("dbo.OutboundCorrespondence", "SchemeId", c => c.Int());
            AlterColumn("dbo.OutboundCorrespondence", "FailReasonId", c => c.Int());
            AlterColumn("dbo.OutboundCorrespondence", "CRMRefPrefixId", c => c.Int());
            AlterColumn("dbo.OutboundCorrespondence", "CorrespondenceTypeId", c => c.Int());
            AlterColumn("dbo.OutboundCorrespondence", "BusinessAreaId", c => c.Int());
            RenameColumn(table: "dbo.OutboundCorrespondence", name: "UniqueIdentifierPrefixId", newName: "UniqueIdentifierPrefix_UniqueIdentifierPrefixId");
            RenameColumn(table: "dbo.OutboundCorrespondence", name: "SchemeId", newName: "Scheme_SchemeId");
            RenameColumn(table: "dbo.OutboundCorrespondence", name: "FailReasonId", newName: "FailReason_FailReasonID");
            RenameColumn(table: "dbo.OutboundCorrespondence", name: "CRMRefPrefixId", newName: "CRMRefPrefix_CRMRefPrefixId");
            RenameColumn(table: "dbo.OutboundCorrespondence", name: "CorrespondenceTypeId", newName: "CorrespondenceType_CorrespondenceTypeId");
            RenameColumn(table: "dbo.OutboundCorrespondence", name: "BusinessAreaId", newName: "BusinessArea_BusinessAreaId");
            CreateIndex("dbo.OutboundCorrespondence", "UniqueIdentifierPrefix_UniqueIdentifierPrefixId");
            CreateIndex("dbo.OutboundCorrespondence", "Scheme_SchemeId");
            CreateIndex("dbo.OutboundCorrespondence", "FailReason_FailReasonID");
            CreateIndex("dbo.OutboundCorrespondence", "CRMRefPrefix_CRMRefPrefixId");
            CreateIndex("dbo.OutboundCorrespondence", "CorrespondenceType_CorrespondenceTypeId");
            CreateIndex("dbo.OutboundCorrespondence", "BusinessArea_BusinessAreaId");
            AddForeignKey("dbo.OutboundCorrespondence", "UniqueIdentifierPrefix_UniqueIdentifierPrefixId", "dbo.UniqueIdentifierPrefix", "UniqueIdentifierPrefixId");
            AddForeignKey("dbo.OutboundCorrespondence", "Scheme_SchemeId", "dbo.Scheme", "SchemeId");
            AddForeignKey("dbo.OutboundCorrespondence", "FailReason_FailReasonID", "dbo.FailReason", "FailReasonID");
            AddForeignKey("dbo.OutboundCorrespondence", "CRMRefPrefix_CRMRefPrefixId", "dbo.CRMRefPrefix", "CRMRefPrefixId");
            AddForeignKey("dbo.OutboundCorrespondence", "CorrespondenceType_CorrespondenceTypeId", "dbo.CorrespondenceType", "CorrespondenceTypeId");
            AddForeignKey("dbo.OutboundCorrespondence", "BusinessArea_BusinessAreaId", "dbo.BusinessArea", "BusinessAreaId");
        }
    }
}
