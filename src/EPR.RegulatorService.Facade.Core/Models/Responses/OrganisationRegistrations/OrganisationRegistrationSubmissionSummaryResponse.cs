using EPR.RegulatorService.Facade.Core.Enums;
using System;

namespace EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations
{
    public class OrganisationRegistrationSubmissionSummaryResponse
    {
        public Guid SubmissionId { get; set; }

        public Guid OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        public string OrganisationReference { get; set; }

        public OrganisationType OrganisationType { get; set; }

        public string ApplicationReferenceNumber { get; set; }

        public string RegistrationReferenceNumber { get; set; }

        public DateTime SubmissionDate { get; set; }

        public string RegistrationYear { get; set; }

        public RegistrationSubmissionStatus RegistrationStatus { get; set; }

        public int NationId { get; set; }
    }
}