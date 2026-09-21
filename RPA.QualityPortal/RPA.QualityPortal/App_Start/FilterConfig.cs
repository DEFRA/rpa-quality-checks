using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.Mvc;

namespace RPA.QualityPortal
{

    [ExcludeFromCodeCoverage]
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
