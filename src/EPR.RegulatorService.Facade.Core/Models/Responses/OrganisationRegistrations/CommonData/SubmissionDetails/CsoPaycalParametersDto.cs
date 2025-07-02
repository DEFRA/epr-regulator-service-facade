using System;
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations.CommonData.SubmissionDetails
{
    [ExcludeFromCodeCoverage]
    public class CsoPaycalParametersDto : PaycalParametersDto
    {
        public string CsoReference { get; set; } = string.Empty;

        public Guid CsoExternalId { get; set; }

        public Guid ComplianceSchemeId { get; set; }

        public bool IsOriginal { get; set; }

        public bool IsNewJoiner { get; set; }
    }
}
