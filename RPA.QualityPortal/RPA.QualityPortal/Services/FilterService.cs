using RPA.QualityPortal.DAL;
using RPA.QualityPortal.Helpers;
using RPA.QualityPortal.Models.CheckTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RPA.QualityPortal.Services
{
    public class FilterService : IFilterService
    {
        IQualityContext db;
        IPeopleContext pdb;
        IUserHelper userHelper;
        IRoleManager roleManager;

        public FilterService(IQualityContext context, IPeopleContext peopleContext, IUserHelper userHelper, IRoleManager roleManager)
        {
            this.db = context;
            this.pdb = peopleContext;
            this.userHelper = userHelper;
            this.roleManager = roleManager;
        }

        public IQueryable<OutboundCorrespondence> OutboundCorrespondenceFilterChecks(string searchString, string pageName)
        {
            DateTime sixteenMonths = DateTime.Now.AddMonths(-16);
            string user = userHelper.CurrentUser();

            IQueryable<OutboundCorrespondence> outboundCorrespondence = db.OutboundCorrespondence;

            if (roleManager.IsUserInRole("Quality Checks: OC Team Member"))
            {
                outboundCorrespondence = outboundCorrespondence.Where(x => x.PersonName == user);
            }

            if (pageName == "All Quality Checks")
            {
                outboundCorrespondence = outboundCorrespondence.Where(x => x.DateQCCompleted > sixteenMonths);
            }
            else if (pageName == "Complete Checks")
            {
                outboundCorrespondence = outboundCorrespondence.Where(x => x.ReCheckRequired == false && x.DateQCCompleted > sixteenMonths);
            }
            else
            {
                outboundCorrespondence = outboundCorrespondence.Where(x => x.ReCheckRequired == true);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                outboundCorrespondence = outboundCorrespondence.Where(x => x.PersonName.ToLower().Contains(searchString.ToLower()) ||
                                                                            x.ManagerName.ToLower().Contains(searchString.ToLower()) ||
                                                                            x.QCCompletedByName.ToLower().Contains(searchString.ToLower()) ||
                                                                            x.SBI.ToString().ToLower().Contains(searchString.ToLower()) ||
                                                                            (x.CRMRefPrefix.Text + x.CRMRef).ToLower().Contains(searchString.ToLower()) ||
                                                                            (x.UniqueIdentifierPrefix.Text + x.UniqueId).ToLower().Contains(searchString.ToLower()));
            }

            return outboundCorrespondence.OrderByDescending(x => x.DateQCCompleted).ThenBy(x => x.PersonName);
        }

        public IQueryable<BankAccountCheck> BankAccountFilterChecks(string searchString, string dateFrom, string dateTo, string pageName, bool? coachingPoint)
        {
            IQueryable<BankAccountCheck> bankAccounts = db.BankAccounts;

            if (pageName == "Complete Checks")
            {
                bankAccounts = bankAccounts.Where(x => x.ReCheckRequired == false);
            }
            else if (pageName == "Outstanding Checks")
            {
                bankAccounts = bankAccounts.Where(x => x.ReCheckRequired == true || x.DateQCCompleted == null);
            }

            if (!(string.IsNullOrEmpty(dateFrom)) || !(string.IsNullOrEmpty(dateTo)))
            {
                bankAccounts = BankAccountDateSearch(bankAccounts, dateFrom, dateTo);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                string searchStringLower = searchString.ToLower();

                bankAccounts = bankAccounts.Where(x => x.PersonName.ToLower().Contains(searchStringLower) ||
                                                                            x.ManagerName.ToLower().Contains(searchStringLower) ||
                                                                            x.QCCompletedByName != null && x.QCCompletedByName.ToLower().Contains(searchStringLower) ||
                                                                            x.FailReason != null && x.FailReason.Text.ToLower().Contains(searchStringLower) ||
                                                                            (x.FRN.ToString().ToLower().Contains(searchStringLower)));
            }

            if (coachingPoint == true)
            {
                bankAccounts = bankAccounts.Where(x => x.CoachingPoint == true);
            }
            else if (coachingPoint == false)
            {
                bankAccounts = bankAccounts.Where(x => x.CoachingPoint == false);
            }
            return bankAccounts;
        }

    private static IQueryable<BankAccountCheck> BankAccountDateSearch(IQueryable<BankAccountCheck> bankAccounts, string dateFrom, string dateTo)
    {
        if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
        {
            DateTime from = DateTime.Parse(dateFrom);
            DateTime to = DateTime.Parse(dateTo);

            bankAccounts = bankAccounts.Where(x => x.DateQCCompleted != null && x.DateQCCompleted >= from && x.DateQCCompleted <= to);
        }
        else if (!string.IsNullOrEmpty(dateFrom))
        {
            DateTime from = DateTime.Parse(dateFrom);

            bankAccounts = bankAccounts.Where(x => x.DateQCCompleted != null && x.DateQCCompleted >= from);
        }
        else if (!string.IsNullOrEmpty(dateTo))
        {
            DateTime to = DateTime.Parse(dateTo);

            bankAccounts = bankAccounts.Where(x => x.DateQCCompleted != null && x.DateQCCompleted <= to);
        }

        return bankAccounts.OrderBy(x => x.DateQCCompleted);
    }

    public List<OutboundCorrespondence> MIfilterChecks()
    {
        DateTime sixteenMonths = DateTime.Now.AddMonths(-16);

        string user = userHelper.CurrentUser();


        List<OutboundCorrespondence> outboundCorrespondences;

        //filter service get outbound corr
        if (roleManager.IsUserInRole("Quality Checks: OC Team Member"))
        {
            outboundCorrespondences = db.OutboundCorrespondence.Where(x => x.PersonName == user && x.ExcludeQCResult == false && x.DateQCCompleted > sixteenMonths).ToList();
        }
        else
        {
            outboundCorrespondences = db.OutboundCorrespondence.Where(x => x.ExcludeQCResult == false && x.DateQCCompleted > sixteenMonths).ToList();
        }

        return outboundCorrespondences;
    }

        public List<int> GetValidCheckIDs()
        {

            DateTime sixteenMonths = DateTime.Now.AddMonths(-16);
            List<int> validCheckIDs = db.OutboundCorrespondence.Where(x => x.ExcludeQCResult == false && x.DateQCCompleted > sixteenMonths).Select(p => p.CheckId).ToList();

            return validCheckIDs;
        }
}
}
