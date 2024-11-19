using System.Security.Cryptography;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Extensions;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Services.CommonData;
using EPR.RegulatorService.Facade.Core.Services.Submissions;

namespace EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission;

public class OrganisationRegistrationSubmissionService(
    ICommonDataService commonDataService,
    ISubmissionService submissionService) : IOrganisationRegistrationSubmissionService
{
    public async Task<PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>> HandleGetRegistrationSubmissionList(
        GetOrganisationRegistrationSubmissionsFilter filter)
    {
        return await commonDataService.GetOrganisationRegistrationSubmissionList(filter);
    }

    public async Task<RegistrationSubmissionOrganisationDetails?> HandleGetOrganisationRegistrationSubmissionDetails(Guid submissionId)
    {
        return await commonDataService.GetOrganisationRegistrationSubmissionDetails(submissionId);
    }

    public async Task<HttpResponseMessage> HandleCreateRegulatorDecisionSubmissionEvent(
        RegulatorDecisionCreateRequest request, Guid userId)
    {
        var regRefNumber =
            request.Status == RegistrationSubmissionStatus.Granted
                ? GenerateReferenceNumber(
                    request.CountryName,
                    request.RegistrationSubmissionType,
                    request.OrganisationAccountManagementId.ToString(),
                    request.TwoDigitYear)
                : string.Empty;

        return await submissionService.CreateSubmissionEvent(
            request.SubmissionId,
            new RegistrationSubmissionDecisionEvent
            {
                OrganisationId = request.OrganisationId,
                SubmissionId = request.SubmissionId,
                Decision = request.Status.GetRegulatorDecision(),
                Comments = request.Comments,
                RegistrationReferenceNumber = regRefNumber
            },
            userId
        );
    }

    public string GenerateReferenceNumber(CountryName countryName,
        RegistrationSubmissionType registrationSubmissionType, string organisationId, string twoDigitYear = null,
        MaterialType materialType = MaterialType.None)
    {
        if (string.IsNullOrEmpty(twoDigitYear))
        {
            twoDigitYear = (DateTime.Now.Year % 100).ToString("D2");
        }

        if (string.IsNullOrEmpty(organisationId))
        {
            throw new ArgumentNullException(nameof(organisationId));
        }

        var countryCode = ((char)countryName).ToString();

        var regType = ((char)registrationSubmissionType).ToString();

        var refNumber = $"R{twoDigitYear}{countryCode}{regType}{organisationId}{Generate4DigitNumber()}";

        if (registrationSubmissionType == RegistrationSubmissionType.Reprocessor ||
            registrationSubmissionType == RegistrationSubmissionType.Exporter)
        {
            refNumber = $"{refNumber}{materialType.GetDisplayName<MaterialType>()}";
        }

        return refNumber;
    }

    public async Task<HttpResponseMessage> HandleCreateRegistrationFeePaymentSubmissionEvent(RegistrationFeePaymentCreateRequest request, Guid userId)
    {
        return await submissionService.CreateSubmissionEvent(
            request.SubmissionId,
            new RegistrationFeePaymentEvent
            {
                SubmissionId = request.SubmissionId,
                PaymentMethod = request.PaymentMethod,
                PaymentStatus = request.PaymentStatus,
                PaidAmount = $"£{request.PaidAmount}"
            },
            userId
        );
    }

    private static string Generate4DigitNumber()
    {
        var min = 1000;
        var max = 10000;
        var randomNumber = RandomNumberGenerator.GetInt32(min, max);

        return randomNumber.ToString();
    }
}