using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.Services
{
    public interface IAccessService
    {
        string ChangeRoles(string name, string access);
    }
}