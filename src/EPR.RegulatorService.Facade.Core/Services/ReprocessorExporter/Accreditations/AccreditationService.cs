using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Accreditations;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Accreditations;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

namespace EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Accreditations;

public class AccreditationService(IAccreditationServiceClient accreditationServiceClient, IAccountServiceClient accountServiceClient, IPaymentServiceClient paymentServiceClient) : IAccreditationService
{
    public async Task<AccreditationPaymentFeeDetailsDto> GetPaymentFeeDetailsByAccreditationMaterialId(Guid id)
    {
        var accreditationFeeRequestInfos = await accreditationServiceClient.GetAccreditationFeeRequestByRegistrationMaterialId(id);

        var organisationNameTask = accountServiceClient.GetOrganisationNameById(accreditationFeeRequestInfos.OrganisationId);
        var nationDetailsTask = accountServiceClient.GetNationDetailsById(accreditationFeeRequestInfos.NationId);

        await Task.WhenAll(organisationNameTask, nationDetailsTask);

        var organisationName = await organisationNameTask;
        var nationDetails = await nationDetailsTask;

        var paymentFee = await paymentServiceClient.GetAccreditationPaymentFee(accreditationFeeRequestInfos.MaterialName,
            nationDetails.NationCode,
            accreditationFeeRequestInfos.CreatedDate,
            accreditationFeeRequestInfos.ApplicationType.ToString(),
            accreditationFeeRequestInfos.ApplicationReferenceNumber);
        
        return new AccreditationPaymentFeeDetailsDto
        {
            RegistrationId = accreditationFeeRequestInfos.RegistrationId,
            AccreditationId = accreditationFeeRequestInfos.AccreditationId,
            RegistrationMaterialId = id,
            OrganisationName = organisationName,
            SiteAddress = accreditationFeeRequestInfos.SiteAddress,
            ApplicationReferenceNumber = accreditationFeeRequestInfos.ApplicationReferenceNumber,
            MaterialName = accreditationFeeRequestInfos.MaterialName,
            SubmittedDate = accreditationFeeRequestInfos.CreatedDate,
            FeeAmount = paymentFee,
            ApplicationType = accreditationFeeRequestInfos.ApplicationType,
            Regulator = nationDetails.NationCode
        };
    }

    public async Task<bool> MarkAccreditationMaterialStatusAsDulyMade(Guid userId, AccreditationMarkAsDulyMadeRequestDto request)
    {
        var markAsDulyMadeRequest = new AccreditationMarkAsDulyMadeWithUserIdDto()
        {
            AccreditationId = request.AccreditationId,
            RegistrationMaterialId = request.RegistrationMaterialId,
            DulyMadeDate = request.DulyMadeDate,
            DeterminationDate = request.DeterminationDate,
            DulyMadeBy = userId
        };

        return await accreditationServiceClient.MarkAccreditationMaterialStatusAsDulyMade(markAsDulyMadeRequest);
    }

    public async Task<bool> UpdateAccreditationMaterialTaskStatus(Guid userId, UpdateAccreditationMaterialTaskStatusDto request)
    {
        var updateAccreditationMaterialTaskStatusWithUserIdDto = new UpdateAccreditationMaterialTaskStatusWithUserIdDto()
        {
            AccreditationId = request.AccreditationId,
            RegistrationMaterialId = request.RegistrationMaterialId,
            TaskId = request.TaskId,
            TaskStatus = request.TaskStatus,
            Comments = request.Comments,
            UpdatedByUserId = userId
        };

        return await accreditationServiceClient.UpdateAccreditationMaterialTaskStatus(updateAccreditationMaterialTaskStatusWithUserIdDto);
    }
}