using EPR;
using EPR.RegulatorService;
using EPR.RegulatorService.Facade;
using EPR.RegulatorService.Facade.Core;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Constants;
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
        result.OrganisationName = await accountServiceClient.GetOrganisationNameById(result.OrganisationId);
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
        return await reprocessorExporterServiceClient.GetWasteLicenceByRegistrationMaterialId(id);
    }

    public async Task<RegistrationMaterialReprocessingIODto> GetReprocessingIOByRegistrationMaterialId(Guid id)
    {
        return await reprocessorExporterServiceClient.GetReprocessingIOByRegistrationMaterialId(id);
    }

    public async Task<RegistrationMaterialSamplingPlanDto> GetSamplingPlanByRegistrationMaterialId(Guid id)
    {
        return await reprocessorExporterServiceClient.GetSamplingPlanByRegistrationMaterialId(id);
    }

    public async Task<SiteAddressDetailsDto> GetSiteAddressByRegistrationId(Guid id)
    {
        var registrationSiteAddress = await reprocessorExporterServiceClient.GetSiteAddressByRegistrationId(id);
        var nationDetails = await accountServiceClient.GetNationDetailsById(registrationSiteAddress.NationId);

        return new SiteAddressDetailsDto
        {
            RegistrationId = registrationSiteAddress.RegistrationId,
            NationName = nationDetails.Name,
            SiteAddress = registrationSiteAddress.SiteAddress,
            GridReference = registrationSiteAddress.GridReference,
            LegalCorrespondenceAddress = registrationSiteAddress.LegalCorrespondenceAddress
        };
    }

    public async Task<MaterialsAuthorisedOnSiteDto> GetAuthorisedMaterialByRegistrationId(Guid id)
    {
        return await reprocessorExporterServiceClient.GetAuthorisedMaterialByRegistrationId(id);
    }

    public async Task<PaymentFeeDetailsDto> GetPaymentFeeDetailsByRegistrationMaterialId(Guid id)
    {
        var registrationFeeRequestInfos = await reprocessorExporterServiceClient.GetRegistrationFeeRequestByRegistrationMaterialId(id);
        var organisationName = await accountServiceClient.GetOrganisationNameById(registrationFeeRequestInfos.OrganisationId);
        var nationDetails = await accountServiceClient.GetNationDetailsById(registrationFeeRequestInfos.NationId);
        var paymentFee = await paymentServiceClient.GetRegistrationPaymentFee(registrationFeeRequestInfos.MaterialName,
                                                                              nationDetails.NationCode,
                                                                              registrationFeeRequestInfos.CreatedDate,
                                                                              registrationFeeRequestInfos.ApplicationType.ToString(),
                                                                              registrationFeeRequestInfos.ApplicationReferenceNumber);
        return new PaymentFeeDetailsDto
        {
            RegistrationId = registrationFeeRequestInfos.RegistrationId,
            RegistrationMaterialId = id,
            OrganisationName = organisationName,
            SiteAddress = registrationFeeRequestInfos.SiteAddress,
            ApplicationReferenceNumber = registrationFeeRequestInfos.ApplicationReferenceNumber,
            MaterialName = registrationFeeRequestInfos.MaterialName,
            SubmittedDate = registrationFeeRequestInfos.CreatedDate,
            FeeAmount = paymentFee,
            ApplicationType = registrationFeeRequestInfos.ApplicationType,
            Regulator = nationDetails.NationCode
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

    public async Task<bool> SaveApplicationTaskQueryNotes(int id, Guid userId, QueryNoteRequestDto request)
    {
        request.CreatedBy = userId;

        return await reprocessorExporterServiceClient.SaveApplicationTaskQueryNotes(id, request);
    }

    public async Task<bool> SaveRegistrationTaskQueryNotes(int id, Guid userId, QueryNoteRequestDto request)
    {
        request.CreatedBy = userId;

        return await reprocessorExporterServiceClient.SaveRegistrationTaskQueryNotes(id, request);
    }
}