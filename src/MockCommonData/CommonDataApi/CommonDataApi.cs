using System;
using System.IO;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace MockCommonData.CommonDataApi
{
    public static class CommonDataApi
{
    public static WireMockServer WithCommonDataApi(this WireMockServer server)
    {
        // GET /api/submission-events/get-last-sync-time
        server.Given(Request.Create()
                .UsingGet()
                .WithPath("/api/submission-events/get-last-sync-time"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile("Responses/CommonDataApi/get-last-sync-time.json"));

        // POST /api/submissions/pom/summary
        server.Given(Request.Create()
                .UsingPost()
                .WithPath("/api/submissions/pom/summary"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile("Responses/CommonDataApi/pom-summary.json"));

        // POST /api/submissions/registrations/summary
        server.Given(Request.Create()
                .UsingPost()
                .WithPath("/api/submissions/registrations/summary"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile("Responses/CommonDataApi/registrations-summary.json"));

        // GET /api/submissions/organisation-registrations/{nationId}
        server.Given(Request.Create()
                .UsingGet()
                .WithPath("/api/submissions/organisation-registrations/*"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile("Responses/CommonDataApi/organisation-registrations.json"));

        // GET /api/submissions/organisation-registration-submission/{submissionId}
        var organisationRegistrationSubmissionDetailsPath = "Responses/CommonDataApi/OrganisationRegistrationSubmissionDetails";
        if (!Directory.Exists(organisationRegistrationSubmissionDetailsPath))
        {
            throw new DirectoryNotFoundException($"Directory not found: {organisationRegistrationSubmissionDetailsPath}");
        }

        foreach (var filePath in Directory.GetFiles(organisationRegistrationSubmissionDetailsPath, "*.json"))
        {
            var submissionGuid = Path.GetFileNameWithoutExtension(filePath);
            if (!Guid.TryParse(submissionGuid, out _))
            {
                throw new InvalidOperationException($"File name is not a valid GUID: {filePath}");
            }

            server.Given(Request.Create()
                    .UsingGet()
                    .WithPath($"/api/submissions/organisation-registration-submission/{submissionGuid}"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyFromFile(filePath));
        }

        // GET /api/submissions/pom-resubmission-paycal-parameters/{submissionId}
        server.Given(Request.Create()
                .UsingGet()
                .WithPath("/api/submissions/pom-resubmission-paycal-parameters/*"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile("Responses/CommonDataApi/pom-resubmission-paycal-parameters.json"));

        return server;
    }
    }
}
