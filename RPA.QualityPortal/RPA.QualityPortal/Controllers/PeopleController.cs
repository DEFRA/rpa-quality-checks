using Newtonsoft.Json;
using RPA.QualityPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace RPA.QualityPortal.Controllers
{
    public class PeopleController : QualityController
    {
        IPeopleContext pdb;

        public PeopleController(IPeopleContext context)
        {
            this.pdb = context;
        }

        public IActionResult PeopleSearch(string searchstring)
        {
            var people = new List<PeopleSearchModel>();

            if (searchstring.Length > 3)
            {
                people = pdb.People
                    .Where(x => x.Name.Contains(searchstring) || x.StaffNumber.Contains(searchstring))
                    .OrderBy(x => x.Name)
                    .Select(x => new PeopleSearchModel
                    {
                        Value = x.Name,
                        StaffNumber = x.StaffNumber,
                        Manager = pdb.People.Where(m => m.StaffNumber == x.Supervisor).Select(m => m.Name).FirstOrDefault()
                    })
                    .ToList();
            }

            return Content(JsonConvert.SerializeObject(people), "application/json");
        }
    }
}