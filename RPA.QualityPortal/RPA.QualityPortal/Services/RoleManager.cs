using System.Web.Security;

namespace RPA.QualityPortal.Services
{
    public class RoleManager : IRoleManager
    {
        public bool IsUserInRole(string roleName)
        {
            return Roles.IsUserInRole(roleName);
        }
    }
}