using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Services.Messaging;
using EPR.RegulatorService.Facade.Core.Services.ServiceRoles;
using Notify.Client;
using Notify.Interfaces;

namespace EPR.RegulatorService.Facade.API.Extensions;

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
    }

    private static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<INotificationClient>(_ => new NotificationClient(configuration.GetValue<string>("MessagingConfig:ApiKey")));        
        services.AddSingleton<IMessagingService, MessagingService>();
        services.AddSingleton<IServiceRolesLookupService, ServiceRolesLookupService>();
    }
}
