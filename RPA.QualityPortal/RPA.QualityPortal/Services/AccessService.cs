using RPA.QualityPortal.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;


namespace RPA.QualityPortal.Services
{
    public class AccessService : IAccessService
    {
        IQualityContext db;
        IPeopleContext pdb;

        public AccessService(IQualityContext context, IPeopleContext peopleContext)
        {
            this.db = context;
            this.pdb = peopleContext;
        }


        public string ChangeRoles(string name, string access)
        {
            var userId = pdb.People.Where(x => x.Name == name).Select(x => x.Id).FirstOrDefault();
            string currentRole = "Quality Checks: OC Team Member";
            string newRole = "Quality Checks: OC Team Manager";
            string error = "";

            if (access == "Team Manager to Team Member")
            {
                currentRole = "Quality Checks: OC Team Manager";
                newRole = "Quality Checks: OC Team Member";
            }
            else if (access == "Team Manager to Admin")
            {
                currentRole = "Quality Checks: OC Team Manager";
                newRole = "Quality Checks: OC Admin";
            }
            else if (access == "Admin to Team Manager")
            {
                currentRole = "Quality Checks: OC Admin";
                newRole = "Quality Checks: OC Team Manager";
            }

            if (CheckRole(userId, currentRole))
            {
                string conString = System.Configuration.ConfigurationManager.ConnectionStrings["IDTSecurityConnection"].ConnectionString;
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlCommand cmd = new SqlCommand("[dbo].[ChangeRoles]", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@UserId", SqlDbType.VarChar).Value = userId;
                        cmd.Parameters.Add("@currentRole", SqlDbType.VarChar).Value = currentRole;
                        cmd.Parameters.Add("@newRole", SqlDbType.VarChar).Value = newRole;

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                error = "Unable to change access as user does not have correct access";
            }
            return error;
        }
        private bool CheckRole(Guid userId, string currentRole)
        {
            string conString = System.Configuration.ConfigurationManager.ConnectionStrings["IDTSecurityConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                using (SqlCommand cmd = new SqlCommand("[dbo].[Check Roles]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserId", SqlDbType.VarChar).Value = userId;
                    cmd.Parameters.Add("@currentRole", SqlDbType.VarChar).Value = currentRole;
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Close();
                        return true;
                    }
                    else
                    {
                        reader.Close();
                        return false;
                    }
                }
            }
        }
    }
}