using RPA.QualityPortal.ViewModels;

namespace RPA.QualityPortal.Services
{
    public interface IAnswerService
    {
        void AddAnswers(QuestionAnswerList qaList, int checkId);

        void EditAnswers(QuestionAnswerList qaList, int checkId);

        void BankComments(QuestionAnswerList qaList, int checkId);

        void EditBankComments(QuestionAnswerList qaList, int checkId);

        string CheckComment(string comments);
    }
}