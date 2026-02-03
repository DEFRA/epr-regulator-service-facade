namespace MockAccountService;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public static class Program
{
    private static void Main()
    {
        const int port = 5000; // same as account-service when run locally, to make it easy to flip between them
        Console.WriteLine($"MockAccountService starting on http://localhost:{port}");
        MockAccountServiceServer.Start(port: port, useSsl: true);

        Console.WriteLine("Press any key to stop.");
        Console.ReadKey();
    }
}
