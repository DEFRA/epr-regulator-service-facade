using System.Text.Json;
using EPR.RegulatorService.Facade.Core.Models.Organisations;
using FluentAssertions;

namespace EPR.RegulatorService.Facade.Tests.Core.Models;

[TestClass]
public class OrganisationDetailsTest
{
    [TestMethod]
    public void OrganisationDetailsCanBeDeserialized()
    {
        //Arrange
        var sampleInput = @"{
            ""company"": {
            ""organisationId"": ""100054"",
            ""organisationTypeId"": 6,
            ""companiesHouseNumber"": null,
            ""organisationName"": ""CakeShop"",
            ""isComplianceScheme"": true,
            ""registeredAddress"": {
            ""city"": null,
            ""postCode"": null,
            ""county"": null,
            ""street"": null
            }
            },
            ""companyUserInformation"": [
            {
            ""firstName"": ""Komal"",
            ""lastName"": ""Polavarapu"",
            ""email"": ""test@data.com"",
            ""personRoleId"": 1,
            ""externalId"": ""9a24e292-058b-4be6-ba86-2fe951f0e13d"",
            ""userEnrolments"": [
            {
            ""serviceRoleId"": 4,
            ""enrolmentStatusId"": 3
            }
            ],
            ""isEmployee"": true,
            ""jobTitle"": null,
            ""phoneNumber"": ""01234567890""
            }
            ]
        }";

        //Act
        var deserialized = JsonSerializer.Deserialize<OrganisationDetailResults>(sampleInput,
            new JsonSerializerOptions {PropertyNameCaseInsensitive = true});

        //Assert
        deserialized.CompanyUserInformation.Should().NotBeEmpty();
        deserialized.CompanyUserInformation.First().UserEnrolments.Should().NotBeEmpty();
        deserialized.Company.OrganisationName.Should().Be("CakeShop");
    }
}