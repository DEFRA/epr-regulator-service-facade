
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;

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
