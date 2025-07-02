using System;
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations.CommonData.SubmissionDetails
{
    [ExcludeFromCodeCoverage]
    public class PaycalParametersDto
    {
        public string CsoReference { get; set; } = string.Empty;
        public string SubmissionPeriod { get; set; } = string.Empty;

        public int RelevantYear { get; set; }

        public DateTime SubmittedDate { get; set; }

        public string OrganisationSize { get; set; } = string.Empty;

        public bool IsOnlineMarketPlace { get; set; }

        public int NoOfSubsidiaries { get; set; }

        public int NoOfSubsidiariesBeingOnlineMarketPlace { get; set; }

        public bool IsLateFee { get; set; }

    }
}
