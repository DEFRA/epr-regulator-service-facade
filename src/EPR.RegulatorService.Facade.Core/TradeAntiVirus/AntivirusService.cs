using EPR.RegulatorService.Facade.Core.Clients;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models.TradeAntiVirus;
using Microsoft.Extensions.Options;

namespace EPR.RegulatorService.Facade.Core.TradeAntiVirus
{
    public class AntivirusService : IAntivirusService
    {
        private readonly IAntivirusClient _antivirusClient;        

        public AntivirusService(IAntivirusClient antivirusClient)
        {
            _antivirusClient = antivirusClient;            
        }

        public async Task<HttpResponseMessage> SendFile(FileDetails fileDetails, string fileName, MemoryStream fileStream)
        {
            return await _antivirusClient.VirusScanFile(fileDetails, fileName, fileStream);
        }
    }
}