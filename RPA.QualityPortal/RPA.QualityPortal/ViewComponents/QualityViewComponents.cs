using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RPA.QualityPortal.DAL;
using RPA.QualityPortal.Infrastructure;
using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.Services;
using RPA.QualityPortal.ViewModels;
using RPA.QualityPortal.ViewModels.BankAccount;
using RPA.QualityPortal.ViewModels.OutboundCorrespondenceVM;
using RPA.QualityPortal.ViewModels.OutboundCorrespondenceVM.MI;
using System.Data.Entity;
using X.PagedList;

namespace RPA.QualityPortal.ViewComponents
{
    // Replace the child actions (Html.RenderAction) that rendered parts of the check, e-mail admin and MI screens.
    // Child actions picked up paging and search values from the parent request's route/query string; here they are
    // read from the query string explicitly.

    internal static class QueryExtensions
    {
        internal static int Int(this IQueryCollection query, string key, int defaultValue)
        {
            return int.TryParse(query[key], out var value) ? value : defaultValue;
        }

        internal static int? NullableInt(this IQueryCollection query, string key)
        {
            return int.TryParse(query[key], out var value) ? value : null;
        }
    }

    public class CheckCommentViewComponent : ViewComponent
    {
        private readonly IQualityContext db;

        public CheckCommentViewComponent(IQualityContext db)
        {
            this.db = db;
        }

        public IViewComponentResult Invoke(int CheckId, int QuestionId)
        {
            BankAccountComments comment = new BankAccountComments
            {
                AnswerComment = db.BankAccountComments.Where(x => x.CheckId == CheckId && x.QuestionId == QuestionId).Select(c => c.AnswerComment).FirstOrDefault() ?? ""
            };

            return View(comment);
        }
    }

    public class CheckAnswerViewComponent : ViewComponent
    {
        private readonly IQualityContext db;

        public CheckAnswerViewComponent(IQualityContext db)
        {
            this.db = db;
        }

        public IViewComponentResult Invoke(int CheckId, int QuestionId)
        {
            QuestionAnswer qa = new QuestionAnswer
            {
                CheckQuestion = db.CheckQuestions.Where(x => x.QuestionId == QuestionId).FirstOrDefault(),
                Answer = db.Answers.Where(x => x.CheckId == CheckId && x.QuestionId == QuestionId).Select(c => c.Text).FirstOrDefault()
            };

            return View(qa);
        }
    }

    public class ChallengeViewViewComponent : ViewComponent
    {
        private readonly IQualityContext db;

        public ChallengeViewViewComponent(IQualityContext db)
        {
            this.db = db;
        }

        public IViewComponentResult Invoke(int CheckId)
        {
            Challenge challenge = db.Challenge.Include("ChallengeOutcome").Where(x => x.CheckId == CheckId).FirstOrDefault();

            return View(challenge);
        }
    }

    public class OutboundCorrespondenceEmailsViewComponent : ViewComponent
    {
        private readonly IQualityContext db;
        private readonly IFilterService filterService;
        private readonly IOutboundCorrespondenceService outboundCorrespondenceService;

        public OutboundCorrespondenceEmailsViewComponent(IQualityContext db, IFilterService filterService, IOutboundCorrespondenceService outboundCorrespondenceService)
        {
            this.db = db;
            this.filterService = filterService;
            this.outboundCorrespondenceService = outboundCorrespondenceService;
        }

        public IViewComponentResult Invoke()
        {
            int page = Request.Query.Int("page", 1);
            int pageSize = Request.Query.Int("pageSize", 50);

            IQueryable<OutboundCorrespondence> outboundCorrespondences = filterService.OutboundCorrespondenceFilterChecks(null, "All Quality Checks");

            IQueryable<OutboundCorrespondenceOverview> OutboundCorrespondenceOverviews = outboundCorrespondenceService
                .RetrieveOutboundCorrespondenceChecks(outboundCorrespondences)
                .Where(o => o.OutboundCorrespondence.EmailNotifications == false);

            ViewBag.Email = db.Control.Where(x => x.Property == "OutboundCorrespondenceEmail").Select(c => c.Active).FirstOrDefault();
            ViewBag.typeOfCheck = "OutboundCorrespondence";

            return View(new PagedList<OutboundCorrespondenceOverview>(OutboundCorrespondenceOverviews, page, pageSize));
        }
    }

    public class BankEmailsViewComponent : ViewComponent
    {
        private readonly IQualityContext db;
        private readonly IFilterService filterService;
        private readonly IBankAccountService bankAccountService;

        public BankEmailsViewComponent(IQualityContext db, IFilterService filterService, IBankAccountService bankAccountService)
        {
            this.db = db;
            this.filterService = filterService;
            this.bankAccountService = bankAccountService;
        }

        public IViewComponentResult Invoke()
        {
            int page = Request.Query.Int("page", 1);
            int pageSize = Request.Query.Int("pageSize", 50);

            IQueryable<BankAccountCheck> bankAccountChecks = filterService.BankAccountFilterChecks(null, null, null, "All Quality Checks", null);

            IQueryable<BankAccountOverview> BankAccountOverviews = bankAccountService
                .RetrieveBankAccountChecks(bankAccountChecks)
                .Where(o => o.BankAccount.EmailNotifications == false);

            ViewBag.Email = db.Control.Where(x => x.Property == "BankEmail").Select(c => c.Active).FirstOrDefault();
            ViewBag.typeOfCheck = "Bank";

            return View(new PagedList<BankAccountOverview>(BankAccountOverviews, page, pageSize));
        }
    }

    public class TurnOffEmailViewComponent : ViewComponent
    {
        private readonly IQualityContext db;

        public TurnOffEmailViewComponent(IQualityContext db)
        {
            this.db = db;
        }

        public IViewComponentResult Invoke(string checkType)
        {
            ViewBag.typeOfCheck = checkType;

            return View(db.Control.Where(x => x.Property == checkType + "Email").FirstOrDefault());
        }
    }

    public class ReCheckEmailsViewComponent : ViewComponent
    {
        private readonly IQualityContext db;
        private readonly IOutboundCorrespondenceService outboundCorrespondenceService;

        public ReCheckEmailsViewComponent(IQualityContext db, IOutboundCorrespondenceService outboundCorrespondenceService)
        {
            this.db = db;
            this.outboundCorrespondenceService = outboundCorrespondenceService;
        }

        public IViewComponentResult Invoke()
        {
            int page = Request.Query.Int("page", 1);
            int pageSize = Request.Query.Int("pageSize", 50);

            IQueryable<OutboundCorrespondenceOverview> qualityChecks = outboundCorrespondenceService
                .RetrieveOutboundCorrespondenceRechecks(db.OutboundCorrespondenceReCheck)
                .Where(o => o.OutboundCorrespondenceReCheck.EmailNotifications == false);

            ViewBag.Email = db.Control.Where(x => x.Property == "OutboundCorrespondenceEmail").Select(c => c.Active).FirstOrDefault();
            ViewBag.typeOfCheck = "OutboundCorrespondence";

            return View(new PagedList<OutboundCorrespondenceOverview>(qualityChecks, page, pageSize));
        }
    }

    public class BankReCheckEmailsViewComponent : ViewComponent
    {
        private readonly IQualityContext db;
        private readonly IBankAccountService bankAccountService;

        public BankReCheckEmailsViewComponent(IQualityContext db, IBankAccountService bankAccountService)
        {
            this.db = db;
            this.bankAccountService = bankAccountService;
        }

        public IViewComponentResult Invoke()
        {
            int page = Request.Query.Int("page", 1);
            int pageSize = Request.Query.Int("pageSize", 50);

            IQueryable<BankAccountOverview> qualityChecks = bankAccountService
                .RetrieveBankAccountRechecks(db.BankAccountReChecks)
                .Where(o => o.BankAccountReCheck.EmailNotifications == false);

            ViewBag.Email = db.Control.Where(x => x.Property == "BankEmail").Select(c => c.Active).FirstOrDefault();
            ViewBag.typeOfCheck = "Bank";

            return View(new PagedList<BankAccountOverview>(qualityChecks, page, pageSize));
        }
    }

    public class AnswerMIViewComponent : ViewComponent
    {
        private readonly IMIService miService;

        public AnswerMIViewComponent(IMIService miService)
        {
            this.miService = miService;
        }

        public IViewComponentResult Invoke()
        {
            List<AnswerMI> answerMIs = miService.AnswerMIList();

            return View(answerMIs);
        }
    }

    public class OutboundCorrespondenceMIViewComponent : ViewComponent
    {
        private readonly IQualityContext db;
        private readonly IMIService miService;
        private readonly IFilterService filterService;
        private readonly ISessionStore sessionStore;

        public OutboundCorrespondenceMIViewComponent(IQualityContext db, IMIService miService, IFilterService filterService, ISessionStore sessionStore)
        {
            this.db = db;
            this.miService = miService;
            this.filterService = filterService;
            this.sessionStore = sessionStore;
        }

        public IViewComponentResult Invoke(bool? back = null)
        {
            string searchString = Request.Query["searchString"];
            string searchOption = Request.Query["searchOption"];
            string dateFrom = Request.Query["dateFrom"];
            string dateTo = Request.Query["dateTo"];
            int? SchemeId = Request.Query.NullableInt("SchemeId");
            int? BusinessAreaId = Request.Query.NullableInt("BusinessAreaId");
            int? CorrespondenceTypeId = Request.Query.NullableInt("CorrespondenceTypeId");

            OutboundCorrespondenceMI outboundCorrespondenceMISaved = sessionStore.Get("MIData") as OutboundCorrespondenceMI;

            SetMIViewData();

            if (string.IsNullOrEmpty(searchString) && string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo) && SchemeId == null && BusinessAreaId == null && CorrespondenceTypeId == null)
            {
                if (back == false || back == null)
                {
                    //use filter service

                    List<OutboundCorrespondence> outboundCorrespondences = filterService.MIfilterChecks();

                    List<OutboundCorrespondenceReCheck> outboundCorrespondenceReChecks = miService.GetAllReChecks(outboundCorrespondences);

                    OutboundCorrespondenceMI outboundCorrespondenceMI = new OutboundCorrespondenceMI
                    {
                        OutboundCorrespondenceList = outboundCorrespondences,
                        OutboundCorrespondenceReCheckList = outboundCorrespondenceReChecks
                    };

                    miService.CollateMI(outboundCorrespondenceMI);

                    sessionStore.Set("MIData", outboundCorrespondenceMI);

                    return View(outboundCorrespondenceMI);
                }
                else
                {
                    ViewBag.Search = "Search";
                    return View(outboundCorrespondenceMISaved);
                }
            }
            else
            {
                ViewBag.Search = "Search";
                ViewBag.SearchName = searchString;

                if (back == false || back == null)
                {
                    OutboundCorrespondenceMI outboundCorrespondenceMI = miService.OutboundCorrespondenceMISearch(searchString, searchOption, dateFrom, dateTo, SchemeId, BusinessAreaId, CorrespondenceTypeId, outboundCorrespondenceMISaved);
                    sessionStore.Set("MIData", outboundCorrespondenceMI);
                    return View(outboundCorrespondenceMI);
                }
                else
                {
                    return View(outboundCorrespondenceMISaved);
                }
            }
        }

        private void SetMIViewData()
        {
            ViewBag.SchemeId = new SelectList(db.Schemes.AsNoTracking().Where(x => x.Active == true).OrderBy(x => x.Text), "SchemeId", "Text");
            ViewBag.BusinessAreaId = new SelectList(db.BusinessAreas.AsNoTracking().Where(x => x.Active == true).OrderBy(x => x.Text), "BusinessAreaId", "Text");
            ViewBag.CorrespondenceTypeId = new SelectList(db.CorrespondenceTypes.AsNoTracking().Where(x => x.Active == true).OrderBy(x => x.Text), "CorrespondenceTypeId", "Text");
        }
    }
}
