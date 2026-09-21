using RPA.QualityPortal.DAL;
using RPA.QualityPortal.Helpers;
using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.Services;
using RPA.QualityPortal.ViewModels.OutboundCorrespondenceVM.MI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RPA.QualityPortal.Controllers
{
    [Authorize(Roles = "Quality Checks: OC Team Member, Quality Checks: OC Team Manager")]
    public class OutboundCorrespondenceMIController : Controller
    {
        IQualityContext db;
        IPeopleContext pdb;
        IMIService miService;
        IUserHelper userHelper;
        IFilterService filterService;
        IRoleManager roleManager;

        public OutboundCorrespondenceMIController()
        {
            db = new QualityContext();
            pdb = new PeopleContext();
            userHelper = new UserHelper(pdb);
            roleManager = new RoleManager();
            filterService = new FilterService(db, pdb, userHelper, roleManager);
            miService = new MIService(db, pdb, userHelper, filterService);
        }

        public OutboundCorrespondenceMIController(IQualityContext context, IPeopleContext peopleContext,  IMIService miService, IUserHelper userHelper, IFilterService filterService, IRoleManager roleManager)
        {
            this.db = context;
            this.pdb = peopleContext;
            this.miService = miService;
            this.userHelper = userHelper;
            this.filterService = filterService;
            this.roleManager = roleManager;
        }

        public ActionResult MIIndex(string searchString = null, string searchOption = null, string dateFrom = null, string dateTo = null, int? SchemeId = null, int? BusinessAreaId = null, int? CorrespondenceTypeId = null, bool? back = null)
        {
            this.Session["MIPage"] = "MIDetailsIndividual";
            ViewBag.Back = back;

            return View();
        }

        public PartialViewResult _OutboundCorrespondenceMI(string searchString = null, string searchOption = null, string dateFrom = null, string dateTo = null, int? SchemeId = null, int? BusinessAreaId = null, int? CorrespondenceTypeId = null, bool? back = null)
        {
            OutboundCorrespondenceMI outboundCorrespondenceMISaved = this.Session["MIData"] as OutboundCorrespondenceMI;

            DateTime sixteenMonths = DateTime.Now.AddMonths(-16);

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

                        this.Session["MIData"] = outboundCorrespondenceMI;

                        return PartialView(outboundCorrespondenceMI);
                    }
                    else
                    {
                        ViewBag.Search = "Search";
                        return PartialView(outboundCorrespondenceMISaved);
                    }
                }
                else
                {
                        ViewBag.Search = "Search";
                        ViewBag.SearchName = searchString;

                if (back == false || back == null)
                    {
                        OutboundCorrespondenceMI outboundCorrespondenceMI = miService.OutboundCorrespondenceMISearch(searchString, searchOption, dateFrom, dateTo, SchemeId, BusinessAreaId, CorrespondenceTypeId, outboundCorrespondenceMISaved);
                        this.Session["MIData"] = outboundCorrespondenceMI;
                        return PartialView(outboundCorrespondenceMI);
                    }
                    else
                    {
                    
                    return PartialView(outboundCorrespondenceMISaved);
                    }
                }
        }

        public PartialViewResult _AnswerMI()
        {
            List<AnswerMI> answerMIs = miService.AnswerMIList();

            return PartialView(answerMIs);
        }

        public ActionResult MIDetails(string personName, bool detailsClick = false)
        {
            OutboundCorrespondenceMI outboundCorrespondenceMI = this.Session["MIData"] as OutboundCorrespondenceMI;

            OutboundCorrespondenceMIDetails outboundCorrespondenceMIDetails = miService.CollateMIDetails(outboundCorrespondenceMI, personName, detailsClick);

            ViewBag.PersonName = personName;

            if (String.IsNullOrEmpty(personName))
            {
                this.Session["MIPage"] = "MIDetailsOverview";
                ViewBag.Page = this.Session["MIPage"] as string;
                return View(outboundCorrespondenceMIDetails);
            }
            else
            {
                ViewBag.Page = this.Session["MIPage"] as string;
                return View("MIDetailsIndividual", outboundCorrespondenceMIDetails);
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