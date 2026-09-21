using System;
using System.Linq;

namespace RPA.QualityPortal.Helpers
{
    public static class EmailHelper
    {
        public static (string teamMemberEmail, string lineManagerEmail) GetOutgoingEmailAddresses(IPeopleContext pdb, string teamMemberName, string lineManagerName)
        {
            var teamMember = pdb.People.FirstOrDefault(x => x.Name == teamMemberName);
            var lineManager = pdb.People.FirstOrDefault(x => x.Name == lineManagerName);

            if (teamMember == null)
            {
                throw new ArgumentException("No team member found with the specified name", nameof(teamMemberName));
            }

            if (lineManager == null)
            {
                throw new ArgumentException("No line manager found with the specified name", nameof(lineManagerName));
            }

            var teamMemberEmail = teamMember.Email;

            switch (teamMember.Name)
            {
                case "Harrison, Victoria":
                    teamMemberEmail = "[REDACTED_EMAIL]";
                    break;
            }

            return (teamMemberEmail, lineManager.Email);
        }
    }
}