using RPA.QualityPortal.DAL;
using RPA.QualityPortal.Helpers;
using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.Services;
using RPA.QualityPortal.ViewModels;
using RPA.QualityPortal.ViewModels.BankAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace RPA.QualityPortal.Controllers.BankAccount
{
    [Authorize(Roles = "Quality Checks: BA Team Member")]
    public class BankAccountReCheckController : Controller
    {
        IQualityContext db;
        IPeopleContext pdb;

        IAnswerService answerService;
        IMessageService messageService;
        ISendService sendService;
        IPeopleService peopleService;
        IUserHelper userHelper;
        IRoleManager roleManager;
        IBankAccountService bankAccountService;
        ILockService lockService;
        IAuditService auditService;

        public BankAccountReCheckController()
        {
            db = new QualityContext();
            pdb = new PeopleContext();

            answerService = new AnswerService(db);
            messageService = new EmailService(db, pdb);
            sendService = new SendService();
            peopleService = new PeopleService(pdb);
            userHelper = new UserHelper(pdb);
            roleManager = new RoleManager();
            lockService = new LockService(db, pdb, userHelper);
            bankAccountService = new BankAccountService(db, answerService, lockService);
            auditService = new AuditService(db);
        }

        public BankAccountReCheckController(IQualityContext context, IPeopleContext peopleContext, IAnswerService answerService, IMessageService messageService, ISendService sendService, IPeopleService peopleService, IUserHelper userHelper, IRoleManager roleManager, ILockService lockService, IBankAccountService bankAccountService, IAuditService auditService)
        {
            this.db = context;
            this.pdb = peopleContext;

            this.answerService = answerService;
            this.messageService = messageService;
            this.sendService = sendService;
            this.peopleService = peopleService;
            this.userHelper = userHelper;
            this.roleManager = roleManager;
            this.lockService = lockService;
            this.bankAccountService = bankAccountService;
            this.auditService = auditService;
        }

        public ActionResult CreateBankReCheckQuestions(int CheckId)
        {
            if (lockService.IsCheckLocked(CheckId))
            {
                return RedirectToAction("Index", "BankAccount", new { locked = true });
            }

            lockService.CreateCheckLock(CheckId);

            ViewBag.AnswerOptions = ListHelper.YesNo();
            ViewBag.CheckId = CheckId;

            BankAccountReCheck bankAccountReCheck = new BankAccountReCheck(CheckId);

            QuestionAnswerList qaListExisting = this.Session["CreateBankRecheckQuestions"] as QuestionAnswerList;
            BankAccountCheck bankAccount = db.BankAccounts.Where(x => x.CheckId == CheckId).FirstOrDefault();

            if (qaListExisting == null)
            {
                CreateListReCheckQuestions();
            }

            return View(qaListExisting);
        }

        public ActionResult CreateListReCheckQuestions()
        {
            List<CheckQuestion> checkQuestions = db.CheckQuestions.Where(x => x.CheckType.Name == "Bank Account ReCheck").OrderBy(c => c.Order).ToList();

            QuestionAnswerList qaList = new QuestionAnswerList();

            foreach (var cq in checkQuestions)
            {
                QuestionAnswer qa = new QuestionAnswer
                {
                    CheckQuestion = cq
                };

                qaList.QuestionAnswers.Add(qa);
            }

            return View("CreateBankReCheckQuestions", qaList);
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

        public PartialViewResult _CheckComment(int CheckId, int QuestionId)
        {
            BankAccountComments comment = new BankAccountComments
            {
                AnswerComment = db.BankAccountComments.Where(x => x.CheckId == CheckId && x.QuestionId == QuestionId).Select(c => c.AnswerComment).FirstOrDefault() ?? ""
            };

            return PartialView(comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBankReCheckQuestions(int CheckId, QuestionAnswerList qaList)
        {
            if (lockService.IsCheckLocked(CheckId))
            {
                return RedirectToAction("Index", "BankAccount", new { locked = true });
            }

            this.Session["CreateBankReCheckQuestions"] = qaList;
            return RedirectToAction("CreateBankReCheckResult", new { CheckId });
        }

        public ActionResult CreateBankReCheckResult(int CheckId)
        {
            BankAccountCheck existingBankAccount = db.BankAccounts.Where(x => x.CheckId == CheckId).FirstOrDefault();

            BankAccountReCheck bankAccountReCheck = new BankAccountReCheck
            {
                PersonName = existingBankAccount.PersonName,
                QCCompletedByName = existingBankAccount.QCCompletedByName,
                DateQCCompleted = existingBankAccount.DateQCCompleted,
                BankAccountId = existingBankAccount.CheckId,
                ReCheckCompletedBy = userHelper.CurrentUser(),
            };

            QuestionAnswerList qaList = this.Session["CreateBankReCheckQuestions"] as QuestionAnswerList;

            if (qaList != null)
            {
                ViewBag.CheckId = CheckId;
                SetViewData(existingBankAccount);

                ViewBag.Email = db.Control.Where(x => x.Property == "BankEmail").Select(c => c.Active).FirstOrDefault();

                this.Session["CreateBankReCheckResult"] = qaList;

                return View(bankAccountReCheck);
            }

            return RedirectToAction("GeneralDetails", "BankAccount");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBankReCheckResult(int CheckId, BankAccountReCheck bankAccountReCheck, string result)
        {
            if (lockService.IsCheckLocked(CheckId))
            {
                return RedirectToAction("Index", "BankAccount", new { locked = true });
            }

            QuestionAnswerList qaList = this.Session["CreateBankReCheckQuestions"] as QuestionAnswerList;

            BankAccountReCheck bankAccountReCheckSaved = this.Session["CreateBankReCheckResult"] as BankAccountReCheck;

            bool email = db.Control.Where(x => x.Property == "BankEmail").Select(c => c.Active).FirstOrDefault();

            List<BankAccountReCheck> bankAccountReChecks = db.BankAccountReChecks.Where(x => x.BankAccountId == CheckId).ToList();

            if (bankAccountReChecks.Count == 2)
            {
                if (result == "Not Approved")
                {
                    ModelState.AddModelError("Result", "This is the third ReCheck. The QC result must be approved.");
                }
            }

            if (qaList != null)
            {
                if (result == "Not Approved" && bankAccountReCheck.FailReasonId == null)
                {
                    ModelState.AddModelError("FailReasonId", "The not approved Reason is required.");
                }

                if (result != "Approved" && bankAccountReCheck.ReCheckComments == null)
                {
                    ModelState.AddModelError("Comments", "As the result is not approved, the reason(s) must be stated.");
                }

                if (ModelState.IsValid)
                {
                    if (email != true)
                    {
                        bankAccountReCheck.EmailNotifications = false;
                    }

                    qaList.QCResult = result;

                    bankAccountService.SaveBankAccountReCheck(bankAccountReCheck, qaList);

                    Session.Clear();

                    if (bankAccountReCheck.EmailNotifications == true)
                    {
                        MailMessage mail = messageService.GenerateBankEmail(db.BankAccounts.Where(x => x.CheckId == CheckId).FirstOrDefault());

                        sendService.sendEmail(mail);
                    }

                    return RedirectToAction("BankAccountDetailsReCheck", new { CheckId });
                }
                else
                {
                    BankAccountCheck existingBankAccount = db.BankAccounts.Where(x => x.CheckId == CheckId).FirstOrDefault();
                    SetViewData(existingBankAccount);
                    ViewBag.Email = db.Control.Where(x => x.Property == "BankEmail").Select(c => c.Active).FirstOrDefault();
                    return View(bankAccountReCheck);
                }
            }

            return RedirectToAction("GeneralDetails", "BankAccount");
        }

        //GET:
        public ActionResult EditBankReCheckQuestions(int ReCheckId)
        {
            QuestionAnswerList qaListExisting = this.Session["EditBankReCheckQuestions"] as QuestionAnswerList;

            ViewBag.CheckId = ReCheckId;

            if (qaListExisting == null)
            {
                List<CheckQuestion> checkQuestions = db.CheckQuestions.Where(x => x.CheckType.Name == "Bank Account ReCheck").OrderBy(c => c.Order).ToList();

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
                return View("EditBankReCheckQuestions", qaList);
            }

            return View("EditBankReCheckQuestions", qaListExisting);
        }

        // POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBankReCheckQuestions(int ReCheckId, QuestionAnswerList qaList)
        {
            this.Session["EditBankReCheckQuestions"] = qaList;
            return RedirectToAction("EditBankReCheckResult", new { ReCheckId });
        }

        //GET:
        public ActionResult EditBankReCheckResult(int ReCheckId)
        {
            QuestionAnswerList qaList = this.Session["EditBankReCheckQuestions"] as QuestionAnswerList;

            this.Session["EditBankReCheckQuestions"] = qaList;

            BankAccountReCheck bankAccountReCheck = db.BankAccountReChecks.Where(x => x.CheckId == ReCheckId).OrderByDescending(c => c.DateQCCompleted).FirstOrDefault();

            BankAccountReCheck bankAccountReCheckExisting = this.Session["EditBankReCheckResult"] as BankAccountReCheck;

            if (bankAccountReCheckExisting != null)
            {
                bankAccountReCheck = bankAccountReCheckExisting;
            }

            return View(bankAccountReCheck);
        }

        // POST:
        [HttpPost]
        public ActionResult EditBankReCheckResult(BankAccountReCheck bankAccountReCheck, string result)
        {
            QuestionAnswerList qaList = this.Session["EditBankReCheckQuestions"] as QuestionAnswerList;

            qaList.QCResult = result;

            this.Session["EditBankReCheckQuestions"] = qaList;

            bool reCheckNameCheck = peopleService.PersonCheck(bankAccountReCheck.ReCheckCompletedBy);

            List<BankAccountReCheck> bankAccountReChecks = db.BankAccountReChecks.Where(x => x.BankAccountId == bankAccountReCheck.BankAccountId).ToList();

            if (bankAccountReChecks.Count == 3)
            {
                if (result == "Not Approved")
                {
                    ModelState.AddModelError("Result", "This is the third ReCheck. The QC result must be approved.");
                }
            }

            if (qaList != null)
            {
                if (result == "Not Approved" && bankAccountReCheck.FailReasonId == null)
                {
                    ModelState.AddModelError("FailReasonId", "The not approved Reason is required.");
                }

                if (result != "Approved" && bankAccountReCheck.ReCheckComments == null)
                {
                    ModelState.AddModelError("Comments", "As the result is not approved, the reason(s) must be stated.");
                }

                if (!reCheckNameCheck)
                {
                    ModelState.AddModelError("PersonName", "Please Select a Valid Name From List");
                }
            }

            if (ModelState.IsValid)
            {
                this.Session["EditBankReCheckResult"] = bankAccountReCheck;

                return RedirectToAction("BankAmendmentReasonReCheck");
            }
            else
            {
                return View(bankAccountReCheck);
            }
        }

        [HttpGet]
        public ActionResult BankAmendmentReasonReCheck()
        {
            BankAccountReCheck bankAccountReCheck = this.Session["EditBankReCheckResult"] as BankAccountReCheck;

            ViewBag.ReCheckId = bankAccountReCheck.CheckId;

            CheckAmendmentReason checkAmendmentReason = new CheckAmendmentReason();

            SetAmendViewData(checkAmendmentReason);

            return View(checkAmendmentReason);
        }

        [HttpPost]
        public ActionResult BankAmendmentReasonReCheck(CheckAmendmentReason checkAmendmentReason)
        {
            QuestionAnswerList qaList = this.Session["EditBankReCheckQuestions"] as QuestionAnswerList;

            BankAccountReCheck bankAccountReCheck = this.Session["EditBankReCheckResult"] as BankAccountReCheck;

            checkAmendmentReason.AmendmentReason = db.AmendmentReasons.Where(x => x.AmendmentReasonId == checkAmendmentReason.AmendmentReasonId).FirstOrDefault();

            if (checkAmendmentReason.AmendmentReason == null)
            {
                ModelState.AddModelError("AmendmentReason", "An amendment reason must be selected - if no amendments have been made please return to details screen");
            }

            if (ModelState.IsValid)
            {
                bankAccountService.SaveEditBankAccountReCheck(bankAccountReCheck, qaList);

                auditService.SaveBankAccountReCheckAmendmentAudit(bankAccountReCheck, checkAmendmentReason);

                checkAmendmentReason.CheckId = bankAccountReCheck.CheckId;

                db.CheckAmendmentReason.Add(checkAmendmentReason);
                db.SaveChanges();

                return RedirectToAction("DetailsReCheck", "BankAccountReCheck", new { CheckId = bankAccountReCheck.BankAccountId });
            }
            else
            {
                SetAmendViewData(checkAmendmentReason);
                return View(checkAmendmentReason);
            }
        }

        private void SetAmendViewData(CheckAmendmentReason checkAmendmentReason)
        {
            ViewBag.AmendmentReasonId = new SelectList(db.AmendmentReasons.AsNoTracking().Where(x => x.Active == true && x.Text != "Challenge Outcome").OrderBy(x => x.Text), "AmendmentReasonId", "Text", checkAmendmentReason.AmendmentReasonId);
            ViewBag.Result = new SelectList(db.Results.AsNoTracking().OrderBy(x => x.ResultId), "Text", "Text");
        }

        public ActionResult BankAccountDetailsReCheck(int CheckId)
        {
            int ReCheckId = db.BankAccountReChecks.Where(x => x.BankAccountId == CheckId && x.ReCheckActive == true).Select(x => x.CheckId).FirstOrDefault();

            int reCheckCount = db.BankAccountReChecks.Where(x => x.BankAccountId == CheckId).Count();

            ViewBag.ReCheckCount = reCheckCount;

            BankAccountDetailsReCheck detailsReCheck = new BankAccountDetailsReCheck
            {
                BankAccount = db.BankAccounts.Where(x => x.CheckId == CheckId).FirstOrDefault(),
                QCResult = db.CheckResults.Where(x => x.CheckId == CheckId).Select(p => p.Result.Text).FirstOrDefault(),
                Answers = db.Answers.Where(x => x.CheckId == CheckId).OrderBy(p => p.QuestionId).ToList(),
                BankAccountReCheck = db.BankAccountReChecks.Where(x => x.CheckId == ReCheckId).FirstOrDefault(),
                ReCheckAnswers = db.Answers.Where(x => x.CheckId == ReCheckId).OrderBy(p => p.QuestionId).ToList(),
                ReCheckResult = db.CheckResults.Where(x => x.CheckId == ReCheckId).Select(p => p.Result.Text).FirstOrDefault(),
                Comments = db.BankAccountComments.Where(x => x.CheckId == CheckId).ToList(),
                ReCheckComments = db.BankAccountComments.Where(x => x.CheckId == ReCheckId).ToList(),
                CheckAmendmentReason = db.CheckAmendmentReason.Where(x => x.CheckId == CheckId).OrderByDescending(x => x.CheckAmendmentReasonId).FirstOrDefault(),
                ReCheckAmendmentReason = db.CheckAmendmentReason.Where(x => x.CheckId == ReCheckId).OrderByDescending(x => x.CheckAmendmentReasonId).FirstOrDefault(),
            };

            return View(detailsReCheck);
        }

        public ActionResult BankAccountDetailsReChecks(int CheckId)
        {
            List<int> bankAccountReCheckIds = db.BankAccountReChecks.Where(x => x.BankAccountId == CheckId).OrderBy(c => c.DateQCCompleted).Select(c => c.CheckId).ToList();

            int FirstReCheckId = bankAccountReCheckIds.FirstOrDefault();
            int SecondReCheckId = bankAccountReCheckIds[1];
            int? ThirdReCheckId = null;

            if (bankAccountReCheckIds.Count > 2)
            {
                ThirdReCheckId = bankAccountReCheckIds[2];
            }

            BankAccountDetailsReChecks detailsReChecks = new BankAccountDetailsReChecks
            {
                BankAccount = db.BankAccounts.Where(x => x.CheckId == CheckId).FirstOrDefault(),
                QCResult = db.CheckResults.Where(x => x.CheckId == CheckId).Select(p => p.Result.Text).FirstOrDefault(),
                Answers = db.Answers.Where(x => x.CheckId == CheckId).OrderBy(p => p.QuestionId).ToList(),
                CheckAmendmentReason = db.CheckAmendmentReason.Where(x => x.CheckId == CheckId).OrderByDescending(x => x.CheckAmendmentReasonId).FirstOrDefault(),
                Comments = db.BankAccountComments.Where(x => x.CheckId == CheckId).ToList(),

                FirstBankAccountReCheck = db.BankAccountReChecks.Where(x => x.CheckId == FirstReCheckId).FirstOrDefault(),
                FirstReCheckAnswers = db.Answers.Where(x => x.CheckId == FirstReCheckId).OrderBy(p => p.QuestionId).ToList(),
                FirstReCheckResult = db.CheckResults.Where(x => x.CheckId == FirstReCheckId).Select(p => p.Result.Text).FirstOrDefault(),
                FirstReCheckAmendmentReason = db.CheckAmendmentReason.Where(x => x.CheckId == FirstReCheckId).OrderByDescending(x => x.CheckAmendmentReasonId).FirstOrDefault(),
                FirstReCheckComments = db.BankAccountComments.Where(x => x.CheckId == FirstReCheckId).ToList(),

                SecondBankAccountReCheck = db.BankAccountReChecks.Where(x => x.CheckId == SecondReCheckId).FirstOrDefault(),
                SecondReCheckAnswers = db.Answers.Where(x => x.CheckId == SecondReCheckId).OrderBy(p => p.QuestionId).ToList(),
                SecondReCheckResult = db.CheckResults.Where(x => x.CheckId == SecondReCheckId).Select(p => p.Result.Text).FirstOrDefault(),
                SecondReCheckAmendmentReason = db.CheckAmendmentReason.Where(x => x.CheckId == SecondReCheckId).OrderByDescending(x => x.CheckAmendmentReasonId).FirstOrDefault(),
                SecondReCheckComments = db.BankAccountComments.Where(x => x.CheckId == SecondReCheckId).ToList(),

                ThirdBankAccountReCheck = db.BankAccountReChecks.Where(x => x.CheckId == ThirdReCheckId).FirstOrDefault(),
                ThirdReCheckAnswers = db.Answers.Where(x => x.CheckId == ThirdReCheckId).OrderBy(p => p.QuestionId).ToList(),
                ThirdReCheckResult = db.CheckResults.Where(x => x.CheckId == ThirdReCheckId).Select(p => p.Result.Text).FirstOrDefault(),
                ThirdReCheckAmendmentReason = db.CheckAmendmentReason.Where(x => x.CheckId == ThirdReCheckId).OrderByDescending(x => x.CheckAmendmentReasonId).FirstOrDefault(),
                ThirdReCheckComments = db.BankAccountComments.Where(x => x.CheckId == ThirdReCheckId).ToList(),
            };

            return View(detailsReChecks);
        }

        private void SetViewData(BankAccountCheck bankAccount)
        {
            ViewBag.BACheckTypeId = new SelectList(db.BACheckTypes.AsNoTracking().Where(x => x.Active == true), "BACheckTypeId", "Text", bankAccount.BACheckTypeId);
            ViewBag.FailReasonId = new SelectList(db.FailReasons.AsNoTracking().Where(x => x.Active == true && x.CheckTypeId == 3).OrderBy(x => x.Text), "FailReasonId", "Text", bankAccount.FailReasonId);
            ViewBag.Result = new SelectList(db.Results.AsNoTracking().OrderBy(x => x.ResultId), "Text", "Text");
        }
    }
}