﻿using System.Threading.Tasks;
using EPR;
using EPR.RegulatorService;
using EPR.RegulatorService.Facade;
using EPR.RegulatorService.Facade.Core;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Services;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;

namespace EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;

public class RegistrationService(IRegistrationServiceClient registrationServiceClient) : IRegistrationService
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
}
