namespace EPR.RegulatorService.Facade.Core.Configs;

public class CommonDataApiConfig
{
    public const string SectionName = "CommonDataApiConfig";

    public string BaseUrl { get; set; } = null!;
    public int Timeout { get; set; }
    public int ServiceRetryCount { get; set; }
    public CommonDataServiceEndpoints Endpoints { get; set; } = null!;
}

public class CommonDataServiceEndpoints
{
    public string GetSubmissionEventsLastSyncTime { get; set; } = null!;
    
    public string GetPoMSubmissions { get; set; } = null!;
    
    public string GetRegistrationSubmissions { get; set; } = null!;
}