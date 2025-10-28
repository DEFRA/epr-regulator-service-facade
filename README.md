# EPR Regulator Service Facade

> API facade service for regulator operations in the Extended Producer Responsibility (EPR) system

## Overview

The EPR Regulator Service Facade provides a unified API interface for regulator-specific operations within the Extended Producer Responsibility system. It handles regulator decisions on submissions, organization management, user enrollment, and payment processing. The service acts as a gateway, orchestrating calls to multiple backend services including Accounts, Submissions, Common Data, PRN Backend, and Payment services.

## Prerequisites

### Required for Development
- .NET 8.0 SDK
- Docker (if running containerized)

### External Services (Not provided by Docker Compose)
List of external services that developers need access to but aren't included in local Docker setup:
- **Azure AD B2C** - Authentication and authorization service for user identity management
- **Accounts Service API** - User account and organization management service (default: localhost:5000)
- **Submissions API** - Packaging data submissions management service (default: localhost:7206)
- **Common Data API** - Shared data and submission events service (default: localhost:5001)
- **PRN Backend Service API** - Producer Registration Number backend operations (default: localhost:5168)
- **Payment Backend Service API** - Payment processing and fee calculation service (default: localhost:7107)
- **Azure Storage Blobs** - File storage for documents and submissions
- **Application Insights** - Monitoring and telemetry collection

### Development Setup Notes
- All external service endpoints are configurable via appsettings.json
- The service requires proper Azure AD B2C configuration for authentication
- External services must be running for full functionality (or mocked for development)
- No Docker Compose configuration is provided with this repository

## Getting Started

### Local Development Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/DEFRA/epr-regulator-service-facade.git
   cd epr-regulator-service-facade
   ```

2. **Install dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure environment**
   - Update `src/EPR.RegulatorService.Facade.API/appsettings.json` with your local configuration
   - Configure Azure AD B2C settings
   - Set external service URLs and authentication details

### Running the Application

```bash
# Development mode
dotnet run --project src/EPR.RegulatorService.Facade.API

# Production mode
dotnet run --project src/EPR.RegulatorService.Facade.API --configuration Release
```

The application will be available at:
- Development: `http://localhost:5076` and `https://localhost:7253`
- IIS Express: `http://localhost:33034` and `https://localhost:44316`
- Docker: `http://localhost:8080`
- API Documentation: Available at `/swagger` endpoint

## Testing

### Run All Tests
```bash
dotnet test
# Or use the provided script
./buildscripts/all-tests.sh
```

### Run Unit Tests
```bash
dotnet test src/EPR.RegulatorService.Facade.UnitTests
# Or use the provided script
./buildscripts/unit-tests.sh
```

## Project Structure

```
├── src/
│   ├── EPR.RegulatorService.Facade.API/        # Main API project
│   │   ├── Controllers/                        # API controllers
│   │   ├── Extensions/                         # Service extensions
│   │   ├── Filters/                           # Action filters
│   │   ├── Handlers/                          # Request handlers
│   │   ├── HealthChecks/                      # Health check implementations
│   │   ├── Middlewares/                       # Custom middleware
│   │   ├── Validations/                       # Request validators
│   │   └── appsettings.json                   # Configuration
│   ├── EPR.RegulatorService.Facade.Core/       # Core business logic
│   │   ├── Clients/                           # External service clients
│   │   ├── Models/                            # Data models
│   │   ├── Services/                          # Business services
│   │   └── Configs/                           # Configuration models
│   └── EPR.RegulatorService.Facade.UnitTests/  # Unit tests
├── buildscripts/                               # Build and test scripts
├── pipelines/                                  # Azure DevOps pipelines
└── regulator_facade.sln                       # Solution file
```

## API Documentation

- **Local Development**: Available at `https://localhost:7253/swagger`
- **Docker**: Available at `http://localhost:8080/swagger`

### Main API Endpoints

- **Organizations**: `/api/organisations/*` - Organization search and user management
- **Submissions**: `/api/pom/*` and `/api/registrations/*` - Submission decisions and management
- **Registration Submissions**: `/api/organisation-registration-*` - Organization registration operations
- **Applications**: `/api/applications/*` - Application management
- **File Downloads**: `/api/file-download/*` - Document and file operations
- **Health Check**: `/admin/health` - Service health monitoring

## Configuration

### Key Configuration Sections in appsettings.json

#### ConnectionStrings / Azure Services
- **AzureAdB2C**: Azure AD B2C authentication configuration including sign-up/sign-in policy
- **ApplicationInsights**: Monitoring and telemetry collection settings

#### External Service Configurations
- **AccountsServiceApiConfig**: User account and organization management service connection details
  - BaseUrl: Service endpoint (default: http://localhost:5000/api/)
  - Timeout: Request timeout in seconds (default: 30)
  - ServiceRetryCount: Number of retry attempts (default: 6)
  - Endpoints: Specific API endpoint configurations for various operations

- **SubmissionsApiConfig**: Packaging data submissions service configuration
  - BaseUrl: Service endpoint (default: https://localhost:7206)
  - Timeout: Extended timeout for submission operations (default: 300 seconds)
  - ApiVersion: API version to use (default: 1)

- **CommonDataApiConfig**: Shared data service configuration
  - BaseUrl: Service endpoint (default: http://localhost:5001/api/)
  - Endpoints: Configuration for submission events and data retrieval

- **PrnBackendServiceApiConfig**: Producer Registration Number backend service
  - BaseUrl: Service endpoint (default: http://localhost:5168)
  - Endpoints: Registration and accreditation management endpoints

- **PaymentBackendServiceApiConfig**: Payment processing service
  - BaseUrl: Service endpoint (default: http://localhost:7107)
  - Endpoints: Fee calculation and payment processing operations

#### Application Settings
- **MessagingConfig**: Email template IDs and messaging service configuration for various notification scenarios
- **rolesConfig**: Role-based access control configuration mapping service roles to person roles
- **FeatureManagement**: Feature flags for enabling/disabling functionality (e.g., ReprocessorExporter)

#### Health and Monitoring
- **HealthCheckPath**: Health check endpoint path (default: /admin/health)
- **Logging**: Application logging configuration with different log levels per component

# Contributing to this project
Please read the [contribution guidelines](CONTRIBUTING.md) before submitting a pull request.

# Licence
[Licence information](LICENCE.md).
