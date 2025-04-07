﻿using System.Threading.Tasks;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

namespace EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;

public interface IRegistrationService
{
    Task<bool> UpdateRegulatorRegistrationTaskStatus(int id, UpdateTaskStatusRequestDto request);
    Task<bool> UpdateRegulatorApplicationTaskStatus(int id, UpdateTaskStatusRequestDto request);
    Task<RegistrationOverviewDto> GetRegistrationByRegistrationId(int id);
    Task<RegistrationMaterialDetailsDto> GetRegistrationMaterialByRegistrationMaterialId(int id);
    Task<bool> UpdateMaterialOutcomeByRegistrationMaterialId(int id, UpdateMaterialOutcomeRequestDto request);
}
