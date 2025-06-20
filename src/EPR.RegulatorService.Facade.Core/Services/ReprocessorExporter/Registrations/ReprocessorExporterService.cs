﻿using Azure.Core;
using EPR;
using EPR.RegulatorService;
using EPR.RegulatorService.Facade;
using EPR.RegulatorService.Facade.Core;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Constants;
using EPR.RegulatorService.Facade.Core.Models.Organisations;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Services;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;

public class ReprocessorExporterService(IReprocessorExporterServiceClient reprocessorExporterServiceClient, IAccountServiceClient accountServiceClient, IPaymentServiceClient paymentServiceClient) : IReprocessorExporterService
{
    public async Task<bool> UpdateRegulatorRegistrationTaskStatus(UpdateRegulatorRegistrationTaskDto request)
    {
        return await reprocessorExporterServiceClient.UpdateRegulatorRegistrationTaskStatus(request);
    }

    public async Task<bool> UpdateRegulatorApplicationTaskStatus(UpdateRegulatorApplicationTaskDto request)
    {
        return await reprocessorExporterServiceClient.UpdateRegulatorApplicationTaskStatus(request);
    }

    public async Task<RegistrationOverviewDto> GetRegistrationByRegistrationId(Guid id)
    {
        var result = await reprocessorExporterServiceClient.GetRegistrationByRegistrationId(id);
        result.OrganisationName = (await accountServiceClient.GetOrganisationDetailsById(result.OrganisationId))?.OrganisationName;
        return result;
    }

    public async Task<RegistrationMaterialDetailsDto> GetRegistrationMaterialByRegistrationMaterialId(Guid id)
    {
        return await reprocessorExporterServiceClient.GetRegistrationMaterialByRegistrationMaterialId(id);
    }

    public async Task<bool> UpdateMaterialOutcomeByRegistrationMaterialId(Guid id, UpdateMaterialOutcomeRequestDto request)
    {
        var referenceNumber = await GenerateRegistrationAccreditationReference(id);
        var outcomeRequest = new UpdateMaterialOutcomeWithReferenceDto 
        { 
            Comments = request.Comments,
            Status = request.Status,
            RegistrationReferenceNumber = referenceNumber 
        };
        return await reprocessorExporterServiceClient.UpdateMaterialOutcomeByRegistrationMaterialId(id, outcomeRequest);
    }

    public async Task<RegistrationMaterialWasteLicencesDto> GetWasteLicenceByRegistrationMaterialId(Guid id)
    {
        var wasteLicence = await reprocessorExporterServiceClient.GetWasteLicenceByRegistrationMaterialId(id);
        wasteLicence.OrganisationName = (await accountServiceClient.GetOrganisationDetailsById(wasteLicence.OrganisationId))?.OrganisationName;

        return wasteLicence;
    }

    public async Task<RegistrationMaterialReprocessingIODto> GetReprocessingIOByRegistrationMaterialId(Guid id)
    {
        var reprocessingIO = await reprocessorExporterServiceClient.GetReprocessingIOByRegistrationMaterialId(id);
        reprocessingIO.OrganisationName = (await accountServiceClient.GetOrganisationDetailsById(reprocessingIO.OrganisationId))?.OrganisationName;

        return reprocessingIO;
    }

    public async Task<RegistrationMaterialSamplingPlanDto> GetSamplingPlanByRegistrationMaterialId(Guid id)
    {
        var samplingPlan = await reprocessorExporterServiceClient.GetSamplingPlanByRegistrationMaterialId(id);
        samplingPlan.OrganisationName = (await accountServiceClient.GetOrganisationDetailsById(samplingPlan.OrganisationId))?.OrganisationName;

        return samplingPlan;
    }

    public async Task<SiteAddressDetailsDto> GetSiteAddressByRegistrationId(Guid id)
    {
        var registrationSiteAddress = await reprocessorExporterServiceClient.GetSiteAddressByRegistrationId(id);
        var nationDetails = await accountServiceClient.GetNationDetailsById(registrationSiteAddress.NationId);
        var organisationDetails = await accountServiceClient.GetOrganisationDetailsById(registrationSiteAddress.OrganisationId);

        return new SiteAddressDetailsDto
        {
            RegistrationId = registrationSiteAddress.RegistrationId,
            NationName = nationDetails.Name,
            SiteAddress = registrationSiteAddress.SiteAddress,
            GridReference = registrationSiteAddress.GridReference,
            LegalCorrespondenceAddress = registrationSiteAddress.LegalCorrespondenceAddress,
            TaskStatus = registrationSiteAddress.TaskStatus,
            RegulatorRegistrationTaskStatusId = registrationSiteAddress.RegulatorRegistrationTaskStatusId,
            OrganisationName = organisationDetails?.OrganisationName,
            QueryNotes = registrationSiteAddress.QueryNotes
        };
    }

    public async Task<RegistrationWasteCarrierDto> GetWasteCarrierDetailsByRegistrationId(Guid id)
    {
        var wasteCarrier = await reprocessorExporterServiceClient.GetWasteCarrierDetailsByRegistrationId(id);
        wasteCarrier.OrganisationName = (await accountServiceClient.GetOrganisationDetailsById(wasteCarrier.OrganisationId))?.OrganisationName;

        return wasteCarrier;
    }

    public async Task<MaterialsAuthorisedOnSiteDto> GetAuthorisedMaterialByRegistrationId(Guid id)
    {
        var authorisedMaterial = await reprocessorExporterServiceClient.GetAuthorisedMaterialByRegistrationId(id);
        authorisedMaterial.OrganisationName = (await accountServiceClient.GetOrganisationDetailsById(authorisedMaterial.OrganisationId))?.OrganisationName;

        return authorisedMaterial;
    }

    public async Task<PaymentFeeDetailsDto> GetPaymentFeeDetailsByRegistrationMaterialId(Guid id)
    {
        var registrationFeeRequestInfos = await reprocessorExporterServiceClient.GetRegistrationFeeRequestByRegistrationMaterialId(id);
        var organisationDetails = await accountServiceClient.GetOrganisationDetailsById(registrationFeeRequestInfos.OrganisationId);
        var nationDetails = await accountServiceClient.GetNationDetailsById(registrationFeeRequestInfos.NationId);

        var paymentFeeRequest = new PaymentFeeRequestDto
        {
            RequestorType = registrationFeeRequestInfos.ApplicationType.ToString(),
            Regulator = nationDetails.NationCode,
            SubmissionDate = registrationFeeRequestInfos.CreatedDate,
            MaterialType = registrationFeeRequestInfos.MaterialName,
            ApplicationReferenceNumber = registrationFeeRequestInfos.ApplicationReferenceNumber
        };

        var paymentFeeResponse = await paymentServiceClient.GetRegistrationPaymentFee(paymentFeeRequest);

        return new PaymentFeeDetailsDto
        {
            RegistrationId = registrationFeeRequestInfos.RegistrationId,
            RegistrationMaterialId = id,
            OrganisationName = organisationDetails?.OrganisationName,
            SiteAddress = registrationFeeRequestInfos.SiteAddress,
            ApplicationReferenceNumber = registrationFeeRequestInfos.ApplicationReferenceNumber,
            MaterialName = registrationFeeRequestInfos.MaterialName,
            SubmittedDate = registrationFeeRequestInfos.CreatedDate,
            FeeAmount = paymentFeeResponse.RegistrationFee,
            ApplicationType = registrationFeeRequestInfos.ApplicationType,
            Regulator = nationDetails.NationCode,
            TaskStatus = registrationFeeRequestInfos.TaskStatus,
            RegulatorApplicationTaskStatusId = registrationFeeRequestInfos.RegulatorApplicationTaskStatusId,
            QueryNotes = registrationFeeRequestInfos.QueryNotes,
            PaymentMethod = paymentFeeResponse.PreviousPaymentDetail?.PaymentMethod,
            PaymentDate = paymentFeeResponse.PreviousPaymentDetail?.PaymentDate,
            DulyMadeDate = registrationFeeRequestInfos.DulyMadeDate,
            DeterminationDate = registrationFeeRequestInfos.DeterminationDate
        };
    }

    public async Task<bool> SaveOfflinePayment(Guid userId, OfflinePaymentRequestDto request)
    {
        var offlinePaymentRequest = new SaveOfflinePaymentRequestDto
        {
            Amount = request.Amount,
            PaymentReference = request.PaymentReference,
            PaymentDate = request.PaymentDate,
            PaymentMethod = request.PaymentMethod,
            Regulator = request.Regulator,
            UserId = userId,
            Description = ReprocessorExporterConstants.OfflinePaymentRegistrationDescription,
            Comments = ReprocessorExporterConstants.OfflinePaymentRegistrationComment
        };

        return await paymentServiceClient.SaveOfflinePayment(offlinePaymentRequest);
    }

    public async Task<bool> MarkAsDulyMadeByRegistrationMaterialId(Guid id, Guid userId, MarkAsDulyMadeRequestDto request)
    {
        var markAsDulyMadeRequest = new MarkAsDulyMadeWithUserIdDto
        {
            DulyMadeDate = request.DulyMadeDate,
            DeterminationDate = request.DeterminationDate,
            DulyMadeBy = userId
        };

        return await reprocessorExporterServiceClient.MarkAsDulyMadeByRegistrationMaterialId(id, markAsDulyMadeRequest);
    }

    private async Task<string> GenerateRegistrationAccreditationReference(Guid id)
    {
        var referenceInfos = await reprocessorExporterServiceClient.GetRegistrationAccreditationReference(id);
        var nationDetails = await accountServiceClient.GetNationDetailsById(referenceInfos.NationId);
        var countryCode = nationDetails.Name.First().ToString().ToUpper();
        var orgTypeInitial = referenceInfos.ApplicationType.First().ToString().ToUpper();

        return $"{referenceInfos.RegistrationType}{referenceInfos.RelevantYear}{countryCode}{orgTypeInitial}{referenceInfos.OrgCode}{referenceInfos.RandomDigits}{referenceInfos.MaterialCode}";
    }

    public async Task<RegistrationOverviewDto> GetRegistrationByIdWithAccreditationsAsync(Guid id, int? year)
    {
        return await reprocessorExporterServiceClient.GetRegistrationByIdWithAccreditationsAsync(id, year);
    }

    public async Task<AccreditationSamplingPlanDto> GetSamplingPlanByAccreditationId(Guid id)
    {
        return await reprocessorExporterServiceClient.GetSamplingPlanByAccreditationId(id);
    }
    
    public async Task<AccreditationPaymentFeeDetailsDto> GetAccreditationPaymentFeeDetailsByAccreditationId(Guid id)
    {
        var accreditationFeeRequestInfos = await reprocessorExporterServiceClient.GetAccreditationPaymentFeeDetailsByAccreditationId(id);

        var organisationDetailsTask = accountServiceClient.GetOrganisationDetailsById(accreditationFeeRequestInfos.OrganisationId);
        var nationDetailsTask = accountServiceClient.GetNationDetailsById(accreditationFeeRequestInfos.NationId);

        await Task.WhenAll(organisationDetailsTask, nationDetailsTask);

        var organisationDetails = await organisationDetailsTask;
        var nationDetails = await nationDetailsTask;

        var paymentFee = await paymentServiceClient.GetAccreditationPaymentFee(accreditationFeeRequestInfos.MaterialName,
            nationDetails.NationCode,
            accreditationFeeRequestInfos.SubmittedDate,
            accreditationFeeRequestInfos.ApplicationType.ToString(),
            accreditationFeeRequestInfos.ApplicationReferenceNumber);

        return new AccreditationPaymentFeeDetailsDto
        {
            AccreditationId = id,
            OrganisationName = organisationDetails?.OrganisationName,
            SiteAddress = accreditationFeeRequestInfos.SiteAddress,
            ApplicationReferenceNumber = accreditationFeeRequestInfos.ApplicationReferenceNumber,
            PrnTonnage= accreditationFeeRequestInfos.PrnTonnage,
            MaterialName = accreditationFeeRequestInfos.MaterialName,
            SubmittedDate = accreditationFeeRequestInfos.SubmittedDate,
            FeeAmount = paymentFee,
            ApplicationType = accreditationFeeRequestInfos.ApplicationType,
            Regulator = nationDetails.NationCode
        };
    }
    
    public async Task<bool> MarkAsDulyMadeByAccreditationId(Guid id, Guid userId, MarkAsDulyMadeRequestDto request)
    {
        var markAsDulyMadeRequest = new MarkAsDulyMadeWithUserIdDto()
        {
            DulyMadeDate = request.DulyMadeDate,
            DeterminationDate = request.DeterminationDate,
            DulyMadeBy = userId
        };

        return await reprocessorExporterServiceClient.MarkAsDulyMadeByAccreditationId(id, markAsDulyMadeRequest);
    }

    public async Task<bool> UpdateRegulatorAccreditationTaskStatus(Guid userId, UpdateAccreditationTaskStatusDto request)
    {
        var updateAccreditationMaterialTaskStatusWithUserIdDto = new UpdateAccreditationTaskStatusWithUserIdDto()
        {
            AccreditationId = request.AccreditationId,
            TaskName = request.TaskName,
            Status = request.Status,
            Comments = request.Comments,
            UpdatedByUserId = userId
        };

        return await reprocessorExporterServiceClient.UpdateRegulatorAccreditationTaskStatus(updateAccreditationMaterialTaskStatusWithUserIdDto);
    }

    public async Task<bool> SaveAccreditationOfflinePayment(Guid userId, OfflinePaymentRequestDto request)
    {
        var offlinePaymentRequest = new SaveOfflinePaymentRequestDto
        {
            Amount = request.Amount,
            PaymentReference = request.PaymentReference,
            PaymentDate = request.PaymentDate,
            PaymentMethod = request.PaymentMethod,
            Regulator = request.Regulator,
            UserId = userId,
            Description = ReprocessorExporterConstants.OfflinePaymentAccreditationDescription,
            Comments = ReprocessorExporterConstants.OfflinePaymentAccreditationComment
        };

        return await paymentServiceClient.SaveAccreditationOfflinePayment(offlinePaymentRequest);
    }

    public async Task<bool> SaveApplicationTaskQueryNotes(Guid id, Guid userId, QueryNoteRequestDto request)
    {
        request.CreatedBy = userId;

        return await reprocessorExporterServiceClient.SaveApplicationTaskQueryNotes(id, request);
    }

    public async Task<bool> SaveRegistrationTaskQueryNotes(Guid id, Guid userId, QueryNoteRequestDto request)
    {
        request.CreatedBy = userId;

        return await reprocessorExporterServiceClient.SaveRegistrationTaskQueryNotes(id, request);
    }
}