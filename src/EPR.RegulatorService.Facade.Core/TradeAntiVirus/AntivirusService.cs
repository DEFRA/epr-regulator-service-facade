using EPR.RegulatorService.Facade.Core.Clients;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Extensions;
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

        public async Task<HttpResponseMessage> SendFile(SubmissionType submissionType, Guid fileId, string fileName, MemoryStream fileStream, Guid userId, string email)
        {
            var fileDetails = new FileDetails
            {
                Key = fileId,
                Extension = Path.GetExtension(fileName),
                FileName = Path.GetFileNameWithoutExtension(fileName),
                Collection = GetCollectionName(submissionType.GetDisplayName<SubmissionType>()),
                UserId = userId,
                UserEmail = email,
                PersistFile = _antivirusApiConfig.PersistFile,
            };

            return await _antivirusClient.VirusScanFile(fileDetails, fileName, fileStream);
        }

        private string GetCollectionName(string submissionType)
        {
            var suffix = _antivirusApiConfig?.CollectionSuffix;
            return suffix is null ? submissionType : submissionType + suffix;
        }
    }
}