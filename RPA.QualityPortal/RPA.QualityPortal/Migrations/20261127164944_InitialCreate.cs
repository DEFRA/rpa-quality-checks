using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RPA.QualityPortal.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AmendmentReason",
                columns: table => new
                {
                    AmendmentReasonId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmendmentReason", x => x.AmendmentReasonId);
                });

            migrationBuilder.CreateTable(
                name: "BACheckType",
                columns: table => new
                {
                    BACheckTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BACheckType", x => x.BACheckTypeId);
                });

            migrationBuilder.CreateTable(
                name: "BankAccountComments",
                columns: table => new
                {
                    BankAccountCommentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnswerComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnswerId = table.Column<int>(type: "int", nullable: false),
                    CheckId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccountComments", x => x.BankAccountCommentId);
                });

            migrationBuilder.CreateTable(
                name: "BusinessArea",
                columns: table => new
                {
                    BusinessAreaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessArea", x => x.BusinessAreaId);
                });

            migrationBuilder.CreateTable(
                name: "ChallengeOutcome",
                columns: table => new
                {
                    ChallengeOutcomeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChallengeOutcome", x => x.ChallengeOutcomeId);
                });

            migrationBuilder.CreateTable(
                name: "CheckType",
                columns: table => new
                {
                    CheckTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckType", x => x.CheckTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Control",
                columns: table => new
                {
                    ControlId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Property = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Control", x => x.ControlId);
                });

            migrationBuilder.CreateTable(
                name: "CorrespondenceType",
                columns: table => new
                {
                    CorrespondenceTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorrespondenceType", x => x.CorrespondenceTypeId);
                });

            migrationBuilder.CreateTable(
                name: "CRMRefPrefix",
                columns: table => new
                {
                    CRMRefPrefixId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRMRefPrefix", x => x.CRMRefPrefixId);
                });

            migrationBuilder.CreateTable(
                name: "LockCheck",
                columns: table => new
                {
                    LockCheckId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime", nullable: false),
                    CheckLockedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LockCheck", x => x.LockCheckId);
                });

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.QuestionId);
                });

            migrationBuilder.CreateTable(
                name: "Result",
                columns: table => new
                {
                    ResultId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Result", x => x.ResultId);
                });

            migrationBuilder.CreateTable(
                name: "Scheme",
                columns: table => new
                {
                    SchemeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scheme", x => x.SchemeId);
                });

            migrationBuilder.CreateTable(
                name: "UniqueIdentifierPrefix",
                columns: table => new
                {
                    UniqueIdentifierPrefixId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniqueIdentifierPrefix", x => x.UniqueIdentifierPrefixId);
                });

            migrationBuilder.CreateTable(
                name: "WorkerServiceLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastChange = table.Column<DateTime>(type: "datetime", nullable: false),
                    Changes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerServiceLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Check",
                columns: table => new
                {
                    CheckId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QCCompletedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateQCCompleted = table.Column<DateTime>(type: "datetime", nullable: true),
                    Archived = table.Column<bool>(type: "bit", nullable: false),
                    CheckTypeId = table.Column<int>(type: "int", nullable: false),
                    EmailNotifications = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Check", x => x.CheckId);
                    table.ForeignKey(
                        name: "FK_Check_CheckType_CheckTypeId",
                        column: x => x.CheckTypeId,
                        principalTable: "CheckType",
                        principalColumn: "CheckTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FailReason",
                columns: table => new
                {
                    FailReasonId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CheckTypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FailReason", x => x.FailReasonId);
                    table.ForeignKey(
                        name: "FK_FailReason_CheckType_CheckTypeId",
                        column: x => x.CheckTypeId,
                        principalTable: "CheckType",
                        principalColumn: "CheckTypeId");
                });

            migrationBuilder.CreateTable(
                name: "Answer",
                columns: table => new
                {
                    AnswerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    CheckId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answer", x => x.AnswerId);
                    table.ForeignKey(
                        name: "FK_Answer_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CheckQuestion",
                columns: table => new
                {
                    CheckTypeId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckQuestion", x => new { x.CheckTypeId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_CheckQuestion_CheckType_CheckTypeId",
                        column: x => x.CheckTypeId,
                        principalTable: "CheckType",
                        principalColumn: "CheckTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CheckQuestion_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Audit",
                columns: table => new
                {
                    AuditId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckId = table.Column<int>(type: "int", nullable: false),
                    DateQCUpdated = table.Column<DateTime>(type: "datetime", nullable: true),
                    AmendmentReason_AmendmentReasonId = table.Column<int>(type: "int", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audit", x => x.AuditId);
                    table.ForeignKey(
                        name: "FK_Audit_AmendmentReason_AmendmentReason_AmendmentReasonId",
                        column: x => x.AmendmentReason_AmendmentReasonId,
                        principalTable: "AmendmentReason",
                        principalColumn: "AmendmentReasonId");
                    table.ForeignKey(
                        name: "FK_Audit_Check_CheckId",
                        column: x => x.CheckId,
                        principalTable: "Check",
                        principalColumn: "CheckId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Challenge",
                columns: table => new
                {
                    ChallengeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckId = table.Column<int>(type: "int", nullable: false),
                    ChallengeOutcomeId = table.Column<int>(type: "int", nullable: false),
                    ChallengeDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Challenge", x => x.ChallengeId);
                    table.ForeignKey(
                        name: "FK_Challenge_ChallengeOutcome_ChallengeOutcomeId",
                        column: x => x.ChallengeOutcomeId,
                        principalTable: "ChallengeOutcome",
                        principalColumn: "ChallengeOutcomeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Challenge_Check_CheckId",
                        column: x => x.CheckId,
                        principalTable: "Check",
                        principalColumn: "CheckId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CheckAmendmentReason",
                columns: table => new
                {
                    CheckAmendmentReasonId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckId = table.Column<int>(type: "int", nullable: false),
                    AmendmentReasonId = table.Column<int>(type: "int", nullable: false),
                    AmendmentReasonComments = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckAmendmentReason", x => x.CheckAmendmentReasonId);
                    table.ForeignKey(
                        name: "FK_CheckAmendmentReason_AmendmentReason_AmendmentReasonId",
                        column: x => x.AmendmentReasonId,
                        principalTable: "AmendmentReason",
                        principalColumn: "AmendmentReasonId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CheckAmendmentReason_Check_CheckId",
                        column: x => x.CheckId,
                        principalTable: "Check",
                        principalColumn: "CheckId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CheckResult",
                columns: table => new
                {
                    CheckResultId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckId = table.Column<int>(type: "int", nullable: false),
                    ResultId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckResult", x => x.CheckResultId);
                    table.ForeignKey(
                        name: "FK_CheckResult_Check_CheckId",
                        column: x => x.CheckId,
                        principalTable: "Check",
                        principalColumn: "CheckId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CheckResult_Result_ResultId",
                        column: x => x.ResultId,
                        principalTable: "Result",
                        principalColumn: "ResultId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutboundCorrespondenceReCheck",
                columns: table => new
                {
                    CheckId = table.Column<int>(type: "int", nullable: false),
                    OutboundCorrespondenceId = table.Column<int>(type: "int", nullable: false),
                    ReCheckActive = table.Column<bool>(type: "bit", nullable: false),
                    ReCheckComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReCheckCompletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FurtherReCheckRequired = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboundCorrespondenceReCheck", x => x.CheckId);
                    table.ForeignKey(
                        name: "FK_OutboundCorrespondenceReCheck_Check_CheckId",
                        column: x => x.CheckId,
                        principalTable: "Check",
                        principalColumn: "CheckId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BankAccount",
                columns: table => new
                {
                    CheckId = table.Column<int>(type: "int", nullable: false),
                    SBI = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FRN = table.Column<int>(type: "int", nullable: false),
                    BACheckTypeId = table.Column<int>(type: "int", nullable: true),
                    BusinessName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ManagerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateQCCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoachingPoint = table.Column<bool>(type: "bit", nullable: false),
                    FailReasonId = table.Column<int>(type: "int", nullable: true),
                    ReCheckRequired = table.Column<bool>(type: "bit", nullable: false),
                    InitialReCheckDecision = table.Column<bool>(type: "bit", nullable: false),
                    Overriden = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccount", x => x.CheckId);
                    table.ForeignKey(
                        name: "FK_BankAccount_BACheckType_BACheckTypeId",
                        column: x => x.BACheckTypeId,
                        principalTable: "BACheckType",
                        principalColumn: "BACheckTypeId");
                    table.ForeignKey(
                        name: "FK_BankAccount_Check_CheckId",
                        column: x => x.CheckId,
                        principalTable: "Check",
                        principalColumn: "CheckId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BankAccount_FailReason_FailReasonId",
                        column: x => x.FailReasonId,
                        principalTable: "FailReason",
                        principalColumn: "FailReasonId");
                });

            migrationBuilder.CreateTable(
                name: "BankAccountReCheck",
                columns: table => new
                {
                    CheckId = table.Column<int>(type: "int", nullable: false),
                    BankAccountId = table.Column<int>(type: "int", nullable: false),
                    ReCheckActive = table.Column<bool>(type: "bit", nullable: false),
                    ReCheckComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReCheckCompletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FurtherReCheckRequired = table.Column<bool>(type: "bit", nullable: false),
                    ReCheckCoachingPoint = table.Column<bool>(type: "bit", nullable: false),
                    FailReasonId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccountReCheck", x => x.CheckId);
                    table.ForeignKey(
                        name: "FK_BankAccountReCheck_Check_CheckId",
                        column: x => x.CheckId,
                        principalTable: "Check",
                        principalColumn: "CheckId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BankAccountReCheck_FailReason_FailReasonId",
                        column: x => x.FailReasonId,
                        principalTable: "FailReason",
                        principalColumn: "FailReasonId");
                });

            migrationBuilder.CreateTable(
                name: "OutboundCorrespondence",
                columns: table => new
                {
                    CheckId = table.Column<int>(type: "int", nullable: false),
                    TemplateReference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UniqueId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UniqueIdentifierPrefixId = table.Column<int>(type: "int", nullable: false),
                    SBI = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CRMRef = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CRMRefPrefixId = table.Column<int>(type: "int", nullable: false),
                    OutboundCorrespondenceDateSent = table.Column<DateTime>(type: "datetime", nullable: true),
                    ManagerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HEO = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SEO = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorrespondenceTypeId = table.Column<int>(type: "int", nullable: false),
                    SchemeId = table.Column<int>(type: "int", nullable: false),
                    BusinessAreaId = table.Column<int>(type: "int", nullable: false),
                    FailReasonId = table.Column<int>(type: "int", nullable: true),
                    ReCheckRequired = table.Column<bool>(type: "bit", nullable: false),
                    InitialReCheckDecision = table.Column<bool>(type: "bit", nullable: false),
                    ExcludeQCResult = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboundCorrespondence", x => x.CheckId);
                    table.ForeignKey(
                        name: "FK_OutboundCorrespondence_BusinessArea_BusinessAreaId",
                        column: x => x.BusinessAreaId,
                        principalTable: "BusinessArea",
                        principalColumn: "BusinessAreaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutboundCorrespondence_CRMRefPrefix_CRMRefPrefixId",
                        column: x => x.CRMRefPrefixId,
                        principalTable: "CRMRefPrefix",
                        principalColumn: "CRMRefPrefixId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutboundCorrespondence_Check_CheckId",
                        column: x => x.CheckId,
                        principalTable: "Check",
                        principalColumn: "CheckId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutboundCorrespondence_CorrespondenceType_CorrespondenceTypeId",
                        column: x => x.CorrespondenceTypeId,
                        principalTable: "CorrespondenceType",
                        principalColumn: "CorrespondenceTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutboundCorrespondence_FailReason_FailReasonId",
                        column: x => x.FailReasonId,
                        principalTable: "FailReason",
                        principalColumn: "FailReasonId");
                    table.ForeignKey(
                        name: "FK_OutboundCorrespondence_Scheme_SchemeId",
                        column: x => x.SchemeId,
                        principalTable: "Scheme",
                        principalColumn: "SchemeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutboundCorrespondence_UniqueIdentifierPrefix_UniqueIdentifierPrefixId",
                        column: x => x.UniqueIdentifierPrefixId,
                        principalTable: "UniqueIdentifierPrefix",
                        principalColumn: "UniqueIdentifierPrefixId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answer_QuestionId",
                table: "Answer",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_AmendmentReason_AmendmentReasonId",
                table: "Audit",
                column: "AmendmentReason_AmendmentReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_CheckId",
                table: "Audit",
                column: "CheckId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccount_BACheckTypeId",
                table: "BankAccount",
                column: "BACheckTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccount_FailReasonId",
                table: "BankAccount",
                column: "FailReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccountReCheck_FailReasonId",
                table: "BankAccountReCheck",
                column: "FailReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Challenge_ChallengeOutcomeId",
                table: "Challenge",
                column: "ChallengeOutcomeId");

            migrationBuilder.CreateIndex(
                name: "IX_Challenge_CheckId",
                table: "Challenge",
                column: "CheckId");

            migrationBuilder.CreateIndex(
                name: "IX_Check_CheckTypeId",
                table: "Check",
                column: "CheckTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckAmendmentReason_AmendmentReasonId",
                table: "CheckAmendmentReason",
                column: "AmendmentReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckAmendmentReason_CheckId",
                table: "CheckAmendmentReason",
                column: "CheckId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckQuestion_QuestionId",
                table: "CheckQuestion",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckResult_CheckId",
                table: "CheckResult",
                column: "CheckId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckResult_ResultId",
                table: "CheckResult",
                column: "ResultId");

            migrationBuilder.CreateIndex(
                name: "IX_FailReason_CheckTypeId",
                table: "FailReason",
                column: "CheckTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundCorrespondence_BusinessAreaId",
                table: "OutboundCorrespondence",
                column: "BusinessAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundCorrespondence_CorrespondenceTypeId",
                table: "OutboundCorrespondence",
                column: "CorrespondenceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundCorrespondence_CRMRefPrefixId",
                table: "OutboundCorrespondence",
                column: "CRMRefPrefixId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundCorrespondence_FailReasonId",
                table: "OutboundCorrespondence",
                column: "FailReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundCorrespondence_SchemeId",
                table: "OutboundCorrespondence",
                column: "SchemeId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundCorrespondence_UniqueIdentifierPrefixId",
                table: "OutboundCorrespondence",
                column: "UniqueIdentifierPrefixId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Answer");

            migrationBuilder.DropTable(
                name: "Audit");

            migrationBuilder.DropTable(
                name: "BankAccount");

            migrationBuilder.DropTable(
                name: "BankAccountComments");

            migrationBuilder.DropTable(
                name: "BankAccountReCheck");

            migrationBuilder.DropTable(
                name: "Challenge");

            migrationBuilder.DropTable(
                name: "CheckAmendmentReason");

            migrationBuilder.DropTable(
                name: "CheckQuestion");

            migrationBuilder.DropTable(
                name: "CheckResult");

            migrationBuilder.DropTable(
                name: "Control");

            migrationBuilder.DropTable(
                name: "LockCheck");

            migrationBuilder.DropTable(
                name: "OutboundCorrespondence");

            migrationBuilder.DropTable(
                name: "OutboundCorrespondenceReCheck");

            migrationBuilder.DropTable(
                name: "WorkerServiceLog");

            migrationBuilder.DropTable(
                name: "BACheckType");

            migrationBuilder.DropTable(
                name: "ChallengeOutcome");

            migrationBuilder.DropTable(
                name: "AmendmentReason");

            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropTable(
                name: "Result");

            migrationBuilder.DropTable(
                name: "BusinessArea");

            migrationBuilder.DropTable(
                name: "CRMRefPrefix");

            migrationBuilder.DropTable(
                name: "CorrespondenceType");

            migrationBuilder.DropTable(
                name: "FailReason");

            migrationBuilder.DropTable(
                name: "Scheme");

            migrationBuilder.DropTable(
                name: "UniqueIdentifierPrefix");

            migrationBuilder.DropTable(
                name: "Check");

            migrationBuilder.DropTable(
                name: "CheckType");
        }
    }
}
