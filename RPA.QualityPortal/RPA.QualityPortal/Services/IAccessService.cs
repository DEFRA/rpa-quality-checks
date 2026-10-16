using System;
using System.Collections.Generic;
using System.Linq;

namespace RPA.QualityPortal.Services
{
    public interface IAccessService
    {
        string ChangeRoles(string name, string access);
    }
}