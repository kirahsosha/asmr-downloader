using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Backend.Asmoner.Services
{
    public class DownloadService
    {
        // track running downloads to allow cancellation in future
        private readonly ConcurrentDictionary<string, CancellationTokenSource> _running = new();

        private readonly HttpClient _http = new HttpClient();

        public DownloadService()
        {
        }

        private static string SanitizeFileName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "file";
            var illegal = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var regex = new Regex($"[{Regex.Escape(illegal)}]");
            return regex.Replace(name, "_");
        }

        private void EnsureDir(string dir)
        {
            try { Directory.CreateDirectory(dir); } catch { }
        }

        // Start download by contacting remote API for tracks and downloading all files to downloads/{mediaId}
        public async Task<bool> StartDownloadAsync(string mediaId, Action<string>? onOutput = null)
        {
            if (string.IsNullOrWhiteSpace(mediaId)) return false;
            if (_running.ContainsKey(mediaId))
            {
                onOutput?.Invoke("Already downloading");
                return false;
            }

            var cts = new CancellationTokenSource();
            if (!_running.TryAdd(mediaId, cts)) return false;
            var token = cts.Token;

            _ = Task.Run(async () =>
            {
                try
                {
                    onOutput?.Invoke($"Starting download for {mediaId}");

                    // fetch tracks
                    var tracksUrl = $"https://api.asmr-300.com/api/tracks/{Uri.EscapeDataString(mediaId)}";
                    var resp = await _http.GetAsync(tracksUrl, token);
                    if (!resp.IsSuccessStatusCode)
                    {
                        onOutput?.Invoke($"Failed to fetch tracks: {resp.StatusCode}");
                        return;
                    }
                    var body = await resp.Content.ReadAsStringAsync(token);
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var trackList = JsonSerializer.Deserialize<TrackDto[]>(body, options) ?? Array.Empty<TrackDto>();

                    // flatten tracks (recursive)
                    var downloads = new ConcurrentBag<(string url, string filename)>();
                    void Walk(TrackDto t, string prefix)
                    {
                        var name = string.IsNullOrWhiteSpace(t.Title) ? "track" : t.Title;
                        var fileName = SanitizeFileName((string.IsNullOrEmpty(prefix) ? "" : prefix + "_") + name);
                        if (!string.IsNullOrWhiteSpace(t.MediaDownloadUrl))
                        {
                            downloads.Add((t.MediaDownloadUrl, fileName));
                        }
                        if (t.Children != null)
                        {
                            foreach (var c in t.Children) Walk(c, fileName);
                        }
                    }
                    foreach (var t in trackList) Walk(t, "");

                    if (downloads.IsEmpty)
                    {
                        onOutput?.Invoke("No downloadable tracks found");
                        return;
                    }

                    var baseDir = Path.Combine(Directory.GetCurrentDirectory(), "downloads", mediaId);
                    EnsureDir(baseDir);

                    var sem = new SemaphoreSlim(4); // max concurrent downloads
                    var tasks = new ConcurrentBag<Task>();
                    int idx = 1;
                    foreach (var item in downloads)
                    {
                        await sem.WaitAsync(token);
                        var fileIdx = idx++;
                        var url = item.url;
                        var filename = item.filename;
                        var ext = Path.GetExtension(url);
                        if (string.IsNullOrEmpty(ext)) ext = ".bin";
                        var outPath = Path.Combine(baseDir, $"{fileIdx:000}_{filename}{ext}");
                        var tsk = Task.Run(async () =>
                        {
                            try
                            {
                                onOutput?.Invoke($"Downloading {url} -> {outPath}");
                                using var r = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);
                                r.EnsureSuccessStatusCode();
                                using var s = await r.Content.ReadAsStreamAsync(token);
                                using var fs = new FileStream(outPath, FileMode.Create, FileAccess.Write, FileShare.None);
                                await s.CopyToAsync(fs, token);
                                onOutput?.Invoke($"Saved {outPath}");
                            }
                            catch (OperationCanceledException)
                            {
                                onOutput?.Invoke($"Download cancelled: {url}");
                            }
                            catch (Exception ex)
                            {
                                onOutput?.Invoke($"Download error: {ex.Message}");
                            }
                            finally { sem.Release(); }
                        }, token);
                        tasks.Add(tsk);
                    }

                    await Task.WhenAll(tasks.ToArray());
                    onOutput?.Invoke($"Download complete for {mediaId}");
                }
                catch (OperationCanceledException)
                {
                    onOutput?.Invoke("Download cancelled");
                }
                catch (Exception ex)
                {
                    onOutput?.Invoke("Download failed: " + ex.Message);
                }
                finally
                {
                    _running.TryRemove(mediaId, out _);
                }
            }, token);

            return true;
        }

        public bool Cancel(string mediaId)
        {
            if (_running.TryRemove(mediaId, out var cts))
            {
                try { cts.Cancel(); return true; } catch { return false; }
            }
            return false;
        }

        public bool IsRunning(string mediaId)
        {
            return _running.ContainsKey(mediaId);
        }

        public string[] GetActiveDownloads()
        {
            return _running.Keys.ToArray();
        }

        private class TrackDto
        {
            public string? Type { get; set; }
            public string? Title { get; set; }
            public TrackDto[]? Children { get; set; }
            public string? MediaStreamUrl { get; set; }
            public string? MediaDownloadUrl { get; set; }
        }
    }
}
