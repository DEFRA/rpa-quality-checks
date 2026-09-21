using RPA.QualityPortal.Models;
using System.Collections.Generic;

namespace RPA.QualityPortal.Tests.Data.Mock
{
    public static class BankAccountCommentData
    {
        public static List<BankAccountComments> Data()
        {
            return new List<BankAccountComments>
            {
                new BankAccountComments
                {
                    BankAccountCommentId = 1,
                    AnswerComment = "Comment",
                    CheckId = 53,
                    QuestionId = 18,
                    AnswerId = 72
                }
            };
        }
    }
}