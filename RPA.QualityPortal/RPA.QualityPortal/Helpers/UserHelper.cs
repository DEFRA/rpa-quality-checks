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
                user = context.User.Identity.Name.ToLower().Replace("earth\\", "").Replace("m0", "m").Replace("demeter\\","");

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
