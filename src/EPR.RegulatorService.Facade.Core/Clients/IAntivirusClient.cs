using EPR.RegulatorService.Facade.Core.Models.TradeAntiVirus;

namespace EPR.RegulatorService.Facade.Core.Clients
{
    public interface IAntivirusClient
    {
        Task<HttpResponseMessage> VirusScanFile(FileDetails fileDetails, string fileName, MemoryStream fileStream);
    }
}
