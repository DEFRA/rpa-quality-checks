using RPA.QualityPortal.Models.CheckTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RPA.QualityPortal.DAL;
using RPA.QualityPortal.Helpers;
using RPA.QualityPortal.Services;
using PagedList;
using RPA.QualityPortal.ViewModels.OutboundCorrespondenceVM;
using RPA.QualityPortal.Models;
using System.Net.Mail;
using System.Text;
using System.IO;
using OfficeOpenXml;

namespace RPA.QualityPortal.Controllers
{
    [Authorize(Roles = "Quality Checks: OC Team Member, Quality Checks: OC Team Manager")]
    public class OutboundCorrespondenceAccreditationController : Controller
    {
        IQualityContext db;
        IPeopleContext pdb;
        IUserHelper userHelper;
        IFilterService filterService;
        IRoleManager roleManager;
        IOutboundCorrespondenceAccreditationService accreditationService;
        IExportService exportService;
        IFileService fileService;

        public OutboundCorrespondenceAccreditationController()
        {
            db = new QualityContext();
            pdb = new PeopleContext();
            userHelper = new UserHelper(pdb);
            filterService = new FilterService(db, pdb, userHelper, roleManager);
            accreditationService = new OutboundCorrespondenceAccreditationService(db, pdb, userHelper, roleManager, filterService);
            fileService = new FileService(db, accreditationService);
            exportService = new ExportService(db, fileService);
        }

        public OutboundCorrespondenceAccreditationController(IQualityContext context, IPeopleContext peopleContext, IUserHelper userHelper, IFileService fileService, IFilterService filterService, IRoleManager roleManager, IOutboundCorrespondenceAccreditationService accreditationService, IExportService exportService)
        {
            this.db = context;
            this.pdb = peopleContext;
            this.userHelper = userHelper;
            this.filterService = filterService;
            this.roleManager = roleManager;
            this.accreditationService = accreditationService;
            this.exportService = exportService;
            this.fileService = fileService;
        }

        // GET: OutboundCorrespondenceAccreditation
        public ActionResult AccreditationIndex(int page = 1, int pageSize = 50)
        {
            return View(new PagedList<OutboundCorrespondenceAccreditation> (db.OutboundCorrespondenceAccreditation.ToList(), page, pageSize));
        }

        public ActionResult Accredit(int accreditationId)
        {
            OutboundCorrespondenceAccreditation accreditation = db.OutboundCorrespondenceAccreditation.Where(x => x.OutboundCorrespondenceAccreditationId == accreditationId).FirstOrDefault();
            db.SetModified(accreditation);
            db.SaveChanges();

            return RedirectToAction("AccreditationIndex");
        }

        public FileResult Export()
        {
            string sourceDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");

            ExcelPackage ePack = exportService.BuildAccreditation(sourceDirectory);

            return new FileContentResult(ePack.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "TestName.xlsx"
            };
        }

        public ActionResult ManualRefreshBulk()
        {
            accreditationService.ManualRefreshBulk();

            return RedirectToAction("AccreditationIndex");
        }

        public ActionResult ManualRefreshIndividual(string name)
        {
            accreditationService.ManualRefreshIndividual(name, false);

            return RedirectToAction("AccreditationIndex");
        }

        //GET:
        public ActionResult UserConsecutivePasses()
        {
            List<OutboundCorrespondenceAccreditation> consecutivePasses = new List<OutboundCorrespondenceAccreditation>();

            foreach(var accreditation in db.OutboundCorrespondenceAccreditation)
            {
                int accreditationCount = (accreditation.NoOfConsecutivePassesBpsEmail +
                    accreditation.NoOfConsecutivePassesBpsLetter +
                    accreditation.NoOfConsecutivePassesCsEmail +
                    accreditation.NoOfConsecutivePassesCsLetter +
                    accreditation.NoOfConsecutivePassesEsEmail +
                    accreditation.NoOfConsecutivePassesEsLetter);

                if (accreditationCount > 4)
                {
                    consecutivePasses.Add(accreditation);
                }
            }

            return View(consecutivePasses);
        }
    }
}