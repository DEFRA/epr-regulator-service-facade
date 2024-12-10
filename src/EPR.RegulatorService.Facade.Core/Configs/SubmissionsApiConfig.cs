namespace EPR.RegulatorService.Facade.Core.Configs;

public class SubmissionsApiConfig
{
    public const string SectionName = "SubmissionsApiConfig";

    public string BaseUrl { get; set; } = null!;
    public int Timeout { get; set; }
    public int ServiceRetryCount { get; set; }
    public int ApiVersion { get; set; }
    public SubmissionsServiceEndpoints Endpoints { get; set; } = null!;
}

public class SubmissionsServiceEndpoints
{
    public string CreateSubmissionEvent { get; set; } = null!;
    
    public string GetPoMSubmissions { get; set; } = null!;
    
    public string GetRegistrationSubmissions { get; set; } = null!;

    public string GetOrganisationRegistrationEvents { get; set; } = null!;
}