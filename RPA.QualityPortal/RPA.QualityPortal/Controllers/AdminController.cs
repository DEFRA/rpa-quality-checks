using Microsoft.AspNetCore.Authorization;
using X.PagedList;
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
using Microsoft.AspNetCore.Mvc;

namespace RPA.QualityPortal.Controllers
{
    [Authorize(Roles = "Quality Checks: OC Admin, Quality Checks: BA Admin")]
    public class AdminController : QualityController
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

        public IActionResult Index(string checkType)
        {
            ViewBag.typeOfCheck = checkType;

            return View();
        }

        public IActionResult UnlockCheck(string checkType)
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
        public IActionResult DeleteLock(int LockCheckId, string checkType)
        {
            db.LockCheck.Remove(db.LockCheck.Where(x => x.LockCheckId == LockCheckId).FirstOrDefault());
            db.SaveChanges();

            return RedirectToAction("UnlockCheck", new { CheckType = checkType });

        }

        [HttpGet]
        public IActionResult Email(string checkType)
        {
            ViewBag.typeOfCheck = checkType;

            return View();
        }

[HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult _OutboundCorrespondenceEmails(List<OutboundCorrespondenceOverview> qualityChecks)
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

[HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult _BankEmails(List<BankAccountOverview> qualityChecks)
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

[HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult _TurnOffEmail(Control emailNofication)
        {
            db.SetModified(emailNofication);

            db.SaveChanges();

            string checkType = emailNofication.Property.Substring(0, emailNofication.Property.Length - 5);

            return RedirectToAction("Index", new { CheckType = checkType });
        }

[HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult _ReCheckEmails(List<OutboundCorrespondenceOverview> qualityChecks)
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

[HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult _BankReCheckEmails(List<BankAccountOverview> qualityChecks)
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
        public IActionResult Access(string checkType)
        {
            ViewBag.AccessRights = ListHelper.Access();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Access(string name, string access)
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