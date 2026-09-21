using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using RPA.QualityPortal.DAL;
using System.IO;
using OfficeOpenXml;

namespace RPA.QualityPortal.Services
{
    public class ExportService : IExportService
    {
        IQualityContext db;

        public ExportService(IQualityContext context)
        {
            this.db = context;
        }

        public string Build(IEnumerable<OutboundCorrespondence> outboundCorrespondences, IEnumerable<OutboundCorrespondenceReCheck> outboundCorrespondenceReChecks)
        {
            List<OutboundCorrespondence> outboundCorrespondencesList = outboundCorrespondences.OrderByDescending(z => z.DateQCCompleted).ToList();
            List<OutboundCorrespondenceReCheck> outboundCorrespondenceReChecksList = outboundCorrespondenceReChecks.ToList();
            List<CheckQuestion> Questions = db.CheckQuestions.AsNoTracking().ToList();
            List<Answer> answers = db.Answers.AsNoTracking().ToList();
            List<CheckResult> checkResults = db.CheckResults.AsNoTracking().ToList();
            List<CheckAmendmentReason> checkAmendmentReasons = db.CheckAmendmentReason.AsNoTracking().ToList();
            List<Challenge> challenges = db.Challenge.AsNoTracking().ToList();


            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\",\"{18}\",\"{19}\",\"{20}\",\"{21}\",\"{22}\",\"{23}\",\"{24}\",\"{25}\",\"{26}\",\"{27}\",\"{28}\",\"{29}\",\"{30}\",\"{31}\",\"{32}\",\"{33}\",\"{34}\",\"{35}\",\"{36}\",\"{37}\",\"{38}\",\"{39}\",\"{40}\",\"{41}\",\"{42}\",\"{43}\",\"{44}\",\"{45}\",\"{46}\",\"{47}\",\"{48}\",\"{49}\",\"{50}\",\"{51}\",\"{52}\",\"{53}\",\"{54}\",\"{55}\",\"{56}\",\"{57}\",\"{58}\",\"{59}\",\"{60}\",\"{61}\",\"{62}\",\"{63}\",\"{64}\",\"{65}\",\"{66}\",\"{67}\",\"{68}\",\"{69}\",\"{70}\",\"{71}\",\"{72}\",\"{73}\",\"{74}\",\"{75}\",\"{76}\",\"{77}\",\"{78}\",\"{79}\",\"{80}\",\"{81}\",\"{82}\",\"{83}\",\"{84}\",\"{85}\",\"{86}\",\"{87}\",\"{88}\",\"{89}\",\"{90}\",\"{91}\",\"{92}\",\"{93}\",\"{94}\",\"{95}\",\"{96}\",\"{97}\"",
                //GENERAL
                "Scheme",
                "Business Area",
                "Correspondence Type",
                "Template Reference",
                "Unique Identifier",
                "CRM Case Number",
                "SBI",
                "Name",
                "Line Manager",
                "HEO",
                "SEO",
                "QC Completed By",
                "Date QC Completed",
                //QUESTIONS
                "1. Has the correct type of response been used?",
                "2. Has the 'To' field on an Email or the first line of the addressee on a Letter been completed and it is the correct Customer / Agent / Business name / Full email address and if a Letter, is the postal address is correct?",
                "3. Is there an appropriate subject line on the Email / Letter?",
                "4. Is the CRN or the SBI & Business Name correct?",
                "5. Has the correspondence been addressed to the correct person? If no this is not approved",
                "6. Does the person receiving the correspondence hold the correct level of permissions for the business in Rural Payments or does the response only contain general information?",
                "7. If the person doesn't hold the correct level of permissions for the business in Rural Payments, or the incoming email is from an unregistered email address, have we only included the business specific information the customer provided?",
                "8. Does the correspondence open with an appropriate introduction (eg - Thank you for your) Email / Letter / Telephone Call and include the correct date the inbound customer contact was received?",
                "9. Have all the issues the customer raised been addressed and any actions required by the customer have been included and are correct?",
                "10. Has the correct advice been given (LTT / Guidance Books / Proforma Response / GOV.UK) and the specific location of the advice has been included on the Case / Internal Note?",
                "11. Is the response correct in terms of Spelling / Grammar / Plain English / Formatting - Black font, Arial 12, full date format, introduced acronyms and abbreviations?",
                "12. Has the optional text within the correspondence been deleted (eg - Your/Your client)",
                "13. Do any attachments / annexes / tables contain the correct information and relate to the correct business?",
                //RESULT
                "Send Email Notification",
                "QC Result",
                "QC Fail Reason",
                "Recheck Required",
                "Exclude Quality Check Result",
                "Initial Recheck Required",
                "Comments",
                //AMENDMENT
                "Amendment Reason",
                "Amendment Comments",
                //CHALLENGE
                "Challenge Outcome",
                //RECHECK GENERAL (1)
                "Recheck Completed by",
                "Date Recheck Completed",
                //RECHECK QUESTIONS (1)
                "1. Has the correct type of response been used?",
                "2. Has the 'To' field on an Email or the first line of the addressee on a Letter been completed and it is the correct Customer / Agent / Business name / Full email address and if a Letter, is the postal address is correct?",
                "3. Is there an appropriate subject line on the Email / Letter?",
                "4. Is the CRN or the SBI & Business Name correct?",
                "5. Has the correspondence been addressed to the correct person? If no this is a not approved",
                "6. Does the person receiving the correspondence hold the correct level of permissions for the business in Rural Payments or does the response only contain general information?",
                "7. If the person doesn't hold the correct level of permissions for the business in Rural Payments, or the incoming email is from an unregistered email address, have we only included the business specific information the customer provided?",
                "8. Does the correspondence open with an appropriate introduction (eg - Thank you for your) Email / Letter / Telephone Call and include the correct date the inbound customer contact was received?",
                "9. Have all the issues the customer raised been addressed and any actions required by the customer have been included and are correct?",
                "10. Has the correct advice been given (LTT / Guidance Books / Proforma Response / GOV.UK) and the specific location of the advice has been included on the Case / Internal Note?",
                "11. Is the response correct in terms of Spelling / Grammar / Plain English / Formatting - Black font, Arial 12, full date format, introduced acronyms and abbreviations?",
                "12. Has the optional text within the correspondence been deleted (eg - Your/Your client)",
                "13. Do any attachments / annexes / tables contain the correct information and relate to the correct business?",
                //RECHECK RESULT (1)
                "Recheck Send Email Notification",
                "Recheck Result",
                "Recheck Comments",
                "Is a Further Recheck Required?",
                //RECHECK AMENDMENT (1)
                "Recheck Amendment Reason",
                "Recheck Amendment Comments",
                //RECHECK GENERAL (2)
                "Recheck Completed by",
                "Date Recheck Completed",
                //RECHECK QUESTIONS (2)
                "1. Has the correct type of response been used?",
                "2. Has the 'To' field on an Email or the first line of the addressee on a Letter been completed and it is the correct Customer / Agent / Business name / Full email address and if a Letter, is the postal address is correct?",
                "3. Is there an appropriate subject line on the Email / Letter?",
                "4. Is the CRN or the SBI & Business Name correct?",
                "5. Has the correspondence been addressed to the correct person? If no this is a not approved",
                "6. Does the person receiving the correspondence hold the correct level of permissions for the business in Rural Payments or does the response only contain general information?",
                "7. If the person doesn't hold the correct level of permissions for the business in Rural Payments, or the incoming email is from an unregistered email address, have we only included the business specific information the customer provided?",
                "8. Does the correspondence open with an appropriate introduction (eg - Thank you for your) Email / Letter / Telephone Call and include the correct date the inbound customer contact was received?",
                "9. Have all the issues the customer raised been addressed and any actions required by the customer have been included and are correct?",
                "10. Has the correct advice been given (LTT / Guidance Books / Proforma Response / GOV.UK) and the specific location of the advice has been included on the Case / Internal Note?",
                "11. Is the response correct in terms of Spelling / Grammar / Plain English / Formatting - Black font, Arial 12, full date format, introduced acronyms and abbreviations?",
                "12. Has the optional text within the correspondence been deleted (eg - Your/Your client)",
                "13. Do any attachments / annexes / tables contain the correct information and relate to the correct business?",
                //RECHECK RESULT (2)
                "Recheck Send Email Notification",
                "Recheck Result",
                "Recheck Comments",
                "Is a Further Recheck Required?",
                //RECHECK AMENDMENT (2)
                "Recheck Amendment Reason",
                "Recheck Amendment Comments",
                //RECHECK GENERAL (3)
                "Recheck Completed by",
                "Date Recheck Completed",
                //RECHECK QUESTIONS (3)
                "1. Has the correct type of response been used?",
                "2. Has the 'To' field on an Email or the first line of the addressee on a Letter been completed and it is the correct Customer / Agent / Business name / Full email address and if a Letter, is the postal address is correct?",
                "3. Is there an appropriate subject line on the Email / Letter?",
                "4. Is the CRN or the SBI & Business Name correct?",
                "5. Has the correspondence been addressed to the correct person? If no this is a not approved",
                "6. Does the person receiving the correspondence hold the correct level of permissions for the business in Rural Payments or does the response only contain general information?",
                "7. If the person doesn't hold the correct level of permissions for the business in Rural Payments, or the incoming email is from an unregistered email address, have we only included the business specific information the customer provided?",
                "8. Does the correspondence open with an appropriate introduction (eg - Thank you for your) Email / Letter / Telephone Call and include the correct date the inbound customer contact was received?",
                "9. Have all the issues the customer raised been addressed and any actions required by the customer have been included and are correct?",
                "10. Has the correct advice been given (LTT / Guidance Books / Proforma Response / GOV.UK) and the specific location of the advice has been included on the Case / Internal Note?",
                "11. Is the response correct in terms of Spelling / Grammar / Plain English / Formatting - Black font, Arial 12, full date format, introduced acronyms and abbreviations?",
                "12. Has the optional text within the correspondence been deleted (eg - Your/Your client)",
                "13. Do any attachments / annexes / tables contain the correct information and relate to the correct business?",
                //RECHECK RESULT (3)
                "Recheck Send Email Notification",
                "Recheck Result",
                "Recheck Comments",
                //RECHECK AMENDMENT (3)
                "Recheck Amendment Reason",
                "Recheck Amendment Comments"
                ));

            foreach (OutboundCorrespondence outboundCorrespondence in outboundCorrespondencesList)
            {
                //RECHECKS
                List<OutboundCorrespondenceReCheck> currentOutboundCorrespondenceReChecks = outboundCorrespondenceReChecksList.Where(x => x.OutboundCorrespondenceId == outboundCorrespondence.CheckId).ToList();

                OutboundCorrespondenceReCheck firstOutboundCorrespondenceReCheck = new OutboundCorrespondenceReCheck();
                OutboundCorrespondenceReCheck secondOutboundCorrespondenceReCheck = new OutboundCorrespondenceReCheck();
                OutboundCorrespondenceReCheck thirdOutboundCorrespondenceReCheck = new OutboundCorrespondenceReCheck();

                currentOutboundCorrespondenceReChecks = currentOutboundCorrespondenceReChecks.Where(reCheck => reCheck.DateQCCompleted.HasValue).OrderBy(reCheck => reCheck.DateQCCompleted.Value).ToList();

                if (currentOutboundCorrespondenceReChecks.Count > 0)
                {
                    firstOutboundCorrespondenceReCheck = currentOutboundCorrespondenceReChecks?.FirstOrDefault();
                }

                if (currentOutboundCorrespondenceReChecks.Count > 1)
                {
                    secondOutboundCorrespondenceReCheck = currentOutboundCorrespondenceReChecks?[1];
                }

                if (currentOutboundCorrespondenceReChecks.Count > 2)
                {
                    thirdOutboundCorrespondenceReCheck = currentOutboundCorrespondenceReChecks?.LastOrDefault();
                }

                outboundCorrespondencesList = outboundCorrespondencesList.Where(check => check.DateQCCompleted.HasValue).OrderBy(check => check.DateQCCompleted.Value).ToList();

                //CHECK ANSWERS
                List<CheckQuestion> outboundCorrespondenceQuestions = Questions.Where(x => x.CheckType.Name == "Outbound Correspondence").OrderBy(x => x.Order).ToList();
                List<string> outboundCorrespondenceAnswers = new List<string>();
                foreach (var outboundCorrespondenceQuestion in outboundCorrespondenceQuestions)
                {
                    string outboundCorrespondenceAnswer = answers.Where(x => x.CheckId == outboundCorrespondence.CheckId && x.QuestionId == outboundCorrespondenceQuestion.QuestionId).Select(c => c.Text).FirstOrDefault();
                    outboundCorrespondenceAnswers.Add(outboundCorrespondenceAnswer);
                }

                //CHECK RESULT
                string outboundCorrespondenceResult = checkResults.Where(x => x.CheckId == outboundCorrespondence.CheckId).Select(c => c.Result.Text).FirstOrDefault();

                string outboundCorrespondenceEmailNotficationYesNo = "No";

                if (outboundCorrespondence.EmailNotifications == true)
                {
                    outboundCorrespondenceEmailNotficationYesNo = "Yes";
                }

                string outboundCorrespondenceFailReason = null;

                if (outboundCorrespondenceResult == "Not Approved")
                {
                    outboundCorrespondenceFailReason = outboundCorrespondence.FailReason.Text;
                }

                string excludeQCResultYesNo = "No";

                if (outboundCorrespondence.ExcludeQCResult)
                {
                    excludeQCResultYesNo = "Yes";
                }

                string reCheckRequiredYesNo = "No";

                if (outboundCorrespondence.ReCheckRequired)
                {

                    reCheckRequiredYesNo = "Yes";
                }

                string initialReCheckDecisionYesNo = "No";

                if (outboundCorrespondence.InitialReCheckDecision)
                {
                    initialReCheckDecisionYesNo = "Yes";
                }

                //CHALLENGE
                string challenge = null;
                int challengeCount = challenges.Where(x => x.CheckId == outboundCorrespondence.CheckId).Count();

                if (challengeCount > 0)
                {
                    challenge = challenges.Where(x => x.CheckId == outboundCorrespondence.CheckId).Select(c => c.ChallengeOutcome.Text).FirstOrDefault();
                }

                //CHECK AMENDMENT REASON
                string outboundCorrespondenceAmendmentReasonText = null;
                string outboundCorrespondenceAmendmentReasonComments = null;
                int outboundCorrespondenceCheckAmendmentreasonCount = checkAmendmentReasons.Where(x => x.CheckId == outboundCorrespondence.CheckId).Count();

                if (outboundCorrespondenceCheckAmendmentreasonCount > 0)
                {
                    outboundCorrespondenceAmendmentReasonText = checkAmendmentReasons.Where(x => x.CheckId == outboundCorrespondence.CheckId).Select(c => c.AmendmentReason.Text).FirstOrDefault();
                    outboundCorrespondenceAmendmentReasonComments = checkAmendmentReasons.Where(x => x.CheckId == outboundCorrespondence.CheckId).Select(c => c.AmendmentReasonComments).FirstOrDefault();
                }

                //FIRST RECHECK ANSWERS
                List<CheckQuestion> firstOutboundCorrespondenceReCheckQuestions = Questions.Where(x => x.CheckType.Name == "Outbound Correspondence ReCheck").OrderBy(x => x.Order).ToList();
                List<string> firstOutboundCorrespondenceReCheckAnswers = new List<string>();
                foreach (var firstOutboundCorrespondenceReCheckQuestion in firstOutboundCorrespondenceReCheckQuestions)
                {
                    string firstOutboundCorrespondenceReCheckAnswer = "";
                    string checkFirstReCheckNull = answers.Where(x => x.CheckId == firstOutboundCorrespondenceReCheck.CheckId && x.QuestionId == firstOutboundCorrespondenceReCheckQuestion.QuestionId).Select(c => c.Text).FirstOrDefault();
                    if (checkFirstReCheckNull != null)
                    {
                        firstOutboundCorrespondenceReCheckAnswer = checkFirstReCheckNull;
                    }
                    firstOutboundCorrespondenceReCheckAnswers.Add(firstOutboundCorrespondenceReCheckAnswer);
                }

                string firstOutboundCorrespondenceReCheckResult = null;
                string firstOutboundCorrespondenceReCheckAmendmentReasonText = null;
                string firstOutboundCorrespondenceReCheckAmendmentReasonComments = null;
                string firstOutboundCorrespondenceReCheckDateCompleted = null;
                string firstOutboundCorrespondenceReCheckFurtherReCheckYesNo = null;
                string firstOutboundCorrespondenceReCheckEmailNotficationYesNo = null;

                if (firstOutboundCorrespondenceReCheckAnswers[0] != "")
                {
                    //FIRST RECHECK EMAIL
                    firstOutboundCorrespondenceReCheckEmailNotficationYesNo = "No";

                    if (firstOutboundCorrespondenceReCheck.EmailNotifications == true)
                    {
                        firstOutboundCorrespondenceReCheckEmailNotficationYesNo = "Yes";
                    }

                    //FIRST RECHECK RESULT
                    firstOutboundCorrespondenceReCheckResult = checkResults.Where(x => x.CheckId == firstOutboundCorrespondenceReCheck.CheckId).Select(c => c.Result.Text).FirstOrDefault();

                    //FIRST RECHECK AMENDMENT REASON
                    int firstOutboundCorrespondenceReCheckAmendmentReasonCount = checkAmendmentReasons.Where(x => x.CheckId == firstOutboundCorrespondenceReCheck.CheckId).Count();
                    if (firstOutboundCorrespondenceReCheckAmendmentReasonCount > 0)
                    {
                        firstOutboundCorrespondenceReCheckAmendmentReasonText = checkAmendmentReasons.Where(x => x.CheckId == firstOutboundCorrespondenceReCheck.CheckId).Select(c => c.AmendmentReason.Text).FirstOrDefault();
                        firstOutboundCorrespondenceReCheckAmendmentReasonComments = checkAmendmentReasons.Where(x => x.CheckId == firstOutboundCorrespondenceReCheck.CheckId).Select(c => c.AmendmentReasonComments).FirstOrDefault();
                    }

                    //FIRST RECHECK DATE
                    firstOutboundCorrespondenceReCheckDateCompleted = firstOutboundCorrespondenceReCheck.DateQCCompleted.ToString();

                    //FIRST RECHECCK FURTHER
                    firstOutboundCorrespondenceReCheckFurtherReCheckYesNo = "No";

                    if (firstOutboundCorrespondenceReCheck.FurtherReCheckRequired)
                    {
                        firstOutboundCorrespondenceReCheckFurtherReCheckYesNo = "Yes";
                    }
                }


                //SECOND RECHECK ANSWERS
                List<CheckQuestion> secondOutboundCorrespondenceReCheckQuestions = Questions.Where(x => x.CheckType.Name == "Outbound Correspondence ReCheck").OrderBy(x => x.Order).ToList();
                List<string> secondOutboundCorrespondenceReCheckAnswers = new List<string>();
                foreach (var secondOutboundCorrespondenceReCheckQuestion in secondOutboundCorrespondenceReCheckQuestions)
                {
                    string secondOutboundCorrespondenceReCheckAnswer = "";
                    string checkSecondReCheckNull = answers.Where(x => x.CheckId == secondOutboundCorrespondenceReCheck.CheckId && x.QuestionId == secondOutboundCorrespondenceReCheckQuestion.QuestionId).Select(c => c.Text).FirstOrDefault();
                    if (checkSecondReCheckNull != null)
                    {
                        secondOutboundCorrespondenceReCheckAnswer = checkSecondReCheckNull;
                    }

                    secondOutboundCorrespondenceReCheckAnswers.Add(secondOutboundCorrespondenceReCheckAnswer);
                }

                string secondOutboundCorrespondenceReCheckResult = null;
                string secondOutboundCorrespondenceReCheckAmendmentReasonText = null;
                string secondOutboundCorrespondenceReCheckAmendmentReasonComments = null;
                string secondOutboundCorrespondenceReCheckDateCompleted = null;
                string secondOutboundCorrespondenceReCheckFurtherReCheckYesNo = null;
                string secondOutboundCorrespondenceReCheckEmailNotficationYesNo = null;

                if (secondOutboundCorrespondenceReCheckAnswers[0] != "")
                {
                    //SECOND RECHECK EMAIL
                    secondOutboundCorrespondenceReCheckEmailNotficationYesNo = "No";

                    if (secondOutboundCorrespondenceReCheck.EmailNotifications == true)
                    {
                        secondOutboundCorrespondenceReCheckEmailNotficationYesNo = "Yes";
                    }

                    //SECOND RECHECK RESULT
                    secondOutboundCorrespondenceReCheckResult = checkResults.Where(x => x.CheckId == secondOutboundCorrespondenceReCheck.CheckId).Select(c => c.Result.Text).FirstOrDefault();

                    //SECOND RECHECK AMENDMENT REASON
                    int secondOutboundCorrespondenceReCheckAmendmentReasonCount = checkAmendmentReasons.Where(x => x.CheckId == secondOutboundCorrespondenceReCheck.CheckId).Count();
                    if (secondOutboundCorrespondenceReCheckAmendmentReasonCount > 0)
                    {
                        secondOutboundCorrespondenceReCheckAmendmentReasonText = checkAmendmentReasons.Where(x => x.CheckId == secondOutboundCorrespondenceReCheck.CheckId).Select(c => c.AmendmentReason.Text).FirstOrDefault();
                        secondOutboundCorrespondenceReCheckAmendmentReasonComments = checkAmendmentReasons.Where(x => x.CheckId == secondOutboundCorrespondenceReCheck.CheckId).Select(c => c.AmendmentReasonComments).FirstOrDefault();
                    }

                    //SECOND RECHECK DATE
                    secondOutboundCorrespondenceReCheckDateCompleted = secondOutboundCorrespondenceReCheck.DateQCCompleted.ToString();

                    //SECOND RECHECCK FURTHER
                    secondOutboundCorrespondenceReCheckFurtherReCheckYesNo = secondOutboundCorrespondenceReCheck.FurtherReCheckRequired.ToString();

                    secondOutboundCorrespondenceReCheckFurtherReCheckYesNo = "No";

                    if (secondOutboundCorrespondenceReCheck.FurtherReCheckRequired)
                    {
                        secondOutboundCorrespondenceReCheckFurtherReCheckYesNo = "Yes";
                    }
                }

                //THIRD RECHECK ANSWERS
                List<CheckQuestion> thirdOutboundCorrespondenceReCheckQuestions = Questions.Where(x => x.CheckType.Name == "Outbound Correspondence ReCheck").OrderBy(x => x.Order).ToList();
                List<string> thirdOutboundCorrespondenceReCheckAnswers = new List<string>();
                foreach (var thirdOutboundCorrespondenceReCheckQuestion in thirdOutboundCorrespondenceReCheckQuestions)
                {
                    string thirdOutboundCorrespondenceReCheckAnswer = "";
                    string checkThirdReCheckNull = answers.Where(x => x.CheckId == thirdOutboundCorrespondenceReCheck.CheckId && x.QuestionId == thirdOutboundCorrespondenceReCheckQuestion.QuestionId).Select(c => c.Text).FirstOrDefault();
                    if (checkThirdReCheckNull != null)
                    {
                        thirdOutboundCorrespondenceReCheckAnswer = checkThirdReCheckNull;
                    }
                    thirdOutboundCorrespondenceReCheckAnswers.Add(thirdOutboundCorrespondenceReCheckAnswer);
                }

                string thirdOutboundCorrespondenceReCheckResult = null;
                string thirdOutboundCorrespondenceReCheckAmendmentReasonText = null;
                string thirdOutboundCorrespondenceReCheckAmendmentReasonComments = null;
                string thirdOutboundCorrespondenceReCheckDateCompleted = null;
                string thirdOutboundCorrespondenceReCheckFurtherReCheck = null;
                string thirdOutboundCorrespondenceReCheckEmailNotficationYesNo = null;

                if (thirdOutboundCorrespondenceReCheckAnswers[0] != "")
                {
                    //THIRD RECHECK EMAIL
                    thirdOutboundCorrespondenceReCheckEmailNotficationYesNo = "No";

                    if (thirdOutboundCorrespondenceReCheck.EmailNotifications == true)
                    {
                        thirdOutboundCorrespondenceReCheckEmailNotficationYesNo = "Yes";
                    }

                    //THIRD RECHECK RESULT
                    thirdOutboundCorrespondenceReCheckResult = checkResults.Where(x => x.CheckId == thirdOutboundCorrespondenceReCheck.CheckId).Select(c => c.Result.Text).FirstOrDefault();

                    //THIRD RECHECK AMENDMENT REASON
                    int thirdOutboundCorrespondenceReCheckAmendmentReasonCount = checkAmendmentReasons.Where(x => x.CheckId == thirdOutboundCorrespondenceReCheck.CheckId).Count();
                    if (thirdOutboundCorrespondenceReCheckAmendmentReasonCount > 0)
                    {
                        thirdOutboundCorrespondenceReCheckAmendmentReasonText = checkAmendmentReasons.Where(x => x.CheckId == thirdOutboundCorrespondenceReCheck.CheckId).Select(c => c.AmendmentReason.Text).FirstOrDefault();
                        thirdOutboundCorrespondenceReCheckAmendmentReasonComments = checkAmendmentReasons.Where(x => x.CheckId == thirdOutboundCorrespondenceReCheck.CheckId).Select(c => c.AmendmentReasonComments).FirstOrDefault();
                    }

                    //THIRD RECHECK DATE
                    thirdOutboundCorrespondenceReCheckDateCompleted = thirdOutboundCorrespondenceReCheck.DateQCCompleted.ToString();

                    //THIRD RECHECCK FURTHER
                    thirdOutboundCorrespondenceReCheckFurtherReCheck = thirdOutboundCorrespondenceReCheck.FurtherReCheckRequired.ToString();
                }

                builder.AppendLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\",\"{18}\",\"{19}\",\"{20}\",\"{21}\",\"{22}\",\"{23}\",\"{24}\",\"{25}\",\"{26}\",\"{27}\",\"{28}\",\"{29}\",\"{30}\",\"{31}\",\"{32}\",\"{33}\",\"{34}\",\"{35}\",\"{36}\",\"{37}\",\"{38}\",\"{39}\",\"{40}\",\"{41}\",\"{42}\",\"{43}\",\"{44}\",\"{45}\",\"{46}\",\"{47}\",\"{48}\",\"{49}\",\"{50}\",\"{51}\",\"{52}\",\"{53}\",\"{54}\",\"{55}\",\"{56}\",\"{57}\",\"{58}\",\"{59}\",\"{60}\",\"{61}\",\"{62}\",\"{63}\",\"{64}\",\"{65}\",\"{66}\",\"{67}\",\"{68}\",\"{69}\",\"{70}\",\"{71}\",\"{72}\",\"{73}\",\"{74}\",\"{75}\",\"{76}\",\"{77}\",\"{78}\",\"{79}\",\"{80}\",\"{81}\",\"{82}\",\"{83}\",\"{84}\",\"{85}\",\"{86}\",\"{87}\",\"{88}\",\"{89}\",\"{90}\",\"{91}\",\"{92}\",\"{93}\",\"{94}\",\"{95}\",\"{96}\",\"{97}\"",
                    outboundCorrespondence.Scheme.Text,
                    outboundCorrespondence.BusinessArea.Text,
                    outboundCorrespondence.CorrespondenceType.Text,
                    outboundCorrespondence.TemplateReference,
                    outboundCorrespondence.UniqueIdentifierFull,
                    outboundCorrespondence.CrmReferenceFull,
                    outboundCorrespondence.SBI,
                    outboundCorrespondence.PersonName,
                    outboundCorrespondence.ManagerName,
                    outboundCorrespondence.HEO,
                    outboundCorrespondence.SEO,
                    outboundCorrespondence.QCCompletedByName,
                    outboundCorrespondence.DateQCCompleted,

                    outboundCorrespondenceAnswers[0],
                    outboundCorrespondenceAnswers[1],
                    outboundCorrespondenceAnswers[2],
                    outboundCorrespondenceAnswers[3],
                    outboundCorrespondenceAnswers[4],
                    outboundCorrespondenceAnswers[5],
                    outboundCorrespondenceAnswers[6],
                    outboundCorrespondenceAnswers[7],
                    outboundCorrespondenceAnswers[8],
                    outboundCorrespondenceAnswers[9],
                    outboundCorrespondenceAnswers[10],
                    outboundCorrespondenceAnswers[11],
                    outboundCorrespondenceAnswers[12],

                    outboundCorrespondenceEmailNotficationYesNo,
                    outboundCorrespondenceResult,
                    outboundCorrespondenceFailReason,
                    reCheckRequiredYesNo,
                    excludeQCResultYesNo,
                    initialReCheckDecisionYesNo,
                    outboundCorrespondence.Comments,
                    outboundCorrespondenceAmendmentReasonText,
                    outboundCorrespondenceAmendmentReasonComments,
                    challenge,

                    firstOutboundCorrespondenceReCheck?.ReCheckCompletedBy,
                    firstOutboundCorrespondenceReCheckDateCompleted,

                    firstOutboundCorrespondenceReCheckAnswers[0],
                    firstOutboundCorrespondenceReCheckAnswers[1],
                    firstOutboundCorrespondenceReCheckAnswers[2],
                    firstOutboundCorrespondenceReCheckAnswers[3],
                    firstOutboundCorrespondenceReCheckAnswers[4],
                    firstOutboundCorrespondenceReCheckAnswers[5],
                    firstOutboundCorrespondenceReCheckAnswers[6],
                    firstOutboundCorrespondenceReCheckAnswers[7],
                    firstOutboundCorrespondenceReCheckAnswers[8],
                    firstOutboundCorrespondenceReCheckAnswers[9],
                    firstOutboundCorrespondenceReCheckAnswers[10],
                    firstOutboundCorrespondenceReCheckAnswers[11],
                    firstOutboundCorrespondenceReCheckAnswers[12],

                    firstOutboundCorrespondenceReCheckEmailNotficationYesNo,
                    firstOutboundCorrespondenceReCheckResult,
                    firstOutboundCorrespondenceReCheck?.ReCheckComments,
                    firstOutboundCorrespondenceReCheckFurtherReCheckYesNo,
                    firstOutboundCorrespondenceReCheckAmendmentReasonText,
                    firstOutboundCorrespondenceReCheckAmendmentReasonComments,

                    secondOutboundCorrespondenceReCheck?.ReCheckCompletedBy,
                    secondOutboundCorrespondenceReCheckDateCompleted,

                    secondOutboundCorrespondenceReCheckAnswers[0],
                    secondOutboundCorrespondenceReCheckAnswers[1],
                    secondOutboundCorrespondenceReCheckAnswers[2],
                    secondOutboundCorrespondenceReCheckAnswers[3],
                    secondOutboundCorrespondenceReCheckAnswers[4],
                    secondOutboundCorrespondenceReCheckAnswers[5],
                    secondOutboundCorrespondenceReCheckAnswers[6],
                    secondOutboundCorrespondenceReCheckAnswers[7],
                    secondOutboundCorrespondenceReCheckAnswers[8],
                    secondOutboundCorrespondenceReCheckAnswers[9],
                    secondOutboundCorrespondenceReCheckAnswers[10],
                    secondOutboundCorrespondenceReCheckAnswers[11],
                    secondOutboundCorrespondenceReCheckAnswers[12],

                    secondOutboundCorrespondenceReCheckEmailNotficationYesNo,
                    secondOutboundCorrespondenceReCheckResult,
                    secondOutboundCorrespondenceReCheck?.ReCheckComments,
                    secondOutboundCorrespondenceReCheckFurtherReCheckYesNo,
                    secondOutboundCorrespondenceReCheckAmendmentReasonText,
                    secondOutboundCorrespondenceReCheckAmendmentReasonComments,

                    thirdOutboundCorrespondenceReCheck?.ReCheckCompletedBy,
                    thirdOutboundCorrespondenceReCheckDateCompleted,

                    thirdOutboundCorrespondenceReCheckAnswers[0],
                    thirdOutboundCorrespondenceReCheckAnswers[1],
                    thirdOutboundCorrespondenceReCheckAnswers[2],
                    thirdOutboundCorrespondenceReCheckAnswers[3],
                    thirdOutboundCorrespondenceReCheckAnswers[4],
                    thirdOutboundCorrespondenceReCheckAnswers[5],
                    thirdOutboundCorrespondenceReCheckAnswers[6],
                    thirdOutboundCorrespondenceReCheckAnswers[7],
                    thirdOutboundCorrespondenceReCheckAnswers[8],
                    thirdOutboundCorrespondenceReCheckAnswers[9],
                    thirdOutboundCorrespondenceReCheckAnswers[10],
                    thirdOutboundCorrespondenceReCheckAnswers[11],
                    thirdOutboundCorrespondenceReCheckAnswers[12],

                    thirdOutboundCorrespondenceReCheckEmailNotficationYesNo,
                    thirdOutboundCorrespondenceReCheckResult,
                    thirdOutboundCorrespondenceReCheck?.ReCheckComments,
                    thirdOutboundCorrespondenceReCheckAmendmentReasonText,
                    thirdOutboundCorrespondenceReCheckAmendmentReasonComments
                ));
            }

            return builder.ToString();
        }

        public string BuildBank(IEnumerable<BankAccountCheck> bankAccountList, IEnumerable<BankAccountReCheck> bankAccountReCheckList)
        {
            List<BankAccountCheck> bankAccounts = bankAccountList.OrderByDescending(z => z.DateQCCompleted).ToList();
            List<BankAccountReCheck> bankAccountReChecks = bankAccountReCheckList.ToList();
            List<CheckQuestion> Questions = db.CheckQuestions.AsNoTracking().ToList();
            List<Answer> answers = db.Answers.AsNoTracking().ToList();
            List<CheckResult> checkResults = db.CheckResults.AsNoTracking().ToList();
            List<CheckAmendmentReason> checkAmendmentReasons = db.CheckAmendmentReason.AsNoTracking().ToList();
            List<Challenge> challenges = db.Challenge.AsNoTracking().ToList();

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\",\"{18}\",\"{19}\",\"{20}\",\"{21}\",\"{22}\",\"{23}\",\"{24}\",\"{25}\",\"{26}\",\"{27}\",\"{28}\",\"{29}\",\"{30}\",\"{31}\",\"{32}\",\"{33}\",\"{34}\",\"{35}\",\"{36}\",\"{37}\",\"{38}\",\"{39}\",\"{40}\",\"{41}\",\"{42}\",\"{43}\",\"{44}\",\"{45}\",\"{46}\",\"{47}\",\"{48}\",\"{49}\",\"{50}\",\"{51}\",\"{52}\",\"{53}\",\"{54}\",\"{55}\",\"{56}\",\"{57}\",\"{58}\",\"{59}\",\"{60}\",\"{61}\",\"{62}\",\"{63}\",\"{64}\",\"{65}\",\"{66}\",\"{67}\",\"{68}\",\"{69}\",\"{70}\",\"{71}\",\"{72}\",\"{73}\",\"{74}\",\"{75}\",\"{76}\",\"{77}\",\"{78}\",\"{79}\",\"{80}\",\"{81}\",\"{82}\",\"{83}\",\"{84}\",\"{85}\",\"{86}\"",
                //GENERAL
                "SBI",
                "FRN",
                "Check Type",
                "Business Name",
                "Name",
                "Line Manager",
                "Date QC Created",
                "QC Completed By",
                "Date QC Completed",
                //QUESTIONS
                "1. Were Security Check questions asked and successfully answered?",
                "2. Did the caller/requestor hold permissions to update bank account details?",
                "3. Were the customer declarations read to the customer?",
                "4. Were the bank account details provided valid? (Use IBAN tool)",
                "5. Has the correct beneficiary name been entered in the bank account name field?",
                "6. Have all bank account details been entered correctly? i.e. account number, sort code, building society roll number(if applicable)",
                "7. Has the correct currency type (EURO or GBP) been selected?",
                "8. Have outdated bank account records for this currency, been made inactive?",
                "9. Have interaction notes been added to CRM, confirming the last 4 digits of the bank account?",
                "10. Have all of the processes been correctly followed in relation to P&I or deregistration?",
                "11. Has the CRM task been queued to the Bank Letter Change Queue?",
                //RESULT
                "Send Email Notification",
                "QC Result",
                "QC Not Approved Reason",
                "Recheck Required",
                "Initial Recheck Required",
                "Comments",
                //AMENDMENT
                "Amendment Reason",
                "Amendment Comments",
                //CHALLENGE
                "Challenge Outcome",

                //RECHECK GENERAL (1)
                "Recheck Completed by",
                "Date Recheck Completed",
                //RECHECK QUESTIONS (1)
                "1. Were Security Check questions asked and successfully answered?",
                "2. Did the caller/requestor hold permissions to update bank account details?",
                "3. Were the customer declarations read to the customer?",
                "4. Were the bank account details provided valid? (Use IBAN tool)",
                "5. Has the correct beneficiary name been entered in the bank account name field?",
                "6. Have all bank account details been entered correctly? i.e. account number, sort code, building society roll number(if applicable)",
                "7. Has the correct currency type (EURO or GBP) been selected?",
                "8. Have outdated bank account records for this currency, been made inactive?",
                "9. Have interaction notes been added to CRM, confirming the last 4 digits of the bank account?",
                "10. Have all of the processes been correctly followed in relation to P&I or deregistration?",
                "11. Has the CRM task been queued to the Bank Letter Change Queue?",
                //RECHECK RESULT (1)
                "Recheck Send Email Notification",
                "Recheck Result",
                "Recheck Not Approved Reason",
                "Recheck Comments",
                "Is a Further Recheck Required?",
                //RECHECK AMENDMENT (1)
                "Recheck Amendment Reason",
                "Recheck Amendment Comments",

                //RECHECK GENERAL (2)
                "Recheck Completed by",
                "Date Recheck Completed",
                //RECHECK QUESTIONS (2)
                "1. Were Security Check questions asked and successfully answered?",
                "2. Did the caller/requestor hold permissions to update bank account details?",
                "3. Were the customer declarations read to the customer?",
                "4. Were the bank account details provided valid? (Use IBAN tool)",
                "5. Has the correct beneficiary name been entered in the bank account name field?",
                "6. Have all bank account details been entered correctly? i.e. account number, sort code, building society roll number(if applicable)",
                "7. Has the correct currency type (EURO or GBP) been selected?",
                "8. Have outdated bank account records for this currency, been made inactive?",
                "9. Have interaction notes been added to CRM, confirming the last 4 digits of the bank account?",
                "10. Have all of the processes been correctly followed in relation to P&I or deregistration?",
                "11. Has the CRM task been queued to the Bank Letter Change Queue?",
                //RECHECK RESULT (2)
                "Recheck Send Email Notification",
                "Recheck Result",
                "Recheck Not Approved Reason",
                "Recheck Comments",
                "Is a Further Recheck Required?",
                //RECHECK AMENDMENT (2)
                "Recheck Amendment Reason",
                "Recheck Amendment Comments",

                //RECHECK GENERAL (3)
                "Recheck Completed by",
                "Date Recheck Completed",
                //RECHECK QUESTIONS (3)
                "1. Were Security Check questions asked and successfully answered?",
                "2. Did the caller/requestor hold permissions to update bank account details?",
                "3. Were the customer declarations read to the customer?",
                "4. Were the bank account details provided valid? (Use IBAN tool)",
                "5. Has the correct beneficiary name been entered in the bank account name field?",
                "6. Have all bank account details been entered correctly? i.e. account number, sort code, building society roll number(if applicable)",
                "7. Has the correct currency type (EURO or GBP) been selected?",
                "8. Have outdated bank account records for this currency, been made inactive?",
                "9. Have interaction notes been added to CRM, confirming the last 4 digits of the bank account?",
                "10. Have all of the processes been correctly followed in relation to P&I or deregistration?",
                "11. Has the CRM task been queued to the Bank Letter Change Queue?",
                //RECHECK RESULT (3)
                "Recheck Send Email Notification",
                "Recheck Result",
                "Recheck Comments",
                //RECHECK AMENDMENT (3)
                "Recheck Amendment Reason",
                "Recheck Amendment Comments"
                ));

            bankAccounts = bankAccounts.OrderBy(check => check.DateQCCompleted.Value).ToList();

            foreach (BankAccountCheck bankAccount in bankAccounts)
            {
                //RECHECKS
                List<BankAccountReCheck> bankAccountReCheck = bankAccountReCheckList.Where(x => x.BankAccountId == bankAccount.CheckId).ToList();

                BankAccountReCheck firstBankAccountReCheck = new BankAccountReCheck();
                BankAccountReCheck secondBankAccountReCheck = new BankAccountReCheck();
                BankAccountReCheck thirdBankAccountReCheck = new BankAccountReCheck();

                bankAccountReCheck = bankAccountReCheck.Where(reCheck => reCheck.DateQCCompleted.HasValue).OrderBy(reCheck => reCheck.DateQCCompleted.Value).ToList();

                if (bankAccountReCheck.Count > 0)
                {
                    firstBankAccountReCheck = bankAccountReCheck?.FirstOrDefault();
                }

                if (bankAccountReCheck.Count > 1)
                {
                    secondBankAccountReCheck = bankAccountReCheck?[1];
                }

                if (bankAccountReCheck.Count > 2)
                {
                    thirdBankAccountReCheck = bankAccountReCheck?.LastOrDefault();
                }

                //CHECK ANSWERS
                List<CheckQuestion> bankAccountQuestions = Questions.Where(x => x.CheckType.Name == "Bank Account").OrderBy(x => x.Order).ToList();
                List<string> bankAccountAnswers = new List<string>();
                foreach (var bankAccountQuestion in bankAccountQuestions)
                {
                    string bankAccountAnswer = answers.Where(x => x.CheckId == bankAccount.CheckId && x.QuestionId == bankAccountQuestion.QuestionId).Select(c => c.Text).FirstOrDefault();
                    bankAccountAnswers.Add(bankAccountAnswer);
                }

                //CHECK RESULT
                string bankAccountResult = checkResults.Where(x => x.CheckId == bankAccount.CheckId).Select(c => c.Result.Text).FirstOrDefault();

                string bankAccountEmailNotficationYesNo = "No";

                if (bankAccount.EmailNotifications == true)
                {
                    bankAccountEmailNotficationYesNo = "Yes";
                }

                string bankAccountFailReason = null;

                if (bankAccountResult == "Not Approved")
                {
                    bankAccountFailReason = bankAccount.FailReason.Text;
                }

                string reCheckRequiredYesNo = "No";

                if (bankAccount.ReCheckRequired)
                {

                    reCheckRequiredYesNo = "Yes";
                }

                string initialReCheckDecisionYesNo = "No";

                if (bankAccount.InitialReCheckDecision)
                {
                    initialReCheckDecisionYesNo = "Yes";
                }

                //CHALLENGE
                string challenge = null;
                int challengeCount = challenges.Where(x => x.CheckId == bankAccount.CheckId).Count();

                if (challengeCount > 0)
                {
                    challenge = challenges.Where(x => x.CheckId == bankAccount.CheckId).Select(c => c.ChallengeOutcome.Text).FirstOrDefault();
                }

                //CHECK AMENDMENT REASON
                string bankAccountAmendmentReasonText = null;
                string bankAccountAmendmentReasonComments = null;
                int bankAccountCheckAmendmentreasonCount = checkAmendmentReasons.Where(x => x.CheckId == bankAccount.CheckId).Count();

                if (bankAccountCheckAmendmentreasonCount > 0)
                {
                    bankAccountAmendmentReasonText = checkAmendmentReasons.Where(x => x.CheckId == bankAccount.CheckId).Select(c => c.AmendmentReason.Text).FirstOrDefault();
                    bankAccountAmendmentReasonComments = checkAmendmentReasons.Where(x => x.CheckId == bankAccount.CheckId).Select(c => c.AmendmentReasonComments).FirstOrDefault();
                }

                //FIRST RECHECK ANSWERS
                List<CheckQuestion> firstBankAccountReCheckQuestions = Questions.Where(x => x.CheckType.Name == "Bank Account ReCheck").OrderBy(x => x.Order).ToList();
                List<string> firstBankAccountReCheckAnswers = new List<string>();
                foreach (var firstBankAccountReCheckQuestion in firstBankAccountReCheckQuestions)
                {
                    string firstBankAccountReCheckAnswer = "";
                    string checkFirstReCheckNull = answers.Where(x => x.CheckId == firstBankAccountReCheck.CheckId && x.QuestionId == firstBankAccountReCheckQuestion.QuestionId).Select(c => c.Text).FirstOrDefault();
                    if (checkFirstReCheckNull != null)
                    {
                        firstBankAccountReCheckAnswer = checkFirstReCheckNull;
                    }
                    firstBankAccountReCheckAnswers.Add(firstBankAccountReCheckAnswer);
                }

                string firstBankAccountReCheckEmailNotficationYesNo = null;
                string firstBankAccountReCheckResult = null;
                string firstBankAccountReCheckAmendmentReasonText = null;
                string firstBankAccountReCheckAmendmentReasonComments = null;
                string firstBankAccountReCheckDateCompleted = null;
                string firstBankAccountReCheckFurtherReCheckYesNo = null;
                string firstBankAccountReCheckFailReason = null;

                if (firstBankAccountReCheckAnswers[0] != "")
                {
                    //FIRST RECHECK EMAIL
                    firstBankAccountReCheckEmailNotficationYesNo = "No";

                    if (firstBankAccountReCheck.EmailNotifications == true)
                    {
                        firstBankAccountReCheckEmailNotficationYesNo = "Yes";
                    }

                    //FIRST RECHECK RESULT
                    firstBankAccountReCheckResult = checkResults.Where(x => x.CheckId == firstBankAccountReCheck.CheckId).Select(c => c.Result.Text).FirstOrDefault();

                    //FIRST RECHECK AMENDMENT REASON
                    int firstBankAccountReCheckAmendmentReasonCount = checkAmendmentReasons.Where(x => x.CheckId == firstBankAccountReCheck.CheckId).Count();
                    if (firstBankAccountReCheckAmendmentReasonCount > 0)
                    {
                        firstBankAccountReCheckAmendmentReasonText = checkAmendmentReasons.Where(x => x.CheckId == firstBankAccountReCheck.CheckId).Select(c => c.AmendmentReason.Text).FirstOrDefault();
                        firstBankAccountReCheckAmendmentReasonComments = checkAmendmentReasons.Where(x => x.CheckId == firstBankAccountReCheck.CheckId).Select(c => c.AmendmentReasonComments).FirstOrDefault();
                    }

                    //FIRST RECHECK DATE
                    firstBankAccountReCheckDateCompleted = firstBankAccountReCheck.DateQCCompleted.ToString();

                    //FIRST RECHECK FURTHER
                    firstBankAccountReCheckFurtherReCheckYesNo = "No";

                    if (firstBankAccountReCheck.FurtherReCheckRequired)
                    {
                        firstBankAccountReCheckFurtherReCheckYesNo = "Yes";
                    }

                    if (firstBankAccountReCheckResult == "Not Approved")
                    {
                        firstBankAccountReCheckFailReason = firstBankAccountReCheck.FailReason.Text;
                    }
                }

                //SECOND RECHECK ANSWERS
                List<CheckQuestion> secondBankAccountReCheckQuestions = Questions.Where(x => x.CheckType.Name == "Bank Account ReCheck").OrderBy(x => x.Order).ToList();
                List<string> secondBankAccountReCheckAnswers = new List<string>();
                foreach (var secondBankAccountReCheckQuestion in secondBankAccountReCheckQuestions)
                {
                    string secondBankAccountReCheckAnswer = "";
                    string checkSecondReCheckNull = answers.Where(x => x.CheckId == secondBankAccountReCheck.CheckId && x.QuestionId == secondBankAccountReCheckQuestion.QuestionId).Select(c => c.Text).FirstOrDefault();
                    if (checkSecondReCheckNull != null)
                    {
                        secondBankAccountReCheckAnswer = checkSecondReCheckNull;
                    }

                    secondBankAccountReCheckAnswers.Add(secondBankAccountReCheckAnswer);
                }

                string secondBankAccountReCheckEmailNotficationYesNo = null;
                string secondBankAccountReCheckResult = null;
                string secondBankAccountReCheckAmendmentReasonText = null;
                string secondBankAccountReCheckAmendmentReasonComments = null;
                string secondBankAccountReCheckDateCompleted = null;
                string secondBankAccountReCheckFurtherReCheckYesNo = null;
                string secondBankAccountReCheckFailReason = null;
                if (secondBankAccountReCheckAnswers[0] != "")
                {
                    //SECOND RECHECK EMAIL
                    secondBankAccountReCheckEmailNotficationYesNo = "No";

                    if (secondBankAccountReCheck.EmailNotifications == true)
                    {
                        secondBankAccountReCheckEmailNotficationYesNo = "Yes";
                    }

                    //SECOND RECHECK RESULT
                    secondBankAccountReCheckResult = checkResults.Where(x => x.CheckId == secondBankAccountReCheck.CheckId).Select(c => c.Result.Text).FirstOrDefault();

                    //SECOND RECHECK AMENDMENT REASON
                    int secondBankAccountReCheckAmendmentReasonCount = checkAmendmentReasons.Where(x => x.CheckId == secondBankAccountReCheck.CheckId).Count();
                    if (secondBankAccountReCheckAmendmentReasonCount > 0)
                    {
                        secondBankAccountReCheckAmendmentReasonText = checkAmendmentReasons.Where(x => x.CheckId == secondBankAccountReCheck.CheckId).Select(c => c.AmendmentReason.Text).FirstOrDefault();
                        secondBankAccountReCheckAmendmentReasonComments = checkAmendmentReasons.Where(x => x.CheckId == secondBankAccountReCheck.CheckId).Select(c => c.AmendmentReasonComments).FirstOrDefault();
                    }

                    //SECOND RECHECK DATE
                    secondBankAccountReCheckDateCompleted = secondBankAccountReCheck.DateQCCompleted.ToString();

                    //SECOND RECHECK FURTHER
                    secondBankAccountReCheckFurtherReCheckYesNo = secondBankAccountReCheck.FurtherReCheckRequired.ToString();

                    secondBankAccountReCheckFurtherReCheckYesNo = "No";

                    if (secondBankAccountReCheck.FurtherReCheckRequired)
                    {
                        secondBankAccountReCheckFurtherReCheckYesNo = "Yes";
                    }

                    if (secondBankAccountReCheckResult == "Not Approved")
                    {
                        secondBankAccountReCheckFailReason = secondBankAccountReCheck.FailReason.Text;
                    }
                }

                //THIRD RECHECK ANSWERS
                List<CheckQuestion> thirdBankAccountReCheckQuestions = Questions.Where(x => x.CheckType.Name == "Bank Account ReCheck").OrderBy(x => x.Order).ToList();
                List<string> thirdBankAccountReCheckAnswers = new List<string>();
                foreach (var thirdBankAccountReCheckQuestion in thirdBankAccountReCheckQuestions)
                {
                    string thirdBankAccountReCheckAnswer = "";
                    string checkThirdReCheckNull = answers.Where(x => x.CheckId == thirdBankAccountReCheck.CheckId && x.QuestionId == thirdBankAccountReCheckQuestion.QuestionId).Select(c => c.Text).FirstOrDefault();
                    if (checkThirdReCheckNull != null)
                    {
                        thirdBankAccountReCheckAnswer = checkThirdReCheckNull;
                    }
                    thirdBankAccountReCheckAnswers.Add(thirdBankAccountReCheckAnswer);
                }

                string thirdBankAccountReCheckEmailNotficationYesNo = null;
                string thirdBankAccountReCheckResult = null;
                string thirdBankAccountReCheckAmendmentReasonText = null;
                string thirdBankAccountReCheckAmendmentReasonComments = null;
                string thirdBankAccountReCheckDateCompleted = null;
                string thirdBankAccountReCheckFurtherReCheck = null;

                if (thirdBankAccountReCheckAnswers[0] != "")
                {
                    //THIRD RECHECK EMAIL
                    thirdBankAccountReCheckEmailNotficationYesNo = "No";

                    if (thirdBankAccountReCheck.EmailNotifications == true)
                    {
                        thirdBankAccountReCheckEmailNotficationYesNo = "Yes";
                    }

                    //THIRD RECHECK RESULT
                    thirdBankAccountReCheckResult = checkResults.Where(x => x.CheckId == thirdBankAccountReCheck.CheckId).Select(c => c.Result.Text).FirstOrDefault();

                    //THIRD RECHECK AMENDMENT REASON
                    int thirdBankAccountReCheckAmendmentReasonCount = checkAmendmentReasons.Where(x => x.CheckId == thirdBankAccountReCheck.CheckId).Count();
                    if (thirdBankAccountReCheckAmendmentReasonCount > 0)
                    {
                        thirdBankAccountReCheckAmendmentReasonText = checkAmendmentReasons.Where(x => x.CheckId == thirdBankAccountReCheck.CheckId).Select(c => c.AmendmentReason.Text).FirstOrDefault();
                        thirdBankAccountReCheckAmendmentReasonComments = checkAmendmentReasons.Where(x => x.CheckId == thirdBankAccountReCheck.CheckId).Select(c => c.AmendmentReasonComments).FirstOrDefault();
                    }

                    //THIRD RECHECK DATE
                    thirdBankAccountReCheckDateCompleted = thirdBankAccountReCheck.DateQCCompleted.ToString();

                    //THIRD RECHECK FURTHER
                    thirdBankAccountReCheckFurtherReCheck = thirdBankAccountReCheck.FurtherReCheckRequired.ToString();
                }

                builder.AppendLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\",\"{18}\",\"{19}\",\"{20}\",\"{21}\",\"{22}\",\"{23}\",\"{24}\",\"{25}\",\"{26}\",\"{27}\",\"{28}\",\"{29}\",\"{30}\",\"{31}\",\"{32}\",\"{33}\",\"{34}\",\"{35}\",\"{36}\",\"{37}\",\"{38}\",\"{39}\",\"{40}\",\"{41}\",\"{42}\",\"{43}\",\"{44}\",\"{45}\",\"{46}\",\"{47}\",\"{48}\",\"{49}\",\"{50}\",\"{51}\",\"{52}\",\"{53}\",\"{54}\",\"{55}\",\"{56}\",\"{57}\",\"{58}\",\"{59}\",\"{60}\",\"{61}\",\"{62}\",\"{63}\",\"{64}\",\"{65}\",\"{66}\",\"{67}\",\"{68}\",\"{69}\",\"{70}\",\"{71}\",\"{72}\",\"{73}\",\"{74}\",\"{75}\",\"{76}\",\"{77}\",\"{78}\",\"{79}\",\"{80}\",\"{81}\",\"{82}\",\"{83}\",\"{84}\",\"{85}\",\"{86}\"",
                    bankAccount.SBI,
                    bankAccount.FRN,
                    bankAccount.BACheckType.Text,
                    bankAccount.BusinessName,
                    bankAccount.PersonName,
                    bankAccount.ManagerName,
                    bankAccount.DateQCCreated,
                    bankAccount.QCCompletedByName,
                    bankAccount.DateQCCompleted,

                    bankAccountAnswers[0],
                    bankAccountAnswers[1],
                    bankAccountAnswers[2],
                    bankAccountAnswers[3],
                    bankAccountAnswers[4],
                    bankAccountAnswers[5],
                    bankAccountAnswers[6],
                    bankAccountAnswers[7],
                    bankAccountAnswers[8],
                    bankAccountAnswers[9],
                    bankAccountAnswers[10],

                    bankAccountEmailNotficationYesNo,
                    bankAccountResult,
                    bankAccountFailReason,
                    reCheckRequiredYesNo,
                    initialReCheckDecisionYesNo,
                    bankAccount.Comments,
                    bankAccountAmendmentReasonText,
                    bankAccountAmendmentReasonComments,
                    challenge,

                    firstBankAccountReCheck?.ReCheckCompletedBy,
                    firstBankAccountReCheckDateCompleted,

                    firstBankAccountReCheckAnswers[0],
                    firstBankAccountReCheckAnswers[1],
                    firstBankAccountReCheckAnswers[2],
                    firstBankAccountReCheckAnswers[3],
                    firstBankAccountReCheckAnswers[4],
                    firstBankAccountReCheckAnswers[5],
                    firstBankAccountReCheckAnswers[6],
                    firstBankAccountReCheckAnswers[7],
                    firstBankAccountReCheckAnswers[8],
                    firstBankAccountReCheckAnswers[9],
                    firstBankAccountReCheckAnswers[10],

                    firstBankAccountReCheckEmailNotficationYesNo,
                    firstBankAccountReCheckResult,
                    firstBankAccountReCheckFailReason,
                    firstBankAccountReCheck?.ReCheckComments,
                    firstBankAccountReCheckFurtherReCheckYesNo,
                    firstBankAccountReCheckAmendmentReasonText,
                    firstBankAccountReCheckAmendmentReasonComments,

                    secondBankAccountReCheck?.ReCheckCompletedBy,
                    secondBankAccountReCheckDateCompleted,

                    secondBankAccountReCheckAnswers[0],
                    secondBankAccountReCheckAnswers[1],
                    secondBankAccountReCheckAnswers[2],
                    secondBankAccountReCheckAnswers[3],
                    secondBankAccountReCheckAnswers[4],
                    secondBankAccountReCheckAnswers[5],
                    secondBankAccountReCheckAnswers[6],
                    secondBankAccountReCheckAnswers[7],
                    secondBankAccountReCheckAnswers[8],
                    secondBankAccountReCheckAnswers[9],
                    secondBankAccountReCheckAnswers[10],

                    secondBankAccountReCheckEmailNotficationYesNo,
                    secondBankAccountReCheckResult,
                    secondBankAccountReCheckFailReason,
                    secondBankAccountReCheck?.ReCheckComments,
                    secondBankAccountReCheckFurtherReCheckYesNo,
                    secondBankAccountReCheckAmendmentReasonText,
                    secondBankAccountReCheckAmendmentReasonComments,

                    thirdBankAccountReCheck?.ReCheckCompletedBy,
                    thirdBankAccountReCheckDateCompleted,

                    thirdBankAccountReCheckAnswers[0],
                    thirdBankAccountReCheckAnswers[1],
                    thirdBankAccountReCheckAnswers[2],
                    thirdBankAccountReCheckAnswers[3],
                    thirdBankAccountReCheckAnswers[4],
                    thirdBankAccountReCheckAnswers[5],
                    thirdBankAccountReCheckAnswers[6],
                    thirdBankAccountReCheckAnswers[7],
                    thirdBankAccountReCheckAnswers[8],
                    thirdBankAccountReCheckAnswers[9],
                    thirdBankAccountReCheckAnswers[10],

                    thirdBankAccountReCheckEmailNotficationYesNo,
                    thirdBankAccountReCheckResult,
                    thirdBankAccountReCheck?.ReCheckComments,
                    thirdBankAccountReCheckAmendmentReasonText,
                    thirdBankAccountReCheckAmendmentReasonComments
                ));
            }

            return builder.ToString();
        }
    }
}
