
using System.Diagnostics.CodeAnalysis;
using EPR;
using EPR.RegulatorService;
using EPR.RegulatorService.Facade;
using EPR.RegulatorService.Facade.Core;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models;
using EPR.RegulatorService.Facade.Core.Models.Requests;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Responses.RegistrationSubmissions;

namespace EPR.RegulatorService.Facade.Core.Models.Responses.RegistrationSubmissions;

[ExcludeFromCodeCoverage]
public class RegistrationSubmissionOrganisationSubmissionSummaryDetails
{
    public class FileDetails
    {
        public string Label { get; set; }
        public string FileName { get; set; }
        public string DownloadUrl { get; set; }
    }

    public RegistrationSubmissionStatus Status { get; set; }
    public DateTime DecisionDate { get; set; }

    public DateTime TimeAndDateOfSubmission { get; set; }
    public bool SubmittedOnTime { get; set; }
    public string SubmittedBy { get; set; }
    public string AccountRole { get; set; }
    public string Telephone { get; set; }
    public string Email { get; set; }
    public string DeclaredBy { get; set; }

    public List<FileDetails> Files { get; set; } = [];
}
