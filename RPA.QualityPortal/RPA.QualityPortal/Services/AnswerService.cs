using RPA.QualityPortal.DAL;
using RPA.QualityPortal.Models;
using RPA.QualityPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.Services
{
    public class AnswerService : IAnswerService
    {
        IQualityContext db;

        public AnswerService(IQualityContext context)
        {
            this.db = context;
        }

        public void AddAnswers(QuestionAnswerList qaList, int checkId)
        {
            List<Answer> answers = new List<Answer>();

            foreach (var questionAnswer in qaList.QuestionAnswers)
            {
                Answer answer = new Answer
                {
                    CheckId = checkId,
                    QuestionId = questionAnswer.CheckQuestion.Question.QuestionId,
                    Text = questionAnswer.Answer
                };
                answers.Add(answer);
            }

            foreach (var qcAnswer in answers)
            {
                db.Answers.Add(qcAnswer);
            }
        }

        public void EditAnswers(QuestionAnswerList qaList, int checkId)
        {
            foreach (var questionAnswer in qaList.QuestionAnswers)
            {
                Answer answer = db.Answers.Where(x => x.CheckId == checkId && x.QuestionId == questionAnswer.CheckQuestion.Question.QuestionId).FirstOrDefault();
                {
                    if (answer.Text != questionAnswer.Answer)
                    {
                        answer.Text = questionAnswer.Answer;
                        db.SetModified(answer);
                    }
                }
            }
        }

        public void BankComments(QuestionAnswerList qaList, int checkId)
        {
            List<BankAccountComments> comments = new List<BankAccountComments>();

            foreach (var questionAnswer in qaList.QuestionAnswers)
            {
                if (questionAnswer.Comment != null)
                {
                    BankAccountComments comment = new BankAccountComments
                    {
                        AnswerComment = CheckComment(questionAnswer.Comment),
                        AnswerId = db.Answers.Where(x => x.CheckId == checkId && x.QuestionId == questionAnswer.CheckQuestion.Question.QuestionId).Select(p => p.AnswerId).FirstOrDefault(),
                        CheckId = checkId,
                        QuestionId = questionAnswer.CheckQuestion.Question.QuestionId
                    };
                    comments.Add(comment);
                }
            }

            foreach (var bankComment in comments)
            {
                db.BankAccountComments.Add(bankComment);
            }
        }

        public void EditBankComments(QuestionAnswerList qaList, int checkId)
        {
            foreach (var questionAnswer in qaList.QuestionAnswers)
            {
                BankAccountComments comment = db.BankAccountComments.Where(x => x.CheckId == checkId && x.QuestionId == questionAnswer.CheckQuestion.Question.QuestionId).FirstOrDefault();

                if (comment != null)
                {
                    if (comment.AnswerComment != questionAnswer.Comment)
                    {
                        comment.AnswerComment = questionAnswer.Comment;

                        if (comment.AnswerComment == null)
                        {
                            db.BankAccountComments.Remove(comment);
                        }
                        else
                        {
                            comment.AnswerComment = CheckComment(questionAnswer.Comment);

                            db.SetModified(comment);
                        }
                    }
                }
                else
                {
                    if (questionAnswer.Comment != null)
                    {
                        BankAccountComments bankComment = new BankAccountComments
                        {
                            AnswerComment = CheckComment(questionAnswer.Comment),
                            QuestionId = db.Questions.Where(x => x.QuestionId == questionAnswer.CheckQuestion.Question.QuestionId).Select(p => p.QuestionId).FirstOrDefault(),
                            AnswerId = db.Answers.Where(x => x.CheckId == checkId && x.QuestionId == questionAnswer.CheckQuestion.Question.QuestionId).Select(p => p.AnswerId).FirstOrDefault(),
                            CheckId = checkId
                        };

                        db.BankAccountComments.Add(bankComment);
                    }
                }
            }
        }

        public string CheckComment(string comments)
        {
            string comment = comments;

            if (comments.Contains("\""))
            {
                comment = comments.Replace('\"', '\'');
            }

            return comment;
        }
    }
}