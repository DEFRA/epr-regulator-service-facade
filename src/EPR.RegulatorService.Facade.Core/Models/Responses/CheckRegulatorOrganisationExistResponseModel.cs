using System.Text.Json.Serialization;

namespace EPR.RegulatorService.Facade.Core.Models.Responses
{
    public class CheckRegulatorOrganisationExistResponseModel
    {
        [JsonPropertyName("createdOn")]
        public DateTimeOffset CreatedOn { get; set; }

        [JsonPropertyName("externalId")]
        public Guid ExternalId { get; set; }
    }
}
