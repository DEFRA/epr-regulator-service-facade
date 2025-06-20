﻿using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Configs;

[ExcludeFromCodeCoverage]
public class PrnBackendServiceApiConfig
{
    public const string SectionName = "PrnBackendServiceApiConfig";

    public string BaseUrl { get; set; } = null!;
    public int Timeout { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public double ApiVersion { get; set; }
    public int ServiceRetryCount { get; set; }
    public PrnServiceApiConfigEndpoints Endpoints { get; set; } = null!;
}

public class PrnServiceApiConfigEndpoints
{
    public string UpdateRegulatorRegistrationTaskStatusById { get; set; }
    public string UpdateRegulatorApplicationTaskStatusById { get; set; }
    public string RegistrationByRegistrationId { get; set; }
    public string RegistrationMaterialByRegistrationMaterialId { get; set; }
    public string UpdateMaterialOutcomeByRegistrationMaterialId { get; set; }
    public string WasteLicensesByRegistrationMaterialId { get; set; }
    public string ReprocessingIOByRegistrationMaterialId { get; set; }
    public string SamplingPlanByRegistrationMaterialId { get; set; }
    public string SiteAddressByRegistrationId { get; set; }
    public string WasteCarrierDetailsByRegistrationId { get; set; }
    public string AuthorisedMaterialByRegistrationId { get; set; }
    public string RegistrationFeeByRegistrationMaterialId { get; set; }
    public string MarkAsDulyMadeByRegistrationMaterialId { get; set; }
    public string RegistrationAccreditationReference { get; set; }
    public string RegistrationByIdWithAccreditations { get; set; }
    public string SamplingPlanByAccreditationId { get; set; } 
    public string AccreditationFeeByAccreditationMaterialId { get; set; }
    public string MarkAsDulyMadeByAccreditationId { get; set; }
    public string UpdateRegulatorAccreditationTaskStatusById { get; set; }
    public string SaveApplicationTaskQueryNotes { get; set; }
    public string SaveRegistrationTaskQueryNotes { get; set; }
}
