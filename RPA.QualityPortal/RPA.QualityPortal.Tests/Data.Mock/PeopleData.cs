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
                    Email = "person1@test.com",
                    Supervisor = "M600200"
                },
                new Person
                {
                    Id = Guid.NewGuid(),
                    Name = "Gordon, Lee George",
                    StaffNumber = "M600300",
                    Email = "person2@test.com",
                    Supervisor = "M600200"
                },
                new Person
                {
                    Id = Guid.NewGuid(),
                    Name = "Dormand, Scott",
                    StaffNumber = "M600400",
                    Email = "person3@test.com",
                    Supervisor = "M600200"
                },
                new Person
                {
                    Id = Guid.NewGuid(),
                    Name = "Fazackerley, Paul",
                    StaffNumber = "M600100",
                    Email = "person4@test.com",
                    Supervisor = "M600200"
                },
                new Person
                {
                    Id = Guid.NewGuid(),
                    Name = "Slee, Alan",
                    StaffNumber = "M600200",
                    Email = "person5@test.com"
                }
            };
        }
    }
}
