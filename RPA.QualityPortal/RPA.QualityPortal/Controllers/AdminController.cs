using PagedList;
using RPA.QualityPortal.DAL;
using RPA.QualityPortal.Helpers;
using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.Services;
using RPA.QualityPortal.ViewModels;
using RPA.QualityPortal.ViewModels.OutboundCorrespondenceVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;

namespace RPA.QualityPortal.Controllers
{
    [Authorize(Roles = "Quality Checks: OC Admin, Quality Checks: BA Admin")]
    public class AdminController : Controller
    {
        IQualityContext db;
        IPeopleContext pdb;
        IUserHelper userHelper;
        IOutboundCorrespondenceService outboundCorrespondenceService;
        IBankAccountService bankAccountService;
        IAnswerService answerService;
        IMessageService messageService;
        IFilterService filterService;
        IRoleManager roleManager;
        IAccessService accessService;
        IPeopleService peopleService;
        ILockService lockService;
        ISendService sendService;

        public AdminController()
        {
            db = new QualityContext();
            pdb = new PeopleContext();
            answerService = new AnswerService(db);
            userHelper = new UserHelper(pdb);
            roleManager = new RoleManager();
            filterService = new FilterService(db, pdb, userHelper, roleManager);
            messageService = new EmailService(db, pdb);
            lockService = new LockService(db, pdb, userHelper);
            outboundCorrespondenceService = new OutboundCorrespondenceService(db, answerService, userHelper, lockService);
            bankAccountService = new BankAccountService(db, answerService, lockService);
            accessService = new AccessService(db, pdb);
            peopleService = new PeopleService(pdb);
            sendService = new SendService();
        }

        public AdminController(IQualityContext context, IPeopleContext peopleContext, IOutboundCorrespondenceService outboundCorrespondenceService, IBankAccountService bankAccountService, IAnswerService answerService, IFilterService filterService, IMessageService messageService, IRoleManager roleManager, IUserHelper userHelper, IAccessService accessService, IPeopleService peopleService, ILockService lockService, ISendService sendService)
        {
            this.db = context;
            this.pdb = peopleContext;
            this.outboundCorrespondenceService = outboundCorrespondenceService;
            this.bankAccountService = bankAccountService;
            this.answerService = answerService;
            this.filterService = filterService;
            this.messageService = messageService;
            this.roleManager = roleManager;
            this.userHelper = userHelper;
            this.accessService = accessService;
            this.peopleService = peopleService;
            this.lockService = lockService;
            this.sendService = sendService;
        }

        public ActionResult Index(string checkType)
        {
            ViewBag.typeOfCheck = checkType;

            return View();
        }

        public ActionResult UnlockCheck(string checkType)
        {
            DateTime twoHoursAgo = DateTime.Now.AddHours(-2);

            List<LockCheckView> listLockedChecks = new List<LockCheckView>();
            List<LockCheck> lockChecks = db.LockCheck.Where(x => x.Timestamp > twoHoursAgo).ToList();

            foreach (var lockCheck in lockChecks)
            {
                LockCheckView lockedCheck = new LockCheckView();

                lockedCheck.LockCheck = lockCheck;

                if (checkType == "OutboundCorrespdondence")
                {
                    lockedCheck.OutboundCorrespondence = db.OutboundCorrespondence.Where(x => x.CheckId == lockCheck.CheckId).FirstOrDefault();
                }
                else if (checkType == "Bank")
                {
                    lockedCheck.BankAccount = db.BankAccounts.Where(x => x.CheckId == lockCheck.CheckId).FirstOrDefault();

                    if (lockedCheck.BankAccount != null)
                    {
                        if (lockedCheck.BankAccount.SBI != null)
                        {
                            lockedCheck.ReCheck = true;
                        }
                    }
                }

                listLockedChecks.Add(lockedCheck);
            }

            ViewBag.TypeOfCheck = checkType;

            return View(listLockedChecks);
        }

        [HttpPost]
        public ActionResult DeleteLock(int LockCheckId, string checkType)
        {
            db.LockCheck.Remove(db.LockCheck.Where(x => x.LockCheckId == LockCheckId).FirstOrDefault());
            db.SaveChanges();

            return RedirectToAction("UnlockCheck", new { CheckType = checkType });

        }

        [HttpGet]
        public ActionResult Email(string checkType)
        {
            ViewBag.typeOfCheck = checkType;

            return View();
        }

        [HttpGet]
        public ActionResult _OutboundCorrespondenceEmails(int page = 1, int pageSize = 50)
        {
            IQueryable<OutboundCorrespondence> outboundCorrespondences = filterService.OutboundCorrespondenceFilterChecks(null, "All Quality Checks");

            IQueryable<OutboundCorrespondenceOverview> OutboundCorrespondenceOverviews = outboundCorrespondenceService
                .RetrieveOutboundCorrespondenceChecks(outboundCorrespondences)
                .Where(o => o.OutboundCorrespondence.EmailNotifications == false);

            ViewBag.Email = db.Control.Where(x => x.Property == "OutboundCorrespondenceEmail").Select(c => c.Active).FirstOrDefault();
            ViewBag.typeOfCheck = "OutboundCorrespondence";

            return PartialView(new PagedList<OutboundCorrespondenceOverview>(OutboundCorrespondenceOverviews, page, pageSize));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _OutboundCorrespondenceEmails(List<OutboundCorrespondenceOverview> qualityChecks)
        {
            if (qualityChecks != null)
            {
                List<OutboundCorrespondence> outboundCorrespondencesSendEmail = qualityChecks.Where(x => x.OutboundCorrespondence.EmailNotifications == true).Select(c => c.OutboundCorrespondence).ToList();

                if (outboundCorrespondencesSendEmail != null)
                {
                    foreach (OutboundCorrespondence outboundCorrespondence in outboundCorrespondencesSendEmail)
                    {
                        OutboundCorrespondence outboundCorrrespondenceNew = db.OutboundCorrespondence.Where(x => x.CheckId == outboundCorrespondence.CheckId).FirstOrDefault();

                        outboundCorrrespondenceNew.EmailNotifications = outboundCorrespondence.EmailNotifications;

                        db.SetModified(outboundCorrrespondenceNew);
                        MailMessage mail = messageService.GenerateOutboundCorrespondenceEmail(outboundCorrespondence);
                        sendService.sendEmail(mail);
                    }

                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index", new { CheckType = "OutboundCorrespondence" });
        }

        [HttpGet]
        public ActionResult _BankEmails(int page = 1, int pageSize = 50)
        {
            IQueryable<BankAccountCheck> bankAccountChecks = filterService.BankAccountFilterChecks(null, null, null, "All Quality Checks", null);

            IQueryable<BankAccountOverview> BankAccountOverviews = bankAccountService
                .RetrieveBankAccountChecks(bankAccountChecks)
                .Where(o => o.BankAccount.EmailNotifications == false);

            ViewBag.Email = db.Control.Where(x => x.Property == "BankEmail").Select(c => c.Active).FirstOrDefault();
            ViewBag.typeOfCheck = "Bank";

            return PartialView(new PagedList<BankAccountOverview>(BankAccountOverviews, page, pageSize));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _BankEmails(List<BankAccountOverview> qualityChecks)
        {
            if (qualityChecks != null)
            {
                List<BankAccountCheck> bankAccountSendEmail = qualityChecks.Where(x => x.BankAccount.EmailNotifications == true).Select(c => c.BankAccount).ToList();

                if (bankAccountSendEmail != null)
                {
                    foreach (BankAccountCheck bankAccountCheck in bankAccountSendEmail)
                    {
                        BankAccountCheck bankAccountCheckNew = db.BankAccounts.Where(x => x.CheckId == bankAccountCheck.CheckId).FirstOrDefault();

                        bankAccountCheckNew.EmailNotifications = bankAccountCheck.EmailNotifications;

                        db.SetModified(bankAccountCheckNew);
                        MailMessage mail = messageService.GenerateBankEmail(bankAccountCheck);

                        sendService.sendEmail(mail);
                    }

                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index", new { CheckType = "Bank" });
        }

        [HttpGet]
        public ActionResult _TurnOffEmail(string checkType)
        {
            ViewBag.typeOfCheck = checkType;

            return PartialView(db.Control.Where(x => x.Property == checkType + "Email").FirstOrDefault());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _TurnOffEmail(Control emailNofication)
        {
            db.SetModified(emailNofication);

            db.SaveChanges();

            string checkType = emailNofication.Property.Substring(0, emailNofication.Property.Length - 5);

            return RedirectToAction("Index", new { CheckType = checkType });
        }

        [HttpGet]
        public ActionResult _ReCheckEmails(int page = 1, int pageSize = 50)
        {
            IQueryable<OutboundCorrespondenceOverview> qualityChecks = outboundCorrespondenceService
                .RetrieveOutboundCorrespondenceRechecks(db.OutboundCorrespondenceReCheck)
                .Where(o => o.OutboundCorrespondenceReCheck.EmailNotifications == false);

            ViewBag.Email = db.Control.Where(x => x.Property == "OutboundCorrespondenceEmail").Select(c => c.Active).FirstOrDefault();
            ViewBag.typeOfCheck = "OutboundCorrespondence";

            return PartialView(new PagedList<OutboundCorrespondenceOverview>(qualityChecks, page, pageSize));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _ReCheckEmails(List<OutboundCorrespondenceOverview> qualityChecks)
        {
            if (qualityChecks != null)
            {
                List<OutboundCorrespondenceReCheck> outboundCorrespondencesReCheckSendEmail = qualityChecks.Where(x => x.OutboundCorrespondenceReCheck.EmailNotifications == true).Select(c => c.OutboundCorrespondenceReCheck).ToList();

                if (outboundCorrespondencesReCheckSendEmail != null)
                {
                    List<OutboundCorrespondence> outboundCorrespondences = new List<OutboundCorrespondence>();

                    foreach (OutboundCorrespondenceReCheck outboundCorrespondenceReCheck in outboundCorrespondencesReCheckSendEmail)
                    {
                        OutboundCorrespondence outboundCorrespondence = db.OutboundCorrespondence.Where(c => c.CheckId == outboundCorrespondenceReCheck.OutboundCorrespondenceId).FirstOrDefault();
                        outboundCorrespondence.EmailNotifications = true;
                        db.SetModified(outboundCorrespondence);
                        MailMessage mail = messageService.GenerateOutboundCorrespondenceEmail(outboundCorrespondence);
                        sendService.sendEmail(mail);
                    }

                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index", new { CheckType = "OutboundCorrespondence" });
        }

        [HttpGet]
        public ActionResult _BankReCheckEmails(int page = 1, int pageSize = 50)
        {
            IQueryable<BankAccountOverview> qualityChecks = bankAccountService
                .RetrieveBankAccountRechecks(db.BankAccountReChecks)
                .Where(o => o.BankAccountReCheck.EmailNotifications == false);

            ViewBag.Email = db.Control.Where(x => x.Property == "BankEmail").Select(c => c.Active).FirstOrDefault();
            ViewBag.typeOfCheck = "Bank";

            return PartialView(new PagedList<BankAccountOverview>(qualityChecks, page, pageSize));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _BankReCheckEmails(List<BankAccountOverview> qualityChecks)
        {
            if (qualityChecks != null)
            {
                List<BankAccountReCheck> bankAccountsReCheckSendEmail = qualityChecks.Where(x => x.BankAccountReCheck.EmailNotifications == true).Select(c => c.BankAccountReCheck).ToList();

                if (bankAccountsReCheckSendEmail != null)
                {
                    foreach (BankAccountReCheck bankAccountReCheck in bankAccountsReCheckSendEmail)
                    {
                        BankAccountCheck bankAccount = db.BankAccounts.Where(c => c.CheckId == bankAccountReCheck.BankAccountId).FirstOrDefault();
                        BankAccountReCheck bankAccountReCheckOriginal = db.BankAccountReChecks.Where(x => x.CheckId == bankAccountReCheck.CheckId).FirstOrDefault();
                        bankAccountReCheckOriginal.EmailNotifications = true;
                        db.SetModified(bankAccountReCheckOriginal);
                        MailMessage mail = messageService.GenerateBankEmail(bankAccount);
                        sendService.sendEmail(mail);
                    }

                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index", new { CheckType = "Bank" });
        }

        [HttpGet]
        public ActionResult Access(string checkType)
        {
            ViewBag.AccessRights = ListHelper.Access();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Access(string name, string access)
        {
            bool nameCheck = peopleService.PersonCheck(name);

            if (!nameCheck)
            {
                ModelState.AddModelError("Name", "Please Select a Valid Name From List");
            }
            else
            {
                ViewBag.Error = accessService.ChangeRoles(name, access);
            }

            return RedirectToAction("Access");
        }
    }
}