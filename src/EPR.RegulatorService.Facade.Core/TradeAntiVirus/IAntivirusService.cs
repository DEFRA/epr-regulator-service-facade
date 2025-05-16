using EPR.RegulatorService.Facade.Core.Models.TradeAntiVirus;

namespace EPR.RegulatorService.Facade.Core.TradeAntiVirus
{
    public interface IAntivirusService
    {        
        Task<HttpResponseMessage> SendFile(FileDetails fileDetails, string fileName, MemoryStream fileStream);
    }
}
