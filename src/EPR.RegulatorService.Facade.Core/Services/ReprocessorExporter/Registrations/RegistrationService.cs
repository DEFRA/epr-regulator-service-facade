using EPR;
using EPR.RegulatorService;
using EPR.RegulatorService.Facade;
using EPR.RegulatorService.Facade.Core;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Registrations;
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
        return await registrationServiceClient.UpdateMaterialOutcomeByRegistrationMaterialId(id, request);
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
        var nationName = await accountServiceClient.GetNationNameById(registrationSiteAddress.NationId);

        return new SiteAddressDetailsDto
        {
            RegistrationId = registrationSiteAddress.RegistrationId,
            NationName = nationName,
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
        var registrationFee = await registrationServiceClient.GetRegistrationFeeByRegistrationMaterialId(id);
        var organisationName = await accountServiceClient.GetOrganisationNameById(id);
        var nationName = await accountServiceClient.GetNationNameById(registrationFee.NationId);
        var registrationPaymentFeeRequest = new RegistrationPaymentFeeRequestDto
        {
            Material = registrationFee.MaterialName,
            Regulator = nationName,
            SubmittedDate = registrationFee.CreatedDate,
            RequestorType = registrationFee.ApplicationType.ToString(),
            Reference = registrationFee.Reference
        };
        var paymentFee = await paymentServiceClient.GetRegistrationPaymentFee(registrationPaymentFeeRequest);
        return new PaymentFeeDetailsDto
        {
            OrganisationName = organisationName,
            SiteAddress = registrationFee.SiteAddress,
            ReferenceNumber = registrationFee.Reference,
            MaterialName = registrationFee.MaterialName,
            ApplicationType = registrationFee.ApplicationType,
            SubmittedDate = registrationFee.CreatedDate,
            FeeAmount = paymentFee
        };
    }
}
