namespace RPA.QualityPortal.Helpers
{
    public class UserHelper : IUserHelper
    {
        IPeopleContext pdb;
        IHttpContextAccessor httpContextAccessor;

        public UserHelper(IPeopleContext peopleContext, IHttpContextAccessor httpContextAccessor)
        {
            this.pdb = peopleContext;
            this.httpContextAccessor = httpContextAccessor;
        }

        public string CurrentUser()
        {
            string user = "Unknown";

            var context = httpContextAccessor.HttpContext;

            if (context != null)
            {
                user = RPA.QualityPortal.Services.AuditIdentity.GetName(
                    context.User, (context.User.Identity as System.Security.Claims.ClaimsIdentity)?.NameClaimType);

                user = pdb.People.Where(x => x.StaffNumber == user).Select(x => x.Name).FirstOrDefault();

                if (string.IsNullOrEmpty(user))
                {
                    user = "Unknown";
                }
            }

            return user;

        }
    }
}
