namespace MockCommonData;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public static class Program
{
    private static void Main()
    {
        const int port = 5001; // same as common-data-api when run locally, to make it easy to flip between them
        Console.WriteLine($"MockCommonData starting on http://localhost:{port}");
        MockCommonDataServer.Start(port: port, useSsl: false);

        Console.WriteLine("Press any key to stop.");
        Console.ReadKey();
    }
}
