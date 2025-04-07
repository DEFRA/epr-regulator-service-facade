using EPR.RegulatorService.Facade.Core.Constants;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.Core.Clients;

public abstract class BaseHttpClient
{
    protected readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    protected BaseHttpClient(HttpClient httpClient, JsonSerializerOptions jsonOptions = null)
    {
        _httpClient = httpClient;
        _jsonOptions = jsonOptions ?? new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
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
