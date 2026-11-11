namespace RPA.QualityPortal.Services
{
    public class RoleManager : IRoleManager
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly QualityPermissions permissions;

        public RoleManager(IHttpContextAccessor httpContextAccessor, QualityPermissions permissions)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.permissions = permissions;
        }

        public bool IsUserInRole(string roleName)
        {
            return permissions.HasPermission(httpContextAccessor.HttpContext?.User, roleName);
        }
    }
}
