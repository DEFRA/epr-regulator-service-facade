using EPR.RegulatorService.Facade.Core.Enums;

namespace EPR.RegulatorService.Facade.Core.Services.BlobStorage
{
    public interface IBlobStorageService
    {
        Task<MemoryStream> DownloadFileStreamAsync(SubmissionType submissionType, string blobName);
    }
}
