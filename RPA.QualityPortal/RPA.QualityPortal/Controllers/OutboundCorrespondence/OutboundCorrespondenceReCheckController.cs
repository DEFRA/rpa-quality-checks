using RPA.QualityPortal.DAL;
using RPA.QualityPortal.Helpers;
using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.Services;
using RPA.QualityPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace RPA.QualityPortal.Controllers
{
    [Authorize(Roles = "Quality Checks: OC Team Member, Quality Checks: OC Team Manager")]
    public class OutboundCorrespondenceReCheckController : Controller
    {
        IQualityContext db;
        IPeopleContext pdb;

        IAnswerService answerService;
        IMessageService messageService;
        ISendService sendService;
        IAuditService auditService;
        IPeopleService peopleService;
        IUserHelper userHelper;
        IRoleManager roleManager;
        IFilterService filterService;
        IOutboundCorrespondenceService outboundCorrespondenceService;
        ILockService lockService;

        public OutboundCorrespondenceReCheckController()
        {
            db = new QualityContext();
            pdb = new PeopleContext();
            answerService = new AnswerService(db);
            sendService = new SendService();
            messageService = new EmailService(db, pdb);
            auditService = new AuditService(db);
            peopleService = new PeopleService(pdb);
            userHelper = new UserHelper(pdb);
            roleManager = new RoleManager();
            filterService = new FilterService(db, pdb, userHelper, roleManager);
            lockService = new LockService(db, pdb, userHelper);
            outboundCorrespondenceService = new OutboundCorrespondenceService(db, answerService, userHelper, lockService);      
        }

        public OutboundCorrespondenceReCheckController(IQualityContext context, IPeopleContext peopleContext, IOutboundCorrespondenceService outboundCorrespondenceService, IMessageService messageService, ISendService sendService, IAuditService auditService, IPeopleService peopleService, IUserHelper userHelper, IRoleManager roleManager, IFilterService filterService, ILockService lockService)
        {
            this.db = context;
            this.pdb = peopleContext;
            this.outboundCorrespondenceService = outboundCorrespondenceService;
            this.sendService = sendService;
            this.messageService = messageService;
            this.auditService = auditService;
            this.peopleService = peopleService;
            this.userHelper = userHelper;
            this.roleManager = roleManager;
            this.filterService = filterService;
            this.lockService = lockService;
        }

        public ActionResult CreateReCheckQuestions(int CheckId)
        {
            if (lockService.IsCheckLocked(CheckId))
            {
                return RedirectToAction("Index", "OutboundCorrespondence", new { locked = true } );
            }

            lockService.CreateCheckLock(CheckId);

            ViewBag.AnswerOptions = ListHelper.YesNo();
            ViewBag.CheckId = CheckId;

            OutboundCorrespondenceReCheck outboundCorrespondenceReCheck = new OutboundCorrespondenceReCheck(CheckId);

            QuestionAnswerList qaListExisting = this.Session["CreateReCheckQuestions"] as QuestionAnswerList;
            OutboundCorrespondence outboundCorrespondence = db.OutboundCorrespondence.Where(x => x.CheckId == CheckId).FirstOrDefault();

            if (qaListExisting == null)
            {
                CreateListReCheckQuestions(outboundCorrespondence);
            }

            return View(qaListExisting);

        }

        public ActionResult CreateListReCheckQuestions(OutboundCorrespondence outboundCorrespondence)
        {
            List<CheckQuestion> checkQuestions = db.CheckQuestions.Where(x => x.CheckType.Name == "Outbound Correspondence ReCheck").OrderBy(c => c.Order).ToList();

            QuestionAnswerList qaList = new QuestionAnswerList();

            foreach (var cq in checkQuestions)
            {
                QuestionAnswer qa = new QuestionAnswer
                {
                    CheckQuestion = cq
                };

                qaList.QuestionAnswers.Add(qa);
            }

            if (outboundCorrespondence.CorrespondenceType.Text == "Letter - Template" || outboundCorrespondence.CorrespondenceType.Text == "Letter - Bespoke")
            {

                qaList.QuestionAnswers.Where(x => x.CheckQuestion.Order == 2).FirstOrDefault().Answer = "N/A";
                ViewBag.Letter = "Letter";
            }

            return View("CreateReCheckQuestions", qaList);
        }

        public PartialViewResult _CheckAnswer(int CheckId, int QuestionId)
        {
            QuestionAnswer qa = new QuestionAnswer
            {
                CheckQuestion = db.CheckQuestions.Where(x => x.QuestionId == QuestionId).FirstOrDefault(),
                Answer = db.Answers.Where(x => x.CheckId == CheckId && x.QuestionId == QuestionId).Select(c => c.Text).FirstOrDefault()
            };

            return PartialView(qa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateReCheckQuestions(int CheckId, QuestionAnswerList qaList)
        {
            if (lockService.IsCheckLocked(CheckId))
            {
                return RedirectToAction("Index", "OutboundCorrespondence", new { locked = true });
            }

            List<OutboundCorrespondenceReCheck> outboundCorrespondenceReChecks = db.OutboundCorrespondenceReCheck.Where(x => x.OutboundCorrespondenceId == CheckId).ToList();

            if (qaList != null)
            {
                qaList = outboundCorrespondenceService.QCResultCalculate(qaList);
            }

            if (outboundCorrespondenceReChecks.Count == 2)
            {
                if (qaList.QCResult == "Not Approved")
                {
                    ModelState.AddModelError("QCResult", "This is the third ReCheck. The QC result must be approved.");
                }
            }

            if (ModelState.IsValid)
            {
                this.Session["CreateReCheckQuestions"] = qaList;
                return RedirectToAction("CreateReCheckResult", new { CheckId });
            }

            ViewBag.AnswerOptions = ListHelper.YesNo();
            ViewBag.CheckId = CheckId;

            return View(qaList);
        }

        public ActionResult CreateReCheckResult(int CheckId)
        {
            OutboundCorrespondence existingOutboundCorrespondence = db.OutboundCorrespondence.Where(x => x.CheckId == CheckId).FirstOrDefault();

            OutboundCorrespondenceReCheck outboundCorrespondenceReCheck = new OutboundCorrespondenceReCheck
            {
                PersonName = existingOutboundCorrespondence.PersonName,
                QCCompletedByName = existingOutboundCorrespondence.QCCompletedByName,
                DateQCCompleted = existingOutboundCorrespondence.DateQCCompleted,
                OutboundCorrespondenceId = existingOutboundCorrespondence.CheckId,
                ReCheckCompletedBy = userHelper.CurrentUser(),
            };

            QuestionAnswerList qaList = this.Session["CreateReCheckQuestions"] as QuestionAnswerList;

            if (qaList != null)
            {
                if (qaList.QCResult == "Not Approved")
                {
                    outboundCorrespondenceReCheck.FurtherReCheckRequired = true;
                }

                ViewBag.Email = db.Control.Where(x => x.Property == "OutboundCorrespondenceEmail").Select(c => c.Active).FirstOrDefault();
                ViewBag.ReCheckResult = qaList.QCResult;
                ViewBag.CheckId = CheckId;

                this.Session["CreateReCheckQuestions"] = qaList;

                return View(outboundCorrespondenceReCheck);
            }

            return RedirectToAction("Index", "OutboundCorrespondence");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateReCheckResult(int CheckId, OutboundCorrespondenceReCheck outboundCorrespondenceReCheck)
        {
            if (lockService.IsCheckLocked(CheckId))
            {
                return RedirectToAction("Index", "OutboundCorrespondence", new { locked = true });
            }

            QuestionAnswerList qaList = this.Session["CreateReCheckQuestions"] as QuestionAnswerList;

            OutboundCorrespondenceReCheck outboundCorrespondenceReCheckSaved = this.Session["CreateReCheckResult"] as OutboundCorrespondenceReCheck;

            bool email = db.Control.Where(x => x.Property == "OutboundCorrespondenceEmail").Select(c => c.Active).FirstOrDefault();

            if (qaList != null)
            {
                if (ModelState.IsValid)
                {                  
                    if (email != true)
                    {
                        outboundCorrespondenceReCheck.EmailNotifications = false;
                    }

                    outboundCorrespondenceService.SaveOutboundCorrespondenceReCheck(outboundCorrespondenceReCheck, qaList);

                    if (outboundCorrespondenceReCheck.EmailNotifications == true)
                    {
                        MailMessage mail = messageService.GenerateOutboundCorrespondenceEmail(db.OutboundCorrespondence.Where(x => x.CheckId == CheckId).FirstOrDefault());

                        sendService.sendEmail(mail);
                    }

                    this.Session.Clear();

                    return RedirectToAction("DetailsReCheck", new { CheckId });
                }
                else
                {
                    ViewBag.QCResult = qaList.QCResult;
                    return View(outboundCorrespondenceReCheck);
                }
            }

            return RedirectToAction("Index", "OutboundCorrespondence");
        }

        public ActionResult DetailsReCheck(int CheckId)
        {
            int ReCheckId = db.OutboundCorrespondenceReCheck.Where(x => x.OutboundCorrespondenceId == CheckId && x.ReCheckActive == true).Select(x => x.CheckId).FirstOrDefault();

            int reCheckCount = db.OutboundCorrespondenceReCheck.Where(x => x.OutboundCorrespondenceId == CheckId).Count();

            ViewBag.ReCheckCount = reCheckCount;

            DetailsReCheck detailsReCheck = new DetailsReCheck
            {
                OutboundCorrespondence = db.OutboundCorrespondence.Where(x => x.CheckId == CheckId).FirstOrDefault(),
                QCResult = db.CheckResults.Where(x => x.CheckId == CheckId).Select(p => p.Result.Text).FirstOrDefault(),
                Answers = db.Answers.Where(x => x.CheckId == CheckId).OrderBy(p => p.QuestionId).ToList(),
                OutboundCorrespondenceReCheck = db.OutboundCorrespondenceReCheck.Where(x => x.CheckId == ReCheckId).FirstOrDefault(),
                ReCheckAnswers = db.Answers.Where(x => x.CheckId == ReCheckId).OrderBy(p => p.QuestionId).ToList(),
                ReCheckResult = db.CheckResults.Where(x => x.CheckId == ReCheckId).Select(p => p.Result.Text).FirstOrDefault(),
                CheckAmendmentReason = db.CheckAmendmentReason.Where(x => x.CheckId == CheckId).OrderByDescending(x => x.CheckAmendmentReasonId).FirstOrDefault(),
                ReCheckAmendmentReason = db.CheckAmendmentReason.Where(x => x.CheckId == ReCheckId).OrderByDescending(x => x.CheckAmendmentReasonId).FirstOrDefault(),
                Challenge = db.Challenge.Any(x => x.CheckId == CheckId)
            };


            return View(detailsReCheck);
        }

        public ActionResult DetailsReChecks(int CheckId)
        {
            List<int> outboundCorrespondenceReChecksIds = db.OutboundCorrespondenceReCheck.Where(x => x.OutboundCorrespondenceId == CheckId).OrderBy(c => c.DateQCCompleted).Select(c => c.CheckId).ToList();

            int FirstReCheckId = outboundCorrespondenceReChecksIds.FirstOrDefault();
            int SecondReCheckId = outboundCorrespondenceReChecksIds[1];
            int? ThirdReCheckId = null;

            if(outboundCorrespondenceReChecksIds.Count > 2)
            {
                ThirdReCheckId = outboundCorrespondenceReChecksIds[2];
            }

            DetailsReChecks detailsReChecks = new DetailsReChecks
            {
                OutboundCorrespondence = db.OutboundCorrespondence.Where(x => x.CheckId == CheckId).FirstOrDefault(),
                QCResult = db.CheckResults.Where(x => x.CheckId == CheckId).Select(p => p.Result.Text).FirstOrDefault(),
                Answers = db.Answers.Where(x => x.CheckId == CheckId).OrderBy(p => p.QuestionId).ToList(),
                CheckAmendmentReason = db.CheckAmendmentReason.Where(x => x.CheckId == CheckId).OrderByDescending(x => x.CheckAmendmentReasonId).FirstOrDefault(),
                Challenge = db.Challenge.Any(x => x.CheckId == CheckId),

                FirstOutboundCorrespondenceReCheck = db.OutboundCorrespondenceReCheck.Where(x => x.CheckId == FirstReCheckId).FirstOrDefault(),
                FirstReCheckAnswers = db.Answers.Where(x => x.CheckId == FirstReCheckId).OrderBy(p => p.QuestionId).ToList(),
                FirstReCheckResult = db.CheckResults.Where(x => x.CheckId == FirstReCheckId).Select(p => p.Result.Text).FirstOrDefault(),
                FirstReCheckAmendmentReason = db.CheckAmendmentReason.Where(x => x.CheckId == FirstReCheckId).OrderByDescending(x => x.CheckAmendmentReasonId).FirstOrDefault(),

                SecondOutboundCorrespondenceReCheck = db.OutboundCorrespondenceReCheck.Where(x => x.CheckId == SecondReCheckId).FirstOrDefault(),
                SecondReCheckAnswers = db.Answers.Where(x => x.CheckId == SecondReCheckId).OrderBy(p => p.QuestionId).ToList(),
                SecondReCheckResult = db.CheckResults.Where(x => x.CheckId == SecondReCheckId).Select(p => p.Result.Text).FirstOrDefault(),
                SecondReCheckAmendmentReason = db.CheckAmendmentReason.Where(x => x.CheckId == SecondReCheckId).OrderByDescending(x => x.CheckAmendmentReasonId).FirstOrDefault(),

                ThirdOutboundCorrespondenceReCheck = db.OutboundCorrespondenceReCheck.Where(x => x.CheckId == ThirdReCheckId).FirstOrDefault(),
                ThirdReCheckAnswers = db.Answers.Where(x => x.CheckId == ThirdReCheckId).OrderBy(p => p.QuestionId).ToList(),
                ThirdReCheckResult = db.CheckResults.Where(x => x.CheckId == ThirdReCheckId).Select(p => p.Result.Text).FirstOrDefault(),
                ThirdReCheckAmendmentReason = db.CheckAmendmentReason.Where(x => x.CheckId == ThirdReCheckId).OrderByDescending(x => x.CheckAmendmentReasonId).FirstOrDefault(),
            };

            return View(detailsReChecks);
        }

        //GET:
        public ActionResult EditReCheckQuestions(int ReCheckId)
        {
            QuestionAnswerList qaListExisting = this.Session["EditReCheckQuestions"] as QuestionAnswerList;

            ViewBag.AnswerOptions = ListHelper.YesNo();
            ViewBag.ReCheckId = ReCheckId;

            if (qaListExisting == null)
            {
                List<CheckQuestion> checkQuestions = db.CheckQuestions.Where(x => x.CheckType.Name == "Outbound Correspondence ReCheck").OrderBy(c => c.Order).ToList();

                QuestionAnswerList qaList = new QuestionAnswerList();

                foreach (var cq in checkQuestions)
                {
                    Answer answer = db.Answers.Where(x => x.CheckId == ReCheckId && x.QuestionId == cq.QuestionId).FirstOrDefault();

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
        public ActionResult EditReCheckQuestions(int ReCheckId, QuestionAnswerList qaList)
        {
            OutboundCorrespondenceReCheck outboundCorrespondenceReCheck = db.OutboundCorrespondenceReCheck.Where(x => x.CheckId == ReCheckId).FirstOrDefault();
            OutboundCorrespondence outboundCorrespondence = db.OutboundCorrespondence.Where(x => x.CheckId == outboundCorrespondenceReCheck.OutboundCorrespondenceId).FirstOrDefault();

            List<OutboundCorrespondenceReCheck> outboundCorrespondenceReChecks = db.OutboundCorrespondenceReCheck.Where(x => x.OutboundCorrespondenceId == outboundCorrespondence.CheckId).ToList();

            qaList = outboundCorrespondenceService.QCResultCalculate(qaList);

            if (outboundCorrespondenceReChecks.Count == 3)
            {
                if (qaList.QCResult == "Not Approved")
                {
                    ModelState.AddModelError("QCResult", "This is the third ReCheck. The QC result must be approved.");
                }
            }

            if (ModelState.IsValid)
            {
                this.Session["EditReCheckQuestions"] = qaList;
                return RedirectToAction("EditReCheckResult", new { ReCheckId });
            }

            ViewBag.AnswerOptions = ListHelper.YesNo();
            return View(qaList);
        }

        //GET:
        public ActionResult EditReCheckResult(int ReCheckId)
        {
            QuestionAnswerList qaList = this.Session["EditReCheckQuestions"] as QuestionAnswerList;

            qaList = outboundCorrespondenceService.QCResultCalculate(qaList);

            ViewBag.ReCheckResult = qaList.QCResult;

            this.Session["EditReCheckQuestions"] = qaList;

            OutboundCorrespondenceReCheck outboundCorrespondenceReCheck = db.OutboundCorrespondenceReCheck.Where(x => x.CheckId == ReCheckId).OrderByDescending(c => c.DateQCCompleted).FirstOrDefault();

            OutboundCorrespondenceReCheck outboundCorrespondenceReCheckExisting = this.Session["EditReCheckResult"] as OutboundCorrespondenceReCheck;

            if (outboundCorrespondenceReCheckExisting != null)
            {
                outboundCorrespondenceReCheck = outboundCorrespondenceReCheckExisting;
            }

            return View(outboundCorrespondenceReCheck);
        }

        // POST:
        [HttpPost]
        public ActionResult EditReCheckResult(OutboundCorrespondenceReCheck outboundCorrespondenceReCheck)
        {
            QuestionAnswerList qaList = this.Session["EditReCheckQuestions"] as QuestionAnswerList;

            bool reCheckNameCheck = peopleService.PersonCheck(outboundCorrespondenceReCheck.ReCheckCompletedBy);

            if (!reCheckNameCheck)
            {
                ModelState.AddModelError("PersonName", "Please Select a Valid Name From List");
            }

            if (ModelState.IsValid)
            {
                this.Session["EditReCheckResult"] = outboundCorrespondenceReCheck;

                return RedirectToAction("AmendmentReasonReCheck");
            }
            else
            {
                ViewBag.QCResult = qaList.QCResult;
                return View(outboundCorrespondenceReCheck);
            }
        }

        [HttpGet]
        public ActionResult AmendmentReasonReCheck()
        {
            OutboundCorrespondenceReCheck outboundCorrespondenceReCheck = this.Session["EditReCheckResult"] as OutboundCorrespondenceReCheck;

            ViewBag.ReCheckId = outboundCorrespondenceReCheck.CheckId;

            QuestionAnswerList qaList = this.Session["EditReCheckQuestions"] as QuestionAnswerList;

            qaList = outboundCorrespondenceService.QCResultCalculate(qaList);

            this.Session["EditReCheckQuestions"] = qaList;

            this.Session["EditReCheckResult"] = outboundCorrespondenceReCheck;

            CheckAmendmentReason checkAmendmentReason = new CheckAmendmentReason();

            SetAmendViewData(checkAmendmentReason);

            return View(checkAmendmentReason);
        }

        [HttpPost]
        public ActionResult AmendmentReasonReCheck(CheckAmendmentReason checkAmendmentReason)
        {
            QuestionAnswerList qaList = this.Session["EditReCheckQuestions"] as QuestionAnswerList;

            OutboundCorrespondenceReCheck outboundCorrespondenceReCheck = this.Session["EditReCheckResult"] as OutboundCorrespondenceReCheck;

            checkAmendmentReason.AmendmentReason = db.AmendmentReasons.Where(x => x.AmendmentReasonId == checkAmendmentReason.AmendmentReasonId).FirstOrDefault();

            if (checkAmendmentReason.AmendmentReason == null)
            {
                ModelState.AddModelError("AmendmentReason", "An amendment reason must be selected - if no amendments have been made please return to details screen");
            }

            if (ModelState.IsValid)
            {
                outboundCorrespondenceService.SaveEditOutboundCorrespondenceReCheck(outboundCorrespondenceReCheck, qaList);

                auditService.SaveOutboundCorrespondenceReCheckAmendmentAudit(outboundCorrespondenceReCheck, checkAmendmentReason);

                checkAmendmentReason.CheckId = outboundCorrespondenceReCheck.CheckId;

                db.CheckAmendmentReason.Add(checkAmendmentReason);
                db.SaveChanges();

                return RedirectToAction("DetailsReCheck", "OutboundCorrespondenceReCheck", new { CheckId = outboundCorrespondenceReCheck.OutboundCorrespondenceId });
            }
            else
            {
                SetAmendViewData(checkAmendmentReason);
                ViewBag.QCResult = qaList.QCResult;
                return View(checkAmendmentReason);
            }
        }

        private void SetAmendViewData(CheckAmendmentReason checkAmendmentReason)
        {
            ViewBag.AmendmentReasonId = new SelectList(db.AmendmentReasons.AsNoTracking().Where(x => x.Active == true && x.Text != "Challenge Outcome").OrderBy(x => x.Text), "AmendmentReasonId", "Text", checkAmendmentReason.AmendmentReasonId);
        }

        //Get
        public ActionResult DeleteReCheckView(int CheckId)
        {

            int ReCheckId = db.OutboundCorrespondenceReCheck.Where(x => x.OutboundCorrespondenceId == CheckId && x.ReCheckActive == true).Select(x => x.CheckId).FirstOrDefault();


            DetailsReCheck detailsReCheck = new DetailsReCheck
            {
                OutboundCorrespondence = db.OutboundCorrespondence.Where(x => x.CheckId == CheckId).FirstOrDefault(),
                QCResult = db.CheckResults.Where(x => x.CheckId == CheckId).Select(p => p.Result.Text).FirstOrDefault(),
                Answers = db.Answers.Where(x => x.CheckId == CheckId).OrderBy(p => p.QuestionId).ToList(),
                OutboundCorrespondenceReCheck = db.OutboundCorrespondenceReCheck.Where(x => x.CheckId == ReCheckId).FirstOrDefault(),
                ReCheckAnswers = db.Answers.Where(x => x.CheckId == ReCheckId).OrderBy(p => p.QuestionId).ToList(),
                ReCheckResult = db.CheckResults.Where(x => x.CheckId == ReCheckId).Select(p => p.Result.Text).FirstOrDefault(),
                CheckAmendmentReason = db.CheckAmendmentReason.Where(x => x.CheckId == CheckId).OrderByDescending(x => x.CheckAmendmentReasonId).FirstOrDefault(),
                ReCheckAmendmentReason = db.CheckAmendmentReason.Where(x => x.CheckId == ReCheckId).OrderByDescending(x => x.CheckAmendmentReasonId).FirstOrDefault(),
                Challenge = db.Challenge.Any(x => x.CheckId == CheckId)
            };


            return View(detailsReCheck);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteReCheck(int CheckId)
        {
            outboundCorrespondenceService.DeleteReCheck(CheckId);

            return RedirectToAction("Details", "OutboundCorrespondence", new { CheckId });
        }
    }
}