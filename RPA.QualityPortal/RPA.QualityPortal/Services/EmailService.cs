using RPA.QualityPortal.DAL;
using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.ViewModels;
using RPA.QualityPortal.ViewModels.BankAccount;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using RPA.QualityPortal.Helpers;

namespace RPA.QualityPortal.Services
{
    public class EmailService : IMessageService
    {
        IQualityContext db;
        IPeopleContext pdb;

        public EmailService(IQualityContext context, IPeopleContext peopleContext)
        {
            this.db = context;
            this.pdb = peopleContext;
        }
        public MailMessage GenerateOutboundCorrespondenceEmail(OutboundCorrespondence outboundCorrespondence)
        {

            OutboundCorrespondence outboundCorrespondenceSaved = db.OutboundCorrespondence.Include("FailReason").Include("Scheme").Include("BusinessArea").Include("CorrespondenceType").Include("UniqueIdentifierPrefix").Include("CRMRefPrefix").Where(x => x.CheckId == outboundCorrespondence.CheckId).FirstOrDefault();

            int ReCheckId = db.OutboundCorrespondenceReCheck.Where(c => c.OutboundCorrespondenceId == outboundCorrespondenceSaved.CheckId && c.ReCheckActive == true).Select(v => v.CheckId).FirstOrDefault();

            bool reCheck = false;

            DetailsReCheck detailsReCheck = new DetailsReCheck
            {
                OutboundCorrespondence = outboundCorrespondenceSaved,
                QCResult = db.CheckResults.Where(x => x.CheckId == outboundCorrespondenceSaved.CheckId).Select(p => p.Result.Text).FirstOrDefault(),
                Answers = db.Answers.Include("Question").Where(x => x.CheckId == outboundCorrespondenceSaved.CheckId).OrderBy(p => p.QuestionId).ToList(),
                OutboundCorrespondenceReCheck = db.OutboundCorrespondenceReCheck.Where(x => x.CheckId == ReCheckId).FirstOrDefault() == null ? null : db.OutboundCorrespondenceReCheck.Where(x => x.CheckId == ReCheckId).FirstOrDefault(),
                ReCheckAnswers = db.Answers.Include("Question").Where(x => x.CheckId == ReCheckId).OrderBy(p => p.QuestionId).ToList() == null ? null : db.Answers.Where(x => x.CheckId == ReCheckId).OrderBy(p => p.QuestionId).ToList(),
                ReCheckResult = db.CheckResults.Where(x => x.CheckId == ReCheckId).Select(p => p.Result.Text).FirstOrDefault() == null ? null : db.CheckResults.Where(x => x.CheckId == ReCheckId).Select(p => p.Result.Text).FirstOrDefault()
            };

            string recheckRequiredYesNo = "No";

            if (detailsReCheck.OutboundCorrespondence.ReCheckRequired)
            {
                recheckRequiredYesNo = "Yes";
            }

            string excludeQCResultYesNo = "No";

            if (detailsReCheck.OutboundCorrespondence.ExcludeQCResult)
            {
                excludeQCResultYesNo = "Yes";
            }

            if (detailsReCheck.OutboundCorrespondenceReCheck != null)
            {
                reCheck = true;
            }

            StringWriter stringWriter = new StringWriter();

            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                writer.AddStyleAttribute("font-family", "Arial,'Segoe UI', Verdana, Helvetica, Sans-Serif"); //Set fonts for body.  You can add any CSS attribute wit this tag.
                writer.AddStyleAttribute("font-size", "12"); //sets font size
                writer.RenderBeginTag(HtmlTextWriterTag.Div);  //start tag 
                writer.RenderBeginTag(HtmlTextWriterTag.H3); //h1 tag
                writer.RenderBeginTag(HtmlTextWriterTag.P); //p tag1
                writer.Write("Hello, please see the QC results below:");
                writer.RenderEndTag(); //p end tag
                writer.RenderEndTag(); //H1 end tag
                writer.RenderBeginTag(HtmlTextWriterTag.H4);//h4 start tag
                writer.Write("Result: ");
                writer.RenderEndTag(); //h4 end tag
                writer.RenderBeginTag(HtmlTextWriterTag.Br);
                writer.Write(detailsReCheck.QCResult);
                writer.RenderEndTag();//end tag br
                writer.RenderBeginTag(HtmlTextWriterTag.H4);//h4 start tag
                writer.Write("Recheck Required: ");
                writer.RenderEndTag(); //h4 end tag
                writer.RenderBeginTag(HtmlTextWriterTag.Br);
                writer.Write(recheckRequiredYesNo);
                writer.RenderEndTag();//end tag br
                writer.RenderBeginTag(HtmlTextWriterTag.H4);//h4 start tag
                writer.Write("Exclude Quality Check Result: ");
                writer.RenderEndTag(); //h4 end tag
                writer.RenderBeginTag(HtmlTextWriterTag.Br);
                writer.Write(excludeQCResultYesNo);
                writer.RenderEndTag();//end tag br
                writer.RenderBeginTag(HtmlTextWriterTag.Br);

                if (outboundCorrespondenceSaved.FailReason != null)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.H4);//h4 start tag
                    writer.Write("Not Approved Reason:");
                    writer.RenderEndTag(); //h4 end tag
                    writer.RenderBeginTag(HtmlTextWriterTag.Br);
                    writer.Write(outboundCorrespondenceSaved.FailReason.Text);
                    writer.RenderEndTag();//end tag br
                }
                writer.RenderEndTag();//end tag br

                writer.RenderBeginTag(HtmlTextWriterTag.Br);
                if (detailsReCheck.OutboundCorrespondenceReCheck == null)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.H4);//h4 start tag
                    writer.Write("Most Recent Recheck Result: ");
                    writer.RenderEndTag(); //h4 end tag
                    writer.RenderBeginTag(HtmlTextWriterTag.Br);
                    writer.Write("N/A");
                    writer.RenderEndTag();//end tag br
                }
                else
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.H4);//h4 start tag
                    writer.Write("Most Recent Recheck Result ");
                    writer.RenderEndTag(); //h4 end tag
                    writer.RenderBeginTag(HtmlTextWriterTag.Br);
                    writer.Write(db.CheckResults.Where(x => x.CheckId == detailsReCheck.OutboundCorrespondenceReCheck.CheckId).Select(c => c.Result.Text).FirstOrDefault());
                    writer.RenderEndTag(); //end tag br
                }
                writer.RenderEndTag();//end tag br

                writer.RenderBeginTag(HtmlTextWriterTag.H3); //h1 tag
                writer.RenderBeginTag(HtmlTextWriterTag.P); //p tag 
                writer.Write("General Details");
                writer.RenderEndTag(); //p end tag
                writer.RenderEndTag(); //H1 end tag

                writer.RenderBeginTag(HtmlTextWriterTag.Br);
                writer.Write("Scheme: ");
                writer.Write(outboundCorrespondenceSaved.Scheme.Text);
                writer.RenderEndTag(); //end tag br

                writer.RenderBeginTag(HtmlTextWriterTag.Br);
                writer.Write("Business Area: ");
                writer.Write(outboundCorrespondenceSaved.BusinessArea.Text);
                writer.RenderEndTag(); //end tag br

                writer.RenderBeginTag(HtmlTextWriterTag.Br);
                writer.Write("Correspondence Type: ");
                writer.Write(outboundCorrespondenceSaved.CorrespondenceType.Text);
                writer.RenderEndTag(); //end tag br

                writer.RenderBeginTag(HtmlTextWriterTag.Br);
                writer.Write("Template Reference: ");
                writer.Write(outboundCorrespondenceSaved.TemplateReference);
                writer.RenderEndTag(); //end tag br

                writer.RenderBeginTag(HtmlTextWriterTag.Br);
                writer.Write("Unique Identifier: ");
                writer.Write(outboundCorrespondenceSaved.UniqueIdentifierPrefix.Text + outboundCorrespondenceSaved.UniqueId);
                writer.RenderEndTag(); //end tag br

                writer.RenderBeginTag(HtmlTextWriterTag.Br);
                writer.Write("CRM Case Number: ");
                writer.Write(outboundCorrespondenceSaved.CRMRefPrefix.Text + outboundCorrespondenceSaved.CRMRef);
                writer.RenderEndTag(); //end tag br

                writer.RenderBeginTag(HtmlTextWriterTag.Br);
                writer.Write("SBI: ");
                writer.Write(outboundCorrespondenceSaved.SBI);
                writer.RenderEndTag(); //end tag br

                writer.RenderBeginTag(HtmlTextWriterTag.Br);
                writer.Write("QC Completed By: ");
                writer.Write(outboundCorrespondenceSaved.QCCompletedByName);
                writer.RenderEndTag(); //end tag br

                if (!reCheck)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Br);//h4 start tag
                    writer.Write("Date QC Completed: ");
                    writer.Write(detailsReCheck.OutboundCorrespondence.DateQCCompleted);
                    writer.RenderEndTag();//end tag br
                }
                else
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Br);//h4 start tag
                    writer.Write("Date QC Completed: ");
                    writer.Write(detailsReCheck.OutboundCorrespondenceReCheck.DateQCCompleted);
                    writer.RenderEndTag();//end tag br
                }

                //Display all Questions and Answers from initial Check and any ReChecks

                for (int x = 0; x < detailsReCheck.Answers.Count; x++)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Br);

                    writer.Write(detailsReCheck.Answers[x].Question.Text + " - ");
                    writer.Write(detailsReCheck.Answers[x].Text);

                    if (reCheck)
                    {
                        writer.RenderBeginTag(HtmlTextWriterTag.Br);
                        writer.Write("Recheck: " + detailsReCheck.ReCheckAnswers[x].Text);
                        writer.RenderEndTag();
                    }

                    writer.RenderEndTag();//end tag br
                }

                writer.RenderBeginTag(HtmlTextWriterTag.H3); //h1 tag
                writer.RenderBeginTag(HtmlTextWriterTag.P); //p tag 
                writer.Write("Comments");
                writer.RenderEndTag(); //p end tag
                writer.RenderEndTag(); //H1 end tag

                writer.RenderBeginTag(HtmlTextWriterTag.Br);
                if (String.IsNullOrEmpty(outboundCorrespondenceSaved.Comments))
                {
                    writer.Write("Comments: ");
                    writer.Write("N/A");
                }
                else
                {
                    writer.Write("Comments: ");
                    writer.Write(outboundCorrespondenceSaved.Comments);
                }
                writer.RenderEndTag(); //end tag br


                //ReCheck Section

                if (reCheck)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Br);
                    writer.Write("Recheck Result: ");
                    writer.Write(detailsReCheck.ReCheckResult);
                    writer.RenderEndTag(); //end tag br

                    writer.RenderBeginTag(HtmlTextWriterTag.Br);
                    writer.Write("Recheck Completed By: ");
                    writer.Write(detailsReCheck.OutboundCorrespondenceReCheck.ReCheckCompletedBy);
                    writer.RenderEndTag(); //end tag br

                    writer.RenderBeginTag(HtmlTextWriterTag.Br);
                    writer.Write("Recheck Comments: ");
                    if (!String.IsNullOrEmpty(detailsReCheck.OutboundCorrespondenceReCheck.ReCheckComments))
                    {
                        writer.Write(detailsReCheck.OutboundCorrespondenceReCheck.ReCheckComments);
                    }
                    else
                    {
                        writer.Write("None");
                    }
                    writer.RenderEndTag(); //end tag br
                }

            }

            var (teamMemberEmail, lineManagerEmail) = EmailHelper.GetOutgoingEmailAddresses(
                    pdb,
                    detailsReCheck.OutboundCorrespondence.PersonName,
                    detailsReCheck.OutboundCorrespondence.ManagerName
                );

            MailAddress lineManagerAddress = new MailAddress(lineManagerEmail);

            //MailAddress RPQ = new MailAddress("RPAInternalDevelopment@rpa.gov.uk"); used for debug testing IDT mailbox 

            MailMessage mail = new MailMessage("no-reply@rpa.gov.uk", teamMemberEmail);

            if (reCheck)
            {
                mail.Subject = "PRIVATE: Outgoing Correspondence Quality Re-Check : " + detailsReCheck.ReCheckResult + " | " + detailsReCheck.OutboundCorrespondence.CorrespondenceType.Text + " | " + detailsReCheck.OutboundCorrespondence.CRMRefPrefix.Text.Trim() + detailsReCheck.OutboundCorrespondence.CRMRef.Trim();
            }
            else
            {
                mail.Subject = "PRIVATE: Outgoing Correspondence Quality Check : " + detailsReCheck.QCResult + " | " + detailsReCheck.OutboundCorrespondence.CorrespondenceType.Text + " | " + detailsReCheck.OutboundCorrespondence.CRMRefPrefix.Text.Trim() + detailsReCheck.OutboundCorrespondence.CRMRef.Trim();
            }

            mail.CC.Add(lineManagerAddress);

            mail.IsBodyHtml = true;
            mail.Body = stringWriter.ToString();

            return mail;
        }

        //BANK EMAIL GENERATION
        public MailMessage GenerateBankEmail(BankAccountCheck bankAccount)
        {

            StringWriter stringWriter = new StringWriter();

            BankAccountCheck bankAccountSaved = db.BankAccounts.Include("FailReason").Where(x => x.CheckId == bankAccount.CheckId).FirstOrDefault();

            int ReCheckId = db.BankAccountReChecks.Where(c => c.BankAccountId == bankAccountSaved.CheckId && c.ReCheckActive == true).Select(v => v.CheckId).FirstOrDefault();

            bool reCheck = false;

            BankAccountDetailsReCheck bankDetails = new BankAccountDetailsReCheck
            {
                BankAccount = bankAccountSaved,
                QCResult = db.CheckResults.Where(x => x.CheckId == bankAccountSaved.CheckId).Select(p => p.Result.Text).FirstOrDefault(),
                Answers = db.Answers.Include("Question").Where(x => x.CheckId == bankAccountSaved.CheckId).OrderBy(p => p.QuestionId).ToList(),
                BankAccountReCheck = db.BankAccountReChecks.Include("FailReason").Where(x => x.CheckId == ReCheckId).FirstOrDefault() == null ? null : db.BankAccountReChecks.Include("FailReason").Where(x => x.CheckId == ReCheckId).FirstOrDefault(),
                ReCheckAnswers = db.Answers.Include("Question").Where(x => x.CheckId == ReCheckId).OrderBy(p => p.QuestionId).ToList() == null ? null : db.Answers.Where(x => x.CheckId == ReCheckId).OrderBy(p => p.QuestionId).ToList(),
                ReCheckResult = db.CheckResults.Where(x => x.CheckId == ReCheckId).Select(p => p.Result.Text).FirstOrDefault() == null ? null : db.CheckResults.Where(x => x.CheckId == ReCheckId).Select(p => p.Result.Text).FirstOrDefault(),
                Comments = db.BankAccountComments.Where(x => x.CheckId == bankAccountSaved.CheckId).ToList(),
                ReCheckComments = db.BankAccountComments.Where(x => x.CheckId == ReCheckId).ToList()
            };

            string recheckRequiredYesNo = "No";

            if (bankDetails.BankAccount.ReCheckRequired)
            {
                recheckRequiredYesNo = "Yes";
            }

            if (bankDetails.BankAccountReCheck != null)
            {
                reCheck = true;
            }

            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                writer.AddStyleAttribute("font-family", "Arial,'Segoe UI', Verdana, Helvetica, Sans-Serif"); //Set fonts for body.  You can add any CSS attribute wit this tag.
                writer.AddStyleAttribute("font-size", "12"); //sets font size
                writer.RenderBeginTag(HtmlTextWriterTag.Div);  //start tag 
                writer.RenderBeginTag(HtmlTextWriterTag.H3); //h1 tag
                writer.RenderBeginTag(HtmlTextWriterTag.P); //p tag1 
                writer.Write("Quality Check For: " + bankDetails.BankAccount.FRN);
                writer.RenderEndTag(); //p end tag
                writer.RenderEndTag(); //H1 end tag
                writer.RenderBeginTag(HtmlTextWriterTag.H4);//h4 start tag
                writer.Write("Result: ");
                writer.RenderEndTag(); //h4 end tag
                writer.RenderBeginTag(HtmlTextWriterTag.Br);
                writer.Write(bankDetails.QCResult);
                writer.RenderEndTag();//end tag br
                writer.RenderBeginTag(HtmlTextWriterTag.H4);//h4 start tag
                writer.Write("Recheck Required: ");
                writer.RenderEndTag(); //h4 end tag
                writer.RenderBeginTag(HtmlTextWriterTag.Br);
                writer.Write(recheckRequiredYesNo);
                writer.RenderEndTag();//end tag br

                writer.RenderBeginTag(HtmlTextWriterTag.Br);
                if (bankDetails.BankAccount.FailReason != null)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.H4);//h4 start tag
                    writer.Write("Not Approved Reason:");
                    writer.RenderEndTag(); //h4 end tag
                    writer.RenderBeginTag(HtmlTextWriterTag.Br);
                    writer.Write(bankDetails.BankAccount.FailReason.Text);
                    writer.RenderEndTag();//end tag br
                }
                writer.RenderEndTag();//end tag br

                writer.RenderBeginTag(HtmlTextWriterTag.H4);//h4 start tag
                writer.Write("Most Recent Recheck Result: ");
                writer.RenderEndTag(); //h4 end tag

                if (bankDetails.BankAccountReCheck == null)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Br);
                    writer.Write("N/A");
                    writer.RenderEndTag();//end tag br
                }
                else
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Br);
                    writer.Write(db.CheckResults.Where(x => x.CheckId == bankDetails.BankAccountReCheck.CheckId).Select(c => c.Result.Text).FirstOrDefault());
                    writer.RenderEndTag(); //end tag br

                    writer.RenderBeginTag(HtmlTextWriterTag.Br);
                    if (bankDetails.BankAccountReCheck.FailReason != null)
                    {
                        writer.RenderBeginTag(HtmlTextWriterTag.H4);//h4 start tag
                        writer.Write("Most Recent Recheck Not Approved Reason:");
                        writer.RenderEndTag(); //h4 end tag
                        writer.RenderBeginTag(HtmlTextWriterTag.Br);
                        writer.Write(bankDetails.BankAccountReCheck.FailReason.Text);
                        writer.RenderEndTag();//end tag br
                    }
                    writer.RenderEndTag();//end tag br
                }
                writer.RenderEndTag();//end tag br

                writer.RenderBeginTag(HtmlTextWriterTag.H3); //h1 tag
                writer.RenderBeginTag(HtmlTextWriterTag.P); //p tag 
                writer.Write("General Details");
                writer.RenderEndTag(); //p end tag
                writer.RenderEndTag(); //H1 end tag

                writer.RenderBeginTag(HtmlTextWriterTag.Br);
                writer.Write("SBI/Trader Number: ");
                writer.Write(bankDetails.BankAccount.SBI);
                writer.RenderEndTag(); //end tag br

                writer.RenderBeginTag(HtmlTextWriterTag.Br);
                writer.Write("FRN: ");
                writer.RenderEndTag();//end tag br
                writer.Write(bankDetails.BankAccount.FRN);

                writer.RenderBeginTag(HtmlTextWriterTag.Br);
                writer.Write("Type Of Check: ");
                writer.Write(bankDetails.BankAccount.CheckType);
                writer.RenderEndTag();//end tag br

                writer.RenderBeginTag(HtmlTextWriterTag.Br);
                writer.Write("Business Name: ");
                writer.Write(bankDetails.BankAccount.BusinessName);
                writer.RenderEndTag(); //end tag br

                writer.RenderBeginTag(HtmlTextWriterTag.Br);
                writer.Write("Date of Change: ");
                writer.Write(bankDetails.BankAccount.DateQCCreated);
                writer.RenderEndTag(); //end tag br

                writer.RenderBeginTag(HtmlTextWriterTag.Br);
                writer.Write("QC Completed by: ");
                writer.Write(bankDetails.BankAccount.QCCompletedByName);
                writer.RenderEndTag(); //end tag br

                writer.RenderBeginTag(HtmlTextWriterTag.H3); //h1 tag
                writer.RenderBeginTag(HtmlTextWriterTag.P); //p tag 
                writer.Write("Processing");
                writer.RenderEndTag(); //p end tag
                writer.RenderEndTag(); //H1 end tag


                if (!reCheck)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Br);//h4 start tag
                    writer.Write("Date QC Completed: ");
                    writer.Write(bankDetails.BankAccount.DateQCCompleted);
                    writer.RenderEndTag();//end tag br
                }
                else
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Br);//h4 start tag
                    writer.Write("Date Recheck QC Completed: ");
                    writer.Write(bankDetails.BankAccountReCheck.DateQCCompleted);
                    writer.RenderEndTag();//end tag br
                }

                writer.RenderBeginTag(HtmlTextWriterTag.H3); //h1 tag
                writer.RenderBeginTag(HtmlTextWriterTag.P); //p tag 
                writer.Write("Customer Verification Questions");
                writer.RenderEndTag(); //p end tag
                writer.RenderEndTag(); //H1 end tag

                //Customer Verification Questions

                for (int x = 0; x < 3; x++)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Br);
                    writer.Write(bankDetails.Answers[x].Question.Text + " - ");
                    writer.Write(bankDetails.Answers[x].Text);

                    string comment = bankDetails.Comments.Where(c => c.QuestionId == bankDetails.Answers[x].QuestionId).Select(p => p.AnswerComment).FirstOrDefault() ?? null;

                    if (comment != null)
                    {
                        writer.Write(comment);
                    }

                    if (reCheck)
                    {
                        writer.RenderBeginTag(HtmlTextWriterTag.Br);
                        writer.Write("Recheck: " + bankDetails.ReCheckAnswers[x].Text);

                        string reComment = bankDetails.ReCheckComments.Where(c => c.QuestionId == bankDetails.Answers[x].QuestionId).Select(p => p.AnswerComment).FirstOrDefault() ?? null;

                        if (reComment != null)
                        {
                            writer.Write(reComment);
                        }

                        writer.RenderEndTag();//end tag br
                    }

                    writer.RenderEndTag();//end tag br
                }

                writer.RenderBeginTag(HtmlTextWriterTag.H2); //h1 tag
                writer.RenderBeginTag(HtmlTextWriterTag.P); //p tag 
                writer.Write("Bank Account Validation");
                writer.RenderEndTag(); //p end tag
                writer.RenderEndTag(); //H1 end tag

                //Bank Account Validation Questions

                writer.RenderBeginTag(HtmlTextWriterTag.Br);
                writer.Write(bankDetails.Answers[3].Question.Text + " - ");
                writer.Write(bankDetails.Answers[3].Text);

                if (reCheck)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Br);
                    writer.Write(bankDetails.Answers[3].Text);
                    writer.RenderEndTag();//end tag br
                }
                writer.RenderEndTag();//end tag br

                writer.RenderBeginTag(HtmlTextWriterTag.H2); //h1 tag
                writer.RenderBeginTag(HtmlTextWriterTag.P); //p tag 
                writer.Write("Bank Account details/amendment to bank details");
                writer.RenderEndTag(); //p end tag
                writer.RenderEndTag(); //H1 end tag

                //Bank Account details/amendment to bank details

                for (int x = 4; x < 8; x++)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Br);
                    writer.Write(bankDetails.Answers[x].Question.Text + " - ");
                    writer.Write(bankDetails.Answers[x].Text);

                    if (reCheck)
                    {
                        writer.RenderBeginTag(HtmlTextWriterTag.Br);
                        writer.Write("Recheck: " + bankDetails.ReCheckAnswers[x].Text);
                        writer.RenderEndTag();//end tag br
                    }

                    writer.RenderEndTag();//end tag br
                }

                writer.RenderBeginTag(HtmlTextWriterTag.H3); //h1 tag
                writer.RenderBeginTag(HtmlTextWriterTag.P); //p tag 
                writer.Write("Confirmation");
                writer.RenderEndTag(); //p end tag
                writer.RenderEndTag(); //H1 end tag

                //Confirmation

                for (int x = 8; x < 11; x++)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Br);
                    writer.Write(bankDetails.Answers[x].Question.Text + " - ");
                    writer.Write(bankDetails.Answers[x].Text);

                    if (reCheck)
                    {
                        writer.RenderBeginTag(HtmlTextWriterTag.Br);
                        writer.Write("ReCheck: " + bankDetails.ReCheckAnswers[x].Text);
                        writer.RenderEndTag();//end tag br
                    }

                    writer.RenderEndTag();//end tag br
                }

                writer.RenderBeginTag(HtmlTextWriterTag.H3); //h1 tag
                writer.RenderBeginTag(HtmlTextWriterTag.P); //p tag 
                writer.Write("Comments");
                writer.RenderEndTag(); //p end tag
                writer.RenderEndTag(); //H1 end tag

                writer.RenderBeginTag(HtmlTextWriterTag.Br);
                if (bankDetails.BankAccount.Comments == null)
                {
                    writer.Write("Comments: ");
                    writer.Write("N/A");
                }
                else
                {
                    writer.Write("Comments: ");
                    writer.Write(bankDetails.BankAccount.Comments);
                }
                writer.RenderEndTag(); //end tag br

                //ReCheck Section

                if (reCheck)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Br);
                    writer.Write("Recheck Result: ");
                    writer.Write(bankDetails.ReCheckResult);
                    writer.RenderEndTag(); //end tag br

                    writer.RenderBeginTag(HtmlTextWriterTag.Br);
                    writer.Write("Recheck Completed By: ");
                    writer.Write(bankDetails.BankAccountReCheck.ReCheckCompletedBy);
                    writer.RenderEndTag(); //end tag br

                    writer.RenderBeginTag(HtmlTextWriterTag.Br);
                    writer.Write("Recheck Comments: ");
                    if (!String.IsNullOrEmpty(bankDetails.BankAccountReCheck.ReCheckComments))
                    {
                        writer.Write(bankDetails.BankAccountReCheck.ReCheckComments);
                    }
                    else
                    {
                        writer.Write("None");
                    }
                    writer.RenderEndTag(); //end tag br
                }

            }

            var (teamMemberEmail, lineManagerEmail) = EmailHelper.GetOutgoingEmailAddresses(
                    pdb,
                    bankDetails.BankAccount.PersonName,
                    bankDetails.BankAccount.ManagerName
                );

            MailAddress lineManagerAddress = new MailAddress(lineManagerEmail);

            MailMessage mail = new MailMessage("no-reply@rpa.gov.uk", teamMemberEmail);

            if (reCheck)
            {
                mail.Subject = "Bank Account Quality Recheck : Reference: : " + bankDetails.BankAccount.FRN;
            }
            else
            {
                mail.Subject = "Bank Account Quality Check : Reference: " + bankDetails.BankAccount.FRN;
            }

            mail.CC.Add(lineManagerAddress);
            mail.IsBodyHtml = true;
            mail.Body = stringWriter.ToString();


            return mail;
        }
    }
}