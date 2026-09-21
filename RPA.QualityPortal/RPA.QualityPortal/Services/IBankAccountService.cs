using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace RPA.QualityPortal.Services
{
    public interface IBankAccountService
    {
        void SaveBankAccount(BankAccountCheck bankAccount, QuestionAnswerList questionAnswerList);

        void SaveBankAccountOverride(BankAccountCheck bankAccount);

        void SaveBankAccountReCheck(BankAccountReCheck bankAccountReCheck, QuestionAnswerList questionAnswerList);

        void SaveEditBankAccount(BankAccountCheck bankAccount, QuestionAnswerList qaList);

        void SaveEditBankAccountReCheck(BankAccountReCheck bankAccountReCheck, QuestionAnswerList qaList);

        IQueryable<BankAccountOverview> RetrieveBankAccountChecks(IQueryable<BankAccountCheck> bankAccounts);

        IQueryable<BankAccountOverview> RetrieveBankAccountRechecks(IQueryable<BankAccountReCheck> bankAccountReChecks);
    }
}
