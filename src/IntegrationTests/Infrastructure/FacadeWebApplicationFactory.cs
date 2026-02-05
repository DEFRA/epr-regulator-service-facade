namespace IntegrationTests.Infrastructure;

using EPR.RegulatorService.Facade.API.Controllers;
using MockCommonData;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

using WireMock.Server;
using System;

public class FacadeWebApplicationFactory : CustomWebApplicationFactory<ApplicationController>
{
    private readonly WireMockServer _commonDataServer = MockCommonData.MockCommonDataServer.Start(useSsl: false);

    public WireMockServer CommonDataServer => _commonDataServer;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        
        // Set environment variables to override appsettings.json (environment variables have higher priority)
        Environment.SetEnvironmentVariable("CommonDataApiConfig__BaseUrl", $"{_commonDataServer.Url}/api/");
        Environment.SetEnvironmentVariable("CommonDataApiConfig__Timeout", "30");
        Environment.SetEnvironmentVariable("CommonDataApiConfig__ServiceRetryCount", "6");
        Environment.SetEnvironmentVariable("CommonDataApiConfig__Endpoints__GetSubmissionEventsLastSyncTime", "submission-events/get-last-sync-time");
        Environment.SetEnvironmentVariable("CommonDataApiConfig__Endpoints__GetPoMSubmissions", "submissions/pom/summary");
        Environment.SetEnvironmentVariable("CommonDataApiConfig__Endpoints__GetRegistrationSubmissions", "submissions/registrations/summary");
        Environment.SetEnvironmentVariable("CommonDataApiConfig__Endpoints__GetOrganisationRegistrationSubmissionDetails", "submissions/organisation-registration-submission");
        Environment.SetEnvironmentVariable("CommonDataApiConfig__Endpoints__GetOrganisationRegistrationSubmissionsSummaries", "submissions/organisation-registrations");
        Environment.SetEnvironmentVariable("CommonDataApiConfig__Endpoints__GetPomResubmissionPaycalParameters", "submissions/pom-resubmission-paycal-parameters");
        
        // SubmissionsApiConfig - mock endpoints will be handled by WireMock in tests
        Environment.SetEnvironmentVariable("SubmissionsApiConfig__BaseUrl", $"{_commonDataServer.Url}/api/");
        Environment.SetEnvironmentVariable("SubmissionsApiConfig__Timeout", "300");
        Environment.SetEnvironmentVariable("SubmissionsApiConfig__ServiceRetryCount", "6");
        Environment.SetEnvironmentVariable("SubmissionsApiConfig__ApiVersion", "1");
        Environment.SetEnvironmentVariable("SubmissionsApiConfig__Endpoints__CreateSubmissionEvent", "v{0}/submissions/{1}/events");
        Environment.SetEnvironmentVariable("SubmissionsApiConfig__Endpoints__GetPoMSubmissions", "v{0}/submissions/events/get-regulator-pom-decision");
        Environment.SetEnvironmentVariable("SubmissionsApiConfig__Endpoints__GetRegistrationSubmissions", "v{0}/submissions/events/get-regulator-registration-decision");
        Environment.SetEnvironmentVariable("SubmissionsApiConfig__Endpoints__GetOrganisationRegistrationEvents", "v{0}/submissions/events/organisation-registration");

        builder.ConfigureTestServices(services =>
        {
            // Remove the existing authentication services
            var authDescriptors = services.Where(d => d.ServiceType.FullName?.Contains("Authentication") == true).ToList();
            foreach (var descriptor in authDescriptors)
            {
                services.Remove(descriptor);
            }

            // Replace with test authentication handler
            services.AddAuthentication(TestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _commonDataServer?.Stop();
            _commonDataServer?.Dispose();
        }
        base.Dispose(disposing);
    }
}
