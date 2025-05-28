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

public class RegistrationService(IRegistrationServiceClient registrationServiceClient, IAccountServiceClient accountServiceClient, IPaymentServiceClient paymentServiceClient) : IRegistrationService
{
    public async Task<bool> UpdateRegulatorRegistrationTaskStatus(UpdateRegulatorRegistrationTaskDto request)
    {
        return await registrationServiceClient.UpdateRegulatorRegistrationTaskStatus(request);
    }

    public async Task<bool> UpdateRegulatorApplicationTaskStatus(UpdateRegulatorApplicationTaskDto request)
    {
        return await registrationServiceClient.UpdateRegulatorApplicationTaskStatus(request);
    }

    public async Task<RegistrationOverviewDto> GetRegistrationByRegistrationId(int id)
    {
        return await registrationServiceClient.GetRegistrationByRegistrationId(id);
    }

    public async Task<RegistrationMaterialDetailsDto> GetRegistrationMaterialByRegistrationMaterialId(int id)
    {
        return await registrationServiceClient.GetRegistrationMaterialByRegistrationMaterialId(id);
    }

    public async Task<bool> UpdateMaterialOutcomeByRegistrationMaterialId(int id, UpdateMaterialOutcomeRequestDto request)
    {
        var referenceNumber = await GenerateRegistrationAccreditationReference(id);
        var outcomeRequest = new UpdateMaterialOutcomeWithReferenceDto 
        { 
            Comments = request.Comments,
            Status = request.Status,
            RegistrationReferenceNumber = referenceNumber 
        };
        return await registrationServiceClient.UpdateMaterialOutcomeByRegistrationMaterialId(id, outcomeRequest);
    }

    public async Task<RegistrationMaterialWasteLicencesDto> GetWasteLicenceByRegistrationMaterialId(int id)
    {
        return await registrationServiceClient.GetWasteLicenceByRegistrationMaterialId(id);
    }

    public async Task<RegistrationMaterialReprocessingIODto> GetReprocessingIOByRegistrationMaterialId(int id)
    {
        return await registrationServiceClient.GetReprocessingIOByRegistrationMaterialId(id);
    }

    public async Task<RegistrationMaterialSamplingPlanDto> GetSamplingPlanByRegistrationMaterialId(int id)
    {
        return await registrationServiceClient.GetSamplingPlanByRegistrationMaterialId(id);
    }

    public async Task<SiteAddressDetailsDto> GetSiteAddressByRegistrationId(int id)
    {
        var registrationSiteAddress = await registrationServiceClient.GetSiteAddressByRegistrationId(id);
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

    public async Task<MaterialsAuthorisedOnSiteDto> GetAuthorisedMaterialByRegistrationId(int id)
    {
        return await registrationServiceClient.GetAuthorisedMaterialByRegistrationId(id);
    }

    public async Task<PaymentFeeDetailsDto> GetPaymentFeeDetailsByRegistrationMaterialId(int id)
    {
        var registrationFeeRequestInfos = await registrationServiceClient.GetRegistrationFeeRequestByRegistrationMaterialId(id);
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

    public async Task<bool> MarkAsDulyMadeByRegistrationMaterialId(int id, Guid userId, MarkAsDulyMadeRequestDto request)
    {
        var markAsDulyMadeRequest = new MarkAsDulyMadeWithUserIdDto
        {
            DulyMadeDate = request.DulyMadeDate,
            DeterminationDate = request.DeterminationDate,
            DulyMadeBy = userId
        };

        return await registrationServiceClient.MarkAsDulyMadeByRegistrationMaterialId(id, markAsDulyMadeRequest);
    }

    public async Task<bool> SaveApplicationTaskQueryNotes(int id, Guid userId, QueryNoteRequestDto request)
    {
        request.CreatedBy = userId;

        return await registrationServiceClient.SaveApplicationTaskQueryNotes(id, request);
    }

    public async Task<bool> SaveRegistrationTaskQueryNotes(int id, Guid userId, QueryNoteRequestDto request)
    {
        request.CreatedBy = userId;

        return await registrationServiceClient.SaveRegistrationTaskQueryNotes(id, request);
    }


    private async Task<string> GenerateRegistrationAccreditationReference(int id)
    {
        var referenceInfos = await registrationServiceClient.GetRegistrationAccreditationReference(id);
        var nationDetails = await accountServiceClient.GetNationDetailsById(referenceInfos.NationId);
        var countryCode = nationDetails.Name.First().ToString().ToUpper();
        var orgTypeInitial = referenceInfos.ApplicationType.First().ToString().ToUpper();

        return $"{referenceInfos.RegistrationType}{referenceInfos.RelevantYear}{countryCode}{orgTypeInitial}{referenceInfos.OrgCode}{referenceInfos.RandomDigits}{referenceInfos.MaterialCode}";
    }
}