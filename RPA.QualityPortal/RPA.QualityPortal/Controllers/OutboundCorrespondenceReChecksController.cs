using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RPA.QualityPortal.Controllers
{
    public class OutboundCorrespondenceReCheckController : Controller
    {
        QualityContext db;
        PeopleContext pdb;

        IOutboundCorrespondenceService OutboundCorrespondenceService;

        public OutboundCorrespondenceReCheckController()
        {
            db = new QualityContext();
            pdb = new PeopleContext();
            OutboundCorrespondenceService = new OutboundCorrespondenceService();

        }

        public OutboundCorrespondenceReCheckController(QualityContext context, PeopleContext peopleContext, IOutboundCorrespondenceService outboundCorrespondenceService)
        {
            this.db = context;
            this.pdb = peopleContext;
            this.OutboundCorrespondenceService = outboundCorrespondenceService;
        }

        public ActionResult CreateReCheck()
        {
            return View();
        }
    }

}