using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AsmrDownloader.Client.Models;

namespace AsmrDownloader.Client.Services
{
    public class DownloadManager
    {
        private readonly GoCliService _cli;
        // map mediaId -> process
        private readonly ConcurrentDictionary<string, Process> _procs = new ConcurrentDictionary<string, Process>();

        public DownloadManager(GoCliService cli)
        {
            _cli = cli;
        }

        public async Task StartDownloadAsync(DownloadQueueItem item, Action<DownloadQueueItem, string> onOutput)
        {
            item.Status = "Starting";
            var (proc, exitTask) = _cli.StartProcessWithCallbacks($"download {item.MediaId}", (line) =>
            {
                // forward output
                onOutput?.Invoke(item, line);
            });

            _procs[item.MediaId] = proc;
            item.Status = "Downloading";

            // monitor exit
            var exit = await exitTask;
            _procs.TryRemove(item.MediaId, out _);
            item.Status = exit == 0 ? "Completed" : $"Failed ({exit})";
        }

        public bool Cancel(string mediaId)
        {
            if (_procs.TryRemove(mediaId, out var p))
            {
                try
                {
                    if (!p.HasExited) p.Kill(true);
                }
                catch { }
                return true;
            }
            return false;
        }

        public bool IsRunning(string mediaId) => _procs.ContainsKey(mediaId);
    }
}
