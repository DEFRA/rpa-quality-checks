using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Primitives;

namespace RPA.QualityPortal.Tests
{
    public static class ViewComponentTestExtensions
    {
        // Gives a view component the request/view data it normally receives from MVC so View(model) and Request.Query work in a unit test.
        public static T WithViewContext<T>(this T component, params (string Key, string Value)[] query) where T : ViewComponent
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Query = new QueryCollection(query.ToDictionary(q => q.Key, q => new StringValues(q.Value)));

            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());

            component.ViewComponentContext = new ViewComponentContext
            {
                ViewContext = new ViewContext { HttpContext = httpContext, ViewData = viewData }
            };

            return component;
        }

        public static TModel ModelOf<TModel>(this IViewComponentResult result)
        {
            return (TModel)((ViewViewComponentResult)result).ViewData.Model;
        }


    }
}
