using RPA.QualityPortal.DAL;
using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RPA.QualityPortal.Services
{
    public class BankAccountService : IBankAccountService
    {
        IQualityContext db;
        IAnswerService answerService;
        ILockService lockService;

        public BankAccountService(IQualityContext context, IAnswerService answerService, ILockService lockService)
        {
            db = context;
            this.answerService = answerService;
            this.lockService = lockService;
        }

        public void SaveBankAccount(BankAccountCheck bankAccount, QuestionAnswerList questionAnswerList)
        {
            bankAccount.Overriden = false;

            SaveBankAccountToContext(bankAccount, questionAnswerList);
        }

        public void SaveBankAccountOverride(BankAccountCheck bankAccount)
        {
            bankAccount.Comments = answerService.CheckComment(bankAccount.Comments);
            bankAccount.EmailNotifications = false;

            List<CheckQuestion> checkQuestions = db.CheckQuestions.Where(x => x.CheckType.Name == "Bank Account").OrderBy(c => c.Order).ToList();

            QuestionAnswerList questionAnswerList = new QuestionAnswerList();

            foreach (var checkQuestion in checkQuestions)
            {
                QuestionAnswer questionAnswer = new QuestionAnswer
                {
                    CheckQuestion = checkQuestion,
                    Answer = "N/A"
                };

                questionAnswerList.QuestionAnswers.Add(questionAnswer);
            }

            questionAnswerList.QCResult = "Not Required";

            SaveBankAccountToContext(bankAccount, questionAnswerList);
        }

        public void SaveBankAccountReCheck(BankAccountReCheck bankAccountReCheck, QuestionAnswerList qaList)
        {
            //set no further recheck required and no recheck required to check if pass
            BankAccountCheck bankAccount = db.BankAccounts.Where(x => x.CheckId == bankAccountReCheck.BankAccountId).FirstOrDefault();

            if (qaList.QCResult != "Not Approved")
            {
                bankAccountReCheck.FurtherReCheckRequired = false;
                bankAccount.ReCheckRequired = false;
            }

            if (bankAccountReCheck.FurtherReCheckRequired == false)
            {
                bankAccount.ReCheckRequired = false;
            }

            //save the check
            bankAccountReCheck.DateQCCompleted = DateTime.Now;
            bankAccountReCheck.ReCheckActive = true;

            if (bankAccountReCheck.ReCheckComments != null)
            {
                bankAccountReCheck.ReCheckComments = answerService.CheckComment(bankAccountReCheck.ReCheckComments);
            }

            db.Checks.Add(bankAccountReCheck);
            db.SetModified(bankAccount);

            db.SaveChanges();

            //delete lock
            lockService.DeleteLock(bankAccountReCheck.BankAccountId);

            //make previous rechecks inactive
            List<BankAccountReCheck> bankAccountReChecks = db.BankAccountReChecks.Where(x => x.BankAccountId == bankAccountReCheck.BankAccountId).OrderBy(c => c.CheckId).ToList();

            if (bankAccountReChecks.Count == 2)
            {
                bankAccountReChecks[0].ReCheckActive = false;
            }
            else if (bankAccountReChecks.Count == 3)
            {
                bankAccountReChecks[0].ReCheckActive = false;
                bankAccountReChecks[1].ReCheckActive = false;
            }

            //save the answers
            answerService.AddAnswers(qaList, bankAccountReCheck.CheckId);

            //save comments
            answerService.BankComments(qaList, bankAccountReCheck.CheckId);
            db.SaveChanges();

            //calculate the ReCheck Result

            CheckResult checkResult = new CheckResult
            {
                CheckId = bankAccountReCheck.CheckId,
                ResultId = db.Results.Where(x => x.Text == qaList.QCResult).Select(c => c.ResultId).FirstOrDefault()
            };

            db.CheckResults.Add(checkResult);

            db.SaveChanges();
        }

        public IQueryable<BankAccountOverview> RetrieveBankAccountChecks(IQueryable<BankAccountCheck> bankAccounts)
        {
            IQueryable<BankAccountOverview> bankAccountOverviews = bankAccounts
                .GroupJoin(db.BankAccountReChecks, check => check.CheckId, recheck => recheck.BankAccountId,(check, recheck) => new { Check = check, Recheck = recheck.DefaultIfEmpty().FirstOrDefault() })
                .Select(ba => new BankAccountOverview
                {
                    BankAccount = ba.Check,
                    QCResult = ba.Check != null ? db.CheckResults.Where(x => x.CheckId == ba.Check.CheckId).Select(p => p.Result.Text).FirstOrDefault() : null,
                    BankAccountReCheck = ba.Recheck,
                    ReCheckQCResult = ba.Recheck != null ? db.CheckResults.Where(x => x.CheckId == ba.Recheck.CheckId).Select(p => p.Result.Text).FirstOrDefault() : null
                });

            return bankAccountOverviews.OrderBy(x => x.BankAccount.DateQCCreated);
        }

        public IQueryable<BankAccountOverview> RetrieveBankAccountRechecks(IQueryable<BankAccountReCheck> bankAccountReChecks)
        {
            IQueryable<BankAccountOverview> bankAccountOverviews = bankAccountReChecks
                .Join(db.BankAccounts, recheck => recheck.BankAccountId, check => check.CheckId, (recheck, check) => new { Recheck = recheck, Check = check })
                .Select(ba => new BankAccountOverview
                {
                    BankAccount = ba.Check,
                    QCResult = ba.Check != null ? db.CheckResults.Where(x => x.CheckId == ba.Check.CheckId).Select(p => p.Result.Text).FirstOrDefault() : null,
                    BankAccountReCheck = ba.Recheck,
                    ReCheckQCResult = ba.Recheck != null ? db.CheckResults.Where(x => x.CheckId == ba.Recheck.CheckId).Select(p => p.Result.Text).FirstOrDefault() : null
                });

            return bankAccountOverviews.OrderBy(x => x.BankAccount.DateQCCreated);
        }

        public void SaveEditBankAccount(BankAccountCheck bankAccount, QuestionAnswerList qaList)
        {
            //set no recheck if checck was pass
            if (qaList.QCResult != "Not Approved")
            {
                bankAccount.ReCheckRequired = false;
            }

            if (bankAccount.Comments != null)
            {
                bankAccount.Comments = answerService.CheckComment(bankAccount.Comments);
            }

            db.SetModified(bankAccount);

            //save the answers

            answerService.EditAnswers(qaList, bankAccount.CheckId);

            // save comments

            answerService.EditBankComments(qaList, bankAccount.CheckId);

            //calculate the QC Result

            CheckResult checkResult = db.CheckResults.Where(x => x.CheckId == bankAccount.CheckId).FirstOrDefault();
            {
                checkResult.ResultId = db.Results.Where(x => x.Text == qaList.QCResult).Select(c => c.ResultId).FirstOrDefault();
                checkResult.Result = db.Results.Where(x => x.Text == qaList.QCResult).FirstOrDefault();
            }

            db.SetModified(checkResult);

            db.SaveChanges();
        }

        public void SaveEditBankAccountReCheck(BankAccountReCheck bankAccountReCheck, QuestionAnswerList qaList)
        {
            //save changes to check
            bankAccountReCheck.ReCheckActive = true;

            //set no recheck if check a pass
            if (qaList.QCResult != "Not Approved")
            {
                bankAccountReCheck.FurtherReCheckRequired = false;
            }

            if (bankAccountReCheck.ReCheckComments != null)
            {
                bankAccountReCheck.ReCheckComments = answerService.CheckComment(bankAccountReCheck.ReCheckComments);
            }

            db.SetModified(bankAccountReCheck);

            //save the answers

            answerService.EditAnswers(qaList, bankAccountReCheck.CheckId);

            // save comments

            answerService.EditBankComments(qaList, bankAccountReCheck.CheckId);

            //calculate the QC Result

            CheckResult checkResult = db.CheckResults.Where(x => x.CheckId == bankAccountReCheck.CheckId).FirstOrDefault();
            {
                checkResult.ResultId = db.Results.Where(x => x.Text == qaList.QCResult).Select(c => c.ResultId).FirstOrDefault();
            }

            db.SetModified(checkResult);

            //set recheck required on check depending on further recheck is required
            BankAccountCheck bankAccount = db.BankAccounts.Where(x => x.CheckId == bankAccountReCheck.BankAccountId).FirstOrDefault();
            if (bankAccountReCheck.FurtherReCheckRequired == true)
            {
                bankAccount.ReCheckRequired = true;
            }
            else
            {
                bankAccount.ReCheckRequired = false;
            }

            db.SetModified(bankAccount);

            db.SaveChanges();
        }

        private void SaveBankAccountToContext(BankAccountCheck bankAccount, QuestionAnswerList questionAnswerList)
        {
            //save the check
            if (bankAccount.Comments != null)
            {
                bankAccount.Comments = answerService.CheckComment(bankAccount.Comments);
            }

            db.SetModified(bankAccount);
            db.SaveChanges();

            //delete lock
            lockService.DeleteLock(bankAccount.CheckId);

            //save the answers
            answerService.AddAnswers(questionAnswerList, bankAccount.CheckId);
            db.SaveChanges();

            //save comments
            answerService.BankComments(questionAnswerList, bankAccount.CheckId);
            db.SaveChanges();

            //save the result
            CheckResult checkResult = new CheckResult
            {
                CheckId = bankAccount.CheckId,
                ResultId = db.Results.Where(x => x.Text == questionAnswerList.QCResult).Select(c => c.ResultId).FirstOrDefault(),
                Result = db.Results.Where(x => x.Text == questionAnswerList.QCResult).FirstOrDefault()
            };

            db.CheckResults.Add(checkResult);
            db.SaveChanges();
        }
    }
}