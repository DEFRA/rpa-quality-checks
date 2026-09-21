using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.Factory
{
    [ExcludeFromCodeCoverage]
    public class HttpContextManager
    {
        private static HttpContextBase mContext;
        public static HttpContextBase Current
        {
            get
            {
                if (mContext != null)
                    return mContext;

                if (HttpContext.Current == null)
                    throw new InvalidOperationException("HttpContext not available");

                return new HttpContextWrapper(HttpContext.Current);
            }
        }

        public static void SetCurrentContext(HttpContextBase context)
        {
            mContext = context;
        }
    }
}