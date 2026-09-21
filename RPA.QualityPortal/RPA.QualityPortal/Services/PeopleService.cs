using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.Services
{
    public class PeopleService : IPeopleService
    {
        IPeopleContext pdb;

        public PeopleService(IPeopleContext context)
        {
            this.pdb = context;
        }

        public bool PersonCheck(string PersonName)
        {
            bool validPerson = true;

            Person person = pdb.People.Where(x => x.Name == PersonName).FirstOrDefault();

            if(person == null)
            {
                validPerson = false;
            }

            return validPerson;
        }



    }
}