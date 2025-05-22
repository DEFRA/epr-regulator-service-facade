namespace EPR.RegulatorService.Facade.Core.Services.BlobStorage
{
    public interface IBlobStorageService
    {        
        Task<MemoryStream> DownloadFileStreamAsync(string containerName, string blobName);
    }
}
