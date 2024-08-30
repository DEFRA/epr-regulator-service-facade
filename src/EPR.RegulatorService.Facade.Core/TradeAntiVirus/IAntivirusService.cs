using EPR.RegulatorService.Facade.Core.Enums;

namespace EPR.RegulatorService.Facade.Core.TradeAntiVirus
{
    public interface IAntivirusService
    {
        Task<HttpResponseMessage> SendFile(SubmissionType submissionType, Guid fileId, string fileName, MemoryStream fileStream, Guid userId, string email);
    }
}
