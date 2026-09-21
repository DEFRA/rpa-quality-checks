using Moq;
using RPA.QualityPortal.Tests.Data.Mock;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.QualityPortal.Tests.DAL.Mock
{
    public class MockPeopleContext
    {
        public Mock<IPeopleContext> MockContext { get; set; }

        public virtual Mock<DbSet<Location>> MockLocations { get; set; }
        public virtual Mock<DbSet<Manager>> MockManagers { get; set; }
        public virtual Mock<DbSet<Person>> MockPeople { get; set; }


        public MockPeopleContext(bool setMocks = true)
        {
            if (setMocks)
            {
                SetMocks();
            }
        }

        public void SetMocks()
        {
            SetMockContext();
            SetMockLocation();
            SetMockManager();
            SetMockPeople();
        }

        public void SetMockLocation()
        {
            MockLocations = new Mock<DbSet<Location>>().SetupData(LocationData.Data());
            MockContext.Setup(x => x.Locations).Returns(MockLocations.Object);
        }

        public void SetMockManager()
        {
            MockManagers = new Mock<DbSet<Manager>>().SetupData(ManagerData.Data());
            MockContext.Setup(x => x.Managers).Returns(MockManagers.Object);
        }

        public void SetMockPeople()
        {
            MockPeople = new Mock<DbSet<Person>>().SetupData(PeopleData.Data());
            MockContext.Setup(x => x.People).Returns(MockPeople.Object);
        }

        public void SetMockContext()
        {
            MockContext = new Mock<IPeopleContext>();
        }
    }
}
