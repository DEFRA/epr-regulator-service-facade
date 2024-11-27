using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.Responses.RegistrationSubmissions;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations.CommonData
{
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
        public bool IsComplianceScheme { get; set; }
        public string OrganisationSize { get; set; }
        public string OrganisationType { get; set; }
        public int NationId { get; set; }

        public string NationCode { get; set; }

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

        public string? RegulatorComment { get; set; }
        public string? ProducerComment { get; set; }
        public string? RegulatorDecisionDate { get; set; }
        public string? ProducerCommentDate { get; set; }
        public Guid? RegulatorUserId { get; set; }

        public string CompanyDetailsFileId { get; set; }
        public string CompanyDetailsFileName { get; set; }
        public string CompanyDetailsBlobName { get; set; }
        public string? PartnershipFileId { get; set; }
        public string? PartnershipFileName { get; set; }
        public string? PartnershipBlobName { get; set; }
        public string? BrandsFileId { get; set; }
        public string? BrandsFileName { get; set; }
        public string? BrandsBlobName { get; set; }

        // Paycal parameters
        public bool IsOnlineMarketPlace { get; set; }
        public int NumberOfSubsidiaries { get; set; }
        public int NumberOfOnlineSubsidiaries { get; set; }

        public static implicit operator RegistrationSubmissionOrganisationDetailsResponse(OrganisationRegistrationDetailsDto dto)
        {
            if (dto is null) return null;

            return new RegistrationSubmissionOrganisationDetailsResponse
            {
                SubmissionId = dto.SubmissionId,
                OrganisationId = dto.OrganisationId,
                OrganisationReference = dto.OrganisationReference,
                OrganisationName = dto.OrganisationName,
                OrganisationSize = dto.OrganisationSize,
                IsComplianceScheme = dto.IsComplianceScheme,
                OrganisationType = Enum.TryParse<RegistrationSubmissionOrganisationType>(
                        dto.OrganisationType, true, out var organisationType)
                        ? organisationType
                        : throw new InvalidCastException($"Invalid OrganisationType: {dto.OrganisationType}"),
                NationId = dto.NationId,
                NationCode = dto.NationCode,
                SubmissionPeriod = dto.SubmissionPeriod,
                RegistrationYear = dto.RelevantYear,
                RegistrationDateTime = DateTime.ParseExact(dto.SubmittedDateTime, "yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture),
                SubmissionStatus = Enum.TryParse<RegistrationSubmissionStatus>(
                        dto.SubmissionStatus, true, out var submissionStatus)
                        ? submissionStatus
                        : throw new InvalidCastException($"Invalid SubmissionStatus: {dto.SubmissionStatus}"),
                SubmissionStatusPendingDate = !string.IsNullOrWhiteSpace(dto.StatusPendingDate)
                        ? DateTime.ParseExact(dto.StatusPendingDate, "yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture)
                        : null,
                RegulatorComments = dto.RegulatorComment ?? string.Empty,
                ProducerComments = dto.ProducerComment ?? string.Empty,
                ApplicationReferenceNumber = dto.ApplicationReferenceNumber,
                RegistrationReferenceNumber = dto.RegistrationReferenceNumber ?? string.Empty,
                CompaniesHouseNumber = dto.CompaniesHouseNumber ?? string.Empty,
                BuildingName = dto.BuildingName,
                SubBuildingName = dto.SubBuildingName,
                BuildingNumber = dto.BuildingNumber,
                Street = dto.Street ?? string.Empty,
                Locality = dto.Locality ?? string.Empty,
                DependentLocality = dto.DependentLocality,
                Town = dto.Town ?? string.Empty,
                County = dto.County ?? string.Empty,
                Country = dto.Country ?? string.Empty,
                Postcode = dto.Postcode ?? string.Empty,
                SubmissionDetails = new RegistrationSubmissionOrganisationSubmissionSummaryDetails
                {
                    AccountRole = dto.ServiceRole,
                    SubmittedOnTime = dto.IsLateSubmission,
                    DecisionDate = DateTime.Parse(dto.StatusPendingDate, CultureInfo.InvariantCulture),
                    Email = dto.Email,
                    TimeAndDateOfSubmission = DateTime.Parse(dto.SubmittedDateTime, CultureInfo.InvariantCulture),
                    Telephone = dto.Telephone,
                    DeclaredBy = $"{dto.FirstName} {dto.LastName}",
                    Status = Enum.Parse<RegistrationSubmissionStatus>(dto.SubmissionStatus),
                    Files = GetSubmissionFileDetails(dto),
                    SubmittedByUserId = dto.SubmittedUserId,
                    SubmissionPeriod = dto.SubmissionPeriod
                },
                RegulatorDecisionDate = dto.RegulatorDecisionDate,
                ProducerCommentDate = dto.ProducerCommentDate,
                RegulatorUserId = dto.RegulatorUserId,
                IsOnlineMarketPlace = dto.IsOnlineMarketPlace,
                NumberOfSubsidiaries = dto.NumberOfSubsidiaries,
                NumberOfOnlineSubsidiaries = dto.NumberOfOnlineSubsidiaries,
                IsLateSubmission = dto.IsLateSubmission
            };
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
                objRet.Add(new() { FileId = dto.BrandsFileId, FileName = dto.BrandsFileName, BlobName = dto.BrandsBlobName });
            }
            if (dto.PartnershipFileId != null)
            {
                objRet.Add(new() { FileId = dto.PartnershipFileId, FileName = dto.PartnershipFileName, BlobName = dto.PartnershipBlobName });
            }

            return objRet;
        }
    }
}
