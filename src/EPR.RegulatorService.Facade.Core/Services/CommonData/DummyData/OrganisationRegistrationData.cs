using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.Responses.Registrations;

namespace EPR.RegulatorService.Facade.Core.Services.CommonData.DummyData;

public class OrganisationRegistrationDataCollection
{
    public List<OrganisationRegistrationData> Items { get; set; }

}

/// <summary>
/// A class to capture the data requirements for the client.  Will be retired when concrete services
/// are available from either CommonData, the BackendAccounts or other
/// </summary>
public class OrganisationRegistrationData
{
    public Guid SubmissionID { get; set; }
    public Guid OrganisationID { get; set; }
    public string OrganisationReference { get; set; }
    public string OrganisationName { get; set; }
    public string OrganisationType { get; set; }
    public int NationID { get; set; }
    public int RegistrationYear { get; set; }
    public string Period { get; set; }
    public DateTime RegistrationDateTime { get; set; }
    public string RegistrationStatus { get; set; }
    public string? RegulatorComments { get; set; } = string.Empty;
    public string? ProducerComments { get; set; } = string.Empty;
    public string ApplicationReferenceNumber { get; set; } = String.Empty;
    public string? RegistrationReferenceNumber { get; set; } = String.Empty;
    public string CompaniesHouseNumber { get; set; }
    public string? BuildingName { get; set; }
    public string? SubBuildingName { get; set; }
    public string? BuildingNumber { get; set; }
    public string Street { get; set; }
    public string Locality { get; set; }
    public string? DependentLocality { get; set; }
    public string Town { get; set; }
    public string County { get; set; }
    public string Country { get; set; }
    public string Postcode { get; set; }

    public static implicit operator OrganisationRegistrationSummaryResponse(OrganisationRegistrationData data)
    {
        if (data == null) return null;

        return new OrganisationRegistrationSummaryResponse
        {
            SubmissionId = data.SubmissionID,
            NationId = data.NationID,
            CompaniesHouseNumber = data.CompaniesHouseNumber,
            OrganisationName = data.OrganisationName,
            OrganisationId = data.OrganisationID.ToString(),
            OrganisationReference = data.OrganisationReference,
            OrganisationType = (Enums.OrganisationType)Enum.Parse(typeof(Enums.OrganisationType), data.OrganisationType),
            RegistrationDateTime = data.RegistrationDateTime,
            RegistrationYear = data.RegistrationYear,
            RegistrationStatus = (Enums.RegistrationStatus)Enum.Parse(typeof(Enums.RegistrationStatus), data.RegistrationStatus),
            ApplicationReferenceNumber= data.ApplicationReferenceNumber,
            RegistrationReferenceNumber= data.RegistrationReferenceNumber,
            ProducerComments = data.ProducerComments,
            RegulatorComments = data.RegulatorComments,
            BuildingName = data.BuildingName,
            SubBuildingName = data.SubBuildingName,
            BuildingNumber = data.BuildingNumber,
            Locality = data.Locality,
            DependantLocality = data.DependentLocality,
            Street = data.Street,
            Town = data.Town,
            County = data.County,
            Country = data.Country,
            PostCode = data.Postcode
        };
    }
}
