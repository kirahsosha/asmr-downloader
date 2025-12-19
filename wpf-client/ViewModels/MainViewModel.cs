using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AsmrDownloader.Client.Helpers;
using AsmrDownloader.Client.Models;
using AsmrDownloader.Client.Services;

namespace AsmrDownloader.Client.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly GoCliService _cliService = new GoCliService();
        private WebApiService _apiService = new WebApiService();
        private readonly DownloadManager _downloadManager;

        public ObservableCollection<FolderInfo> Items { get; } = new ObservableCollection<FolderInfo>();
        public ObservableCollection<DownloadQueueItem> Queue { get; } = new ObservableCollection<DownloadQueueItem>();

        private FolderInfo? _selectedItem;
        public FolderInfo? SelectedItem { get => _selectedItem; set { _selectedItem = value; OnPropertyChanged(); } }

        private string _log = "";
        public string Log { get => _log; set { _log = value; OnPropertyChanged(); } }

        private bool _isBusy = false;
        public bool IsBusy { get => _isBusy; set { _isBusy = value; OnPropertyChanged(); } }

        private double _progress = 0;
        public double Progress { get => _progress; set { _progress = value; OnPropertyChanged(); } }

        private string _status = "";
        public string Status { get => _status; set { _status = value; OnPropertyChanged(); } }

        public ICommand SearchCommand { get; }
        public ICommand DownloadCommand { get; }
        public ICommand SyncCommand { get; }
        public ICommand StartWebUICommand { get; }

        public MainViewModel()
        {
            _downloadManager = new DownloadManager(_cliService);
            SearchCommand = new RelayCommand<string>(async (s) => await SearchAsync(s));
            DownloadCommand = new RelayCommand(async () => await DownloadSelectedAsync());
            SyncCommand = new RelayCommand(async () => await SyncAsync());
            StartWebUICommand = new RelayCommand(async () => await StartWebUIAsync());

            EnqueueDownloadCommand = new RelayCommand(async () => await EnqueueSelectedDownload());
            CancelDownloadCommand = new RelayCommand<string>(async (id) => await CancelDownload(id));
            RetryDownloadCommand = new RelayCommand<string>(async (id) => await RetryDownload(id));

            // initialize: auto-login (from config) then refresh list
            _ = InitAsync();
        }

        private async Task InitAsync()
        {
            try
            {
                string? configPath = null;
                // prefer executable base dir, then working dir
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var try1 = System.IO.Path.Combine(baseDir, "webui.config.json");
                var try2 = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "webui.config.json");
                if (System.IO.File.Exists(try1)) configPath = try1;
                else if (System.IO.File.Exists(try2)) configPath = try2;

                string username = "guest";
                string password = "guest";
                string baseUrl = "https://api.asmr-300.com";

                if (configPath != null)
                {
                    try
                    {
                        var txt = await System.IO.File.ReadAllTextAsync(configPath);
                        using var doc = System.Text.Json.JsonDocument.Parse(txt);
                        var root = doc.RootElement;
                        if (root.TryGetProperty("username", out var u)) username = u.GetString() ?? username;
                        if (root.TryGetProperty("password", out var p)) password = p.GetString() ?? password;
                        if (root.TryGetProperty("baseUrl", out var b)) baseUrl = b.GetString() ?? baseUrl;
                    }
                    catch { }
                }

                // recreate API service with configured baseUrl
                try { _apiService = new WebApiService(baseUrl); } catch { _apiService = new WebApiService(); }

                AppendLog($"Attempting auto-login as {username} to {baseUrl}...");
                var ok = await _apiService.LoginAsync(username, password);
                if (!ok && (baseUrl.Contains("localhost") || baseUrl.Contains("127.0.0.1")))
                {
                    // local backend has no auth endpoint; try remote API as fallback
                    var remote = "https://api.asmr-300.com";
                    AppendLog($"Local login failed — trying remote API {remote}...");
                    _apiService = new WebApiService(remote);
                    ok = await _apiService.LoginAsync(username, password);
                }
                AppendLog(ok ? "Auto-login successful" : "Auto-login failed (will continue unauthenticated)");
            }
            catch (Exception ex)
            {
                AppendLog("Auto-login error: " + ex.Message);
            }

            await RefreshListAsync();
        }

        public ICommand EnqueueDownloadCommand { get; }
        public ICommand CancelDownloadCommand { get; }
        public ICommand RetryDownloadCommand { get; }

        private async Task EnqueueSelectedDownload()
        {
            if (SelectedItem == null) { AppendLog("No item selected"); return; }
            var q = new DownloadQueueItem { MediaId = SelectedItem.MediaId ?? "", Title = SelectedItem.Title };
            Queue.Add(q);
            _ = StartQueueItemAsync(q);
        }

        private async Task StartQueueItemAsync(DownloadQueueItem q)
        {
            await _downloadManager.StartDownloadAsync(q, (item, line) =>
            {
                AppendLog($"[{item.MediaId}] {line}");
                // parse percent
                try
                {
                    var idx = line.IndexOf('%');
                    if (idx > 0)
                    {
                        int start = idx - 1;
                        while (start >= 0 && (char.IsDigit(line[start]) || line[start] == ' ')) start--;
                        var numStr = line.Substring(start + 1, idx - start - 1).Trim();
                        if (int.TryParse(numStr, out var val)) { item.Progress = val; }
                    }
                }
                catch { }
            });
        }

        private async Task CancelDownload(string mediaId)
        {
            if (string.IsNullOrWhiteSpace(mediaId)) return;
            var ok = _downloadManager.Cancel(mediaId);
            AppendLog($"Cancel {mediaId}: {ok}");
            var it = Queue.FirstOrDefault(x => x.MediaId == mediaId);
            if (it != null) it.Status = ok ? "Canceled" : "NotRunning";
        }

        private async Task RetryDownload(string mediaId)
        {
            var it = Queue.FirstOrDefault(x => x.MediaId == mediaId);
            if (it == null) return;
            it.Status = "Retrying"; it.Progress = 0;
            _ = StartQueueItemAsync(it);
        }

        public async Task RefreshListAsync()
        {
            AppendLog("Refreshing list...");
            var resp = await _apiService.GetListAsync();
            Items.Clear();
            if (resp != null)
            {
                foreach (var it in resp)
                {
                    Items.Add(it);
                }
                AppendLog($"Loaded {resp.Count} items");
            }
            else
            {
                AppendLog("WebUI not reachable or returned no data");
            }
        }

        private async Task SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            AppendLog($"Running search: {query} (API)...");
            try
            {
                var apiResult = await _apiService.SearchAsync(query, 1, 100);
                if (apiResult != null)
                {
                    Items.Clear();
                    foreach (var it in apiResult)
                        Items.Add(it);
                    AppendLog($"Search returned {apiResult.Count} items via API");
                    return;
                }
            }
            catch (Exception ex)
            {
                AppendLog($"API search failed: {ex.Message}");
            }

            AppendLog("API search returned no results or failed; skipping CLI fallback.");
            await RefreshListAsync();
        }

        private async Task DownloadSelectedAsync()
        {
            if (SelectedItem == null) { AppendLog("No item selected"); return; }
            AppendLog($"Downloading {SelectedItem.Title} ({SelectedItem.MediaId})...");
            IsBusy = true; Progress = 0; Status = "Downloading...";
            await Task.Run(async () =>
            {
                var (exit, output) = await _cliService.RunCommandStreamAsync($"download {SelectedItem.MediaId}", (line) =>
                {
                    AppendLog(line);
                    ParseProgressFromLine(line);
                });
                AppendLog($"Exit code: {exit}");
            });
            Status = "Download finished";
            IsBusy = false;
            await RefreshListAsync();
        }

        private async Task SyncAsync()
        {
            AppendLog("Running sync metadata...");
            IsBusy = true; Progress = 0; Status = "Syncing...";
            await Task.Run(async () =>
            {
                await _cliService.RunCommandStreamAsync("sync", (line) =>
                {
                    AppendLog(line);
                    ParseProgressFromLine(line);
                });
            });
            Status = "Sync finished";
            IsBusy = false;
            await RefreshListAsync();
        }

        private async Task StartWebUIAsync()
        {
            AppendLog("Starting web UI (listen)...");
            var (ok, output) = await _cliService.StartProcessDetachedAsync("listen");
            AppendLog(output);
            await Task.Delay(500);
            await RefreshListAsync();
        }

        private void ParseProgressFromLine(string line)
        {
            // 简单解析百分比
            try
            {
                if (string.IsNullOrWhiteSpace(line)) return;
                var idx = line.IndexOf('%');
                if (idx > 0)
                {
                    // 向前找数字
                    int start = idx - 1;
                    while (start >= 0 && (char.IsDigit(line[start]) || line[start] == ' ')) start--;
                    var numStr = line.Substring(start + 1, idx - start - 1).Trim();
                    if (int.TryParse(numStr, out var val))
                    {
                        Progress = val;
                    }
                }
            }
            catch { }
        }

        private void AppendLog(string s)
        {
            Log += s + "\n";
            try
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var logPath = System.IO.Path.Combine(baseDir, "webui.log");
                System.IO.File.AppendAllText(logPath, System.DateTime.UtcNow.ToString("o") + " " + s + "\n");
            }
            catch { }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
