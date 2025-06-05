using EPR.RegulatorService.Facade.API.Handlers;
using EPR.RegulatorService.Facade.Core.Clients;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Services.Application;
using EPR.RegulatorService.Facade.Core.Services.CommonData;
using EPR.RegulatorService.Facade.Core.Services.Producer;
using EPR.RegulatorService.Facade.Core.Services.Regulator;
using EPR.RegulatorService.Facade.Core.Services.Submissions;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter;

namespace EPR.RegulatorService.Facade.API.Extensions;

[ExcludeFromCodeCoverage]
public static class HttpClientServiceCollectionExtension
{
    public static IServiceCollection AddServicesAndHttpClients(this IServiceCollection services)
    {
        services.AddScoped<IOrganisationRegistrationSubmissionService, OrganisationRegistrationSubmissionService>();
        services.AddTransient<AccountServiceAuthorisationHandler>();
        services.AddTransient<PrnBackendServiceAuthorisationHandler>();
        services.AddTransient<PaymentBackendServiceAuthorisationHandler>();

        var settings = services.BuildServiceProvider().GetRequiredService<IOptions<AccountsServiceApiConfig>>().Value;
        var submissionSettings =
            services.BuildServiceProvider().GetRequiredService<IOptions<SubmissionsApiConfig>>().Value;
        var commonDataSettings =
            services.BuildServiceProvider().GetRequiredService<IOptions<CommonDataApiConfig>>().Value;
        var PrnServiceApiSettings =
            services.BuildServiceProvider().GetRequiredService<IOptions<PrnBackendServiceApiConfig>>().Value;
        var blobStorageSettings = services.BuildServiceProvider().GetRequiredService<IOptions<BlobStorageConfig>>();
        var antivirusSettings = services.BuildServiceProvider().GetRequiredService<IOptions<AntivirusApiConfig>>().Value;
        var paymentServiceApiSettings =
            services.BuildServiceProvider().GetRequiredService<IOptions<PaymentBackendServiceApiConfig>>().Value;

        services.AddHttpClient<IApplicationService, ApplicationService>((sp, client) =>
        {
            client.BaseAddress = new Uri(settings.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(settings.Timeout);
        })
            .AddHttpMessageHandler<AccountServiceAuthorisationHandler>()
            .AddPolicyHandler(GetRetryPolicy(settings.ServiceRetryCount))
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(settings.ServicePooledConnectionLifetime)
            });

        services.AddHttpClient<IRegulatorOrganisationService, RegulatorOrganisationService>((serviceProvider, client) =>
        {
            client.DefaultRequestHeaders.Accept.Clear();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            client.BaseAddress = new Uri(settings.BaseUrl);
        })
            .AddHttpMessageHandler<AccountServiceAuthorisationHandler>()
            .AddPolicyHandler(GetRetryPolicy(settings.ServiceRetryCount))
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(settings.ServicePooledConnectionLifetime)
            });

        services.AddHttpClient<ISubmissionService, SubmissionsService>((sp, client) =>
        {
            client.BaseAddress = new Uri(submissionSettings.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(submissionSettings.Timeout);
        })
            .AddPolicyHandler(GetRetryPolicy(submissionSettings.ServiceRetryCount));

        services.AddHttpClient<ICommonDataService, CommonDataService>((sp, client) =>
        {
            client.BaseAddress = new Uri(commonDataSettings.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(commonDataSettings.Timeout);
        })
            .AddPolicyHandler(GetRetryPolicy(commonDataSettings.ServiceRetryCount));

        services.AddHttpClient<IReprocessorExporterServiceClient, ReprocessorExporterServiceClient>((sp, client) =>
        {
            client.BaseAddress = new Uri(PrnServiceApiSettings.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(PrnServiceApiSettings.Timeout);
        })
        .AddHttpMessageHandler<PrnBackendServiceAuthorisationHandler>()
        .AddPolicyHandler(GetRetryPolicy(PrnServiceApiSettings.ServiceRetryCount));
        
        services.AddHttpClient<IPaymentServiceClient, PaymentServiceClient>((sp, client) =>
        {
            client.BaseAddress = new Uri(paymentServiceApiSettings.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(paymentServiceApiSettings.Timeout);
        })
        .AddHttpMessageHandler<PaymentBackendServiceAuthorisationHandler>()
        .AddPolicyHandler(GetRetryPolicy(paymentServiceApiSettings.ServiceRetryCount));

        services.AddHttpClient<IAccountServiceClient, AccountServiceClient>((sp, client) =>
        {
            client.BaseAddress = new Uri(settings.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(settings.Timeout);
        })
        .AddHttpMessageHandler<AccountServiceAuthorisationHandler>()
        .AddPolicyHandler(GetRetryPolicy(settings.ServiceRetryCount));

        services.AddHttpClient<IProducerService, ProducerService>((sp, client) =>
        {
            client.BaseAddress = new Uri(settings.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(settings.Timeout);
        })
            .AddHttpMessageHandler<AccountServiceAuthorisationHandler>()
            .AddPolicyHandler(GetRetryPolicy(settings.ServiceRetryCount))
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(settings.ServicePooledConnectionLifetime)
            });
        if (antivirusSettings.EnableDirectAccess)
        {
            services.AddHttpClient<IAntivirusClient, AntivirusClient>(client =>
            {
                client.BaseAddress = new Uri($"{antivirusSettings.BaseUrl}/");
                client.Timeout = TimeSpan.FromSeconds(antivirusSettings.Timeout);
            });
        }
        else
        {
            services.AddHttpClient<IAntivirusClient, AntivirusClient>(client =>
            {
                client.BaseAddress = new Uri($"{antivirusSettings.BaseUrl}/v1/");
                client.Timeout = TimeSpan.FromSeconds(antivirusSettings.Timeout);
                client.DefaultRequestHeaders.Add("OCP-APIM-Subscription-Key", antivirusSettings.SubscriptionKey);
            }).AddHttpMessageHandler<AntivirusApiAuthorizationHandler>();
        }

        services.AddAzureClients(cb =>
        {
            cb.AddBlobServiceClient(blobStorageSettings.Value.ConnectionString);
        });
        return services;
    }

    private static Polly.Retry.AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}