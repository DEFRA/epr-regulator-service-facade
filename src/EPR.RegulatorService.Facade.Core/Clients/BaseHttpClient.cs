using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EPR.RegulatorService.Facade.Core.Constants;
using EPR.RegulatorService.Facade.Core.Helpers.Converter;

namespace EPR.RegulatorService.Facade.Core.Clients;

public abstract class BaseHttpClient
{
    protected readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    protected BaseHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        RegisterEnumConverters();
    }

    private void RegisterEnumConverters()
    {
        var enumTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsEnum || (Nullable.GetUnderlyingType(t) != null && Nullable.GetUnderlyingType(t).IsEnum))
            .ToList();

        foreach (var enumType in enumTypes)
        {
            var converterType = typeof(CustomEnumConverter<>).MakeGenericType(enumType);
            var converter = (JsonConverter)Activator.CreateInstance(converterType);
            _jsonOptions.Converters.Add(converter);
        }
    }

    protected async Task<TResponse> GetAsync<TResponse>(string url)
    {
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(json, _jsonOptions);
    }

    protected async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest data)
    {
        var content = CreateJsonContent(data);
        var response = await _httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(json, _jsonOptions);
    }

    protected async Task<TResponse> PutAsync<TRequest, TResponse>(string url, TRequest data)
    {
        var content = CreateJsonContent(data);
        var response = await _httpClient.PutAsync(url, content);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(json, _jsonOptions);
    }

    protected async Task<bool> PatchAsync<TRequest>(string url, TRequest data)
    {
        var content = CreateJsonContent(data);
        var response = await _httpClient.PatchAsync(url, content);
        response.EnsureSuccessStatusCode();

        return response.IsSuccessStatusCode;
    }

    protected async Task<bool> DeleteAsync(string url)
    {
        var response = await _httpClient.DeleteAsync(url);
        return response.IsSuccessStatusCode;
    }

    private StringContent CreateJsonContent<T>(T data)
    {
        var json = JsonSerializer.Serialize(data, _jsonOptions);
        return new StringContent(json, Encoding.UTF8, ContentType.ApplicationJson);
    }
}
