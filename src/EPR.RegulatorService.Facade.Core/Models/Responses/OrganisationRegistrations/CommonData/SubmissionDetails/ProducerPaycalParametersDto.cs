using System;
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations.CommonData.SubmissionDetails
{
    [ExcludeFromCodeCoverage]
    public class ProducerPaycalParametersDto : PaycalParametersDto
    {
        public string ProducerReference { get; set; } = string.Empty;
    }
}
