using EPR.RegulatorService.Facade.API.Handlers;
using EPR.RegulatorService.Facade.Core.Clients;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Accreditations;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Services.BlobStorage;
using EPR.RegulatorService.Facade.Core.Services.Messaging;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Accreditations;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Services.ServiceRoles;
using EPR.RegulatorService.Facade.Core.TradeAntiVirus;
using Notify.Client;
using Notify.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.API.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtension
{
    public static void RegisterComponents(this IServiceCollection services, IConfiguration configuration)
    {
        RegisterConfigs(services, configuration);
        RegisterServices(services, configuration);
    }

    private static void RegisterConfigs(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AccountsServiceApiConfig>(configuration.GetSection(AccountsServiceApiConfig.SectionName));
        services.Configure<SubmissionsApiConfig>(configuration.GetSection(SubmissionsApiConfig.SectionName));
        services.Configure<CommonDataApiConfig>(configuration.GetSection(CommonDataApiConfig.SectionName));
        services.Configure<ServiceRolesConfig>(configuration.GetSection(ServiceRolesConfig.SectionName));
        services.Configure<MessagingConfig>(configuration.GetSection(MessagingConfig.SectionName));
        services.Configure<BlobStorageConfig>(configuration.GetSection(BlobStorageConfig.SectionName));
        services.Configure<AntivirusApiConfig>(configuration.GetSection(AntivirusApiConfig.SectionName));
        services.Configure<PrnBackendServiceApiConfig>(configuration.GetSection(PrnBackendServiceApiConfig.SectionName));
        services.Configure<PaymentBackendServiceApiConfig>(configuration.GetSection(PaymentBackendServiceApiConfig.SectionName));
    }

    private static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<INotificationClient>(_ => new NotificationClient(configuration.GetValue<string>("MessagingConfig:ApiKey")));
        services.AddSingleton<IMessagingService, MessagingService>();
        services.AddSingleton<IServiceRolesLookupService, ServiceRolesLookupService>();
        services.AddSingleton<IBlobStorageService, BlobStorageService>();
        services.AddScoped<IAntivirusService, AntivirusService>();
        services.AddScoped<IAntivirusClient, AntivirusClient>();
        services.AddScoped<AntivirusApiAuthorizationHandler>();
        services.AddScoped<IReprocessorExporterServiceClient, ReprocessorExporterServiceClient>();
        services.AddScoped<IReprocessorExporterService, ReprocessorExporterService>();
        services.AddScoped<IAccountServiceClient, AccountServiceClient>();
        services.AddScoped<IPaymentServiceClient, PaymentServiceClient>();
        services.AddScoped<IAccreditationServiceClient, AccreditationServiceClient>();
        services.AddScoped<IAccreditationService, AccreditationService>();
    }
}