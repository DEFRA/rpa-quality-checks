using PagedList;
using RPA.QualityPortal.DAL;
using RPA.QualityPortal.Helpers;
using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns;
using RPA.QualityPortal.Services;
using RPA.QualityPortal.ViewModels;
using RPA.QualityPortal.ViewModels.BankAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;

namespace RPA.QualityPortal.Controllers
{
    [Authorize(Roles = "Quality Checks: BA Team Member")]
    public class BankAccountController : Controller
    {
        IQualityContext db;
        IPeopleContext pdb;

        IUserHelper userHelper;
        IRoleManager roleManager;
        IFilterService filterService;
        IPeopleService peopleService;
        IAnswerService answerService;
        ILockService lockService;
        IBankAccountService bankAccountService;
        IAuditService auditService;
        IExportService exportService;
        IMessageService messageService;
        ISendService sendService;

        public BankAccountController()
        {
            db = new QualityContext();
            pdb = new PeopleContext();

            userHelper = new UserHelper(pdb);
            roleManager = new RoleManager();
            filterService = new FilterService(db, pdb, userHelper, roleManager);
            peopleService = new PeopleService(pdb);
            answerService = new AnswerService(db);
            lockService = new LockService(db, pdb, userHelper);
            bankAccountService = new BankAccountService(db, answerService, lockService);
            auditService = new AuditService(db);
            exportService = new ExportService(db);
            sendService = new SendService();
            messageService = new EmailService(db, pdb);
        }

        public BankAccountController(IQualityContext context, IPeopleContext peopleContext, IUserHelper userHelper, IRoleManager roleManager, IFilterService filterService, IPeopleService peopleService, IAnswerService answerService, ILockService lockService, IBankAccountService bankAccountService, IAuditService auditService, IExportService exportService)
        {
            db = context;
            pdb = peopleContext;

            this.userHelper = userHelper;
            this.roleManager = roleManager;
            this.filterService = filterService;
            this.peopleService = peopleService;
            this.answerService = answerService;
            this.lockService = lockService;
            this.bankAccountService = bankAccountService;
            this.auditService = auditService;
            this.exportService = exportService;
        }

        public ActionResult Index(bool locked = false, string searchString = null, string dateFrom = null, string dateTo = null, int page = 1, int pageSize = 50, string btnCoachingPoint = null)
        {
            bool? coachingPoint = CoachingPoint(btnCoachingPoint);

            ViewBag.CurrentSearch = searchString;
            ViewBag.DateFrom = dateFrom;
            ViewBag.DateTo = dateTo;

            if (locked)
            {
                ViewBag.Locked = "1";
            }

            return GetListViews(searchString, dateFrom, dateTo, page, pageSize, "All Quality Checks", coachingPoint);
        }

        public ActionResult Complete(string searchString = null, string dateFrom = null, string dateTo = null, int page = 1, int pageSize = 50, string btnCoachingPoint = null)
        {
            bool? coachingPoint = CoachingPoint(btnCoachingPoint);

            ViewBag.CurrentSearch = searchString;
            ViewBag.DateFrom = dateFrom;
            ViewBag.DateTo = dateTo;

            return GetListViews(searchString, dateFrom, dateTo, page, pageSize, "Complete Checks", coachingPoint);
        }

        public ActionResult Outstanding(string searchString = null, string dateFrom = null, string dateTo = null, int page = 1, int pageSize = 50, string btnCoachingPoint = null)
        {
            bool? coachingPoint = CoachingPoint(btnCoachingPoint);

            ViewBag.CurrentSearch = searchString;
            ViewBag.DateFrom = dateFrom;
            ViewBag.DateTo = dateTo;

            return GetListViews(searchString, dateFrom, dateTo, page, pageSize, "Outstanding Checks", coachingPoint);
        }

        public bool? CoachingPoint(string btnCoachingPoint = null)
        {
            bool? coachingPoint = null;

            switch (btnCoachingPoint)
            {
                case "coachingPointYes":
                    coachingPoint = true;
                    break;
                case "coachingPointNo":
                    coachingPoint = false;
                    break;
            }

            return coachingPoint;
        }

        //GET:
        public ActionResult Export()
        {
            DateTime sixteenMonths = DateTime.Now.AddMonths(-16);

            var bankAccounts = db.BankAccounts.Where(x => x.DateQCCompleted > sixteenMonths);
            var bankAccountReChecks = db.BankAccountReChecks;

            return File(new UTF8Encoding().GetBytes(exportService.BuildBank(bankAccounts, bankAccountReChecks)), "text/csv", string.Format("BankAccountQC_{0}.csv", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }

        public ActionResult CreateGeneral(int CheckId)
        {
            if (lockService.IsCheckLocked(CheckId))
            {
                return RedirectToAction("Index", "BankAccount", new { locked = true });
            }

            lockService.CreateCheckLock(CheckId);

            BankAccountCheck bankAccount = new BankAccountCheck();

            bankAccount = db.BankAccounts.Where(x => x.CheckId == CheckId).FirstOrDefault();

            bankAccount.DateQCCompleted = DateTime.Now;

            bankAccount.CheckType = db.CheckTypes.Where(x => x.CheckTypeId == bankAccount.CheckTypeId).FirstOrDefault();

            BankAccountCheck bankAccountInSession = this.Session["CreateGeneral"] as BankAccountCheck;

            bankAccount.QCCompletedByName = userHelper.CurrentUser();

            if (bankAccountInSession != null)
            {
                bankAccount = bankAccountInSession;
            }

            SetViewData(bankAccount);

            return View(bankAccount);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateGeneral(BankAccountCheck bankAccount)
        {
            if (lockService.IsCheckLocked(bankAccount.CheckId))
            {
                return RedirectToAction("Index", "BankAccount", new { locked = true });
            }

            NameCheck(bankAccount);

            this.Session["CreateGeneral"] = bankAccount;

            if (bankAccount.SBI == null)
            {
                ModelState.AddModelError("SBI", "The SBI field is required.");
            }

            if (bankAccount.BACheckTypeId == null)
            {
                ModelState.AddModelError("BACheckTypeId", "A Bank Account Check Type is required.");
            }

            if (!ModelState.IsValid)
            {
                SetViewData(bankAccount);

                return View(bankAccount);
            }

            if (bankAccount.Overriden)
            {
                bankAccount.Comments = "Check overriden via Quality Check Application";

                bankAccountService.SaveBankAccountOverride(bankAccount);

                return RedirectToAction("Index");
            }

            return RedirectToAction("CreateQuestions");
        }

        public ActionResult CreateQuestions()
        {
            BankAccountCheck bankAccountInSession = this.Session["CreateGeneral"] as BankAccountCheck;

            if (bankAccountInSession != null)
            {
                QuestionAnswerList questionAnswerListInSession = this.Session["CreateQuestions"] as QuestionAnswerList;

                ViewBag.AnswerOptions = ListHelper.YesNo();

                if (questionAnswerListInSession == null)
                {
                    List<CheckQuestion> checkQuestions = db.CheckQuestions.Where(x => x.CheckType.Name == "Bank Account").OrderBy(c => c.Order).ToList();

                    QuestionAnswerList questionAnswerList = new QuestionAnswerList();

                    foreach (var checkQuestion in checkQuestions)
                    {
                        QuestionAnswer questionAnswer = new QuestionAnswer
                        {
                            CheckQuestion = checkQuestion
                        };

                        questionAnswerList.QuestionAnswers.Add(questionAnswer);
                    }

                    ViewBag.CheckId = bankAccountInSession.CheckId;

                    return View(questionAnswerList);
                }

                return View(questionAnswerListInSession);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateQuestions(QuestionAnswerList questionAnswerList)
        {
            BankAccountCheck bankAccount = this.Session["CreateGeneral"] as BankAccountCheck;

            if (lockService.IsCheckLocked(bankAccount.CheckId))
            {
                return RedirectToAction("Index", "BankAccount", new { locked = true });
            }

            this.Session["CreateQuestions"] = questionAnswerList;

            return RedirectToAction("CreateResult");
        }

        public ActionResult CreateResult()
        {
            BankAccountCheck bankAccountInSession = this.Session["CreateGeneral"] as BankAccountCheck;

            QuestionAnswerList questionAnswerListInSession = this.Session["CreateQuestions"] as QuestionAnswerList;

            if (bankAccountInSession != null && questionAnswerListInSession != null)
            {
                SetViewData(bankAccountInSession);

                ViewBag.Email = db.Control.Where(x => x.Property == "BankEmail").Select(c => c.Active).FirstOrDefault();

                return View(bankAccountInSession);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateResult(BankAccountCheck bankAccount, string result)
        {
            if (lockService.IsCheckLocked(bankAccount.CheckId))
            {
                return RedirectToAction("Index", "BankAccount", new { locked = true });
            }

            bool email = db.Control.Where(x => x.Property == "BankEmail").Select(c => c.Active).FirstOrDefault();

            QuestionAnswerList questionAnswerList = this.Session["CreateQuestions"] as QuestionAnswerList;

            if (questionAnswerList != null)
            {
                if (result == "Not Approved" && bankAccount.FailReasonId == null)
                {
                    ModelState.AddModelError("FailReasonId", "The Not Approved Reason is required.");
                }

                if (result != "Approved" && bankAccount.Comments == null)
                {
                    ModelState.AddModelError("Comments", "As the result is not approved, the reason(s) must be stated.");
                }

                if (ModelState.IsValid)
                {
                    if (result == "Approved")
                    {
                        bankAccount.FailReason = null;
                        bankAccount.FailReasonId = null;
                    }

                    questionAnswerList.QCResult = result;

                    bankAccount.InitialReCheckDecision = bankAccount.ReCheckRequired;

                    bankAccountService.SaveBankAccount(bankAccount, questionAnswerList);

                    if (email != true)
                    {
                        bankAccount.EmailNotifications = false;
                    }

                    if (bankAccount.EmailNotifications == true)
                    {
                        MailMessage mail = messageService.GenerateBankEmail(bankAccount);

                        sendService.sendEmail(mail);
                    }

                    return RedirectToAction("Index");
                }

                ViewBag.QCResult = questionAnswerList.QCResult;

                SetViewData(bankAccount);

                ViewBag.Email = db.Control.Where(x => x.Property == "BankEmail").Select(c => c.Active).FirstOrDefault();

                return View(bankAccount);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Details(int CheckId)
        {
            int reCheckCount = db.BankAccountReChecks.Where(x => x.BankAccountId == CheckId).Count();

            if (reCheckCount == 0)
            {
                BankAccountDetails bankAccountDetails = new BankAccountDetails
                {
                    BankAccount = db.BankAccounts.Where(x => x.CheckId == CheckId).FirstOrDefault(),
                    QCResult = db.CheckResults.Where(x => x.CheckId == CheckId).Select(p => p.Result.Text).FirstOrDefault(),
                    Answers = db.Answers.Where(x => x.CheckId == CheckId).OrderBy(p => p.QuestionId).ToList(),
                    CheckAmendmentReason = db.CheckAmendmentReason.Where(x => x.CheckId == CheckId).OrderByDescending(x => x.AmendmentReasonId).FirstOrDefault()
                };

                return View(bankAccountDetails);
            }
            else
            {
                return RedirectToAction("BankAccountDetailsReCheck", "BankAccountReCheck", new { CheckId });
            }
        }

        public PartialViewResult _CheckComment(int CheckId, int QuestionId)
        {
            BankAccountComments comment = new BankAccountComments
            {
                AnswerComment = db.BankAccountComments.Where(x => x.CheckId == CheckId && x.QuestionId == QuestionId).Select(c => c.AnswerComment).FirstOrDefault() ?? ""
            };

            return PartialView(comment);
        }

        //GET:
        public ActionResult Challenge(int CheckId)
        {
            Challenge challenge = new Challenge
            {
                CheckId = CheckId
            };

            ViewBag.ReCheck = db.BankAccounts.Where(x => x.CheckId == CheckId).Select(p => p.ReCheckRequired).FirstOrDefault() == true ? "Yes" : "No";
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

            this.Session["ChallengeBankData"] = challenge;
            BankAccountCheck bankAccount = db.BankAccounts.Where(x => x.CheckId == challenge.CheckId).FirstOrDefault();

            if (ReCheck == "Yes")
            {
                int reCheckCount = db.BankAccountReChecks.Where(x => x.BankAccountId == challenge.CheckId).Count();

                if (challengeOutcome.Text == "Original Result Overturned")
                {
                    bankAccount.ReCheckRequired = false;

                    if (reCheckCount != 0)
                    {
                        List<BankAccountReCheck> bankAccountReChecks = db.BankAccountReChecks.Where(x => x.BankAccountId == challenge.CheckId).ToList();

                        foreach (BankAccountReCheck reCheck in bankAccountReChecks)
                        {
                            db.BankAccountReChecks.Remove(reCheck);
                        }
                    }

                    db.SetModified(bankAccount);

                    db.SaveChanges();

                    //send to edit questions as QC Result will now have to be changed to a pass (challenge itelf saved at later stage)

                    this.Session["PreviousBankResult"] = db.CheckResults.Where(x => x.CheckId == bankAccount.CheckId).Select(c => c.Result.Text).FirstOrDefault();

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
            BankAccountCheck bankAccount = db.BankAccounts.Where(x => x.CheckId == CheckId).FirstOrDefault();

            List<CheckQuestion> checkQuestions = db.CheckQuestions.Where(x => x.CheckType.Name == "Bank Account").OrderBy(c => c.Order).ToList();

            QuestionAnswerList qaList = new QuestionAnswerList();

            foreach (var cq in checkQuestions)
            {
                Answer answer = db.Answers.Where(x => x.CheckId == bankAccount.CheckId && x.QuestionId == cq.QuestionId).FirstOrDefault();

                QuestionAnswer qa = new QuestionAnswer
                {
                    CheckQuestion = cq,
                    Answer = answer.Text
                };

                qaList.QuestionAnswers.Add(qa);
            }

            ViewBag.AnswerOptions = ListHelper.YesNo();
            ViewBag.CheckId = bankAccount.CheckId;

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
                this.Session["ChallengeBankQuestions"] = qaList;
                return RedirectToAction("ChallengeResult", new { CheckId });
            }

            ViewBag.AnswerOptions = ListHelper.YesNo();
            return View(qaList);
        }

        //GET:
        public ActionResult ChallengeResult(int CheckId)
        {
            QuestionAnswerList qaList = this.Session["ChallengeBankQuestions"] as QuestionAnswerList;

            BankAccountCheck bankAccount = db.BankAccounts.AsNoTracking().Where(x => x.CheckId == CheckId).FirstOrDefault();

            SetViewData(bankAccount);

            this.Session["ChallengeBankGeneral"] = bankAccount;

            this.Session["ChallengeBankQuestions"] = qaList;

            return View(bankAccount);
        }

        //POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChallengeResult(BankAccountCheck bankAccount, string result)
        {
            //Challenge check - if there has been a successful challenge ensure that the QC Result is now a pass

            //obtain existing models from session data
            QuestionAnswerList qaList = this.Session["ChallengeBankQuestions"] as QuestionAnswerList;
            BankAccountCheck bankAccountCheck = this.Session["ChallengeBankGeneral"] as BankAccountCheck;

            if (ModelState.IsValid)
            {
                bankAccountCheck.ReCheckRequired = false;
                bankAccountCheck.InitialReCheckDecision = false;
                bankAccountCheck.FailReasonId = null;

                this.Session["ChallengeBankGeneral"] = bankAccountCheck;

                qaList.QCResult = result;

                return RedirectToAction("ChallengeAmendmentReason", new { bankAccountCheck.CheckId });
            }
            else
            {
                SetViewData(bankAccount);
                return View(bankAccount);
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
            QuestionAnswerList qaList = this.Session["ChallengeBankQuestions"] as QuestionAnswerList;

            BankAccountCheck bankAccount = this.Session["ChallengeBankGeneral"] as BankAccountCheck;

            checkAmendmentReason.AmendmentReason = db.AmendmentReasons.Where(x => x.AmendmentReasonId == checkAmendmentReason.AmendmentReasonId).FirstOrDefault();

            if (qaList.QCResult == "Not Approved")
            {
                ModelState.AddModelError("AmendmentReasonComments", "As a successful challenge has been made QC Result must be approved - please return to questions and review");
            }

            if (ModelState.IsValid)
            {
                qaList.Challenge = true;

                bankAccountService.SaveEditBankAccount(bankAccount, qaList);

                auditService.SaveBankAccountAmendmentAudit(bankAccount, checkAmendmentReason);

                checkAmendmentReason.CheckId = bankAccount.CheckId;

                db.CheckAmendmentReason.Add(checkAmendmentReason);
                db.SaveChanges();

                return RedirectToAction("Details", "BankAccount", new { bankAccount.CheckId });
            }
            else
            {
                bool challengeAmend = true;
                SetAmendViewData(checkAmendmentReason, challengeAmend);
                return View(checkAmendmentReason);
            }
        }

        //GET:
        public ActionResult EditGeneral(int CheckId)
        {
            BankAccountCheck bankAccount = db.BankAccounts.Where(x => x.CheckId == CheckId).FirstOrDefault();

            BankAccountCheck bankAccountExisting = this.Session["EditBankGeneral"] as BankAccountCheck;

            if (bankAccountExisting != null)
            {
                bankAccount = bankAccountExisting;
            }

            SetViewData(bankAccount);

            return View(bankAccount);
        }

        // POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditGeneral(BankAccountCheck bankAccount)
        {
            bankAccount.CheckType = db.CheckTypes.Where(x => x.CheckTypeId == bankAccount.CheckTypeId).FirstOrDefault();

            //set model to session data
            this.Session["EditBankGeneral"] = bankAccount;

            if (ModelState.IsValid)
            {
                this.Session["PreviousBankResult"] = db.CheckResults.Where(x => x.CheckId == bankAccount.CheckId).Select(c => c.Result.Text).FirstOrDefault();

                return RedirectToAction("EditQuestions");
            }

            SetViewData(bankAccount);

            return View(bankAccount);
        }

        public ActionResult EditQuestions()
        {
            //get models from session data 

            BankAccountCheck bankAccount = this.Session["EditBankGeneral"] as BankAccountCheck;

            QuestionAnswerList qaListExisting = this.Session["EditBankQuestions"] as QuestionAnswerList;

            ViewBag.CheckId = bankAccount.CheckId;
            ViewBag.AnswerOptions = ListHelper.YesNo();


            if (qaListExisting == null)
            {
                List<CheckQuestion> checkQuestions = db.CheckQuestions.Where(x => x.CheckType.Name == "Bank Account").OrderBy(c => c.Order).ToList();

                QuestionAnswerList qaList = new QuestionAnswerList();

                foreach (var cq in checkQuestions)
                {
                    Answer answer = db.Answers.Where(x => x.CheckId == bankAccount.CheckId && x.QuestionId == cq.QuestionId).FirstOrDefault();

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
                this.Session["EditBankQuestions"] = qaList;
                return RedirectToAction("EditResult");
            }

            ViewBag.AnswerOptions = ListHelper.YesNo();
            return View(qaList);
        }

        //GET:
        public ActionResult EditResult()
        {
            BankAccountCheck bankAccount = this.Session["EditBankGeneral"] as BankAccountCheck;

            QuestionAnswerList qaList = this.Session["EditBankQuestions"] as QuestionAnswerList;

            ViewBag.QCResult = qaList.QCResult;

            this.Session["EditBankQuestions"] = qaList;

            SetViewData(bankAccount);

            return View(bankAccount);
        }

        public ActionResult GeneralDetails(int CheckId)
        {
            return View(db.BankAccounts.Where(x => x.CheckId == CheckId).FirstOrDefault());
        }

        //POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditResult(BankAccountCheck bankAccount, string result)
        {
            //obtain existing models from session data

            QuestionAnswerList qaList = this.Session["EditBankQuestions"] as QuestionAnswerList;

            qaList.QCResult = result;

            BankAccountCheck bankAccountSaved = this.Session["EditBankGeneral"] as BankAccountCheck;

            if (ModelState.IsValid)
            {
                if (qaList.QCResult == "Approved" || qaList.QCResult == "Approved Advisory")
                {
                    bankAccountSaved.FailReasonId = null;
                }
                else
                {
                    bankAccountSaved.FailReasonId = bankAccount.FailReasonId;
                }

                bankAccountSaved.ReCheckRequired = bankAccount.ReCheckRequired;
                bankAccountSaved.Comments = bankAccount.Comments;

                this.Session["EditBankGeneral"] = bankAccountSaved;

                return RedirectToAction("AmendmentReason");
            }
            else
            {
                SetViewData(bankAccount);
                ViewBag.QCResult = qaList.QCResult;
                return View(bankAccount);
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
            QuestionAnswerList qaList = this.Session["EditBankQuestions"] as QuestionAnswerList;

            BankAccountCheck bankAccount = this.Session["EditBankGeneral"] as BankAccountCheck;

            checkAmendmentReason.AmendmentReason = db.AmendmentReasons.Where(x => x.AmendmentReasonId == checkAmendmentReason.AmendmentReasonId).FirstOrDefault();

            if (checkAmendmentReason.AmendmentReason == null)
            {
                ModelState.AddModelError("AmendmentReason", "An amendment reason must be selected - if no amendments have been made please return to details screen");
            }

            if (ModelState.IsValid)
            {
                bankAccountService.SaveEditBankAccount(bankAccount, qaList);

                auditService.SaveBankAccountAmendmentAudit(bankAccount, checkAmendmentReason);

                checkAmendmentReason.CheckId = bankAccount.CheckId;

                db.CheckAmendmentReason.Add(checkAmendmentReason);
                db.SaveChanges();

                return RedirectToAction("Details", "BankAccount", new { bankAccount.CheckId });
            }
            else
            {
                bool challengeAmend = false;

                SetAmendViewData(checkAmendmentReason, challengeAmend);
                ViewBag.QCResult = qaList.QCResult;
                return View(checkAmendmentReason);
            }
        }

        public PartialViewResult _ReCheckResults(int CheckId)
        {
            int ReCheckId = db.BankAccountReChecks.Where(x => x.BankAccountId == CheckId).OrderByDescending(c => c.DateQCCompleted).Select(x => x.CheckId).FirstOrDefault();

            BankAccountDetailsReCheck detailsReCheck = new BankAccountDetailsReCheck
            {
                ReCheckResult = db.CheckResults.Where(x => x.CheckId == ReCheckId).Select(p => p.Result.Text).FirstOrDefault()
            };

            return PartialView(detailsReCheck);
        }

        private ViewResult GetListViews(string serchString, string dateFrom, string dateTo, int page, int pageSize, string pageName, bool? coachingPoint)
        {
            Session.Clear();

            ViewBag.PageName = pageName;

            IQueryable<BankAccountCheck> bankAccounts = filterService.BankAccountFilterChecks(serchString, dateFrom, dateTo, pageName, coachingPoint);
            IQueryable<BankAccountOverview> bankAccountOverviews = bankAccountService.RetrieveBankAccountChecks(bankAccounts);

            return View("Index", new PagedList<BankAccountOverview>(bankAccountOverviews, page, pageSize));
        }

        private void NameCheck(BankAccountCheck bankAccount)
        {
            bool personName = peopleService.PersonCheck(bankAccount.PersonName);
            bool managerName = peopleService.PersonCheck(bankAccount.ManagerName);

            if (!personName)
            {
                ModelState.AddModelError("PersonName", "Please select a valid name from the list.");
            }

            if (!managerName)
            {
                ModelState.AddModelError("ManagerName", "Please select a valid name from the list.");
            }
        }

        private void SetViewData(BankAccountCheck bankAccount)
        {
            ViewBag.BACheckTypeId = new SelectList(db.BACheckTypes.AsNoTracking().Where(x => x.Active == true), "BACheckTypeId", "Text", bankAccount.BACheckTypeId);
            ViewBag.FailReasonId = new SelectList(db.FailReasons.AsNoTracking().Where(x => x.Active == true && x.CheckTypeId == 3).OrderBy(x => x.Text), "FailReasonId", "Text", bankAccount.FailReasonId);
            ViewBag.Result = new SelectList(db.Results.AsNoTracking().OrderBy(x => x.ResultId), "Text", "Text");
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

    }
}