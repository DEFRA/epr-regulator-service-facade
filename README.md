# Regulator Facade

Provides data to the [regulator-frontend](https://github.com/DEFRA/epr-regulator-service)

## Dependencies


### backend-account-service

[DEFRA/epr-backend-account-microservice](https://github.com/DEFRA/epr-backend-account-microservice)

This service is behind Azure authentication. Obtaining tokens for this is handled by `AccountServiceAuthorisationHandler`.

The auth handler is not needed for connecting to backend-account-service running locally, and can be bypassed by leaving configuration value `AccountsServiceApiConfig:AccountServiceClientId` blank.

### common-data-api

[DEFRA/epr-common-data-api](https://github.com/DEFRA/epr-common-data-api)

### Anti Virus API

See [ADR-023: Anti-Virus Service](https://eaflood.atlassian.net/wiki/spaces/MWR/pages/4318167185/ADR-023+Anti-Virus+Service)

See [DEFRA Trade Anti Virus API - secret renewal](https://eaflood.atlassian.net/wiki/spaces/EDIA/pages/6447759417/DEFRA+Trade+Anti+Virus+API+-+secret+renewal)


# Contributing to this project
Please read the [contribution guidelines](CONTRIBUTING.md) before submitting a pull request.

# Licence
[Licence information](LICENCE.md).