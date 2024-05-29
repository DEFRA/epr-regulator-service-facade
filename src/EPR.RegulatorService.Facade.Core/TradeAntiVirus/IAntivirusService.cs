using EPR.RegulatorService.Facade.Core.Enums;

namespace EPR.RegulatorService.Facade.Core.Services.TradeAntiVirus
{
    public interface IAntivirusService
    {
        Task<HttpResponseMessage> SendFile(SubmissionType submissionType, Guid fileId, string fileName, MemoryStream fileStream, Guid userId, string email);
    }
}
