using System;
using System.Collections.Generic;

namespace RPA.QualityPortal.Tests.Data.Mock
{
    public static class PeopleData
    {
        public static List<Person> Data()
        {
            return new List<Person>
            {
                new Person
                {
                    Id = Guid.NewGuid(),
                    Name = "Toward, Fay",
                    StaffNumber = "M600500",
                    Email = "[REDACTED_EMAIL]",
                    Supervisor = "M600200"
                },
                new Person
                {
                    Id = Guid.NewGuid(),
                    Name = "Gordon, [REDACTED_NAME]",
                    StaffNumber = "M600300",
                    Email = "[REDACTED_EMAIL]",
                    Supervisor = "M600200"
                },
                new Person
                {
                    Id = Guid.NewGuid(),
                    Name = "Dormand, Scott",
                    StaffNumber = "M600400",
                    Email = "[REDACTED_EMAIL]",
                    Supervisor = "M600200"
                },
                new Person
                {
                    Id = Guid.NewGuid(),
                    Name = "Fazackerley, Paul",
                    StaffNumber = "M600100",
                    Email = "[REDACTED_EMAIL]",
                    Supervisor = "M600200"
                },
                new Person
                {
                    Id = Guid.NewGuid(),
                    Name = "Slee, Alan",
                    StaffNumber = "M600200",
                    Email = "[REDACTED_EMAIL]"
                }
            };
        }
    }
}
