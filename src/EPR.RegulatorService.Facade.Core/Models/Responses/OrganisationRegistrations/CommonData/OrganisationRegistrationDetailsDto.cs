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

        public string CompanyDetailsFileId { get; set; }
        public string CompanyDetailsFileName { get; set; }
        public string CompanyDetailsBlobName { get; set; }
        public string? PartnershipFileId { get; set; }
        public string? PartnershipFileName { get; set; }
        public string? PartnershipBlobName { get; set; }
        public string? BrandsFileId { get; set; }
        public string? BrandsFileName { get; set; }
        public string? BrandsBlobName { get; set; }

        public static implicit operator RegistrationSubmissionOrganisationDetailsResponse(OrganisationRegistrationDetailsDto dto)
        {
            DateTime tempDate;

            if (dto is null) return null;

            var response = new RegistrationSubmissionOrganisationDetailsResponse();

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
            response.StatusPendingDate = !string.IsNullOrWhiteSpace(dto.StatusPendingDate)
                ? DateTime.ParseExact(dto.StatusPendingDate, "yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture)
                : null;
            response.SubmissionDate = DateTime.ParseExact( dto.SubmittedDateTime, "yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture);
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
            response.RegulatorDecisionDate = (DateTime.TryParse(dto.RegulatorDecisionDate, CultureInfo.InvariantCulture, out tempDate)
                                                                        ? tempDate
                                                                        : (DateTime?)null);
            response.ProducerCommentDate = (DateTime.TryParse(dto.ProducerCommentDate, CultureInfo.InvariantCulture, out tempDate)
                                                                        ? tempDate
                                                                        : (DateTime?)null);
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
                SubmittedOnTime = dto.IsLateSubmission,
                DecisionDate = (DateTime.TryParse(dto.StatusPendingDate, CultureInfo.InvariantCulture, out tempDate)
                                                                        ? tempDate
                                                                        : (DateTime?)null),
                Email = dto.Email,
                TimeAndDateOfSubmission = DateTime.Parse(dto.SubmittedDateTime, CultureInfo.InvariantCulture),
                Telephone = dto.Telephone,
                SubmittedBy = $"{dto.FirstName} {dto.LastName}",
                DeclaredBy = response.OrganisationType == RegistrationSubmissionOrganisationType.compliance
                                                          ? "Not required (compliance scheme)"
                                                          : $"{dto.FirstName} {dto.LastName}",
                Status = Enum.Parse<RegistrationSubmissionStatus>(dto.SubmissionStatus),
                Files = GetSubmissionFileDetails(dto),
                SubmittedByUserId = dto.SubmittedUserId,
                SubmissionPeriod = dto.SubmissionPeriod
            };
            response.SubmissionDetails = submissionDetails;

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
