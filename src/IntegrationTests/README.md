# EPR Regulator Service Facade - Integration Tests

This test project demonstrates integration testing for the EPR Regulator Service Facade API, testing integration with the CommonData API using MockCommonData.

## Structure

- **FacadeWebApplicationFactory** - Builds and runs the real facade API application for testing
- **MockCommonData** - Provides fake CommonData API responses using WireMock
- **TestAuthHandler** - Replaces production authentication with test authentication, allowing control over auth success/failure and returned claims
- **IntegrationTestBase** - Base class providing common setup, teardown, and helper methods for mocking CommonData API endpoints

## Running Tests

### Run All Integration Tests

```bash
cd c:\Source\Repos\DEFRA\epr-regulator-service-facade
dotnet test src\IntegrationTests\IntegrationTests.csproj
```

### Run Specific Test Class

```bash
# Run OrganisationRegistrationSubmissionsTests
dotnet test src\IntegrationTests\IntegrationTests.csproj --filter "FullyQualifiedName~OrganisationRegistrationSubmissionsTests"

# Run SubmissionsTests
dotnet test src\IntegrationTests\IntegrationTests.csproj --filter "FullyQualifiedName~SubmissionsTests"
```

### Run Specific Test Method

```bash
dotnet test src\IntegrationTests\IntegrationTests.csproj --filter "FullyQualifiedName~GetRegistrationSubmissionList_ReturnsSuccess_WithValidFilter"
```

### Run with Verbose Output

To see detailed logs from the test execution:

```bash
dotnet test src\IntegrationTests\IntegrationTests.csproj --logger "console;verbosity=detailed"
```

### Run with Code Coverage

```bash
dotnet test src\IntegrationTests\IntegrationTests.csproj --collect:"XPlat Code Coverage"
```

## Test Execution

Tests run sequentially (not in parallel) because they share the MockCommonData server instance. This is controlled by the `[Collection("Sequential")]` attribute on test classes.

## Debugging

1. Set breakpoints in your test code or in the facade API code
2. Run tests in debug mode from Visual Studio or VS Code
3. Or use the test explorer to run individual tests with debugging

## References

- [Integration tests in ASP.NET Core | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0&pivots=xunit)
- [WireMock.Net Documentation](https://github.com/WireMock-Net/WireMock.Net)
