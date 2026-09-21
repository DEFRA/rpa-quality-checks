using RPA.QualityPortal.DAL;
using RPA.QualityPortal.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.Helpers
{
    public class UserHelper : IUserHelper
    {
        IPeopleContext pdb;

        public UserHelper(IPeopleContext peopleContext)
        {
            this.pdb = peopleContext;
        }

        public string CurrentUser()
        {
            string user = "Unknown";

            var context = HttpContextManager.Current;
      


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