using RPA.QualityPortal.DAL;
using RPA.QualityPortal.Helpers;
using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.ViewModels.OutboundCorrespondenceVM;
using RPA.QualityPortal.ViewModels.OutboundCorrespondenceVM.MI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace RPA.QualityPortal.Services
{
    public class MIService : IMIService
    {
        IQualityContext db;
        IPeopleContext pdb;
        IUserHelper userHelper;
        IFilterService filterService;

        public MIService(IQualityContext context, IPeopleContext peopleContext, IUserHelper userHelper, IFilterService filterService)
        {
            this.db = context;
            this.pdb = peopleContext;
            this.userHelper = userHelper;
            this.filterService = filterService;
        }

        public OutboundCorrespondenceMI OutboundCorrespondenceMISearch(string searchString, string searchOption, string dateFrom, string dateTo, int? SchemeId, int? BusinessAreaId, int? CorrespondenceTypeId, OutboundCorrespondenceMI outboundCorrespondenceMI)
        {
            outboundCorrespondenceMI = OutboundCorrespondenceDateSearch(outboundCorrespondenceMI, dateFrom, dateTo);

            switch (searchOption)
            {

                //search on person QC'd
                case "QCName":

                    outboundCorrespondenceMI.OutboundCorrespondenceList = outboundCorrespondenceMI.OutboundCorrespondenceList.Where(x => x.PersonName == searchString && x.ExcludeQCResult == false).ToList();
                    outboundCorrespondenceMI.OutboundCorrespondenceMIPersonStatsList = OutboundCorrespondencePersonStats(outboundCorrespondenceMI, "");
                    outboundCorrespondenceMI.SearchType = "QC Name: " + searchString;
                    outboundCorrespondenceMI.ResultHeader = "Individual";
                    break;

                //search by Line Manager

                case "LineManager":

                    outboundCorrespondenceMI.OutboundCorrespondenceList = outboundCorrespondenceMI.OutboundCorrespondenceList.Where(x => x.ManagerName == searchString && x.ExcludeQCResult == false).ToList();

                    //collate stats for each member of staff under that line manager

                    outboundCorrespondenceMI.OutboundCorrespondenceMIPersonStatsList = OutboundCorrespondencePersonStats(outboundCorrespondenceMI, "Line");

                    outboundCorrespondenceMI.SearchType = "Line Manager Name: " + searchString;

                    outboundCorrespondenceMI.ResultHeader = " on Staff Managed By: " + searchString;

                    break;

                //Search by QC'er

                case "QCChecker":

                    outboundCorrespondenceMI.OutboundCorrespondenceList = outboundCorrespondenceMI.OutboundCorrespondenceList.Where(x => x.QCCompletedByName == searchString && x.ExcludeQCResult == false).ToList();

                    outboundCorrespondenceMI.OutboundCorrespondenceMIPersonStatsList = OutboundCorrespondencePersonStats(outboundCorrespondenceMI, "QC");

                    outboundCorrespondenceMI.SearchType = "QC Checker Name: " + searchString;

                    outboundCorrespondenceMI.ResultHeader = " Staff QC'd By: " + searchString;

                    break;

                //Search by HEO

                case "HEO":

                    outboundCorrespondenceMI.OutboundCorrespondenceList = outboundCorrespondenceMI.OutboundCorrespondenceList.Where(x => x.HEO == searchString && x.ExcludeQCResult == false).ToList();

                    outboundCorrespondenceMI.OutboundCorrespondenceMIPersonStatsList = OutboundCorrespondencePersonStats(outboundCorrespondenceMI, "HEO");

                    outboundCorrespondenceMI.SearchType = "HEO: " + searchString;

                    outboundCorrespondenceMI.ResultHeader = " Line Managers under HEO: " + searchString;

                    break;

                //Search by SEO

                case "SEO":

                    outboundCorrespondenceMI.OutboundCorrespondenceList = outboundCorrespondenceMI.OutboundCorrespondenceList.Where(x => x.SEO == searchString && x.ExcludeQCResult == false).ToList();

                    outboundCorrespondenceMI.OutboundCorrespondenceMIPersonStatsList = OutboundCorrespondencePersonStats(outboundCorrespondenceMI, "SEO");

                    outboundCorrespondenceMI.SearchType = "SEO: " + searchString;

                    outboundCorrespondenceMI.ResultHeader = " HEO's under SEO: " + searchString;

                    break;

                case "Scheme":

                    string schemeText = db.Schemes.Where(x => x.SchemeId == SchemeId).Select(p => p.Text).FirstOrDefault();
                    outboundCorrespondenceMI.OutboundCorrespondenceList = outboundCorrespondenceMI.OutboundCorrespondenceList.Where(x => x.Scheme.Text == schemeText && x.ExcludeQCResult == false).ToList();
                    outboundCorrespondenceMI.SearchType = searchString;
                    outboundCorrespondenceMI.ResultHeader = searchOption + " : " + schemeText;

                    break;

                case "BusinessArea":

                    string businessText = db.BusinessAreas.Where(x => x.BusinessAreaId == BusinessAreaId).Select(p => p.Text).FirstOrDefault();
                    outboundCorrespondenceMI.OutboundCorrespondenceList = outboundCorrespondenceMI.OutboundCorrespondenceList.Where(x => x.BusinessArea.Text == businessText && x.ExcludeQCResult == false).ToList();
                    outboundCorrespondenceMI.SearchType = searchString;
                    outboundCorrespondenceMI.ResultHeader = searchOption + " Area : " + businessText;

                    break;

                case "CorrespondenceType":

                    string correspondenceText = db.CorrespondenceTypes.Where(x => x.CorrespondenceTypeId == CorrespondenceTypeId).Select(p => p.Text).FirstOrDefault();
                    outboundCorrespondenceMI.OutboundCorrespondenceList = outboundCorrespondenceMI.OutboundCorrespondenceList.Where(x => x.CorrespondenceType.Text == correspondenceText && x.ExcludeQCResult == false).ToList();
                    outboundCorrespondenceMI.SearchType = searchString;
                    outboundCorrespondenceMI.ResultHeader = searchOption + " Type : " + correspondenceText;

                    break;

                //Search by only Date

                default:
                    outboundCorrespondenceMI.SearchType = "By Date Only";

                    break;
            }

            //get all rechecks in relation to the checks found

            List<OutboundCorrespondenceReCheck> outboundCorrespondenceReChecks = GetAllReChecks(outboundCorrespondenceMI.OutboundCorrespondenceList);

            outboundCorrespondenceMI.OutboundCorrespondenceReCheckList = outboundCorrespondenceReChecks;

            //collate MI based on the list found

            outboundCorrespondenceMI = CollateMI(outboundCorrespondenceMI);

            return outboundCorrespondenceMI;
        }


        public OutboundCorrespondenceMI OutboundCorrespondenceDateSearch(OutboundCorrespondenceMI outboundCorrespondenceMI, string dateFrom, string dateTo)
        {
            List<OutboundCorrespondence> outboundCorrespondences = filterService.MIfilterChecks();

            if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
            {
                DateTime from = DateTime.Parse(dateFrom);
                DateTime to = DateTime.Parse(dateTo);

                outboundCorrespondences = outboundCorrespondences.Where(x => x.DateQCCompleted.Value.Date >= from && x.DateQCCompleted.Value.Date <= to).ToList();

                outboundCorrespondenceMI.DateTo = to;
                outboundCorrespondenceMI.DateFrom = from;

                outboundCorrespondenceMI.OutboundCorrespondenceList = outboundCorrespondences;
            }
            else if (!string.IsNullOrEmpty(dateFrom))
            {
                DateTime from = DateTime.Parse(dateFrom);

                outboundCorrespondences = outboundCorrespondences.Where(x => x.DateQCCompleted.Value.Date >= from).ToList();

                outboundCorrespondenceMI.DateFrom = from;

                outboundCorrespondenceMI.OutboundCorrespondenceList = outboundCorrespondences;
            }
            else if (!string.IsNullOrEmpty(dateTo))
            {
                DateTime to = DateTime.Parse(dateTo);

                outboundCorrespondences = outboundCorrespondences.Where(x => x.DateQCCompleted.Value.Date <= to).ToList();

                outboundCorrespondenceMI.DateTo = to;

                outboundCorrespondenceMI.OutboundCorrespondenceList = outboundCorrespondences;
            }
            else
            {


                outboundCorrespondenceMI.OutboundCorrespondenceList = outboundCorrespondences;
            }


            return outboundCorrespondenceMI;
        }

        public List<OutboundCorrespondenceMIPersonStats> OutboundCorrespondencePersonStats(OutboundCorrespondenceMI outboundCorrespondenceMI, string searchType)
        {
            List<OutboundCorrespondenceMIPersonStats> personStats = new List<OutboundCorrespondenceMIPersonStats>();

            List<OutboundCorrespondence> workingGroup = new List<OutboundCorrespondence>();

            if (searchType == "HEO")
            {
                workingGroup = outboundCorrespondenceMI.OutboundCorrespondenceList.GroupBy(x => x.ManagerName).Select(x => x.First()).Distinct().ToList();
            }
            else if (searchType == "SEO")
            {
                workingGroup = outboundCorrespondenceMI.OutboundCorrespondenceList.GroupBy(x => x.HEO).Select(x => x.First()).Distinct().ToList();
            }
            else
            {
                workingGroup = outboundCorrespondenceMI.OutboundCorrespondenceList.GroupBy(x => x.PersonName).Select(x => x.First()).Distinct().ToList();
            }

            foreach (OutboundCorrespondence outboundCorrespondence in workingGroup)
            {
                OutboundCorrespondenceMIPersonStats miPerson = new OutboundCorrespondenceMIPersonStats
                {
                    OutboundCorrespondenceMIStats = new OutboundCorrespondenceMIStats()
                };

                List<OutboundCorrespondence> checkstoCheck = new List<OutboundCorrespondence>();

                if (searchType == "HEO")
                {
                    checkstoCheck = outboundCorrespondenceMI.OutboundCorrespondenceList.Where(x => x.ManagerName == outboundCorrespondence.ManagerName).ToList();
                }
                else if (searchType == "SEO")
                {
                    checkstoCheck = outboundCorrespondenceMI.OutboundCorrespondenceList.Where(x => x.HEO == outboundCorrespondence.HEO).ToList();
                }
                else
                {
                    checkstoCheck = outboundCorrespondenceMI.OutboundCorrespondenceList.Where(x => x.PersonName == outboundCorrespondence.PersonName).ToList();
                }

                List<OutboundCorrespondenceReCheck> reCheckstoCheck = GetAllReChecks(checkstoCheck);


                int outboundCorrespondenceCount = checkstoCheck.Count();
                int outboundCorrespondencePass = 0;
                int outboundCorrespondencePassAd = 0;
                int outboundCorrespondenceFail = 0;

                foreach (var check in checkstoCheck)
                {
                    if (db.CheckResults.Where(x => x.CheckId == check.CheckId).Select(c => c.Result.Text).FirstOrDefault() == "Approved")
                    {
                        outboundCorrespondencePass = outboundCorrespondencePass + 1;
                    }
                    else if (db.CheckResults.Where(x => x.CheckId == check.CheckId).Select(c => c.Result.Text).FirstOrDefault() == "Approved Advisory")
                    {
                        outboundCorrespondencePassAd = outboundCorrespondencePassAd + 1;
                    }
                    else
                    {
                        outboundCorrespondenceFail = outboundCorrespondenceFail + 1;
                    }
                }

                miPerson.OutboundCorrespondenceMIStats.TotalQcs = outboundCorrespondenceCount;
                miPerson.OutboundCorrespondenceMIStats.Passed = outboundCorrespondencePass;
                miPerson.OutboundCorrespondenceMIStats.PassedPercentage = outboundCorrespondenceCount == 0 ? 0 : decimal.Divide(outboundCorrespondencePass, outboundCorrespondenceCount) * 100;
                miPerson.OutboundCorrespondenceMIStats.PassAdvisory = outboundCorrespondencePassAd;
                miPerson.OutboundCorrespondenceMIStats.PassedAdvisoryPercentage = outboundCorrespondenceCount == 0 ? 0 : decimal.Divide(outboundCorrespondencePassAd, outboundCorrespondenceCount) * 100;
                miPerson.OutboundCorrespondenceMIStats.Failed = outboundCorrespondenceFail;
                miPerson.OutboundCorrespondenceMIStats.FailedPercentage = outboundCorrespondenceCount == 0 ? 0 : decimal.Divide(outboundCorrespondenceFail, outboundCorrespondenceCount) * 100;

                int outboundCorrespondenceReCheckCount = reCheckstoCheck.Count();
                int outboundCorrespondenceReChecksRequired = checkstoCheck.Where(x => x.InitialReCheckDecision == true).Count();
                int outboundCorrespondenceReChecksNotRequired = checkstoCheck.Where(x => x.InitialReCheckDecision == false).Count();
                int outboundCorrespondenceReCheckPass = 0;
                int outboundCorrespondenceReCheckPassAd = 0;
                int outboundCorrespondenceReCheckFail = 0;

                foreach (var check in reCheckstoCheck)
                {
                    if (db.CheckResults.Where(x => x.CheckId == check.CheckId).Select(c => c.Result.Text).FirstOrDefault() == "Approved")
                    {
                        outboundCorrespondenceReCheckPass = outboundCorrespondenceReCheckPass + 1;
                    }
                    else if (db.CheckResults.Where(x => x.CheckId == check.CheckId).Select(c => c.Result.Text).FirstOrDefault() == "Approved Advisory")
                    {
                        outboundCorrespondenceReCheckPassAd = outboundCorrespondenceReCheckPassAd + 1;
                    }
                    else
                    {
                        outboundCorrespondenceReCheckFail = outboundCorrespondenceReCheckFail + 1;
                    }
                }

                List<OutboundCorrespondence> reCheckList = new List<OutboundCorrespondence>();

                if (searchType == "SEO")
                {
                    reCheckList = outboundCorrespondenceMI.OutboundCorrespondenceList.Where(x => x.ReCheckRequired == true && x.HEO == outboundCorrespondence.HEO).ToList();
                }
                else if (searchType == "HEO")
                {
                    reCheckList = outboundCorrespondenceMI.OutboundCorrespondenceList.Where(x => x.ReCheckRequired == true && x.ManagerName == outboundCorrespondence.ManagerName).ToList();
                }
                else
                {
                    reCheckList = outboundCorrespondenceMI.OutboundCorrespondenceList.Where(x => x.ReCheckRequired == true && x.PersonName == outboundCorrespondence.PersonName).ToList();
                }

                foreach (OutboundCorrespondence outboundCorrespondenceCheck in reCheckList)
                {
                    int recCheckcount = reCheckstoCheck.Where(x => x.OutboundCorrespondenceId == outboundCorrespondenceCheck.CheckId).Count();

                    if (recCheckcount == 0)
                    {

                        if (outboundCorrespondenceCheck.DateQCCompleted > DateTime.Now.AddDays(-6))
                        {
                            miPerson.OutboundCorrespondenceMIStats.ReCheckOutstandingZeroToFive = miPerson.OutboundCorrespondenceMIStats.ReCheckOutstandingZeroToFive + 1;
                        }
                        else if (outboundCorrespondenceCheck.DateQCCompleted > DateTime.Now.AddDays(-11) && outboundCorrespondenceCheck.DateQCCompleted < DateTime.Now.AddDays(-6))
                        {
                            miPerson.OutboundCorrespondenceMIStats.ReCheckOutstandingSixToTen = miPerson.OutboundCorrespondenceMIStats.ReCheckOutstandingSixToTen + 1;
                        }
                        else
                        {
                            miPerson.OutboundCorrespondenceMIStats.ReCheckOutstandingGreaterThanTen = miPerson.OutboundCorrespondenceMIStats.ReCheckOutstandingGreaterThanTen + 1;
                        }
                    }
                    else if (recCheckcount > 0)
                    {
                        OutboundCorrespondenceReCheck latestReCheck = reCheckstoCheck.Where(x => x.OutboundCorrespondenceId == outboundCorrespondenceCheck.CheckId).OrderBy(p => p.CheckId).LastOrDefault();

                        if (latestReCheck.DateQCCompleted > DateTime.Now.AddDays(-6))
                        {
                            miPerson.OutboundCorrespondenceMIStats.ReCheckOutstandingZeroToFive = miPerson.OutboundCorrespondenceMIStats.ReCheckOutstandingZeroToFive + 1;
                        }
                        else if (latestReCheck.DateQCCompleted > DateTime.Now.AddDays(-11) && latestReCheck.DateQCCompleted < DateTime.Now.AddDays(-6))
                        {
                            miPerson.OutboundCorrespondenceMIStats.ReCheckOutstandingSixToTen = miPerson.OutboundCorrespondenceMIStats.ReCheckOutstandingSixToTen + 1;
                        }
                        else
                        {
                            miPerson.OutboundCorrespondenceMIStats.ReCheckOutstandingGreaterThanTen = miPerson.OutboundCorrespondenceMIStats.ReCheckOutstandingGreaterThanTen + 1;
                        }
                    }
                }

                miPerson.OutboundCorrespondenceMIStats.ReChecks = outboundCorrespondenceReCheckCount;
                miPerson.OutboundCorrespondenceMIStats.ReChecksRequired = outboundCorrespondenceReChecksRequired;
                miPerson.OutboundCorrespondenceMIStats.ReChecksNotRequired = outboundCorrespondenceReChecksNotRequired;
                miPerson.OutboundCorrespondenceMIStats.ReCheckPassed = outboundCorrespondenceReCheckPass;
                miPerson.OutboundCorrespondenceMIStats.ReCheckPassedPercentage = outboundCorrespondenceReCheckCount == 0 ? 0 : decimal.Divide(outboundCorrespondenceReCheckPass, outboundCorrespondenceReCheckCount) * 100;
                miPerson.OutboundCorrespondenceMIStats.ReCheckPassAdvisory = outboundCorrespondenceReCheckPassAd;
                miPerson.OutboundCorrespondenceMIStats.ReCheckPassedAdvisoryPercentage = outboundCorrespondenceReCheckCount == 0 ? 0 : decimal.Divide(outboundCorrespondenceReCheckPassAd, outboundCorrespondenceReCheckCount) * 100;
                miPerson.OutboundCorrespondenceMIStats.ReCheckFailed = outboundCorrespondenceReCheckFail;
                miPerson.OutboundCorrespondenceMIStats.ReCheckFailedPercentage = outboundCorrespondenceReCheckCount == 0 ? 0 : decimal.Divide(outboundCorrespondenceReCheckFail, outboundCorrespondenceReCheckCount) * 100;
                miPerson.OutboundCorrespondenceMIStats.ReChecksOutstanding = miPerson.OutboundCorrespondenceMIStats.ReCheckOutstandingZeroToFive + miPerson.OutboundCorrespondenceMIStats.ReCheckOutstandingSixToTen + miPerson.OutboundCorrespondenceMIStats.ReCheckOutstandingGreaterThanTen;
                miPerson.PersonName = outboundCorrespondence.PersonName;
                miPerson.LineManagerName = outboundCorrespondence.ManagerName;
                miPerson.HEOName = outboundCorrespondence.HEO;
                miPerson.SEOName = outboundCorrespondence.SEO;
                miPerson.OutboundCorrespondenceMIStats = miPerson.OutboundCorrespondenceMIStats;

                personStats.Add(miPerson);
            }

            return personStats;
        }

        public OutboundCorrespondenceMI CollateMI(OutboundCorrespondenceMI outboundCorrespondenceMI)
        {
            OutboundCorrespondenceMIStats miStats = new OutboundCorrespondenceMIStats();

            outboundCorrespondenceMI.OutboundCorrespondenceMIStats = miStats;

            int outboundCorrespondenceCount = outboundCorrespondenceMI.OutboundCorrespondenceList.Count();
            int outboundCorrespondencePass = 0;
            int outboundCorrespondenceCheckPassAd = 0;
            int outboundCorrespondenceCheckFail = 0;
            int outboundCorrespondenceReCheckRequired = outboundCorrespondenceMI.OutboundCorrespondenceList.Where(x => x.InitialReCheckDecision == true).Count();
            int outboundCorrespondenceReCheckNotRequired = 0;

            foreach (var check in outboundCorrespondenceMI.OutboundCorrespondenceList)
            {
                if (db.CheckResults.Where(x => x.CheckId == check.CheckId).Select(c => c.Result.Text).FirstOrDefault() == "Approved")
                {
                    outboundCorrespondencePass = outboundCorrespondencePass + 1;
                }
                else if (db.CheckResults.Where(x => x.CheckId == check.CheckId).Select(c => c.Result.Text).FirstOrDefault() == "Approved Advisory")
                {
                    outboundCorrespondenceCheckPassAd = outboundCorrespondenceCheckPassAd + 1;
                }
                else
                {
                    outboundCorrespondenceCheckFail = outboundCorrespondenceCheckFail + 1;

                    if (check.InitialReCheckDecision == false)
                    {
                        outboundCorrespondenceReCheckNotRequired = outboundCorrespondenceReCheckNotRequired + 1;
                    }
                }
            }

            outboundCorrespondenceMI.OutboundCorrespondenceMIStats.TotalQcs = outboundCorrespondenceCount;
            outboundCorrespondenceMI.OutboundCorrespondenceMIStats.Passed = outboundCorrespondencePass;
            outboundCorrespondenceMI.OutboundCorrespondenceMIStats.PassedPercentage = outboundCorrespondenceCount == 0 ? 0 : decimal.Divide(outboundCorrespondencePass, outboundCorrespondenceCount) * 100;
            outboundCorrespondenceMI.OutboundCorrespondenceMIStats.PassAdvisory = outboundCorrespondenceCheckPassAd;
            outboundCorrespondenceMI.OutboundCorrespondenceMIStats.PassedAdvisoryPercentage = outboundCorrespondenceCount == 0 ? 0 : decimal.Divide(outboundCorrespondenceCheckPassAd, outboundCorrespondenceCount) * 100;
            outboundCorrespondenceMI.OutboundCorrespondenceMIStats.Failed = outboundCorrespondenceCheckFail;
            outboundCorrespondenceMI.OutboundCorrespondenceMIStats.FailedPercentage = outboundCorrespondenceCount == 0 ? 0 : decimal.Divide(outboundCorrespondenceCheckFail, outboundCorrespondenceCount) * 100;


            int outcorCheckReCheckCount = outboundCorrespondenceMI.OutboundCorrespondenceReCheckList.Count();
            int outboundCorrespondenceReCheckPass = 0;
            int outboundCorrespondenceReCheckPassAd = 0;
            int outboundCorrespondenceReCheckFail = 0;

            foreach (var check in outboundCorrespondenceMI.OutboundCorrespondenceReCheckList)
            {
                if (db.CheckResults.Where(x => x.CheckId == check.CheckId).Select(c => c.Result.Text).FirstOrDefault() == "Approved")
                {
                    outboundCorrespondenceReCheckPass = outboundCorrespondenceReCheckPass + 1;
                }
                else if (db.CheckResults.Where(x => x.CheckId == check.CheckId).Select(c => c.Result.Text).FirstOrDefault() == "Approved Advisory")
                {
                    outboundCorrespondenceReCheckPassAd = outboundCorrespondenceReCheckPassAd + 1;
                }
                else
                {
                    outboundCorrespondenceReCheckFail = outboundCorrespondenceReCheckFail + 1;
                }
            }

            foreach (OutboundCorrespondence outboundCorrespondence in outboundCorrespondenceMI.OutboundCorrespondenceList.Where(x => x.ReCheckRequired == true))
            {
                int recCheckcount = outboundCorrespondenceMI.OutboundCorrespondenceReCheckList.Where(x => x.OutboundCorrespondenceId == outboundCorrespondence.CheckId).Count();

                if (recCheckcount == 0)
                {
                    OutboundCorrespondenceReCheck reCheck = outboundCorrespondenceMI.OutboundCorrespondenceReCheckList.Where(x => x.OutboundCorrespondenceId == outboundCorrespondence.CheckId).FirstOrDefault();

                    if (outboundCorrespondence.DateQCCompleted > DateTime.Now.AddDays(-6))
                    {
                        outboundCorrespondenceMI.OutboundCorrespondenceMIStats.ReCheckOutstandingZeroToFive = outboundCorrespondenceMI.OutboundCorrespondenceMIStats.ReCheckOutstandingZeroToFive + 1;
                    }
                    else if (outboundCorrespondence.DateQCCompleted > DateTime.Now.AddDays(-11) && outboundCorrespondence.DateQCCompleted < DateTime.Now.AddDays(-6))
                    {
                        outboundCorrespondenceMI.OutboundCorrespondenceMIStats.ReCheckOutstandingSixToTen = outboundCorrespondenceMI.OutboundCorrespondenceMIStats.ReCheckOutstandingSixToTen + 1;
                    }
                    else
                    {
                        outboundCorrespondenceMI.OutboundCorrespondenceMIStats.ReCheckOutstandingGreaterThanTen = outboundCorrespondenceMI.OutboundCorrespondenceMIStats.ReCheckOutstandingGreaterThanTen + 1;
                    }
                }
                else if (recCheckcount > 0)
                {
                    OutboundCorrespondenceReCheck latestReCheck = outboundCorrespondenceMI.OutboundCorrespondenceReCheckList.Where(x => x.OutboundCorrespondenceId == outboundCorrespondence.CheckId).OrderBy(p => p.CheckId).LastOrDefault();

                    if (latestReCheck.DateQCCompleted > DateTime.Now.AddDays(-6))
                    {
                        outboundCorrespondenceMI.OutboundCorrespondenceMIStats.ReCheckOutstandingZeroToFive = outboundCorrespondenceMI.OutboundCorrespondenceMIStats.ReCheckOutstandingZeroToFive + 1;
                    }
                    else if (latestReCheck.DateQCCompleted > DateTime.Now.AddDays(-11) && latestReCheck.DateQCCompleted < DateTime.Now.AddDays(-6))
                    {
                        outboundCorrespondenceMI.OutboundCorrespondenceMIStats.ReCheckOutstandingSixToTen = outboundCorrespondenceMI.OutboundCorrespondenceMIStats.ReCheckOutstandingSixToTen + 1;
                    }
                    else
                    {
                        outboundCorrespondenceMI.OutboundCorrespondenceMIStats.ReCheckOutstandingGreaterThanTen = outboundCorrespondenceMI.OutboundCorrespondenceMIStats.ReCheckOutstandingGreaterThanTen + 1;
                    }
                }
            }


            outboundCorrespondenceMI.OutboundCorrespondenceMIStats.ReChecks = outcorCheckReCheckCount;
            outboundCorrespondenceMI.OutboundCorrespondenceMIStats.ReCheckPassed = outboundCorrespondenceReCheckPass;
            outboundCorrespondenceMI.OutboundCorrespondenceMIStats.ReCheckPassedPercentage = outcorCheckReCheckCount == 0 ? 0 : decimal.Divide(outboundCorrespondenceReCheckPass, outcorCheckReCheckCount) * 100;
            outboundCorrespondenceMI.OutboundCorrespondenceMIStats.ReCheckPassAdvisory = outboundCorrespondenceReCheckPassAd;
            outboundCorrespondenceMI.OutboundCorrespondenceMIStats.ReCheckPassedAdvisoryPercentage = outcorCheckReCheckCount == 0 ? 0 : decimal.Divide(outboundCorrespondenceReCheckPassAd, outcorCheckReCheckCount) * 100;
            outboundCorrespondenceMI.OutboundCorrespondenceMIStats.ReCheckFailed = outboundCorrespondenceReCheckFail;
            outboundCorrespondenceMI.OutboundCorrespondenceMIStats.ReCheckFailedPercentage = outcorCheckReCheckCount == 0 ? 0 : decimal.Divide(outboundCorrespondenceReCheckFail, outcorCheckReCheckCount) * 100;
            outboundCorrespondenceMI.OutboundCorrespondenceMIStats.ReChecksOutstanding = outboundCorrespondenceMI.OutboundCorrespondenceMIStats.ReCheckOutstandingZeroToFive + outboundCorrespondenceMI.OutboundCorrespondenceMIStats.ReCheckOutstandingSixToTen + outboundCorrespondenceMI.OutboundCorrespondenceMIStats.ReCheckOutstandingGreaterThanTen;
            outboundCorrespondenceMI.OutboundCorrespondenceMIStats.ReChecksRequired = outboundCorrespondenceReCheckRequired;
            outboundCorrespondenceMI.OutboundCorrespondenceMIStats.ReChecksNotRequired = outboundCorrespondenceReCheckNotRequired;


            return outboundCorrespondenceMI;
        }

        public List<OutboundCorrespondenceReCheck> GetAllReChecks(List<OutboundCorrespondence> outboundCorrespondence)
        {
            List<OutboundCorrespondenceReCheck> outboundCorrespondenceReChecks = new List<OutboundCorrespondenceReCheck>();

            foreach (var outboundCheck in outboundCorrespondence)
            {
                List<OutboundCorrespondenceReCheck> reCheckList = db.OutboundCorrespondenceReCheck.Where(x => x.OutboundCorrespondenceId == outboundCheck.CheckId).ToList();

                if (reCheckList.Count > 0)
                {
                    foreach (var reCheck in reCheckList)
                    {
                        outboundCorrespondenceReChecks.Add(reCheck);
                    }
                }
            }

            return outboundCorrespondenceReChecks;

        }



        public OutboundCorrespondenceMIDetails CollateMIDetails(OutboundCorrespondenceMI outboundCorrespondenceMI, string PersonName, bool detailsClick)
        {
            OutboundCorrespondenceMIDetails outboundCorrespondenceMIDetails = new OutboundCorrespondenceMIDetails
            {
                Failed = outboundCorrespondenceMI.OutboundCorrespondenceMIStats.Failed,
                FailedPercentage = outboundCorrespondenceMI.OutboundCorrespondenceMIStats.FailedPercentage,
                DateFrom = outboundCorrespondenceMI.DateFrom,
                DateTo = outboundCorrespondenceMI.DateTo,
                SearchType = outboundCorrespondenceMI.SearchType,
                ResultHeader = outboundCorrespondenceMI.ResultHeader,
                OutboundCorrespondenceMIPersonStatsList = outboundCorrespondenceMI.OutboundCorrespondenceMIPersonStatsList
            };


            List<OutboundCorrespondence> outboundCorrespondenceFails = new List<OutboundCorrespondence>();

            if (String.IsNullOrEmpty(PersonName))
            {
                outboundCorrespondenceFails = outboundCorrespondenceMI.OutboundCorrespondenceList.Where(x => x.FailReasonId != null).ToList();
            }
            else
            {
                if (outboundCorrespondenceMI.SearchType.Substring(0, 3) == "SEO")
                {
                    if (detailsClick)
                    {
                        outboundCorrespondenceFails = outboundCorrespondenceMI.OutboundCorrespondenceList.Where(x => x.FailReasonId != null && x.HEO == PersonName).ToList();
                        outboundCorrespondenceMIDetails.Failed = outboundCorrespondenceMI.OutboundCorrespondenceMIPersonStatsList.Where(x => x.HEOName == PersonName).Select(p => p.OutboundCorrespondenceMIStats.Failed).FirstOrDefault();
                        outboundCorrespondenceMIDetails.FailedPercentage = outboundCorrespondenceMI.OutboundCorrespondenceMIPersonStatsList.Where(x => x.HEOName == PersonName).Select(p => p.OutboundCorrespondenceMIStats.FailedPercentage).FirstOrDefault();
                        outboundCorrespondenceMIDetails.AnswerMIStats = AnswerMIListSingleUser(PersonName);
                    }
                    else
                    {
                        List<OutboundCorrespondence> outboundCorrespondences = outboundCorrespondenceMI.OutboundCorrespondenceList.Where(x => x.SEO == PersonName).ToList();
                        outboundCorrespondenceFails = outboundCorrespondences.Where(x => x.FailReasonId != null).ToList();
                        outboundCorrespondenceMIDetails.Failed = outboundCorrespondenceFails.Count();
                        outboundCorrespondenceMIDetails.FailedPercentage = outboundCorrespondenceMIDetails.Failed == 0 ? 0 : decimal.Divide(outboundCorrespondenceMIDetails.Failed, outboundCorrespondences.Count) * 100;
                    }
                }
                else if (outboundCorrespondenceMI.SearchType.Substring(0, 3) == "HEO")
                {
                    if (detailsClick)
                    {
                        outboundCorrespondenceFails = outboundCorrespondenceMI.OutboundCorrespondenceList.Where(x => x.FailReasonId != null && x.ManagerName == PersonName).ToList();
                        outboundCorrespondenceMIDetails.Failed = outboundCorrespondenceMI.OutboundCorrespondenceMIPersonStatsList.Where(x => x.LineManagerName == PersonName).Select(p => p.OutboundCorrespondenceMIStats.Failed).FirstOrDefault();
                        outboundCorrespondenceMIDetails.FailedPercentage = outboundCorrespondenceMI.OutboundCorrespondenceMIPersonStatsList.Where(x => x.LineManagerName == PersonName).Select(p => p.OutboundCorrespondenceMIStats.FailedPercentage).FirstOrDefault();
                        outboundCorrespondenceMIDetails.AnswerMIStats = AnswerMIListSingleUser(PersonName);
                    }
                    else
                    {
                        List<OutboundCorrespondence> outboundCorrespondences = outboundCorrespondenceMI.OutboundCorrespondenceList.Where(x => x.HEO == PersonName).ToList();
                        outboundCorrespondenceFails = outboundCorrespondences.Where(x => x.FailReasonId != null).ToList();
                        outboundCorrespondenceMIDetails.Failed = outboundCorrespondenceFails.Count();
                        outboundCorrespondenceMIDetails.FailedPercentage = outboundCorrespondenceMIDetails.Failed == 0 ? 0 : decimal.Divide(outboundCorrespondenceMIDetails.Failed, outboundCorrespondences.Count) * 100;
                    }
                }
                else if (outboundCorrespondenceMI.SearchType.Substring(0, 4) == "Line"  && detailsClick == false)
                {
                    List<OutboundCorrespondence> outboundCorrespondences = outboundCorrespondenceMI.OutboundCorrespondenceList.Where(x => x.ManagerName == PersonName).ToList();
                    outboundCorrespondenceFails = outboundCorrespondences.Where(x => x.FailReasonId != null).ToList();
                    outboundCorrespondenceMIDetails.Failed = outboundCorrespondenceFails.Count();
                    outboundCorrespondenceMIDetails.FailedPercentage = outboundCorrespondenceMIDetails.Failed == 0 ? 0 : decimal.Divide(outboundCorrespondenceMIDetails.Failed, outboundCorrespondences.Count) * 100;
                }
                else
                {
                    outboundCorrespondenceFails = outboundCorrespondenceMI.OutboundCorrespondenceList.Where(x => x.FailReasonId != null && x.PersonName == PersonName).ToList();
                    outboundCorrespondenceMIDetails.Failed = outboundCorrespondenceMI.OutboundCorrespondenceMIPersonStatsList.Where(x => x.PersonName == PersonName).Select(p => p.OutboundCorrespondenceMIStats.Failed).FirstOrDefault();
                    outboundCorrespondenceMIDetails.FailedPercentage = outboundCorrespondenceMI.OutboundCorrespondenceMIPersonStatsList.Where(x => x.PersonName == PersonName).Select(p => p.OutboundCorrespondenceMIStats.FailedPercentage).FirstOrDefault();
                    outboundCorrespondenceMIDetails.AnswerMIStats = AnswerMIListSingleUser(PersonName);
                }
            }

            outboundCorrespondenceMIDetails.FailReasonCount = GetFailDetails(outboundCorrespondenceFails);


            return outboundCorrespondenceMIDetails;

        }

        public List<FailReasonCount> GetFailDetails(List<OutboundCorrespondence> outboundCorrespondenceFails)
        {
            List<FailReason> failReasons = db.FailReasons.Where(x => x.Active == true && x.CheckType.Name == "Outbound Correspondence").ToList();
            List<FailReasonCount> failReasonCounts = new List<FailReasonCount>();



            foreach (var failReason in failReasons)
            {
                int failedNumber = outboundCorrespondenceFails.Where(x => x.FailReason.FailReasonId == failReason.FailReasonId).Count();

                FailReasonCount failReasonCount = new FailReasonCount
                {
                    FailedNumber = failedNumber,
                    FailedPercentage = outboundCorrespondenceFails.Count == 0 ? 0 : decimal.Divide(failedNumber, outboundCorrespondenceFails.Count) * 100,
                    FailReason = failReason.Text

                };

                failReasonCounts.Add(failReasonCount);
            }

            return failReasonCounts;
        }
        public List<AnswerMI> AnswerMIList()
        {
            List<AnswerMI> answerMIs = new List<AnswerMI>();


            foreach (var question in db.CheckQuestions.Where(x => x.CheckType.Name == "Outbound Correspondence"))
            {
                List<int> validCheckIDs = filterService.GetValidCheckIDs();

                List<Answer> allAnswers = db.Answers.Where(x => x.QuestionId == question.QuestionId && validCheckIDs.Contains(x.CheckId)).ToList();

                AnswerMI answerMI = new AnswerMI
                {
                    Question = question.Question,
                    TotalAnswers = allAnswers.Count(),
                    AnswerYes = allAnswers.Where(x => x.Text == "Yes").Count(),
                    AnswerNo = allAnswers.Where(x => x.Text == "No").Count(),
                    AnswerNA = allAnswers.Where(x => x.Text == "N/A").Count()
                };

                answerMI.AnswerYesPercent = answerMI.AnswerYes == 0 ? 0 : decimal.Divide(answerMI.AnswerYes, answerMI.TotalAnswers) * 100;
                answerMI.AnswerNoPercent = answerMI.AnswerNo == 0 ? 0 : decimal.Divide(answerMI.AnswerNo, answerMI.TotalAnswers) * 100;
                answerMI.AnswerNAPercent = answerMI.AnswerNA == 0 ? 0 : decimal.Divide(answerMI.AnswerNA, answerMI.TotalAnswers) * 100;

                answerMIs.Add(answerMI);
            }

            return answerMIs;
        }

        public List<AnswerMI> AnswerMIListSingleUser(string personName)
        {
            List<AnswerMI> answerMIs = new List<AnswerMI>();

            foreach (var question in db.CheckQuestions.Where(x => x.CheckType.Name == "Outbound Correspondence"))
            {
                List<int> userChecks = new List<int>();

                List<int> validCheckIDs = filterService.GetValidCheckIDs();

                userChecks = db.OutboundCorrespondence.Where(x => x.PersonName == personName).Select(p => p.CheckId).ToList();

                List<Answer> allAnswers = db.Answers.Where(x => x.QuestionId == question.QuestionId && userChecks.Contains(x.CheckId) && validCheckIDs.Contains(x.CheckId)).ToList();

                AnswerMI answerMI = new AnswerMI
                {
                    Question = question.Question,
                    TotalAnswers = allAnswers.Count(),
                    AnswerYes = allAnswers.Where(x => x.Text == "Yes").Count(),
                    AnswerNo = allAnswers.Where(x => x.Text == "No").Count(),
                    AnswerNA = allAnswers.Where(x => x.Text == "N/A").Count()
                };

                answerMI.AnswerYesPercent = answerMI.AnswerYes == 0 ? 0 : decimal.Divide(answerMI.AnswerYes, answerMI.TotalAnswers) * 100;
                answerMI.AnswerNoPercent = answerMI.AnswerNo == 0 ? 0 : decimal.Divide(answerMI.AnswerNo, answerMI.TotalAnswers) * 100;
                answerMI.AnswerNAPercent = answerMI.AnswerNA == 0 ? 0 : decimal.Divide(answerMI.AnswerNA, answerMI.TotalAnswers) * 100;

                answerMIs.Add(answerMI);
            }

            return answerMIs;
        }
    }

}