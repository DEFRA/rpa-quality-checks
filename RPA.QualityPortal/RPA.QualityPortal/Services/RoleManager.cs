namespace RPA.QualityPortal.Services
{
    public class RoleManager : IRoleManager
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public RoleManager(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public bool IsUserInRole(string roleName)
        {
            return httpContextAccessor.HttpContext?.User?.IsInRole(roleName) ?? false;
        }
    }
}
