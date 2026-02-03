namespace MockCommonData;

using System.Diagnostics.CodeAnalysis;

using CommonDataApi;

using WireMock.Logging;
using WireMock.Net.StandAlone;
using WireMock.Server;
using WireMock.Settings;

[ExcludeFromCodeCoverage]
public static class MockCommonDataServer
{
    public static WireMockServer Start(int? port = null, bool useSsl = false)
    {
        var settings = new WireMockServerSettings
        {
            UseSSL = useSsl,
            Logger = new WireMockConsoleLogger()
        };
        if (port.HasValue)
        {
            settings.Port = port.Value;
        }

        var server = StandAloneApp.Start(settings)
            .WithCommonDataApi();

        return server;
    }
}
