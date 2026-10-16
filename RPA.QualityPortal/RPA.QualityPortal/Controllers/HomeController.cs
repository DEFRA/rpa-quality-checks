using Microsoft.AspNetCore.Authorization;
using RPA.QualityPortal.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace RPA.QualityPortal.Controllers
{
    [Authorize(Roles = "Quality Checks: Basic Access")]
    public class HomeController : QualityController
    {
        public IActionResult Index()
        {
            ViewBag.CheckType = "Home";
            return View();
        }
    }
}