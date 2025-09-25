using Azure.Core;
using EPR.RegulatorService.Facade.Core.Clients;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Constants;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Extensions;
using EPR.RegulatorService.Facade.Core.Helpers;
using EPR.RegulatorService.Facade.Core.Models.TradeAntiVirus;
using Microsoft.Extensions.Options;

namespace EPR.RegulatorService.Facade.Core.TradeAntiVirus
{
    public class AntivirusService : IAntivirusService
    {
        private readonly IAntivirusClient _antivirusClient;
        private readonly AntivirusApiConfig _antivirusApiConfig;
        public AntivirusService(IAntivirusClient antivirusClient, IOptions<AntivirusApiConfig> antivirusApiConfig)
        {
            _antivirusClient = antivirusClient;
            _antivirusApiConfig = antivirusApiConfig.Value;
        }

        public async Task<HttpResponseMessage> SendFile(FileDetails fileDetails, string fileName, MemoryStream fileStream)
        {
            return await _antivirusClient.VirusScanFile(fileDetails, fileName, fileStream);
        }

        public async Task<HttpResponseMessage> SendFile(AntiVirusDetails antiVirusDetails, MemoryStream fileStream)
        {
            var userId = antiVirusDetails.UserId;
            var email = antiVirusDetails.UserEmail;

            var truncatedFileName = FileHelpers.GetTruncatedFileName(antiVirusDetails.FileName, FileConstants.FileNameTruncationLength);
            var suffix = _antivirusApiConfig.CollectionSuffix;

            var antiVirusContainer = GetContainerName(antiVirusDetails.SubmissionType.GetDisplayName<SubmissionType>(), suffix);

            var fileDetails = new FileDetails
            {
                Key = antiVirusDetails.FileId,
                Extension = Path.GetExtension(antiVirusDetails.FileName),
                FileName = Path.GetFileNameWithoutExtension(antiVirusDetails.FileName),
                Collection = antiVirusContainer,
                UserId = userId,
                UserEmail = email,
                PersistFile = _antivirusApiConfig.PersistFile
            };

            return await _antivirusClient.VirusScanFile(fileDetails, truncatedFileName, fileStream);
        }

        private static string GetContainerName(string submissionType, string suffix) =>
            suffix is null ? submissionType : submissionType + suffix;
    }
}