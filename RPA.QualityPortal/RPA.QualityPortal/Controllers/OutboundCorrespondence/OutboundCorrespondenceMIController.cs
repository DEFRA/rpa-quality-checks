using Microsoft.AspNetCore.Authorization;
using RPA.QualityPortal.DAL;
using RPA.QualityPortal.Helpers;
using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.Services;
using RPA.QualityPortal.ViewModels.OutboundCorrespondenceVM.MI;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace RPA.QualityPortal.Controllers
{
    [Authorize(Roles = "Quality Checks: OC Team Member, Quality Checks: OC Team Manager")]
    public class OutboundCorrespondenceMIController : QualityController
    {
        IQualityContext db;
        IPeopleContext pdb;
        IMIService miService;
        IUserHelper userHelper;
        IFilterService filterService;
        IRoleManager roleManager;

        public OutboundCorrespondenceMIController(IQualityContext context, IPeopleContext peopleContext,  IMIService miService, IUserHelper userHelper, IFilterService filterService, IRoleManager roleManager)
        {
            this.db = context;
            this.pdb = peopleContext;
            this.miService = miService;
            this.userHelper = userHelper;
            this.filterService = filterService;
            this.roleManager = roleManager;
        }

        public IActionResult MIIndex(string searchString = null, string searchOption = null, string dateFrom = null, string dateTo = null, int? SchemeId = null, int? BusinessAreaId = null, int? CorrespondenceTypeId = null, bool? back = null)
        {
            this.Session["MIPage"] = "MIDetailsIndividual";
            ViewBag.Back = back;

            return View();
        }

public IActionResult MIDetails(string personName, bool detailsClick = false)
        {
            OutboundCorrespondenceMI outboundCorrespondenceMI = this.Session["MIData"] as OutboundCorrespondenceMI;

            OutboundCorrespondenceMIDetails outboundCorrespondenceMIDetails = miService.CollateMIDetails(outboundCorrespondenceMI, personName, detailsClick);

            ViewBag.PersonName = personName;

            if (String.IsNullOrEmpty(personName))
            {
                this.Session["MIPage"] = "MIDetailsOverview";
                ViewBag.Page = this.Session["MIPage"] as string;
                return View(outboundCorrespondenceMIDetails);
            }
            else
            {
                ViewBag.Page = this.Session["MIPage"] as string;
                return View("MIDetailsIndividual", outboundCorrespondenceMIDetails);
            }
        }
    }
}