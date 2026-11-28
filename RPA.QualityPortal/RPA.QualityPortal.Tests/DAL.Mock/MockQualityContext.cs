using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RPA.QualityPortal.DAL;
using RPA.QualityPortal.Tests.Data.Mock;

namespace RPA.QualityPortal.Tests.DAL.Mock
{
    // SQLite-backed replacement for the former Moq-based context: a real QualityContext seeded from the Data.Mock lists.
    public class MockQualityContext : IDisposable
    {
        private readonly SqliteConnection connection;
        private readonly CountingQualityContext db;

        public ContextHandle<IQualityContext> MockContext { get; }

        public QualityContext Db => db;

        public MockQualityContext(bool setMocks = true)
        {
            connection = TestDb.OpenConnection();
            var options = new DbContextOptionsBuilder<QualityContext>().UseSqlite(connection).Options;
            db = new CountingQualityContext(options);
            db.Database.EnsureCreated();
            MockContext = new ContextHandle<IQualityContext>(db, () => db.SaveCount);

            if (setMocks)
            {
                SetMocks();
            }
        }

        // Seed an answer for every question of the check type so controllers that join questions to answers find a row each.
        public void EnsureAnswers(int checkId, int checkTypeId)
        {
            var questionIds = db.CheckQuestions.AsNoTracking().Where(x => x.CheckTypeId == checkTypeId).Select(x => x.QuestionId).Distinct().ToList();
            var answered = db.Answers.AsNoTracking().Where(x => x.CheckId == checkId).Select(x => x.QuestionId).ToList();
            foreach (var questionId in questionIds.Except(answered))
            {
                db.Answers.Add(new RPA.QualityPortal.Models.Answer { CheckId = checkId, QuestionId = questionId, Text = "Yes" });
            }

            var saves = db.SaveCount;
            db.SaveChanges();
            db.SaveCount = saves;
            db.ChangeTracker.Clear();
        }
        public void SetMocks()
        {
            TestDb.Seed(db, AmendmentReasonData.Data());
            TestDb.Seed(db, AnswerData.Data());
            TestDb.Seed(db, AuditData.Data());
            TestDb.Seed(db, BACheckTypeData.Data());
            TestDb.Seed(db, BankAccountCommentData.Data());
            TestDb.Seed(db, BankAccountData.Data());
            TestDb.Seed(db, BankAccountReCheckData.Data());
            TestDb.Seed(db, BusinessAreaData.Data());
            TestDb.Seed(db, ChallengeData.Data());
            TestDb.Seed(db, ChallengeOutcomeData.Data());
            TestDb.Seed(db, CheckAmendmentReasonData.Data());
            TestDb.Seed(db, CheckData.Data());
            TestDb.Seed(db, CheckQuestionData.Data());
            TestDb.Seed(db, CheckResultData.Data());
            TestDb.Seed(db, CheckTypeData.Data());
            TestDb.Seed(db, ControlData.Data());
            TestDb.Seed(db, CorrespondenceTypeData.Data());
            TestDb.Seed(db, CRMRefPrefixData.Data());
            TestDb.Seed(db, FailReasonData.Data());
            TestDb.Seed(db, LockCheckData.Data());
            TestDb.Seed(db, OutboundCorrespondenceData.Data());
            TestDb.Seed(db, OutboundCorrespondenceReCheckData.Data());
            TestDb.Seed(db, QuestionData.Data());
            TestDb.Seed(db, ResultData.Data());
            TestDb.Seed(db, SchemeData.Data());
            TestDb.Seed(db, UniqueIdentifierPrefixData.Data());

            db.SaveChanges();
            db.ChangeTracker.Clear();
            db.SaveCount = 0;
            db.EmulateIdentity = true;
        }

        public void Dispose()
        {
            db.Dispose();
            connection.Dispose();
        }
    }
}
