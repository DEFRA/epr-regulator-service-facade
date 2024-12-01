using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations;
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

    public static List<RegistrationSubmissionOrganisationDetailsResponse> DummyData { get; }

    public static async Task<RegistrationSubmissionOrganisationDetailsResponse?> GetRegistrationSubmissionDetails(Guid submissionId, string url)
    {
        var result = DummyData.Find(x => x.SubmissionId == submissionId);
        if (null == result) return null;
        result.SubmissionDetails = GenerateRandomSubmissionData(result.SubmissionStatus);
        result.SubmissionDate = result.SubmissionDate = result.SubmissionDetails.TimeAndDateOfSubmission;
        result.StatusPendingDate = (result.SubmissionStatus == RegistrationSubmissionStatus.Cancelled)
                                            ? DateTime.Now + TimeSpan.FromDays(2)
                                            : null;
        return result;
    }

    private static List<RegistrationSubmissionOrganisationDetailsResponse> GenerateRegistrationSubmissionDataCollection()
    {
        List<RegistrationSubmissionOrganisationDetailsResponse> objRet = [];

        int count = 0;
        foreach (var line in RegistrationSubmissionTestData.TSVData)
        {
            var existingIdRow = ExistingIds[count].Split("\t");
            var fields = line.Split('\t');

            var dateTime = DateTime.ParseExact(fields[8], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            objRet.Add(new RegistrationSubmissionOrganisationDetailsResponse
            {
                OrganisationReference = fields[0],
                OrganisationName = fields[1],
                OrganisationType = Enum.Parse<RegistrationSubmissionOrganisationType>(fields[2]),
                SubmissionStatus = Enum.Parse<RegistrationSubmissionStatus>(fields[3], true),
                ApplicationReferenceNumber = fields[4],
                RegistrationReferenceNumber = fields[5],
                SubmissionDate = dateTime,
                RelevantYear = dateTime.Year,
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

    private static List<RegistrationSubmissionOrganisationSubmissionSummaryDetails.FileDetails> GenerateRandomFiles()
    {
        var files = new List<RegistrationSubmissionOrganisationSubmissionSummaryDetails.FileDetails>();

        files.Add(new RegistrationSubmissionOrganisationSubmissionSummaryDetails.FileDetails()
        {
            FileId = Guid.NewGuid(),
            FileName = "org.details.acme.csv",
            BlobName = "SubmissionDetails.OrganisationDetails",
            Type = RegistrationSubmissionOrganisationSubmissionSummaryDetails.FileType.company
        });
        files.Add(new RegistrationSubmissionOrganisationSubmissionSummaryDetails.FileDetails()
        {
            FileId = Guid.NewGuid(),
            FileName = "brand.details.acme.csv",
            BlobName = "SubmissionDetails.BrandDetails",
            Type = RegistrationSubmissionOrganisationSubmissionSummaryDetails.FileType.brands
        });
        files.Add(new RegistrationSubmissionOrganisationSubmissionSummaryDetails.FileDetails()
        {
            FileId = Guid.NewGuid(),
            FileName = "partner.details.acme.csv",
            BlobName = "SubmissionDetails.PartnerDetails",
            Type = RegistrationSubmissionOrganisationSubmissionSummaryDetails.FileType.partnership
        });
        return files;
    }
}
