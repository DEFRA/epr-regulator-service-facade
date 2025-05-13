namespace EPR.RegulatorService.Facade.Core.TradeAntiVirus
{
    public interface IAntivirusService
    {
        Task<HttpResponseMessage> SendFile(string containerName, Guid fileId, string fileName, MemoryStream fileStream, Guid userId, string email);
    }
}
