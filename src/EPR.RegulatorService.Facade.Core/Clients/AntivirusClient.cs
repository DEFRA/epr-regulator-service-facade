using EPR.RegulatorService.Facade.Core.Models.TradeAntiVirus;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;

namespace EPR.RegulatorService.Facade.Core.Clients
{
    public class AntivirusClient : IAntivirusClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AntivirusClient> _logger;

        public AntivirusClient(HttpClient httpClient, ILogger<AntivirusClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<HttpResponseMessage> VirusScanFile(FileDetails fileDetails, string fileName, MemoryStream fileStream)
        {
            try
            {
                fileDetails.Content = Convert.ToBase64String(fileStream.ToArray());

                var jsonRequest = JsonSerializer.Serialize(fileDetails);
                var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"SyncAV/{fileDetails.Collection}/{fileDetails.Key}", stringContent);

                response.EnsureSuccessStatusCode();

                _logger.LogInformation("The file was successfully sent to antivirus app");

                return response;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error sending file to antivirus api");
                throw;
            }
        }
    }
}
