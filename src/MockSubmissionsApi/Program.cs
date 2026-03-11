namespace MockSubmissionsApi;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public static class Program
{
    private static void Main()
    {
        const int port = 5002;
        Console.WriteLine($"MockSubmissionsApi starting on https://localhost:{port}");
        MockSubmissionsApiServer.Start(port: port, useSsl: true);

        Console.WriteLine("Press any key to stop.");
        Console.ReadKey();
    }
}
