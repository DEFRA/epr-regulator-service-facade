namespace MockSubmissionsApi;

using System.Diagnostics.CodeAnalysis;

using MockSubmissionsApi.SubmissionsApi;

using WireMock.Logging;
using WireMock.Net.StandAlone;
using WireMock.Server;
using WireMock.Settings;

[ExcludeFromCodeCoverage]
public static class MockSubmissionsApiServer
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
            .WithSubmissionsApi();

        return server;
    }
}
