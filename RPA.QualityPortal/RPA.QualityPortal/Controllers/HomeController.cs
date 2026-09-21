using RPA.QualityPortal.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RPA.QualityPortal.Controllers
{
    [Authorize(Roles = "Quality Checks: Basic Access")]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.CheckType = "Home";
            return View();
        }
    }
}