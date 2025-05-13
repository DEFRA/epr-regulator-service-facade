using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.Responses.RegistrationSubmissions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;

namespace EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations.CommonData;

[ExcludeFromCodeCoverage]
public class OrganisationRegistrationDetailsDto
{
    public Guid SubmissionId { get; set; }
    public Guid OrganisationId { get; set; }
    public string OrganisationName { get; set; }
    public string OrganisationReference { get; set; }
    public string ApplicationReferenceNumber { get; set; }
    public string? RegistrationReferenceNumber { get; set; }

    public string SubmissionStatus { get; set; }
    public string? StatusPendingDate { get; set; }
    public string SubmittedDateTime { get; set; }
    public bool IsLateSubmission { get; set; }
    public string SubmissionPeriod { get; set; }
    public int RelevantYear { get; set; }

    public bool IsResubmission { get; set; }
    public string ResubmissionStatus { get; set; }
    public string? RegistrationDate { get; set; }
    public string? ResubmissionDate { get; set; }
    public string? ResubmissionFileId { get; set; }
    public bool IsComplianceScheme { get; set; }
    public string OrganisationSize { get; set; }
    public string OrganisationType { get; set; }
    public int NationId { get; set; }
    public string NationCode { get; set; }

    public string? RegulatorComment { get; set; }
    public string? ProducerComment { get; set; }
    public string? RegulatorDecisionDate { get; set; }
    public string? ProducerCommentDate { get; set; }
    public Guid? RegulatorUserId { get; set; }

    // organisation details
    public string? CompaniesHouseNumber { get; set; }
    public string? BuildingName { get; set; }
    public string? SubBuildingName { get; set; }
    public string? BuildingNumber { get; set; }
    public string? Street { get; set; }
    public string? Locality { get; set; }
    public string? DependentLocality { get; set; }
    public string? Town { get; set; }
    public string? County { get; set; }
    public string? Country { get; set; }
    public string? Postcode { get; set; }

    // submittor details

    public Guid? SubmittedUserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Telephone { get; set; }
    public string? ServiceRole { get; set; }
    public int? ServiceRoleId { get; set; }

    // Paycal parameters
    public bool IsOnlineMarketPlace { get; set; }
    public int NumberOfSubsidiaries { get; set; }
    public int NumberOfOnlineSubsidiaries { get; set; }

    public Guid? CompanyDetailsFileId { get; set; }
    public string? CompanyDetailsFileName { get; set; }
    public string? CompanyDetailsBlobName { get; set; }
    public Guid? PartnershipFileId { get; set; }
    public string? PartnershipFileName { get; set; }
    public string? PartnershipBlobName { get; set; }
    public Guid? BrandsFileId { get; set; }
    public string? BrandsFileName { get; set; }
    public string? BrandsBlobName { get; set; }
    public string? CSOJson { get; set; }

    public static implicit operator RegistrationSubmissionOrganisationDetailsFacadeResponse(OrganisationRegistrationDetailsDto dto)
    {
        static DateTime? convertDateTime(string dateTimeString)
        {

            if (!DateTime.TryParseExact(dateTimeString, "yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime tempDate)
                && !DateTime.TryParse(dateTimeString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out tempDate))
            {
                return null;
            }

            DateTime utcDateTime = DateTime.SpecifyKind(tempDate, DateTimeKind.Utc);

            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TimeZoneInfo.FindSystemTimeZoneById("Europe/London"));
        }

        if (dto is null) return null;

        RegistrationSubmissionOrganisationDetailsFacadeResponse response = new ();

        response.SubmissionId = dto.SubmissionId;
        response.OrganisationId = dto.OrganisationId;
        response.OrganisationName = dto.OrganisationName;
        response.OrganisationReference = dto.OrganisationReference;
        response.ApplicationReferenceNumber = dto.ApplicationReferenceNumber;
        response.RegistrationReferenceNumber = dto.RegistrationReferenceNumber ?? string.Empty;

        if (Enum.TryParse<RegistrationSubmissionStatus>(dto.SubmissionStatus, true, out var submissionStatus))
        {
            response.SubmissionStatus = submissionStatus;
        }
        else
        {
            response.SubmissionStatus = RegistrationSubmissionStatus.Pending;
        }

        if (Enum.TryParse<RegistrationSubmissionStatus>(dto.ResubmissionStatus, true, out submissionStatus))
        {
            response.ResubmissionStatus = submissionStatus;
        }
        else
        {
            response.ResubmissionStatus = RegistrationSubmissionStatus.Pending;
        }

        response.RegistrationDate = convertDateTime(dto.RegistrationDate);
        response.ResubmissionDate = convertDateTime(dto.ResubmissionDate);
        response.IsResubmission = dto.IsResubmission;
        response.ResubmissionFileId = dto.ResubmissionFileId;
        response.StatusPendingDate = convertDateTime(dto.StatusPendingDate);
        response.SubmissionDate = convertDateTime(dto.SubmittedDateTime) ?? DateTime.UtcNow;
        response.IsLateSubmission = dto.IsLateSubmission;
        response.SubmissionPeriod = dto.SubmissionPeriod;
        response.RelevantYear = dto.RelevantYear;

        response.IsComplianceScheme = dto.IsComplianceScheme;
        response.OrganisationSize = dto.OrganisationSize;

        if (Enum.TryParse<RegistrationSubmissionOrganisationType>(dto.OrganisationType, true, out var organisationType))
        {
            response.OrganisationType = organisationType;
        }
        else
        {
            response.OrganisationType = RegistrationSubmissionOrganisationType.none;
        }

        response.NationId = dto.NationId;
        response.NationCode = dto.NationCode;

        response.RegulatorComments = dto.RegulatorComment ?? string.Empty;
        response.ProducerComments = dto.ProducerComment ?? string.Empty;
        response.RegulatorDecisionDate = convertDateTime(dto.RegulatorDecisionDate);
        response.ProducerCommentDate = convertDateTime(dto.ProducerCommentDate);
        response.RegulatorUserId = dto.RegulatorUserId;

        response.CompaniesHouseNumber = dto.CompaniesHouseNumber ?? string.Empty;
        response.BuildingName = dto.BuildingName;
        response.SubBuildingName = dto.SubBuildingName;
        response.BuildingNumber = dto.BuildingNumber;
        response.Street = dto.Street ?? string.Empty;
        response.Locality = dto.Locality ?? string.Empty;
        response.DependentLocality = dto.DependentLocality;
        response.Town = dto.Town ?? string.Empty;
        response.County = dto.County ?? string.Empty;
        response.Country = dto.Country ?? string.Empty;
        response.Postcode = dto.Postcode ?? string.Empty;

        response.IsOnlineMarketPlace = dto.IsOnlineMarketPlace;
        response.NumberOfSubsidiaries = dto.NumberOfSubsidiaries;
        response.NumberOfOnlineSubsidiaries = dto.NumberOfOnlineSubsidiaries;
        response.IsLateSubmission = dto.IsLateSubmission;

        // Creating and assigning SubmissionDetails
        var submissionDetails = new RegistrationSubmissionOrganisationSubmissionSummaryDetails
        {
            AccountRoleId = dto.ServiceRoleId,
            AccountRole = dto.ServiceRole,
            SubmittedOnTime = !dto.IsLateSubmission,
            DecisionDate = convertDateTime(dto.StatusPendingDate ?? dto.RegulatorDecisionDate),
            Email = dto.Email,
            TimeAndDateOfSubmission = convertDateTime(dto.SubmittedDateTime) ?? DateTime.UtcNow,
            Telephone = dto.Telephone,
            SubmittedBy = $"{dto.FirstName} {dto.LastName}",
            DeclaredBy = response.OrganisationType == RegistrationSubmissionOrganisationType.compliance
                                                      ? "Not required (compliance scheme)"
                                                      : $"{dto.FirstName} {dto.LastName}",
            Status = Enum.Parse<RegistrationSubmissionStatus>(dto.SubmissionStatus),
            Files = GetSubmissionFileDetails(dto),
            SubmittedByUserId = dto.SubmittedUserId,
            SubmissionPeriod = dto.SubmissionPeriod,
            ResubmissionStatus = dto.ResubmissionStatus,
            RegistrationDate = convertDateTime(dto.RegistrationDate),
            ResubmissionDate = convertDateTime(dto.ResubmissionDate),
            ResubmissionFileId = dto.ResubmissionFileId,
            IsResubmission = dto.IsResubmission
        };
        response.SubmissionDetails = submissionDetails;
        response.IsResubmission = dto.IsResubmission;
        return response;
    }

    private static List<RegistrationSubmissionOrganisationSubmissionSummaryDetails.FileDetails> GetSubmissionFileDetails(OrganisationRegistrationDetailsDto dto)
    {
        List<RegistrationSubmissionOrganisationSubmissionSummaryDetails.FileDetails> objRet =
        [
            new()
            {
                Type = RegistrationSubmissionOrganisationSubmissionSummaryDetails.FileType.company,
                FileId = dto.CompanyDetailsFileId,
                FileName = dto.CompanyDetailsFileName,
                BlobName = dto.CompanyDetailsBlobName
            }
        ];

        if (dto.BrandsFileId != null)
        {
            objRet.Add(new() { Type = RegistrationSubmissionOrganisationSubmissionSummaryDetails.FileType.brands, FileId = dto.BrandsFileId, FileName = dto.BrandsFileName, BlobName = dto.BrandsBlobName });
        }
        if (dto.PartnershipFileId != null)
        {
            objRet.Add(new() { Type = RegistrationSubmissionOrganisationSubmissionSummaryDetails.FileType.partnership, FileId = dto.PartnershipFileId, FileName = dto.PartnershipFileName, BlobName = dto.PartnershipBlobName });
        }

        return objRet;
    }
}
