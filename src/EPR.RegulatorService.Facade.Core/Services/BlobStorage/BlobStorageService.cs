using Azure.Storage.Blobs;
using EPR.RegulatorService.Facade.Core.Exceptions;
using Microsoft.Extensions.Logging;

namespace EPR.RegulatorService.Facade.Core.Services.BlobStorage
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<BlobStorageService> _logger;

        public BlobStorageService(BlobServiceClient blobServiceClient, ILogger<BlobStorageService> logger)
        {
            _blobServiceClient = blobServiceClient;
            _logger = logger;
        }

        public async Task<MemoryStream> DownloadFileStreamAsync(string containerName, string blobName)
        {
            try
            {
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
    }
}