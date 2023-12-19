using System.Diagnostics.CodeAnalysis;
using EPR.RegulatorService.Facade.API.Handler;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Services.Application;
using EPR.RegulatorService.Facade.Core.Services.Regulator;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http.Headers;
using EPR.RegulatorService.Facade.Core.Services.CommonData;
using EPR.RegulatorService.Facade.Core.Services.Producer;
using EPR.RegulatorService.Facade.Core.Services.Submissions;

namespace EPR.RegulatorService.Facade.API.Extensions;

[ExcludeFromCodeCoverage]
public static class HttpClientServiceCollectionExtension
{
    public static IServiceCollection AddServicesAndHttpClients(this IServiceCollection services)
    {
        services.AddTransient<AccountServiceAuthorisationHandler>();

        var settings = services.BuildServiceProvider().GetRequiredService<IOptions<AccountsServiceApiConfig>>().Value;
        var submissionSettings =
            services.BuildServiceProvider().GetRequiredService<IOptions<SubmissionsApiConfig>>().Value;
        var commonDataSettings =
            services.BuildServiceProvider().GetRequiredService<IOptions<CommonDataApiConfig>>().Value;

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
        
        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,retryAttempt)));
    }
}