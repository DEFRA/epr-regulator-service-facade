using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.Responses.RegistrationSubmissions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations.CommonData.SubmissionDetails
{
    [ExcludeFromCodeCoverage]
    public static class SubmissionDetailsMapper
    {
        public static OrganisationRegistrationSubmissionDetailsResponse MapFromSubmissionDetailsResponse(SubmissionDetailsDto dto)
        {
            if (dto is null) return null;

            OrganisationRegistrationSubmissionDetailsResponse response = new()
            {
                SubmissionId = dto.SubmissionId,
                OrganisationId = dto.OrganisationId,
                OrganisationName = dto.OrganisationName,
                OrganisationReference = dto.OrganisationReference,
                ApplicationReferenceNumber = dto.ApplicationReferenceNumber,
                RegistrationReferenceNumber = dto.RegistrationReferenceNumber ?? string.Empty
            };

            if (Enum.TryParse<RegistrationSubmissionStatus>(dto.SubmissionStatus, true, out var parsedSubmissionStatus))
            {
                response.SubmissionStatus = parsedSubmissionStatus;
            }
            else
            {
                response.SubmissionStatus = RegistrationSubmissionStatus.Pending;
            }

            if (Enum.TryParse<RegistrationSubmissionStatus>(dto.ResubmissionStatus, true, out var parsedResubmissionStatus))
            {
                response.ResubmissionStatus = parsedResubmissionStatus;
            }
            else
            {
                response.ResubmissionStatus = RegistrationSubmissionStatus.None;
            }

            response.RegistrationDate = dto.RegistrationDate;
            response.ResubmissionDate = dto.ResubmissionDate;
            response.IsResubmission = dto.IsResubmission;
            response.ResubmissionFileId = dto.ResubmissionFileId;
            response.StatusPendingDate = dto.StatusPendingDate;
            response.SubmissionDate = dto.SubmittedDateTime ?? DateTime.UtcNow;
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
            response.RegulatorDecisionDate = dto.RegulatorDecisionDate;
            response.RegulatorResubmissionDecisionDate = dto.RegulatorResubmissionDecisionDate;
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

            // Creating and assigning SubmissionDetails
            var submissionDetails = new RegistrationSubmissionOrganisationSubmissionSummaryDetails
            {
                AccountRoleId = dto.ServiceRoleId,
                AccountRole = dto.ServiceRole,
                DecisionDate = dto.RegulatorDecisionDate,
                ResubmissionDecisionDate = dto.RegulatorResubmissionDecisionDate,
                StatusPendingDate = dto.StatusPendingDate,
                Email = dto.Email,
                TimeAndDateOfSubmission = dto.SubmittedDateTime ?? DateTime.UtcNow,
                Telephone = dto.Telephone,
                SubmittedBy = $"{dto.FirstName} {dto.LastName}",
                DeclaredBy = response.OrganisationType == RegistrationSubmissionOrganisationType.compliance
                                                          ? "Not required (compliance scheme)"
                                                          : $"{dto.FirstName} {dto.LastName}",
                Status = parsedSubmissionStatus == RegistrationSubmissionStatus.None ? RegistrationSubmissionStatus.Pending : parsedSubmissionStatus,
                Files = GetSubmissionFileDetails(dto),
                SubmittedByUserId = dto.SubmittedUserId,
                SubmissionPeriod = dto.SubmissionPeriod,
                ResubmissionStatus = parsedResubmissionStatus,
                RegistrationDate = dto.RegistrationDate,
                ResubmissionDate = dto.ResubmissionDate,
                ResubmissionFileId = dto.ResubmissionFileId,
                IsResubmission = dto.IsResubmission
            };
            response.SubmissionDetails = submissionDetails;
            response.IsResubmission = dto.IsResubmission;

            return response;
        }

        public static void MapFromProducerPaycalParametersResponse(OrganisationRegistrationSubmissionDetailsResponse response,
                                                                    PaycalParametersDto producerPaycalParametersDto)
        {
            if(response == null || producerPaycalParametersDto == null) return;

            response.IsOnlineMarketPlace = producerPaycalParametersDto.IsOnlineMarketPlace;
            response.NumberOfSubsidiaries = producerPaycalParametersDto.NoOfSubsidiaries;
            response.NumberOfOnlineSubsidiaries = producerPaycalParametersDto.NoOfSubsidiariesBeingOnlineMarketPlace;
            response.IsLateSubmission = producerPaycalParametersDto.IsLateFee;

            response.SubmissionDetails.SubmittedOnTime = !producerPaycalParametersDto.IsLateFee;
        }

        public static void MapFromCsoPaycalParametersResponse(OrganisationRegistrationSubmissionDetailsResponse response,
                                                               List<PaycalParametersDto> csoPaycalParametersDtos)
        {
            response.CsoMembershipDetails = [.. csoPaycalParametersDtos.Select(x => new CsoMembershipDetailsDto
            {
                MemberId = x.CsoReference,
                MemberType = x.OrganisationSize.Equals("L", StringComparison.CurrentCultureIgnoreCase) ? "large" : "small",
                IsOnlineMarketPlace = x.IsOnlineMarketPlace,
                IsLateFeeApplicable = x.IsLateFee,
                NoOfSubsidiariesOnlineMarketplace = x.NoOfSubsidiariesBeingOnlineMarketPlace,
                NumberOfSubsidiaries = x.NoOfSubsidiaries,
                RelevantYear = x.RelevantYear,
                SubmittedDate = x.SubmittedDate,
                SubmissionPeriodDescription = x.SubmissionPeriod
            })];

            response.SubmissionDetails.SubmittedOnTime = !csoPaycalParametersDtos.TrueForAll(x => x.IsLateFee);
        }

        private static List<RegistrationSubmissionOrganisationSubmissionSummaryDetails.FileDetails> GetSubmissionFileDetails(SubmissionDetailsDto dto)
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
}
