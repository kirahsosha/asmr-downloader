using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AsmrDownloader.Client.Models;

namespace AsmrDownloader.Client.Services
{
    public class WebApiService
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;
        private string? _jwt;

        public WebApiService(string baseUrl = "https://api.asmr-300.com")
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _http = new HttpClient { BaseAddress = new Uri(_baseUrl) };
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var content = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("name", username),
                    new KeyValuePair<string, string>("password", password)
                });
                var resp = await _http.PostAsync("/api/auth/me", content);
                if (!resp.IsSuccessStatusCode) return false;
                var doc = await System.Text.Json.JsonDocument.ParseAsync(await resp.Content.ReadAsStreamAsync());
                if (!doc.RootElement.TryGetProperty("token", out var tok)) return false;
                var token = tok.GetString();
                if (string.IsNullOrEmpty(token)) return false;
                _jwt = token;
                _http.DefaultRequestHeaders.Remove("Authorization");
                _http.DefaultRequestHeaders.Add("Authorization", "Bearer " + _jwt);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<FolderInfo>?> GetListAsync(int page = 1, int pageSize = 50)
        {
            try
            {
                // Try /api/list first
                var res = await _http.GetAsync($"/api/list?page={page}&pageSize={pageSize}");
                if (res.IsSuccessStatusCode)
                {
                    var resp = await res.Content.ReadFromJsonAsync<ApiListResponse>();
                    if (resp == null) return null;
                    if (resp.Data == null) return new List<FolderInfo>();
                    var data = (System.Text.Json.JsonElement)resp.Data;
                    if (!data.TryGetProperty("infos", out var infos)) return new List<FolderInfo>();
                    var list = System.Text.Json.JsonSerializer.Deserialize<List<FolderInfo>>(infos.GetRawText());
                    return list ?? new List<FolderInfo>();
                }
                // fallback: if 404, try /api/works (remote API uses works)
                if (res.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    var r2 = await _http.GetAsync($"/api/works?page={page}&pageSize={pageSize}");
                    if (!r2.IsSuccessStatusCode) return null;
                    using var doc = await System.Text.Json.JsonDocument.ParseAsync(await r2.Content.ReadAsStreamAsync());
                    if (!doc.RootElement.TryGetProperty("works", out var works)) return new List<FolderInfo>();
                    var list = new List<FolderInfo>();
                    foreach (var w in works.EnumerateArray())
                    {
                        var fi = new FolderInfo();
                        if (w.TryGetProperty("id", out var idp)) fi.MediaId = idp.ToString();
                        if (w.TryGetProperty("title", out var tp)) fi.Title = tp.GetString();
                        if (w.TryGetProperty("name", out var np)) fi.Name = np.GetString();
                        if (w.TryGetProperty("source_url", out var sup)) fi.BaseDir = sup.GetString();
                        list.Add(fi);
                    }
                    return list;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<FolderInfo>?> SearchAsync(string q, int page = 1, int pageSize = 50)
        {
            if (string.IsNullOrWhiteSpace(q)) return new List<FolderInfo>();
            try
            {
                var res = await _http.GetAsync($"/api/search?q={Uri.EscapeDataString(q)}&page={page}&pageSize={pageSize}");
                if (res.IsSuccessStatusCode)
                {
                    var resp = await res.Content.ReadFromJsonAsync<ApiSearchResponse>();
                    if (resp == null || resp.Data == null) return new List<FolderInfo>();
                    var data = (System.Text.Json.JsonElement)resp.Data;
                    if (!data.TryGetProperty("items", out var items)) return new List<FolderInfo>();
                    var list = System.Text.Json.JsonSerializer.Deserialize<List<FolderInfo>>(items.GetRawText());
                    return list ?? new List<FolderInfo>();
                }

                if (res.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // fallback: try /api/works and filter locally
                    var r2 = await _http.GetAsync($"/api/works?page={page}&pageSize={pageSize}");
                    if (!r2.IsSuccessStatusCode) return null;
                    using var doc = await System.Text.Json.JsonDocument.ParseAsync(await r2.Content.ReadAsStreamAsync());
                    if (!doc.RootElement.TryGetProperty("works", out var works)) return new List<FolderInfo>();
                    var list = new List<FolderInfo>();
                    foreach (var w in works.EnumerateArray())
                    {
                        var title = w.TryGetProperty("title", out var tp) ? (tp.GetString() ?? "") : "";
                        var name = w.TryGetProperty("name", out var np) ? (np.GetString() ?? "") : "";
                        var tags = w.TryGetProperty("tags", out var tg) ? tg.GetRawText() : "";
                        if (title.Contains(q, StringComparison.OrdinalIgnoreCase) || name.Contains(q, StringComparison.OrdinalIgnoreCase) || tags.Contains(q, StringComparison.OrdinalIgnoreCase))
                        {
                            var fi = new FolderInfo();
                            if (w.TryGetProperty("id", out var idp)) fi.MediaId = idp.ToString();
                            if (w.TryGetProperty("title", out var tp2)) fi.Title = tp2.GetString();
                            if (w.TryGetProperty("name", out var np2)) fi.Name = np2.GetString();
                            if (w.TryGetProperty("source_url", out var sup)) fi.BaseDir = sup.GetString();
                            list.Add(fi);
                        }
                    }
                    return list;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private class ApiListResponse
        {
            public int Code { get; set; }
            public string? Msg { get; set; }
            public object? Data { get; set; }
        }

        private class ApiSearchResponse
        {
            public int Code { get; set; }
            public string? Msg { get; set; }
            public object? Data { get; set; }
        }
    }
}
