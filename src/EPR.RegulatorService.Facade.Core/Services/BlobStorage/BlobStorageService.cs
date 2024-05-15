using Azure.Storage.Blobs;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Exceptions;
using EPR.RegulatorService.Facade.Core.Configs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EPR.RegulatorService.Facade.Core.Services.BlobStorage
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobStorageConfig _options;
        private readonly ILogger<BlobStorageService> _logger;

        public BlobStorageService(BlobServiceClient blobServiceClient, IOptions<BlobStorageConfig> options, ILogger<BlobStorageService> logger)
        {
            _blobServiceClient = blobServiceClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<MemoryStream> DownloadFileStreamAsync(SubmissionType submissionType, string blobName)
        {
            try
            {
                var containerName = SetContainerName(submissionType);
                var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blob = blobContainerClient.GetBlobClient(blobName);
                var stream = new MemoryStream();

                await blob.DownloadToAsync(stream);

                _logger.LogInformation("File retrieved from blob storage");

                return stream;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred during download from blob storage");
                throw new BlobStorageServiceException("Error occurred during download from blob storage", exception);
            }
        }

        private string SetContainerName(SubmissionType submissionType) =>
            submissionType == SubmissionType.Producer
                ? _options.PomContainerName
                : _options.RegistrationContainerName;
    }
}