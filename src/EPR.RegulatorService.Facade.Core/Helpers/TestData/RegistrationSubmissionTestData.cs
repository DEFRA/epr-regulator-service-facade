using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.Responses.RegistrationSubmissions;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace EPR.RegulatorService.Facade.Core.Helpers.TestData;


[ExcludeFromCodeCoverage]
public static partial class RegistrationSubmissionTestData
{
    static RegistrationSubmissionTestData()
    {
        DummyData = GenerateRegistrationSubmissionDataCollection();
    }

    public static List<RegistrationSubmissionOrganisationDetails> DummyData { get; }

    public static async Task<RegistrationSubmissionOrganisationDetails?> GetRegistrationSubmissionDetails(Guid submissionId, string url)
    {
        var result = DummyData.Find(x => x.SubmissionId == submissionId);
        if (null == result) return null;

        result.SubmissionDetails = GenerateRandomSubmissionData(result.SubmissionStatus);
        result.PaymentDetails = GeneratePaymentDetails();
        result.SubmissionStatusPendingDate = (result.SubmissionStatus == RegistrationSubmissionStatus.Cancelled)
                                            ? DateTime.Now + TimeSpan.FromDays(2)
                                            : null;
        return result;
    }

    private static List<RegistrationSubmissionOrganisationDetails> GenerateRegistrationSubmissionDataCollection()
    {
        List<RegistrationSubmissionOrganisationDetails> objRet = [];

        int count = 0;
        foreach (var line in RegistrationSubmissionTestData.TSVData)
        {
            var existingIdRow = ExistingIds[count].Split("\t");
            var fields = line.Split('\t');

            var dateTime = DateTime.ParseExact(fields[8], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            objRet.Add(new RegistrationSubmissionOrganisationDetails
            {
                OrganisationReference = fields[0],
                OrganisationName = fields[1],
                OrganisationType = Enum.Parse<RegistrationSubmissionOrganisationType>(fields[2]),
                SubmissionStatus = Enum.Parse<RegistrationSubmissionStatus>(fields[3], true),
                ApplicationReferenceNumber = fields[4],
                RegistrationReferenceNumber = fields[5],
                RegistrationDateTime = dateTime,
                RegistrationYear = dateTime.Year.ToString(CultureInfo.InvariantCulture),
                CompaniesHouseNumber = fields[9],
                BuildingName = fields[10],
                SubBuildingName = fields[11],
                BuildingNumber = fields[12],
                Street = fields[13],
                Locality = fields[14],
                DependentLocality = fields[15],
                Town = fields[16],
                County = fields[17],
                Country = fields[18],
                Postcode = fields[19],
                OrganisationId = Guid.Parse(fields[22]),
                SubmissionId = Guid.Parse(fields[23]),
                NationId = int.Parse(fields[24], CultureInfo.InvariantCulture),
                RegulatorComments = fields[20],
                ProducerComments = fields[21]
            });
            count++;

            if (count == ExistingIds.Length - 1)
            {
                count = 0;
            }
        }

        return objRet;
    }

    private static RegistrationSubmissionOrganisationSubmissionSummaryDetails GenerateRandomSubmissionData(RegistrationSubmissionStatus registrationStatus)
    {
        var random = new Random(); // NOSONAR - this is dummy disposable data

        string[] sampleNames = ["Alice", "Bob", "Charlie", "Diana", "Edward"];
        var sampleRoles = Enum.GetValues(typeof(ServiceRole));
        var generateRandomPhoneNumber = (Random random) => $"{random.Next(100, 999)}-{random.Next(100, 999)}-{random.Next(1000, 9999)}";

        return new RegistrationSubmissionOrganisationSubmissionSummaryDetails
        {
            Status = registrationStatus,
            DecisionDate = DateTime.Now.AddDays(-random.Next(1, 100)),
            TimeAndDateOfSubmission = DateTime.Now.AddDays(-random.Next(1, 100)),
            SubmittedOnTime = random.Next(2) == 0,
            SubmittedBy = sampleNames[random.Next(sampleNames.Length)],
            AccountRole = ((ServiceRole)sampleRoles.GetValue(random.Next(sampleRoles.Length))).ToString(),
            Telephone = generateRandomPhoneNumber(random),
            Email = $"syed.raza.external@eviden.com",
            DeclaredBy = sampleNames[random.Next(sampleNames.Length)],
            Files = GenerateRandomFiles()
        };
    }

    private static RegistrationSubmissionsOrganisationPaymentDetails GeneratePaymentDetails()
    {
        var random = new Random(); // NOSONAR - this is dummy disposable data

        var generateRandomDecimal = (int min, int max) => Math.Round((decimal)((random.NextDouble() * (max - min)) + min), 2);

        return new RegistrationSubmissionsOrganisationPaymentDetails()
        {
            ApplicationProcessingFee = generateRandomDecimal(1000, 6000),
            OnlineMarketplaceFee = generateRandomDecimal(1000, 5000),
            PreviousPaymentsReceived = generateRandomDecimal(1000, 10000000),
            SubsidiaryFee = generateRandomDecimal(1000, 10000)
        };
    }

    private static List<RegistrationSubmissionOrganisationSubmissionSummaryDetails.FileDetails> GenerateRandomFiles()
    {
        var files = new List<RegistrationSubmissionOrganisationSubmissionSummaryDetails.FileDetails>();

        files.Add(new RegistrationSubmissionOrganisationSubmissionSummaryDetails.FileDetails()
        {
            DownloadUrl = "#",
            FileName = "org.details.acme.csv",
            Label = "SubmissionDetails.OrganisationDetails"
        });
        files.Add(new RegistrationSubmissionOrganisationSubmissionSummaryDetails.FileDetails()
        {
            DownloadUrl = "#",
            FileName = "brand.details.acme.csv",
            Label = "SubmissionDetails.BrandDetails"
        });
        files.Add(new RegistrationSubmissionOrganisationSubmissionSummaryDetails.FileDetails()
        {
            DownloadUrl = "#",
            FileName = "partner.details.acme.csv",
            Label = "SubmissionDetails.PartnerDetails"
        });
        return files;
    }
}
