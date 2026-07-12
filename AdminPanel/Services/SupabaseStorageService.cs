using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace AdminPanel.Services;

public class SupabaseStorageOptions
{
    public string Url { get; set; } = "";
    public string ServiceRoleKey { get; set; } = "";
    public string StorageBucket { get; set; } = "project-files";
}

public class SupabaseStorageService
{
    private readonly HttpClient _http;
    private readonly SupabaseStorageOptions _options;

    public SupabaseStorageService(HttpClient http, IOptions<SupabaseStorageOptions> options)
    {
        _options = options.Value;
        _http = http;
        _http.BaseAddress = new Uri($"{_options.Url.TrimEnd('/')}/storage/v1/");
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.ServiceRoleKey);
        _http.DefaultRequestHeaders.Add("apikey", _options.ServiceRoleKey);
    }

    public async Task UploadAsync(string path, Stream content, string contentType, CancellationToken ct = default)
    {
        using var streamContent = new StreamContent(content);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(
            string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType);

        var response = await _http.PostAsync($"object/{_options.StorageBucket}/{path}", streamContent, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<string> CreateSignedUrlAsync(string path, int expiresInSeconds = 300, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync(
            $"object/sign/{_options.StorageBucket}/{path}", new { expiresIn = expiresInSeconds }, ct);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<SignedUrlResponse>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Supabase did not return a signed URL.");

        return $"{_options.Url.TrimEnd('/')}/storage/v1{result.SignedURL}";
    }

    public async Task DeleteAsync(string path, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync($"object/{_options.StorageBucket}/{path}", ct);
        response.EnsureSuccessStatusCode();
    }

    private class SignedUrlResponse
    {
        public string SignedURL { get; set; } = "";
    }
}
