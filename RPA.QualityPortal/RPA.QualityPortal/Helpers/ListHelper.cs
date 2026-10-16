using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace RPA.QualityPortal.Helpers
{
    public static class ListHelper
    {
        internal static List<SelectListItem> YesNo()
        {
            //Create list of Scheme Codes.
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem() { Text = "N/A", Value = "N/A" });
            list.Add(new SelectListItem() { Text = "Yes", Value = "Yes" });
            list.Add(new SelectListItem() { Text = "No", Value = "No" });

            return list;
        }

        internal static List<SelectListItem> Access()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem() { Text = "Team Member to Team Manager", Value = "Team Member to Team Manager" });
            list.Add(new SelectListItem() { Text = "Team Manager to Team Member", Value = "Team Manager to Team Member" });
            list.Add(new SelectListItem() { Text = "Team Manager to Admin", Value = "Team Manager to Admin" });
            list.Add(new SelectListItem() { Text = "Admin to Team Manager", Value = "Admin to Team Manager" });

            return list;
        }
    }
}