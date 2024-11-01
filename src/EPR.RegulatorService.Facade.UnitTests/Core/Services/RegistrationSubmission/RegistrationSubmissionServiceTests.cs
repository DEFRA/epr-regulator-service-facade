using EPR.RegulatorService.Facade.Core.Models.Submissions.Events;
using EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.UnitTests.Core.Services.RegistrationSubmission
{
    [TestClass]
    public class RegistrationSubmissionServiceTests
    {

        [TestMethod]
        public async Task Should_return_valid_referencenumber()
        {
            //Arrange  
            string year = "99";
            string orgId = "123456";
            var service = new RegistrationSubmissionService();

            // Act 
            var result = service.GenerateReferenceNumber(Facade.Core.Enums.CountryName.Eng, Facade.Core.Enums.RegistrationSubmissionType.Producer, orgId, year);

            // Assert  
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length == 15);
            Assert.IsTrue(result.StartsWith("R"));
            Assert.IsTrue(result.StartsWith($"R{year}"));
            Assert.IsTrue(result.StartsWith($"R{year}EP"));
        }

        [TestMethod]
        public async Task Should_return_valid_referencenumber_with_null_year()
        {
            //Arrange  
            string year = (DateTime.Now.Year % 100).ToString("D2");
            string orgId = "123456";
            var service = new RegistrationSubmissionService();

            // Act 
            var result = service.GenerateReferenceNumber(Facade.Core.Enums.CountryName.Eng, Facade.Core.Enums.RegistrationSubmissionType.Producer, orgId, null);

            // Assert  
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length == 15);
            Assert.IsTrue(result.StartsWith("R"));
            Assert.IsTrue(result.StartsWith($"R{year}"));
            Assert.IsTrue(result.StartsWith($"R{year}EP"));
        }

        [TestMethod]
        public async Task Should_throw_exception_if_orgid_isnull()
        {
            //Arrange  
            string year = (DateTime.Now.Year % 100).ToString("D2");
            string orgId = "12345678";
            var service = new RegistrationSubmissionService();

            // Act 
            Assert.ThrowsException<ArgumentNullException>(() => service.GenerateReferenceNumber(Facade.Core.Enums.CountryName.Eng, Facade.Core.Enums.RegistrationSubmissionType.Producer, string.Empty, null) );
             
        }
    }
}
