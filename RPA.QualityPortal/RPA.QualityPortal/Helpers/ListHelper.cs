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

    }
}