using System;
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations.CommonData.SubmissionDetails
{
    [ExcludeFromCodeCoverage]
    public class PaycalParametersDto
    {
        public bool IsCso { get; set; }

        public string SubmissionPeriod { get; set; } = string.Empty;

        public int ReferenceNumber { get; set; }

        public string ExternalId { get; set; } = string.Empty;

        public string OrganisationName { get; set; } = string.Empty;

        public int RelevantYear { get; set; }

        public DateTime SubmittedDate { get; set; }

        public DateTime EarliestSubmissionDate { get; set; }

        public string OrganisationSize { get; set; } = string.Empty;

        public string? LeaverCode { get; set; }

        public DateTime? LeaverDate { get; set; }

        public DateTime? JoinerDate { get; set; }

        public string OrganisationChangeReason { get; set; } = string.Empty;

        public bool IsOnlineMarketPlace { get; set; }

        public int NoOfSubsidiaries { get; set; }

        public int NoOfSubsidiariesBeingOnlineMarketPlace { get; set; }

        public Guid FileName { get; set; }

        public Guid FileId { get; set; }

        public bool IsLateFee { get; set; }

    }
}
