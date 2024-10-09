using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.Responses.Registrations;

namespace EPR.RegulatorService.Facade.Core.Services.CommonData.DummyData;

public class OrganisationRegistrationDataCollection
{
    public List<OrganisationRegistrationData> Items { get; set; }

}

public class OrganisationRegistrationData
{
    public string OrganisationName { get; set; }
    public string OrganisationId { get; set; }
    public string OrganisationReference { get; set; }
    public OrganisationType OrganisationType { get; set; }
    public DateTime RegistrationDateTime { get; set; }
    public int RegistrationYear { get; set; }
    public RegistrationStatus RegistrationStatus { get; set; }

    public static implicit operator OrganisationRegistrationSummaryResponse(OrganisationRegistrationData data)
    {
        if (data == null) return null;

        return new OrganisationRegistrationSummaryResponse
        {
            OrganisationName = data.OrganisationName,
            OrganisationId = data.OrganisationId,
            OrganisationReference = data.OrganisationReference,
            OrganisationType = data.OrganisationType,
            RegistrationDateTime = data.RegistrationDateTime,
            RegistrationYear = data.RegistrationYear,
            RegistrationStatus = data.RegistrationStatus
        };
    }
}
