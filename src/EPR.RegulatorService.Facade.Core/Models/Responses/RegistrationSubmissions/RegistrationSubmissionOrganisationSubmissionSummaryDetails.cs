﻿using System.Diagnostics.CodeAnalysis;
using EPR.RegulatorService.Facade.Core.Enums;

namespace EPR.RegulatorService.Facade.Core.Models.Responses.RegistrationSubmissions;

[ExcludeFromCodeCoverage]
public class RegistrationSubmissionOrganisationSubmissionSummaryDetails
{
    public enum FileType { company, brands, partnership }
    public class FileDetails
    {
        public FileType Type { get; set; }
        public Guid? FileId { get; set; }
        public string FileName { get; set; }
        public string? BlobName { get; set; }
    }

    public RegistrationSubmissionStatus Status { get; set; }
    public DateTime? DecisionDate { get; set; }
    public DateTime? ResubmissionDecisionDate { get; set; }
    public DateTime? StatusPendingDate { get; set; }
    public DateTime TimeAndDateOfSubmission { get; set; }
    public bool SubmittedOnTime { get; set; }
    public Guid? SubmittedByUserId { get; set; }
    public string? AccountRole { get; set; }
    public string? Telephone { get; set; }
    public string? Email { get; set; }
    public string? DeclaredBy { get; set; }

    public List<FileDetails> Files { get; set; } = [];
    public string SubmissionPeriod { get; internal set; }
    public int? AccountRoleId { get; internal set; }
    public string SubmittedBy { get; internal set; }
    public string ResubmissionStatus { get; internal set; }
    public DateTime? RegistrationDate { get; internal set; }
    public DateTime? ResubmissionDate { get; internal set; }
    public bool IsResubmission { get; internal set; }
    public string? ResubmissionFileId { get; internal set; }
}
