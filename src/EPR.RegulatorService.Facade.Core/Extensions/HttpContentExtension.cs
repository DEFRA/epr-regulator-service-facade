using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EPR.RegulatorService.Facade.Core.Extensions
{
    public static class HttpContentExtension
    {
        public static async Task<T?> ReadFromJsonWithEnumsAsync<T>(this HttpContent content)
        {
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web); 
            options.Converters.Add(new JsonStringEnumConverter());

            return await content.ReadFromJsonAsync<T>(options);
        }
    }
}
