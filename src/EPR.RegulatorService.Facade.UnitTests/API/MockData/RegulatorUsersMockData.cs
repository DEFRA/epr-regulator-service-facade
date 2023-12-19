using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.Organisations;

namespace EPR.RegulatorService.Facade.Tests.API.MockData
{
    public static class RegulatorUsersMockData
    {
        public static List<OrganisationUser> GetRegulatorUsers()
        {
            return new List<OrganisationUser>()
            {
                new()
                {
                    FirstName = "test",
                    LastName = " user 1",
                    Email = "testuser1@test.com",
                    Enrolments = new List<OrganisationUserEnrolment>()
                    {
                        new()
                        {
                            EnrolmentStatus = EnrolmentStatus.Approved,
                            ServiceRoleId = 1
                        }
                    }
                },
                new()
                {
                    FirstName = "test",
                    LastName = " user 2",
                    Email = "testuser2@test.com",
                    Enrolments = new List<OrganisationUserEnrolment>()
                    {
                        new()
                        {
                            EnrolmentStatus = EnrolmentStatus.Approved,
                            ServiceRoleId = 2
                        }
                    }
                },
            };
        }
    }
}