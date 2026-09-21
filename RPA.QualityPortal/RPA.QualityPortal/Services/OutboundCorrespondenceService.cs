using RPA.QualityPortal.DAL;
using RPA.QualityPortal.Helpers;
using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RPA.QualityPortal.Services
{
    public class OutboundCorrespondenceService : IOutboundCorrespondenceService
    {
        IQualityContext db;
        IAnswerService answerService;
        IUserHelper userHelper;
        ILockService lockService;

        public OutboundCorrespondenceService(IQualityContext context, IAnswerService answerService,  IUserHelper userHelper, ILockService lockService)
        {
            this.db = context;
            this.answerService = answerService;
            this.userHelper = userHelper;
            this.lockService = lockService;
        }

        public QuestionAnswerList QCResultCalculate(QuestionAnswerList qaList)
        {

            List<QuestionAnswer> QANo = qaList.QuestionAnswers.Where(x => x.Answer == "No").ToList();

            bool fail = false;

            bool passAd = false;

            if (QANo.Count > 0)
            {
                foreach (var qa in QANo)
                {
                    if (qa.CheckQuestion.Order == 2 || qa.CheckQuestion.Order == 4 || qa.CheckQuestion.Order == 5 || qa.CheckQuestion.Order == 6 || qa.CheckQuestion.Order == 7 || qa.CheckQuestion.Order == 8 || qa.CheckQuestion.Order == 9 || qa.CheckQuestion.Order == 10 || qa.CheckQuestion.Order == 13)
                    {
                        fail = true;
                    }
                    else if (qa.CheckQuestion.Order == 1 || qa.CheckQuestion.Order == 3 || qa.CheckQuestion.Order == 11 || qa.CheckQuestion.Order == 12)
                    {
                        passAd = true;
                    }
                }

                if (fail)
                {
                    qaList.QCResult = "Not Approved";
                }
                else
                {
                    if (passAd)
                    {
                        qaList.QCResult = "Approved Advisory";
                    }
                }

            }
            else
            {
                qaList.QCResult = "Approved";
            }

            return qaList;
        }

        public void SaveOutboundCorrespondence(OutboundCorrespondence outboundCorrespondence, QuestionAnswerList qaList)
        {
            //save the check
            outboundCorrespondence.CRMRef = outboundCorrespondence.CRMRef.ToUpper();
            outboundCorrespondence.UniqueId = outboundCorrespondence.UniqueId.ToUpper();

            //Set no recheck if pass
            if (qaList.QCResult != "Not Approved")
            {
                outboundCorrespondence.ReCheckRequired = false;
            }

            db.Checks.Add(outboundCorrespondence);
            db.SaveChanges();

            //save the answers

            answerService.AddAnswers(qaList, outboundCorrespondence.CheckId);

            //calculate the QC Result

            CheckResult checkResult = new CheckResult
            {
                CheckId = outboundCorrespondence.CheckId,
                ResultId = db.Results.Where(x => x.Text == qaList.QCResult).Select(c => c.ResultId).FirstOrDefault(),
                Result = db.Results.Where(x => x.Text == qaList.QCResult).FirstOrDefault(),
            };

            db.CheckResults.Add(checkResult);

            db.SaveChanges();
        }

        public void SaveOutboundCorrespondenceReCheck(OutboundCorrespondenceReCheck outboundCorrespondenceReCheck, QuestionAnswerList qaList)
        {
            //set no further recheck required and no recheck required to check if pass
            OutboundCorrespondence outboundCorrespondence = db.OutboundCorrespondence.Where(x => x.CheckId == outboundCorrespondenceReCheck.OutboundCorrespondenceId).FirstOrDefault();
            if (qaList.QCResult != "Not Approved")
            {
                outboundCorrespondenceReCheck.FurtherReCheckRequired = false;
                outboundCorrespondence.ReCheckRequired = false;
            }

            if (outboundCorrespondenceReCheck.FurtherReCheckRequired == false)
            {
                outboundCorrespondence.ReCheckRequired = false;
            }

            //save the check
            outboundCorrespondenceReCheck.DateQCCompleted = DateTime.Now;
            outboundCorrespondenceReCheck.ReCheckActive = true;

            db.Checks.Add(outboundCorrespondenceReCheck);
            db.SetModified(outboundCorrespondence);

            db.SaveChanges();

            //delete lock
            lockService.DeleteLock(outboundCorrespondenceReCheck.OutboundCorrespondenceId);

            //make previous rechecks inactive
            List<OutboundCorrespondenceReCheck> outboundCorrespondenceReChecks = db.OutboundCorrespondenceReCheck.Where(x => x.OutboundCorrespondenceId == outboundCorrespondenceReCheck.OutboundCorrespondenceId).OrderBy(c => c.CheckId).ToList();

            if (outboundCorrespondenceReChecks.Count == 2)
            {
                outboundCorrespondenceReChecks[0].ReCheckActive = false;
            }
            else if (outboundCorrespondenceReChecks.Count == 3)
            {
                outboundCorrespondenceReChecks[0].ReCheckActive = false;
                outboundCorrespondenceReChecks[1].ReCheckActive = false;
            }

            //save the answers

            answerService.AddAnswers(qaList, outboundCorrespondenceReCheck.CheckId);

            //calculate the ReCheck Result

            CheckResult checkResult = new CheckResult
            {
                CheckId = outboundCorrespondenceReCheck.CheckId,
                ResultId = db.Results.Where(x => x.Text == qaList.QCResult).Select(c => c.ResultId).FirstOrDefault()
            };

            db.CheckResults.Add(checkResult);

            db.SaveChanges();
        }

        public void SaveEditOutboundCorrespondence(OutboundCorrespondence outboundCorrespondence, QuestionAnswerList qaList, string previousResult)
        {
            //save changes to check
            outboundCorrespondence.CRMRef = outboundCorrespondence.CRMRef.ToUpper();
            outboundCorrespondence.UniqueId = outboundCorrespondence.UniqueId.ToUpper();

            //set no recheck if checck was pass
            if (qaList.QCResult != "Not Approved")
            {
                outboundCorrespondence.ReCheckRequired = false;
            }

            db.SetModified(outboundCorrespondence);

            //save the answers

            answerService.EditAnswers(qaList, outboundCorrespondence.CheckId);

            //calculate the QC Result

            CheckResult checkResult = db.CheckResults.Where(x => x.CheckId == outboundCorrespondence.CheckId).FirstOrDefault();
            {
                checkResult.ResultId = db.Results.Where(x => x.Text == qaList.QCResult).Select(c => c.ResultId).FirstOrDefault();
                checkResult.Result = db.Results.Where(x => x.Text == qaList.QCResult).FirstOrDefault();
            }

            db.SetModified(checkResult);

            //save any challenges

            if (qaList.Challenge != null && qaList.Challenge != false)
            {
                Challenge challenge = new Challenge
                {
                    ChallengeOutcomeId = db.ChallengeOutcomes.Where(x => x.Text == "Original Result Overturned").Select(p => p.ChallengeOutcomeId).FirstOrDefault(),
                    CheckId = outboundCorrespondence.CheckId
                };

                db.Challenge.Add(challenge);
            }

            db.SaveChanges();
        }

        public void SaveEditOutboundCorrespondenceReCheck(OutboundCorrespondenceReCheck outboundCorrespondenceReCheck, QuestionAnswerList qaList)
        {
            //save changes to check
            outboundCorrespondenceReCheck.ReCheckActive = true;

            //set no recheck if check a pass
            if (qaList.QCResult != "Not Approved")
            {
                outboundCorrespondenceReCheck.FurtherReCheckRequired = false;
            }

            db.SetModified(outboundCorrespondenceReCheck);

            //save the answers

            answerService.EditAnswers(qaList, outboundCorrespondenceReCheck.CheckId);

            //calculate the QC Result

            CheckResult checkResult = db.CheckResults.Where(x => x.CheckId == outboundCorrespondenceReCheck.CheckId).FirstOrDefault();
            {
                checkResult.ResultId = db.Results.Where(x => x.Text == qaList.QCResult).Select(c => c.ResultId).FirstOrDefault();
            }

            db.SetModified(checkResult);

            //set recheck required on check depending on further recheck is required
            OutboundCorrespondence outboundCorrespondence = db.OutboundCorrespondence.Where(x => x.CheckId == outboundCorrespondenceReCheck.OutboundCorrespondenceId).FirstOrDefault();
            if (outboundCorrespondenceReCheck.FurtherReCheckRequired == true)
            {
                outboundCorrespondence.ReCheckRequired = true;
            }
            else
            {
                outboundCorrespondence.ReCheckRequired = false;
            }

            db.SetModified(outboundCorrespondence);

            db.SaveChanges();
        }

        public bool CheckDuplicates(string UniqueId, int UniqueIdentifierPrefixId, string CrmRef, int CRMRefPrefixId, bool DuplicatedRecord = false)
        {
            List<OutboundCorrespondence> outboundCorrespondenceList = db.OutboundCorrespondence.ToList();

            var duplicateRecord = outboundCorrespondenceList.Where(x => x.UniqueId == UniqueId &&
                                                                        x.UniqueIdentifierPrefixId == UniqueIdentifierPrefixId &&
                                                                        x.CRMRef == CrmRef &&
                                                                        x.CRMRefPrefixId == CRMRefPrefixId).FirstOrDefault();

            if (duplicateRecord != null)
            {
                DuplicatedRecord = true;
            }

            return DuplicatedRecord;
        }

        public bool CheckEditDuplicates(string UniqueId, int UniqueIdentifierPrefixId, string CrmRef, int CRMRefPrefixId, int CheckId, bool DuplicatedRecord = false)
        {
            List<OutboundCorrespondence> outboundCorrespondenceList = db.OutboundCorrespondence.ToList();

            var duplicateRecord = outboundCorrespondenceList.Where(x => x.UniqueId == UniqueId &&
                                                                        x.UniqueIdentifierPrefixId == UniqueIdentifierPrefixId &&
                                                                        x.CRMRef == CrmRef &&
                                                                        x.CRMRefPrefixId == CRMRefPrefixId
                                                                        && x.CheckId != CheckId).FirstOrDefault();

            if (duplicateRecord != null)
            {
                DuplicatedRecord = true;
            }

            return DuplicatedRecord;
        }

        public IQueryable<OutboundCorrespondenceOverview> RetrieveOutboundCorrespondenceChecks(IQueryable<OutboundCorrespondence> outboundCorrespondenceChecks)
        {
            IQueryable<OutboundCorrespondenceOverview> outboundCorrespondenceOverviews = outboundCorrespondenceChecks
                .GroupJoin(db.OutboundCorrespondenceReCheck, check => check.CheckId, recheck => recheck.OutboundCorrespondenceId, (check, recheck) => new { Check = check, Recheck = recheck.DefaultIfEmpty().FirstOrDefault() })
                .Select(oc => new OutboundCorrespondenceOverview
                {
                    OutboundCorrespondence = oc.Check,
                    QCResult = oc.Check != null ? db.CheckResults.Where(x => x.CheckId == oc.Check.CheckId).Select(p => p.Result.Text).FirstOrDefault() : null,
                    OutboundCorrespondenceReCheck = oc.Recheck,
                    ReCheckQCResult = oc.Recheck != null ? db.CheckResults.Where(x => x.CheckId == oc.Recheck.CheckId).Select(p => p.Result.Text).FirstOrDefault() : null
                });

            return outboundCorrespondenceOverviews.OrderByDescending(x => x.OutboundCorrespondence.DateQCCompleted);
        }

        public IQueryable<OutboundCorrespondenceOverview> RetrieveOutboundCorrespondenceRechecks(IQueryable<OutboundCorrespondenceReCheck> outboundCorrespondenceRechecks)
        {
            IQueryable<OutboundCorrespondenceOverview> outboundCorrespondenceOverviews = outboundCorrespondenceRechecks
                .Join(db.OutboundCorrespondence, recheck => recheck.OutboundCorrespondenceId, check => check.CheckId, (recheck, check) => new { Recheck = recheck, Check = check })
                .Select(oc => new OutboundCorrespondenceOverview
                {
                    OutboundCorrespondence = oc.Check,
                    QCResult = oc.Check != null ? db.CheckResults.Where(x => x.CheckId == oc.Check.CheckId).Select(p => p.Result.Text).FirstOrDefault() : null,
                    OutboundCorrespondenceReCheck = oc.Recheck,
                    ReCheckQCResult = oc.Recheck != null ? db.CheckResults.Where(x => x.CheckId == oc.Recheck.CheckId).Select(p => p.Result.Text).FirstOrDefault() : null
                });

            return outboundCorrespondenceOverviews.OrderByDescending(x => x.OutboundCorrespondence.DateQCCompleted);
        }

        public void DeleteCheck(int CheckId)
        {
            OutboundCorrespondence outboundCorrespondence = db.OutboundCorrespondence.Where(x => x.CheckId == CheckId).FirstOrDefault();
            List<OutboundCorrespondenceReCheck> outboundCorrespondenceReChecks = db.OutboundCorrespondenceReCheck.Where(x => x.OutboundCorrespondenceId == CheckId).ToList();

            db.Answers.RemoveRange(db.Answers.Where(x => x.CheckId == outboundCorrespondence.CheckId));

            if (db.Audits.Any(x => x.CheckId == CheckId))
            {
                db.Audits.RemoveRange(db.Audits.Where(x => x.CheckId == outboundCorrespondence.CheckId));
            }
            if (db.Challenge.Any(x => x.CheckId == CheckId))
            {
                db.Challenge.RemoveRange(db.Challenge.Where(x => x.CheckId == outboundCorrespondence.CheckId));
            }
            if (db.CheckAmendmentReason.Any(x => x.CheckId == CheckId))
            {
                db.CheckAmendmentReason.Remove(db.CheckAmendmentReason.Where(x => x.CheckId == CheckId).FirstOrDefault());
            }

            db.CheckResults.Remove(db.CheckResults.Where(x => x.CheckId == outboundCorrespondence.CheckId).FirstOrDefault());

            if (outboundCorrespondenceReChecks.Count > 0)
            {
                foreach (var outboundCorrespondenceReCheck in outboundCorrespondenceReChecks)
                {
                    DeleteReCheck(CheckId);
                }
            }

            db.OutboundCorrespondence.Remove(outboundCorrespondence);
            db.Checks.Remove(db.Checks.Where(x => x.CheckId == CheckId).FirstOrDefault());

            db.SaveChanges();
        }

        public void DeleteReCheck(int CheckId)
        {
            OutboundCorrespondenceReCheck reCheck = db.OutboundCorrespondenceReCheck.Where(x => x.OutboundCorrespondenceId == CheckId).OrderByDescending(c => c.DateQCCompleted).FirstOrDefault();

            db.Answers.RemoveRange(db.Answers.Where(x => x.CheckId == reCheck.CheckId));
            if (db.Audits.Any(x => x.CheckId == reCheck.CheckId))
            {
                db.Audits.RemoveRange(db.Audits.Where(x => x.CheckId == reCheck.CheckId));
            }
            if (db.CheckAmendmentReason.Any(x => x.CheckId == reCheck.CheckId))
            {
                db.CheckAmendmentReason.Remove(db.CheckAmendmentReason.Where(x => x.CheckId == reCheck.CheckId).FirstOrDefault());
            }
            db.CheckResults.Remove(db.CheckResults.Where(x => x.CheckId == reCheck.CheckId).FirstOrDefault());
            db.OutboundCorrespondenceReCheck.Remove(reCheck);
            db.Checks.Remove(db.Checks.Where(x => x.CheckId == reCheck.CheckId).FirstOrDefault());

            List<OutboundCorrespondenceReCheck> outboundCorrespondenceReChecks = db.OutboundCorrespondenceReCheck.Where(x => x.OutboundCorrespondenceId == CheckId).OrderBy(c => c.CheckId).ToList();
            OutboundCorrespondence outboundCorrespondence = db.OutboundCorrespondence.Where(x => x.CheckId == CheckId).FirstOrDefault();

            outboundCorrespondence.ReCheckRequired = true;

            if (outboundCorrespondenceReChecks.Count == 2)
            {
                outboundCorrespondenceReChecks[0].ReCheckActive = true;
            }
            else if (outboundCorrespondenceReChecks.Count == 3)
            {
                outboundCorrespondenceReChecks[1].ReCheckActive = true;
            }

            db.SaveChanges();
        }


    }
}