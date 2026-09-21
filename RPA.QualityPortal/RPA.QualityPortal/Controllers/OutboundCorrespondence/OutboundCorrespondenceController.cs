using PagedList;
using RPA.QualityPortal.DAL;
using RPA.QualityPortal.Helpers;
using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns;
using RPA.QualityPortal.Services;
using RPA.QualityPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;

namespace RPA.QualityPortal.Controllers
{
    [Authorize(Roles = "Quality Checks: OC Team Member, Quality Checks: OC Team Manager")]
    public class OutboundCorrespondenceController : Controller
    {
        IQualityContext db;
        IPeopleContext pdb;

        IOutboundCorrespondenceService outboundCorrespondenceService;
        IPeopleService peopleService;
        IAnswerService answerService;
        IMessageService messageService;
        ISendService sendService;
        IFilterService filterService;
        IAuditService auditService;
        IExportService exportService;
        IUserHelper userHelper;
        IRoleManager roleManager;
        ILockService lockService;

        public OutboundCorrespondenceController()
        {
            db = new QualityContext();
            pdb = new PeopleContext();
            answerService = new AnswerService(db);           
            peopleService = new PeopleService(pdb);
            userHelper = new UserHelper(pdb);
            roleManager = new RoleManager();
            filterService = new FilterService(db, pdb, userHelper, roleManager);
            auditService = new AuditService(db);
            sendService = new SendService();
            messageService = new EmailService(db, pdb);
            exportService = new ExportService(db);
            lockService = new LockService(db, pdb, userHelper);
            outboundCorrespondenceService = new OutboundCorrespondenceService(db, answerService, userHelper, lockService);
        }

        public OutboundCorrespondenceController(IQualityContext context, IPeopleContext peopleContext, IOutboundCorrespondenceService outboundCorrespondenceService, IPeopleService peopleService, IMessageService messageService, ISendService sendService, IAuditService auditService, IFilterService filterService, IAnswerService answerService, IUserHelper userHelper, IExportService exportService, IRoleManager roleManager, ILockService lockService)

        {
            this.db = context;
            this.pdb = peopleContext;
            this.outboundCorrespondenceService = outboundCorrespondenceService;
            this.peopleService = peopleService;
            this.sendService = sendService;
            this.messageService = messageService;
            this.auditService = auditService;
            this.filterService = filterService;
            this.answerService = answerService;
            this.exportService = exportService;
            this.userHelper = userHelper;
            this.roleManager = roleManager;
            this.lockService = lockService;
        }

        //GET:
        public ActionResult Index(bool locked = false, string searchString = null, int page = 1, int pageSize = 50, int filterId = 0)
        {
            if (locked)
            {
                ViewBag.Locked = "1";
            }

            return GetListViews(searchString, page, pageSize, "All Quality Checks");
        }

        //GET:
        public ActionResult Complete(string searchString = null, int page = 1, int pageSize = 50, int filterId = 0)
        {
            return GetListViews(searchString, page, pageSize, "Complete Checks");
        }

        //GET:
        public ActionResult Outstanding(string searchString = null, int page = 1, int pageSize = 50, int filterId = 0)
        {
            return GetListViews(searchString, page, pageSize, "Outstanding Checks");
        }

        private ViewResult GetListViews(string searchString, int page, int pageSize, string pageName)
        {
            Session.Clear();

            ViewBag.PageName = pageName;

            IQueryable<OutboundCorrespondence> outboundCorrespondences = filterService.OutboundCorrespondenceFilterChecks(searchString, pageName);
            IQueryable<OutboundCorrespondenceOverview> outboundCorrespondenceOverviews = outboundCorrespondenceService.RetrieveOutboundCorrespondenceChecks(outboundCorrespondences);

            ViewBag.CurrentSearch = searchString;

            return View("Index", new PagedList<OutboundCorrespondenceOverview>(outboundCorrespondenceOverviews, page, pageSize));
        }

        public PartialViewResult _ReCheckResults(int CheckId)
        {
            int ReCheckId = db.OutboundCorrespondenceReCheck.Where(x => x.OutboundCorrespondenceId == CheckId).OrderByDescending(c => c.DateQCCompleted).Select(x => x.CheckId).FirstOrDefault();

            DetailsReCheck detailsReCheck = new DetailsReCheck
            {
                ReCheckResult = db.CheckResults.Where(x => x.CheckId == ReCheckId).Select(p => p.Result.Text).FirstOrDefault()
            };

            return PartialView(detailsReCheck);
        }

        //GET:
        public ActionResult Export()
        {
            DateTime sixteenMonths = DateTime.Now.AddMonths(-16);

            var outboundCorrespondences = db.OutboundCorrespondence.Where(x => x.DateQCCompleted > sixteenMonths);
            var outboundCorrespondenceReChecks = db.OutboundCorrespondenceReCheck;

            return File(new UTF8Encoding().GetBytes(exportService.Build(outboundCorrespondences, outboundCorrespondenceReChecks)), "text/csv", string.Format("OutBoundCorrespondenceQC_{0}.csv", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }

        //GET:
        public ActionResult ExportArchive()
        {
            DateTime sixteenMonths = DateTime.Now.AddMonths(-16);

            var outboundCorrespondences = db.OutboundCorrespondence.Where(x => x.DateQCCompleted < sixteenMonths);
            var outboundCorrespondenceReChecks = db.OutboundCorrespondenceReCheck;

            return File(new UTF8Encoding().GetBytes(exportService.Build(outboundCorrespondences, outboundCorrespondenceReChecks)), "text/csv", string.Format("OutBoundCorrespondenceQC_{0}.csv", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }

        //GET:
        public ActionResult GeneralDetails(int CheckId)
        {
            return View(db.OutboundCorrespondence.Where(x => x.CheckId == CheckId).FirstOrDefault());
        }

        //GET: OutboundCorrespondence/CreateGeneral
        public ActionResult CreateGeneral()
        {
            OutboundCorrespondence outboundCorrespondence = new OutboundCorrespondence();

            OutboundCorrespondence outboundCorrespondenceExisting = this.Session["CreateGeneral"] as OutboundCorrespondence;

            outboundCorrespondence.QCCompletedByName = userHelper.CurrentUser();

            if (outboundCorrespondenceExisting != null)
            {
                outboundCorrespondence = outboundCorrespondenceExisting;
            }

            SetViewData(outboundCorrespondence);

            return View(outboundCorrespondence);
        }

        //POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateGeneral(OutboundCorrespondence outboundCorrespondence)
        {
            NameCheck(outboundCorrespondence);

            bool duplicate = outboundCorrespondenceService.CheckDuplicates(outboundCorrespondence.UniqueId.ToUpper(), outboundCorrespondence.UniqueIdentifierPrefixId, outboundCorrespondence.CRMRef.ToUpper(), outboundCorrespondence.CRMRefPrefixId);

            if (duplicate)
            {
                ModelState.AddModelError("CRMRef", "A record exists with this CRM Reference and Unique Identififer");
            }

            var correspondenceType = db.CorrespondenceTypes.Where(x => x.CorrespondenceTypeId == outboundCorrespondence.CorrespondenceTypeId).FirstOrDefault();


            this.Session["CreateGeneral"] = outboundCorrespondence;

            if (ModelState.IsValid)
            {
                return RedirectToAction("CreateQuestions");
            }

            SetViewData(outboundCorrespondence);
            return View(outboundCorrespondence);

        }

        public void NameCheck(OutboundCorrespondence outboundCorrespondence)
        {
            //Check All names in People List

            bool nameCheck = peopleService.PersonCheck(outboundCorrespondence.PersonName);
            bool managerCheck = peopleService.PersonCheck(outboundCorrespondence.ManagerName);
            bool seoCheck = peopleService.PersonCheck(outboundCorrespondence.SEO);
            bool heoCheck = peopleService.PersonCheck(outboundCorrespondence.HEO);

            if (!nameCheck)
            {
                ModelState.AddModelError("PersonName", "Please Select a Valid Name From List");
            }
            if (!managerCheck)
            {
                ModelState.AddModelError("ManagerName", "Please Select a Valid Name From List");
            }
            if (!seoCheck)
            {
                ModelState.AddModelError("SEO", "Please Select a Valid Name From List");
            }
            if (!heoCheck)
            {
                ModelState.AddModelError("HEO", "Please Select a Valid Name From List");
            }
        }

        //GET:
        public ActionResult CreateQuestions()
        {
            OutboundCorrespondence outboundCorrespondence = this.Session["CreateGeneral"] as OutboundCorrespondence;

            if (outboundCorrespondence != null)
            {
                QuestionAnswerList qaListExisting = this.Session["CreateQuestions"] as QuestionAnswerList;

                ViewBag.AnswerOptions = ListHelper.YesNo();

                if (qaListExisting == null)
                {
                    CreateListQuestions(outboundCorrespondence);
                }

                return View(qaListExisting);
            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        public ActionResult CreateListQuestions(OutboundCorrespondence outboundCorrespondence)
        {
            List<CheckQuestion> checkQuestions = db.CheckQuestions.Where(x => x.CheckType.Name == "Outbound Correspondence").OrderBy(c => c.Order).ToList();

            QuestionAnswerList qaList = new QuestionAnswerList();

            foreach (var cq in checkQuestions)
            {
                QuestionAnswer qa = new QuestionAnswer
                {
                    CheckQuestion = cq
                };

                qaList.QuestionAnswers.Add(qa);
            }

            return View("CreateQuestions", qaList);
        }

        //POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateQuestions(QuestionAnswerList qaList)
        {
            OutboundCorrespondence outboundCorrespondence = this.Session["CreateGeneral"] as OutboundCorrespondence;

            if (outboundCorrespondence != null && qaList != null)
            {
                qaList = outboundCorrespondenceService.QCResultCalculate(qaList);
            }

            this.Session["CreateQuestions"] = qaList;
            return RedirectToAction("CreateResult");


        }

        //GET:
        public ActionResult CreateResult()
        {
            OutboundCorrespondence outboundCorrespondence = this.Session["CreateGeneral"] as OutboundCorrespondence;

            QuestionAnswerList qaList = this.Session["CreateQuestions"] as QuestionAnswerList;

            if (outboundCorrespondence != null && qaList != null)
            {
                if (qaList.QCResult == "Not Approved")
                {
                    outboundCorrespondence.ReCheckRequired = true;
                }

                ViewBag.Email = db.Control.Where(x => x.Property == "OutboundCorrespondenceEmail").Select(c => c.Active).FirstOrDefault();
                ViewBag.QCResult = qaList.QCResult;

                SetViewData(outboundCorrespondence);

                return View(outboundCorrespondence);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        //POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateResult(OutboundCorrespondence outboundCorrespondence)
        {
            QuestionAnswerList qaList = this.Session["CreateQuestions"] as QuestionAnswerList;

            OutboundCorrespondence outboundCorrespondenceSaved = this.Session["CreateGeneral"] as OutboundCorrespondence;

            bool email = db.Control.Where(x => x.Property == "OutboundCorrespondenceEmail").Select(c => c.Active).FirstOrDefault();

            if (outboundCorrespondenceSaved != null && qaList != null)
            {
                bool duplicate = outboundCorrespondenceService.CheckDuplicates(outboundCorrespondenceSaved.UniqueId, outboundCorrespondenceSaved.UniqueIdentifierPrefixId, outboundCorrespondenceSaved.CRMRef, outboundCorrespondenceSaved.CRMRefPrefixId);

                if (duplicate)
                {
                    ModelState.AddModelError("Commments", "An identical record already exists within database");
                }

                if (qaList.QCResult == "Not Approved" && outboundCorrespondence.FailReasonId == null)
                {
                    ModelState.AddModelError("FailReasonId", "A not approved reason must be selected.");
                }

                if (ModelState.IsValid)
                {
                    if (qaList.QCResult == "Approved" || qaList.QCResult == "Approved Advisory")
                    {
                        outboundCorrespondenceSaved.FailReasonId = null;
                    }
                    else
                    {
                        outboundCorrespondenceSaved.FailReasonId = outboundCorrespondence.FailReasonId;
                    }
                    outboundCorrespondenceSaved.ReCheckRequired = outboundCorrespondence.ReCheckRequired;
                    outboundCorrespondenceSaved.InitialReCheckDecision = outboundCorrespondence.ReCheckRequired;
                    outboundCorrespondenceSaved.Comments = outboundCorrespondence.Comments;
                    outboundCorrespondenceSaved.ExcludeQCResult = outboundCorrespondence.ExcludeQCResult;
                    outboundCorrespondenceSaved.EmailNotifications = outboundCorrespondence.EmailNotifications;

                    outboundCorrespondenceService.SaveOutboundCorrespondence(outboundCorrespondenceSaved, qaList);

                    if (email != true)
                    {
                        outboundCorrespondenceSaved.EmailNotifications = false;
                    }

                    if (outboundCorrespondenceSaved.EmailNotifications == true)
                    {
                        MailMessage mail = messageService.GenerateOutboundCorrespondenceEmail(outboundCorrespondenceSaved);

                        sendService.sendEmail(mail);
                    }

                    this.Session.Clear();

                    return RedirectToAction("Details", new { CheckId = outboundCorrespondenceSaved.CheckId });

                }
                else
                {
                    SetViewData(outboundCorrespondence);
                    ViewBag.Email = db.Control.Where(x => x.Property == "OutboundCorrespondenceEmail").Select(c => c.Active).FirstOrDefault();
                    ViewBag.QCResult = qaList.QCResult;
                    return View(outboundCorrespondence);
                }
            }
            return RedirectToAction("Index");
        }

        //GET:
        public ActionResult Details(int CheckId)
        {
            int reCheckCount = db.OutboundCorrespondenceReCheck.Where(x => x.OutboundCorrespondenceId == CheckId).Count();

            if (reCheckCount == 0)
            {
                Details detailView = new Details
                {
                    OutboundCorrespondence = db.OutboundCorrespondence.Where(x => x.CheckId == CheckId).FirstOrDefault(),
                    QCResult = db.CheckResults.Where(x => x.CheckId == CheckId).Select(p => p.Result.Text).FirstOrDefault(),
                    Answers = db.Answers.Where(x => x.CheckId == CheckId).OrderBy(p => p.QuestionId).ToList(),
                    CheckAmendmentReason = db.CheckAmendmentReason.Where(x => x.CheckId == CheckId).OrderByDescending(x => x.AmendmentReasonId).FirstOrDefault(),
                    Challenge = db.Challenge.Any(x => x.CheckId == CheckId)
                };

                return View(detailView);
            }
            else
            {
                return RedirectToAction("DetailsReCheck", "OutboundCorrespondenceReCheck", new { CheckId });
            }
        }

        //GET:
        public ActionResult EditGeneral(int CheckId)
        {
            OutboundCorrespondence outboundCorrespondence = db.OutboundCorrespondence.Where(x => x.CheckId == CheckId).FirstOrDefault();

            OutboundCorrespondence outboundCorrespondenceExisting = this.Session["EditGeneral"] as OutboundCorrespondence;

            if (outboundCorrespondenceExisting != null)
            {
                outboundCorrespondence = outboundCorrespondenceExisting;
            }

            SetViewData(outboundCorrespondence);

            return View(outboundCorrespondence);
        }

        // POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditGeneral(OutboundCorrespondence outboundCorrespondence)
        {
            //set model to session data

            this.Session["EditGeneral"] = outboundCorrespondence;

            bool duplicate = outboundCorrespondenceService.CheckEditDuplicates(outboundCorrespondence.UniqueId.ToUpper(), outboundCorrespondence.UniqueIdentifierPrefixId, outboundCorrespondence.CRMRef.ToUpper(), outboundCorrespondence.CRMRefPrefixId, outboundCorrespondence.CheckId);

            if (duplicate)
            {
                ModelState.AddModelError("CRMRef", "A record exists with this CRM Reference and Unique Identififer");
            }

            outboundCorrespondence.CorrespondenceType = db.CorrespondenceTypes.Where(x => x.CorrespondenceTypeId == outboundCorrespondence.CorrespondenceTypeId).FirstOrDefault();

            outboundCorrespondence.DateQCCompleted = db.OutboundCorrespondence.Where(x => x.CheckId == outboundCorrespondence.CheckId).Select(c => c.DateQCCompleted).FirstOrDefault();

            if (ModelState.IsValid)
            {
                this.Session["PreviousResult"] = db.CheckResults.Where(x => x.CheckId == outboundCorrespondence.CheckId).Select(c => c.Result.Text).FirstOrDefault();

                return RedirectToAction("EditQuestions");
            }

            SetViewData(outboundCorrespondence);

            return View(outboundCorrespondence);
        }

        //GET:
        public ActionResult EditQuestions()
        {
            //get models from session data 

            OutboundCorrespondence outboundCorrespondence = this.Session["EditGeneral"] as OutboundCorrespondence;

            QuestionAnswerList qaListExisting = this.Session["EditQuestions"] as QuestionAnswerList;

            ViewBag.CheckId = outboundCorrespondence.CheckId;
            ViewBag.AnswerOptions = ListHelper.YesNo();


            if (qaListExisting == null)
            {
                List<CheckQuestion> checkQuestions = db.CheckQuestions.Where(x => x.CheckType.Name == "Outbound Correspondence").OrderBy(c => c.Order).ToList();

                QuestionAnswerList qaList = new QuestionAnswerList();

                foreach (var cq in checkQuestions)
                {
                    Answer answer = db.Answers.Where(x => x.CheckId == outboundCorrespondence.CheckId && x.QuestionId == cq.QuestionId).FirstOrDefault();

                    QuestionAnswer qa = new QuestionAnswer
                    {
                        CheckQuestion = cq,
                        Answer = answer.Text
                    };

                    qaList.QuestionAnswers.Add(qa);
                }

                return View(qaList);
            }

            return View(qaListExisting);
        }

        // POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuestions(QuestionAnswerList qaList)
        {
            if (ModelState.IsValid)
            {
                qaList = outboundCorrespondenceService.QCResultCalculate(qaList);
                this.Session["EditQuestions"] = qaList;
                return RedirectToAction("EditResult");
            }

            ViewBag.AnswerOptions = ListHelper.YesNo();
            return View(qaList);
        }

        //GET:
        public ActionResult EditResult()
        {
            OutboundCorrespondence outboundCorrespondence = this.Session["EditGeneral"] as OutboundCorrespondence;

            QuestionAnswerList qaList = this.Session["EditQuestions"] as QuestionAnswerList;

            ViewBag.QCResult = qaList.QCResult;

            this.Session["EditQuestions"] = qaList;

            SetViewData(outboundCorrespondence);

            return View(outboundCorrespondence);
        }

        //POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditResult(OutboundCorrespondence outboundCorrespondence)
        {
            //obtain existing models from session data

            QuestionAnswerList qaList = this.Session["EditQuestions"] as QuestionAnswerList;

            OutboundCorrespondence outboundCorrespondenceSaved = this.Session["EditGeneral"] as OutboundCorrespondence;

            if (ModelState.IsValid)
            {
                if (qaList.QCResult == "Approved" || qaList.QCResult == "Approved Advisory")
                {
                    outboundCorrespondenceSaved.FailReasonId = null;
                }
                else
                {
                    outboundCorrespondenceSaved.FailReasonId = outboundCorrespondence.FailReasonId;
                }

                outboundCorrespondenceSaved.ReCheckRequired = outboundCorrespondence.ReCheckRequired;
                outboundCorrespondenceSaved.Comments = outboundCorrespondence.Comments;
                outboundCorrespondenceSaved.ExcludeQCResult = outboundCorrespondence.ExcludeQCResult;

                this.Session["EditGeneral"] = outboundCorrespondenceSaved;

                return RedirectToAction("AmendmentReason");
            }
            else
            {
                SetViewData(outboundCorrespondence);
                ViewBag.QCResult = qaList.QCResult;
                return View(outboundCorrespondence);
            }
        }

        //GET:
        public ActionResult AmendmentReason()
        {
            CheckAmendmentReason checkAmendmentReason = new CheckAmendmentReason();

            bool challengeAmend = false;

            SetAmendViewData(checkAmendmentReason, challengeAmend);

            return View(checkAmendmentReason);
        }

        //POST:
        [HttpPost]
        public ActionResult AmendmentReason(CheckAmendmentReason checkAmendmentReason)
        {
            QuestionAnswerList qaList = this.Session["EditQuestions"] as QuestionAnswerList;

            OutboundCorrespondence outboundCorrespondence = this.Session["EditGeneral"] as OutboundCorrespondence;

            checkAmendmentReason.AmendmentReason = db.AmendmentReasons.Where(x => x.AmendmentReasonId == checkAmendmentReason.AmendmentReasonId).FirstOrDefault();

            if (checkAmendmentReason.AmendmentReason == null)
            {
                ModelState.AddModelError("AmendmentReason", "An amendment reason must be selected - if no amendments have been made please return to details screen");
            }

            if (ModelState.IsValid)
            {
                string previousResult = this.Session["PreviousResult"] as string;

                outboundCorrespondenceService.SaveEditOutboundCorrespondence(outboundCorrespondence, qaList, previousResult);

                auditService.SaveOutboundCorrespondenceAmendmentAudit(outboundCorrespondence, checkAmendmentReason);

                checkAmendmentReason.CheckId = outboundCorrespondence.CheckId;

                db.CheckAmendmentReason.Add(checkAmendmentReason);
                db.SaveChanges();

                return RedirectToAction("Details", "OutboundCorrespondence", new { outboundCorrespondence.CheckId });
            }
            else
            {
                bool challengeAmend = false;

                SetAmendViewData(checkAmendmentReason, challengeAmend);
                ViewBag.QCResult = qaList.QCResult;
                return View(checkAmendmentReason);
            }
        }

        private void SetViewData(OutboundCorrespondence outboundCorrespondence)
        {
            ViewBag.SchemeId = new SelectList(db.Schemes.AsNoTracking().Where(x => x.Active == true).OrderBy(x => x.Text), "SchemeId", "Text", outboundCorrespondence.SchemeId);
            ViewBag.BusinessAreaId = new SelectList(db.BusinessAreas.AsNoTracking().Where(x => x.Active == true).OrderBy(x => x.Text), "BusinessAreaId", "Text", outboundCorrespondence.BusinessAreaId);
            ViewBag.CorrespondenceTypeId = new SelectList(db.CorrespondenceTypes.AsNoTracking().Where(x => x.Active == true).OrderBy(x => x.Text), "CorrespondenceTypeId", "Text", outboundCorrespondence.CorrespondenceTypeId);
            ViewBag.UniqueIdentifierPrefixId = new SelectList(db.UniqueIdentifierPrefixes.AsNoTracking().Where(x => x.Active == true).OrderBy(x => x.Text), "UniqueIdentifierPrefixId", "Text", outboundCorrespondence.UniqueIdentifierPrefixId);
            ViewBag.CRMRefPrefixId = new SelectList(db.CRMRefPrefixes.AsNoTracking().Where(x => x.Active == true).OrderBy(x => x.Text), "CRMRefPrefixId", "Text", outboundCorrespondence.CRMRefPrefixId);
            ViewBag.FailReasonId = new SelectList(db.FailReasons.AsNoTracking().Where(x => x.Active == true && x.CheckTypeId == 1).OrderBy(x => x.Text), "FailReasonId", "Text", outboundCorrespondence.FailReasonId);
        }

        private void SetAmendViewData(CheckAmendmentReason checkAmendmentReason, bool challengeAmend)
        {
            if (challengeAmend)
            {
                ViewBag.AmendmentReasonId = new SelectList(db.AmendmentReasons.AsNoTracking().Where(x => x.Active == true).OrderBy(x => x.Text), "AmendmentReasonId", "Text", checkAmendmentReason.AmendmentReasonId);
            }
            else
            {
                ViewBag.AmendmentReasonId = new SelectList(db.AmendmentReasons.AsNoTracking().Where(x => x.Active == true && x.Text != "Challenge Outcome").OrderBy(x => x.Text), "AmendmentReasonId", "Text", checkAmendmentReason.AmendmentReasonId);
            }
        }

        //GET:
        public ActionResult Challenge(int CheckId)
        {
            Challenge challenge = new Challenge
            {
                CheckId = CheckId
            };

            ViewBag.ReCheck = db.OutboundCorrespondence.Where(x => x.CheckId == CheckId).Select(p => p.ReCheckRequired).FirstOrDefault() == true ? "Yes" : "No";
            ViewBag.ChallengeOutcomeId = new SelectList(db.ChallengeOutcomes.AsNoTracking().Where(x => x.Active == true).OrderBy(x => x.Text), "ChallengeOutcomeId", "Text");
            ViewBag.QCResult = db.CheckResults.AsNoTracking().Where(x => x.CheckId == CheckId).Select(p => p.Result.Text).FirstOrDefault();

            return View(challenge);
        }

        //POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Challenge(string ReCheck, Challenge challenge)
        {

            ChallengeOutcome challengeOutcome = db.ChallengeOutcomes.AsNoTracking().Where(x => x.ChallengeOutcomeId == challenge.ChallengeOutcomeId).FirstOrDefault();


            this.Session["ChallengeData"] = challenge;
            OutboundCorrespondence outboundCorrespondence = db.OutboundCorrespondence.Where(x => x.CheckId == challenge.CheckId).FirstOrDefault();

            if (ReCheck == "Yes")
            {
                int reCheckCount = db.OutboundCorrespondenceReCheck.Where(x => x.OutboundCorrespondenceId == challenge.CheckId).Count();

                if (challengeOutcome.Text == "Original Result Overturned")
                {
                    outboundCorrespondence.ReCheckRequired = false;

                    if (reCheckCount != 0)
                    {
                        List<OutboundCorrespondenceReCheck> outboundCorrespondenceReChecks = db.OutboundCorrespondenceReCheck.Where(x => x.OutboundCorrespondenceId == challenge.CheckId).ToList();

                        foreach (OutboundCorrespondenceReCheck rc in outboundCorrespondenceReChecks)
                        {
                            db.OutboundCorrespondenceReCheck.Remove(rc);
                        }

                    }


                    db.SetModified(outboundCorrespondence);

                    db.SaveChanges();

                    //send to edit questions as QC Result will now have to be changed to a pass (challenge itelf saved at later stage)

                    this.Session["PreviousResult"] = db.CheckResults.Where(x => x.CheckId == outboundCorrespondence.CheckId).Select(c => c.Result.Text).FirstOrDefault();

                    return RedirectToAction("ChallengeQuestions", new { challenge.CheckId });
                }
                //no action required for result upheld - just save challenge for audit purposes
                else
                {
                    db.Challenge.Add(challenge);

                    db.SaveChanges();
                }

            }
            else
            {
                if (challengeOutcome.Text == "Original Result Overturned")
                {

                    //send to edit questions as QC Result will now have to be changed to a pass (challenge itelf saved at later stage)

                    return RedirectToAction("ChallengeQuestions", new { challenge.CheckId });

                }
                //no action required for result upheld - just save challenge for audit purposes
                else
                {
                    db.Challenge.Add(challenge);

                    db.SaveChanges();
                }
            }

            return RedirectToAction("Details", new { challenge.CheckId });

        }

        public PartialViewResult _ChallengeView(int CheckId)
        {
            Challenge challenge = db.Challenge.Include("ChallengeOutcome").Where(x => x.CheckId == CheckId).FirstOrDefault();

            return PartialView(challenge);
        }

        //GET:
        //Method to edit questions when challenge raised
        public ActionResult ChallengeQuestions(int CheckId)
        {
            OutboundCorrespondence outboundCorrespondence = db.OutboundCorrespondence.Where(x => x.CheckId == CheckId).FirstOrDefault();

            List<CheckQuestion> checkQuestions = db.CheckQuestions.Where(x => x.CheckType.Name == "Outbound Correspondence").OrderBy(c => c.Order).ToList();

            QuestionAnswerList qaList = new QuestionAnswerList();

            foreach (var cq in checkQuestions)
            {
                Answer answer = db.Answers.Where(x => x.CheckId == outboundCorrespondence.CheckId && x.QuestionId == cq.QuestionId).FirstOrDefault();

                QuestionAnswer qa = new QuestionAnswer
                {
                    CheckQuestion = cq,
                    Answer = answer.Text
                };

                qaList.QuestionAnswers.Add(qa);

            }

            ViewBag.AnswerOptions = ListHelper.YesNo();
            ViewBag.CheckId = outboundCorrespondence.CheckId;

            return View(qaList);
        }

        //POST:
        //Method to edit questions when challenge raised
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChallengeQuestions(int CheckId, QuestionAnswerList qaList)
        {

            if (ModelState.IsValid)
            {

                qaList = outboundCorrespondenceService.QCResultCalculate(qaList);
                this.Session["ChallengeQuestions"] = qaList;
                return RedirectToAction("ChallengeResult", new { CheckId });
            }

            ViewBag.AnswerOptions = ListHelper.YesNo();
            return View(qaList);
        }

        //GET:
        public ActionResult ChallengeResult(int CheckId)
        {

            QuestionAnswerList qaList = this.Session["ChallengeQuestions"] as QuestionAnswerList;

            OutboundCorrespondence outboundCorrespondence = db.OutboundCorrespondence.AsNoTracking().Where(x => x.CheckId == CheckId).FirstOrDefault();

            this.Session["ChallengeGeneral"] = outboundCorrespondence;

            ViewBag.QCResult = qaList.QCResult;

            this.Session["ChallengeQuestions"] = qaList;

            return View(outboundCorrespondence);
        }

        //POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChallengeResult(OutboundCorrespondence outboundCorrespondence)
        {
            //Challenge check - if there has been a successful challenge ensure that the QC Result is now a pass

            //obtain existing models from session data

            QuestionAnswerList qaList = this.Session["ChallengeQuestions"] as QuestionAnswerList;
            OutboundCorrespondence outboundCorrespondenceSaved = this.Session["ChallengeGeneral"] as OutboundCorrespondence;

            if (ModelState.IsValid)
            {

                outboundCorrespondenceSaved.ReCheckRequired = false;
                outboundCorrespondenceSaved.InitialReCheckDecision = false;
                outboundCorrespondenceSaved.FailReasonId = null;

                this.Session["ChallengeGeneral"] = outboundCorrespondenceSaved;

                return RedirectToAction("ChallengeAmendmentReason", new { outboundCorrespondenceSaved.CheckId });

            }
            else
            {
                SetViewData(outboundCorrespondence);
                ViewBag.QCResult = qaList.QCResult;
                return View(outboundCorrespondence);
            }
        }

        //GET:
        public ActionResult ChallengeAmendmentReason(int CheckId)
        {

            CheckAmendmentReason checkAmendmentReason = new CheckAmendmentReason
            {
                AmendmentReasonId = db.AmendmentReasons.Where(x => x.Text == "Challenge Outcome").Select(p => p.AmendmentReasonId).FirstOrDefault(),
                CheckId = CheckId
            };

            bool challengeAmend = true;

            SetAmendViewData(checkAmendmentReason, challengeAmend);

            return View(checkAmendmentReason);
        }

        //POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChallengeAmendmentReason(CheckAmendmentReason checkAmendmentReason)
        {
            QuestionAnswerList qaList = this.Session["ChallengeQuestions"] as QuestionAnswerList;

            OutboundCorrespondence outboundCorrespondence = this.Session["ChallengeGeneral"] as OutboundCorrespondence;

            checkAmendmentReason.AmendmentReason = db.AmendmentReasons.Where(x => x.AmendmentReasonId == checkAmendmentReason.AmendmentReasonId).FirstOrDefault();

            if (qaList.QCResult == "Not Approved")
            {
                ModelState.AddModelError("AmendmentReasonComments", "As a successful challenge has been made QC Result must be approved - please return to questions and review");
            }

            if (ModelState.IsValid)
            {
                string previousResult = this.Session["PreviousResult"] as string;

                qaList.Challenge = true;

                outboundCorrespondenceService.SaveEditOutboundCorrespondence(outboundCorrespondence, qaList, previousResult);

                auditService.SaveOutboundCorrespondenceAmendmentAudit(outboundCorrespondence, checkAmendmentReason);

                checkAmendmentReason.CheckId = outboundCorrespondence.CheckId;

                db.CheckAmendmentReason.Add(checkAmendmentReason);
                db.SaveChanges();

                return RedirectToAction("Details", "OutboundCorrespondence", new { outboundCorrespondence.CheckId });
            }
            else
            {
                bool challengeAmend = true;
                SetAmendViewData(checkAmendmentReason, challengeAmend);
                ViewBag.QCResult = qaList.QCResult;
                return View(checkAmendmentReason);
            }

        }

        //GET:
        public ActionResult DeleteView(int CheckId)
        {
            Details detailView = new Details
            {
                OutboundCorrespondence = db.OutboundCorrespondence.Where(x => x.CheckId == CheckId).FirstOrDefault(),
                QCResult = db.CheckResults.Where(x => x.CheckId == CheckId).Select(p => p.Result.Text).FirstOrDefault(),
                Answers = db.Answers.Where(x => x.CheckId == CheckId).OrderBy(p => p.QuestionId).ToList(),
                CheckAmendmentReason = db.CheckAmendmentReason.Where(x => x.CheckId == CheckId).OrderByDescending(x => x.AmendmentReasonId).FirstOrDefault(),
                Challenge = db.Challenge.Any(x => x.CheckId == CheckId)
            };

            return View(detailView);
        }

        //POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int CheckId)
        {
            outboundCorrespondenceService.DeleteCheck(CheckId);

            return RedirectToAction("Index");
        }
    }
}
