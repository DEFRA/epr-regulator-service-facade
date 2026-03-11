# Regulator Facade

Provides data to the [regulator-frontend](https://github.com/DEFRA/epr-regulator-service)

## Dependencies


### backend-account-service

[DEFRA/epr-backend-account-microservice](https://github.com/DEFRA/epr-backend-account-microservice)

This service is behind Azure authentication. Obtaining tokens for this is handled by `AccountServiceAuthorisationHandler`.

The auth handler is not needed for connecting to backend-account-service running locally, and can be bypassed by leaving configuration value `AccountsServiceApiConfig:AccountServiceClientId` blank.

### common-data-api

[DEFRA/epr-common-data-api](https://github.com/DEFRA/epr-common-data-api)

### submissions-api

[DEFRA/epr-pom-api-submission-status](https://github.com/DEFRA/epr-pom-api-submission-status)

### Anti Virus API

See [ADR-023: Anti-Virus Service](https://eaflood.atlassian.net/wiki/spaces/MWR/pages/4318167185/ADR-023+Anti-Virus+Service)

See [DEFRA Trade Anti Virus API - secret renewal](https://eaflood.atlassian.net/wiki/spaces/EDIA/pages/6447759417/DEFRA+Trade+Anti+Virus+API+-+secret+renewal)

## Running locally

The backend dependencies can be run as:

1. **Azure dev environments** (private) - URLs configured in user-secrets
2. **Locally checked out services** - use the `local-backends` launch profile to connect
3. **Mock projects in this sln** - use the `local-backends` launch profile as shown below

### Running with mock backends

Start the mock services:

```sh
dotnet run --project src/MockAccountService/MockAccountService.csproj &
dotnet run --project src/MockCommonData/MockCommonData.csproj &
dotnet run --project src/MockSubmissionsApi/MockSubmissionsApi.csproj &
```

Then run the facade using the `local-backends` launch profile:

```sh
dotnet run --launch-profile "local-backends" --project src/EPR.RegulatorService.Facade.API/EPR.RegulatorService.Facade.API.csproj
```

Or select the `local-backends` profile from your IDE's run configuration menu.

# Contributing to this project
Please read the [contribution guidelines](CONTRIBUTING.md) before submitting a pull request.

# Licence
[Licence information](LICENCE.md).
