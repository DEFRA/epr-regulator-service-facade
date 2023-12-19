using EPR.RegulatorService.Facade.Core.Enums;
using System.Text.Json.Serialization;

namespace EPR.RegulatorService.Facade.Core.Models.Organisations;

public class OrganisationUserEnrolment
{
    [JsonPropertyName("EnrolmentStatusId")]
    public EnrolmentStatus EnrolmentStatus { get; set; }
    public int ServiceRoleId { get; set; }
}
