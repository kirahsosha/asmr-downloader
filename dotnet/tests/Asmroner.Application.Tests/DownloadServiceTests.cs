using Asmroner.Application.Services;
using Asmroner.Core.Download;

namespace Asmroner.Application.Tests;

public class DownloadServiceTests
{
    [Fact]
    public async Task DirectoryNameStrategy_ShouldMatchGoCompatibilityRule()
    {
        var tempRoot = CreateTempRoot("path-compat");
        var sourceId = "RJ6301";
        var workInfo = new Asmroner.Core.Api.WorkInfoDto
        {
            Id = 6301,
            SourceId = sourceId,
            Title = "Title:Illegal/Name*Test",
            Release = "2026-03-15",
            HasSubtitle = true,
        };

        var searchStateStore = new SearchStateStore();
        searchStateStore.EnqueueForDownload(new[] { sourceId });

        var apiClient = new ScriptedApiClient(workInfos: new[] { workInfo });
        var sut = new DownloadService(
            apiClient,
            new TestConfigurationService(tempRoot),
            searchStateStore,
            new TestAppPathService(tempRoot),
            new NoopRateLimiterService());

        var tasks = await sut.RunQueuedAsync();
        var task = Assert.Single(tasks);

        Assert.Equal(DownloadTaskStatus.Completed, task.Status);
        var folderName = new DirectoryInfo(task.TargetDirectory).Name;
        Assert.Equal("[RJ6301]Title_Illegal_Name_Test", folderName);
        Assert.DoesNotContain(folderName, ch => Path.GetInvalidFileNameChars().Contains(ch));
    }

    [Fact]
    public async Task RunQueuedAsync_ShouldCreateCompletedTaskAndFiles()
    {
        var tempRoot = CreateTempRoot("run-queued-completed");

        var apiClient = new ScriptedApiClient(trackCount: 2);
        var configService = new TestConfigurationService(tempRoot, preferFormats: "mp3,m4a", preferMedia: "mp3,m4a");
        var searchStateStore = new SearchStateStore();
        searchStateStore.EnqueueForDownload(new[] { "RJ4001" });

        var sut = new DownloadService(
            apiClient,
            configService,
            searchStateStore,
            new TestAppPathService(tempRoot),
            new NoopRateLimiterService());

        var tasks = await sut.RunQueuedAsync();

        Assert.Single(tasks);
        var task = tasks[0];
        Assert.Equal(DownloadTaskStatus.Completed, task.Status);
        Assert.Equal("RJ4001", task.SourceId);
        Assert.Equal(2, task.TotalFiles);
        Assert.Equal(2, task.CompletedFiles);
        Assert.True(Directory.Exists(task.TargetDirectory));
        Assert.True(Directory.EnumerateFiles(task.TargetDirectory).Any());
        Assert.Empty(searchStateStore.GetQueuedSourceIds());
    }

    [Fact]
    public async Task RunQueuedAsync_ShouldMarkTaskFailed_WhenApiThrows()
    {
        var tempRoot = CreateTempRoot("run-queued-failed");

        var apiClient = new ScriptedApiClient(throwOnWorkInfo: true);
        var configService = new TestConfigurationService(tempRoot);
        var searchStateStore = new SearchStateStore();
        searchStateStore.EnqueueForDownload(new[] { "RJ4002" });

        var sut = new DownloadService(
            apiClient,
            configService,
            searchStateStore,
            new TestAppPathService(tempRoot),
            new NoopRateLimiterService());

        var tasks = await sut.RunQueuedAsync();

        Assert.Single(tasks);
        var task = tasks[0];
        Assert.Equal(DownloadTaskStatus.Failed, task.Status);
        Assert.False(string.IsNullOrWhiteSpace(task.ErrorMessage));
        Assert.Empty(searchStateStore.GetQueuedSourceIds());
    }

    [Fact]
    public async Task CancelAsync_ShouldCancelRunningTask()
    {
        var tempRoot = CreateTempRoot("cancel-running");

        var apiClient = new ScriptedApiClient(trackCount: 20);
        var configService = new TestConfigurationService(tempRoot);
        var searchStateStore = new SearchStateStore();
        searchStateStore.EnqueueForDownload(new[] { "RJ4003" });

        var sut = new DownloadService(
            apiClient,
            configService,
            searchStateStore,
            new TestAppPathService(tempRoot),
            new DelayRateLimiterService(delayMilliseconds: 40));

        var runTask = sut.RunQueuedAsync();
        DownloadTaskItem? target = null;

        for (var index = 0; index < 50; index++)
        {
            target = sut.GetTasks().FirstOrDefault();
            if (target?.Status == DownloadTaskStatus.Running)
            {
                break;
            }

            await Task.Delay(20);
        }

        Assert.NotNull(target);
        Assert.True(await sut.CancelAsync(target!.TaskId));

        await runTask;

        var updated = sut.GetTasks().Single(static task => task.SourceId == "RJ4003");
        Assert.Equal(DownloadTaskStatus.Canceled, updated.Status);
    }

    [Fact]
    public async Task CancelAsync_ShouldCancelQueuedTask_BeforeWorkerStarts()
    {
        var tempRoot = CreateTempRoot("cancel-queued");

        var apiClient = new ScriptedApiClient(trackCount: 15);
        var configService = new TestConfigurationService(tempRoot, maxWorkers: 1);
        var searchStateStore = new SearchStateStore();
        searchStateStore.EnqueueForDownload(new[] { "RJ4010", "RJ4011" });

        var sut = new DownloadService(
            apiClient,
            configService,
            searchStateStore,
            new TestAppPathService(tempRoot),
            new DelayRateLimiterService(delayMilliseconds: 20));

        var runTask = sut.RunQueuedAsync();
        DownloadTaskItem? queuedTask = null;

        for (var index = 0; index < 60; index++)
        {
            var tasks = sut.GetTasks();
            queuedTask = tasks.FirstOrDefault(static item => item.Status == DownloadTaskStatus.Queued);
            var hasRunning = tasks.Any(static item => item.Status == DownloadTaskStatus.Running);
            if (queuedTask is not null && hasRunning)
            {
                break;
            }

            await Task.Delay(20);
        }

        Assert.NotNull(queuedTask);
        Assert.True(await sut.CancelAsync(queuedTask!.TaskId));

        await runTask;

        var updatedQueuedTask = sut.GetTasks().Single(task => task.TaskId == queuedTask.TaskId);
        Assert.Equal(DownloadTaskStatus.Canceled, updatedQueuedTask.Status);
    }

    [Fact]
    public async Task RetryFailedAsync_ShouldRetryAndCompleteTask()
    {
        var tempRoot = CreateTempRoot("retry-complete");

        var apiClient = new ScriptedApiClient(failWorkInfoAttempts: 1);
        var configService = new TestConfigurationService(tempRoot);
        var searchStateStore = new SearchStateStore();
        searchStateStore.EnqueueForDownload(new[] { "RJ4004" });

        var sut = new DownloadService(
            apiClient,
            configService,
            searchStateStore,
            new TestAppPathService(tempRoot),
            new NoopRateLimiterService());

        var tasks = await sut.RunQueuedAsync();
        var failedTask = Assert.Single(tasks);
        Assert.Equal(DownloadTaskStatus.Failed, failedTask.Status);

        var retried = await sut.RetryFailedAsync(failedTask.TaskId);

        Assert.NotNull(retried);
        Assert.Equal(1, retried!.RetryCount);
        Assert.Equal(DownloadTaskStatus.Completed, retried.Status);
        Assert.True(Directory.Exists(retried.TargetDirectory));
    }

    [Fact]
    public async Task RunQueuedAsync_ShouldRespectConfiguredMaxWorkers()
    {
        var tempRoot = CreateTempRoot("respect-workers");

        var apiClient = new ScriptedApiClient(trackCount: 1, workInfoDelayMilliseconds: 80);
        var configService = new TestConfigurationService(tempRoot, maxWorkers: 2);
        var searchStateStore = new SearchStateStore();
        searchStateStore.EnqueueForDownload(new[] { "RJ4101", "RJ4102", "RJ4103", "RJ4104" });

        var sut = new DownloadService(
            apiClient,
            configService,
            searchStateStore,
            new TestAppPathService(tempRoot),
            new NoopRateLimiterService());

        var tasks = await sut.RunQueuedAsync();

        Assert.Equal(4, tasks.Count);
        Assert.All(tasks, static task => Assert.Equal(DownloadTaskStatus.Completed, task.Status));
        Assert.InRange(apiClient.MaxObservedWorkInfoConcurrency, 1, 2);
        Assert.Equal(2, apiClient.MaxObservedWorkInfoConcurrency);
    }

    [Fact]
    public async Task StartAsync_ShouldTrackFailedTask_WhenNewTaskFails()
    {
        var tempRoot = CreateTempRoot("start-track-failed");

        var apiClient = new ScriptedApiClient(throwOnWorkInfo: true);
        var configService = new TestConfigurationService(tempRoot);
        var searchStateStore = new SearchStateStore();

        var sut = new DownloadService(
            apiClient,
            configService,
            searchStateStore,
            new TestAppPathService(tempRoot),
            new NoopRateLimiterService());

        var result = await sut.StartAsync("RJ4301");

        Assert.NotNull(result);
        Assert.Equal(DownloadTaskStatus.Failed, result!.Status);
        Assert.False(string.IsNullOrWhiteSpace(result.ErrorMessage));

        var tracked = sut.GetTasks();
        Assert.Single(tracked);
        Assert.Equal("RJ4301", tracked[0].SourceId);
        Assert.Equal(DownloadTaskStatus.Failed, tracked[0].Status);
    }

    [Fact]
    public async Task StartAsync_ShouldReuseFailedTask_WhenPreferredTaskProvided()
    {
        var tempRoot = CreateTempRoot("restart-failed");

        var failingApi = new ScriptedApiClient(failWorkInfoAttempts: 1);
        var configService = new TestConfigurationService(tempRoot);
        var searchStateStore = new SearchStateStore();
        searchStateStore.EnqueueForDownload(new[] { "RJ4201" });

        var sut = new DownloadService(
            failingApi,
            configService,
            searchStateStore,
            new TestAppPathService(tempRoot),
            new NoopRateLimiterService());

        var first = Assert.Single(await sut.RunQueuedAsync());
        Assert.Equal(DownloadTaskStatus.Failed, first.Status);

        var restarted = await sut.StartAsync("RJ4201", preferredTaskId: first.TaskId);

        Assert.NotNull(restarted);
        Assert.Equal(first.TaskId, restarted!.TaskId);
        Assert.Equal(DownloadTaskStatus.Completed, restarted.Status);
        Assert.Single(sut.GetTasks());
    }

    [Fact]
    public async Task StartAsync_ShouldReuseCanceledTask_WhenPreferredTaskProvided()
    {
        var tempRoot = CreateTempRoot("restart-canceled");

        var apiClient = new ScriptedApiClient(trackCount: 20);
        var configService = new TestConfigurationService(tempRoot);
        var searchStateStore = new SearchStateStore();
        searchStateStore.EnqueueForDownload(new[] { "RJ4202" });

        var sut = new DownloadService(
            apiClient,
            configService,
            searchStateStore,
            new TestAppPathService(tempRoot),
            new DelayRateLimiterService(delayMilliseconds: 20));

        var runTask = sut.RunQueuedAsync();
        DownloadTaskItem? target = null;

        for (var index = 0; index < 50; index++)
        {
            target = sut.GetTasks().FirstOrDefault();
            if (target?.Status == DownloadTaskStatus.Running)
            {
                break;
            }

            await Task.Delay(10);
        }

        Assert.NotNull(target);
        Assert.True(await sut.CancelAsync(target!.TaskId));
        await runTask;

        var canceled = sut.GetTasks().Single(static task => task.SourceId == "RJ4202");
        Assert.Equal(DownloadTaskStatus.Canceled, canceled.Status);

        var restarted = await sut.StartAsync("RJ4202", preferredTaskId: canceled.TaskId);

        Assert.NotNull(restarted);
        Assert.Equal(canceled.TaskId, restarted!.TaskId);
        Assert.Equal(DownloadTaskStatus.Completed, restarted.Status);
        Assert.Single(sut.GetTasks());
    }

    [Fact]
    public async Task RunQueuedAsync_ShouldReuseCanceledTask_WhenSameSourceRequeued()
    {
        var tempRoot = CreateTempRoot("runqueue-reuse-canceled");

        var apiClient = new ScriptedApiClient(trackCount: 20);
        var configService = new TestConfigurationService(tempRoot);
        var searchStateStore = new SearchStateStore();
        searchStateStore.EnqueueForDownload(new[] { "RJ4203" });

        var sut = new DownloadService(
            apiClient,
            configService,
            searchStateStore,
            new TestAppPathService(tempRoot),
            new DelayRateLimiterService(delayMilliseconds: 20));

        var firstRun = sut.RunQueuedAsync();
        DownloadTaskItem? running = null;

        for (var index = 0; index < 50; index++)
        {
            running = sut.GetTasks().FirstOrDefault();
            if (running?.Status == DownloadTaskStatus.Running)
            {
                break;
            }

            await Task.Delay(10);
        }

        Assert.NotNull(running);
        Assert.True(await sut.CancelAsync(running!.TaskId));
        await firstRun;

        var canceled = sut.GetTasks().Single(static task => task.SourceId == "RJ4203");
        Assert.Equal(DownloadTaskStatus.Canceled, canceled.Status);

        searchStateStore.EnqueueForDownload(new[] { "RJ4203" });
        var created = await sut.RunQueuedAsync();
        var reused = Assert.Single(created);

        Assert.Equal(canceled.TaskId, reused.TaskId);
        Assert.Equal(DownloadTaskStatus.Completed, reused.Status);
        Assert.Single(sut.GetTasks());
    }

    [Fact]
    public void UpsertPrefetchedWorkInfo_ShouldExposeSnapshot_ForCrossViewTitleReuse()
    {
        var tempRoot = CreateTempRoot("prefetched-snapshot");
        var sut = CreateSut(tempRoot, new ScriptedApiClient(), new SearchStateStore());

        sut.UpsertPrefetchedWorkInfo(new Dictionary<string, Asmroner.Core.Api.WorkInfoDto>
        {
            ["RJ6401"] = new Asmroner.Core.Api.WorkInfoDto
            {
                Id = 6401,
                SourceId = "RJ6401",
                Title = "From Search",
                Release = "2026-03-16",
                HasSubtitle = true,
            },
        });

        var snapshot = sut.GetPrefetchedWorkInfoSnapshot();
        Assert.True(snapshot.ContainsKey("RJ6401"));
        Assert.Equal("From Search", snapshot["RJ6401"].Title);
    }

    [Fact]
    public async Task RunQueuedAsync_ShouldDownloadAllFormats_WhenPreferFormatsEmpty()
    {
        var tempRoot = CreateTempRoot("all-formats-when-empty");
        var searchStateStore = new SearchStateStore();
        searchStateStore.EnqueueForDownload(new[] { "RJ6001" });

        var apiClient = new ScriptedApiClient(tracks: new[]
        {
            new Asmroner.Core.Api.TrackDto { Title = "audio", MediaDownloadUrl = "https://cdn.example.com/file/audio.mp3" },
            new Asmroner.Core.Api.TrackDto { Title = "image", MediaDownloadUrl = "https://cdn.example.com/file/cover.jpg" },
            new Asmroner.Core.Api.TrackDto { Title = "text", MediaDownloadUrl = "https://cdn.example.com/file/readme.txt" },
        });

        var sut = new DownloadService(
            apiClient,
            new TestConfigurationService(tempRoot, preferFormats: string.Empty, preferMedia: string.Empty),
            searchStateStore,
            new TestAppPathService(tempRoot),
            new NoopRateLimiterService());

        var tasks = await sut.RunQueuedAsync();
        var task = Assert.Single(tasks);

        Assert.Equal(DownloadTaskStatus.Completed, task.Status);
        Assert.Equal(3, task.TotalFiles);
        Assert.Equal(3, task.CompletedFiles);
    }

    [Fact]
    public async Task RunQueuedAsync_ShouldSkipTextSidecars_WhenHdAudioOnlyRemovesMp3()
    {
        var tempRoot = CreateTempRoot("hd-sidecar-skip");
        var searchStateStore = new SearchStateStore();
        searchStateStore.EnqueueForDownload(new[] { "RJ7001" });

        var apiClient = new ScriptedApiClient(tracks: new[]
        {
            new Asmroner.Core.Api.TrackDto { Title = "voice", MediaDownloadUrl = "https://cdn.example.com/a/voice.mp3" },
            new Asmroner.Core.Api.TrackDto { Title = "voice", MediaDownloadUrl = "https://cdn.example.com/a/voice.wav" },
            new Asmroner.Core.Api.TrackDto { Title = "voice", MediaDownloadUrl = "https://cdn.example.com/a/voice.txt" },
            new Asmroner.Core.Api.TrackDto { Title = "voice", MediaDownloadUrl = "https://cdn.example.com/a/voice.lrc" },
            new Asmroner.Core.Api.TrackDto { Title = "voice", MediaDownloadUrl = "https://cdn.example.com/a/voice.ass" },
            new Asmroner.Core.Api.TrackDto { Title = "readme", MediaDownloadUrl = "https://cdn.example.com/a/readme.txt" },
        });

        var sut = new DownloadService(
            apiClient,
            new TestConfigurationService(tempRoot),
            searchStateStore,
            new TestAppPathService(tempRoot),
            new NoopRateLimiterService());

        var tasks = await sut.RunQueuedAsync(hdAudioOnly: true);
        var task = Assert.Single(tasks);

        Assert.Equal(DownloadTaskStatus.Completed, task.Status);
        Assert.Equal(2, task.TotalFiles);

        var downloadedFiles = Directory
            .EnumerateFiles(task.TargetDirectory, "*", SearchOption.AllDirectories)
            .Select(Path.GetFileName)
            .ToArray();

        Assert.Contains("voice.wav", downloadedFiles, StringComparer.OrdinalIgnoreCase);
        Assert.Contains("readme.txt", downloadedFiles, StringComparer.OrdinalIgnoreCase);
        Assert.DoesNotContain("voice.mp3", downloadedFiles, StringComparer.OrdinalIgnoreCase);
        Assert.DoesNotContain("voice.txt", downloadedFiles, StringComparer.OrdinalIgnoreCase);
        Assert.DoesNotContain("voice.lrc", downloadedFiles, StringComparer.OrdinalIgnoreCase);
        Assert.DoesNotContain("voice.ass", downloadedFiles, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RunQueuedAsync_ShouldKeepTextSidecars_WhenHdAudioOnlyIsFalse()
    {
        var tempRoot = CreateTempRoot("hd-sidecar-keep");
        var searchStateStore = new SearchStateStore();
        searchStateStore.EnqueueForDownload(new[] { "RJ7002" });

        var apiClient = new ScriptedApiClient(tracks: new[]
        {
            new Asmroner.Core.Api.TrackDto { Title = "voice", MediaDownloadUrl = "https://cdn.example.com/a/voice.mp3" },
            new Asmroner.Core.Api.TrackDto { Title = "voice", MediaDownloadUrl = "https://cdn.example.com/a/voice.wav" },
            new Asmroner.Core.Api.TrackDto { Title = "voice", MediaDownloadUrl = "https://cdn.example.com/a/voice.txt" },
            new Asmroner.Core.Api.TrackDto { Title = "voice", MediaDownloadUrl = "https://cdn.example.com/a/voice.lrc" },
            new Asmroner.Core.Api.TrackDto { Title = "voice", MediaDownloadUrl = "https://cdn.example.com/a/voice.ass" },
        });

        var sut = new DownloadService(
            apiClient,
            new TestConfigurationService(tempRoot),
            searchStateStore,
            new TestAppPathService(tempRoot),
            new NoopRateLimiterService());

        var tasks = await sut.RunQueuedAsync(hdAudioOnly: false);
        var task = Assert.Single(tasks);

        Assert.Equal(DownloadTaskStatus.Completed, task.Status);
        Assert.Equal(5, task.TotalFiles);

        var downloadedFiles = Directory
            .EnumerateFiles(task.TargetDirectory, "*", SearchOption.AllDirectories)
            .Select(Path.GetFileName)
            .ToArray();

        Assert.Contains("voice.mp3", downloadedFiles, StringComparer.OrdinalIgnoreCase);
        Assert.Contains("voice.wav", downloadedFiles, StringComparer.OrdinalIgnoreCase);
        Assert.Contains("voice.txt", downloadedFiles, StringComparer.OrdinalIgnoreCase);
        Assert.Contains("voice.lrc", downloadedFiles, StringComparer.OrdinalIgnoreCase);
        Assert.Contains("voice.ass", downloadedFiles, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RunQueuedAsync_ShouldUsePrefetchedWorkInfo_WithoutApiWorkInfoCall()
    {
        var tempRoot = CreateTempRoot("prefetched-workinfo");
        var searchStateStore = new SearchStateStore();
        searchStateStore.EnqueueForDownload(new[] { "RJ6002" });

        var apiClient = new ScriptedApiClient(
            throwOnAnyWorkInfoCall: true,
            tracks: new[]
            {
                new Asmroner.Core.Api.TrackDto { Title = "audio", MediaDownloadUrl = "https://cdn.example.com/file/audio.wav" },
            });

        var sut = new DownloadService(
            apiClient,
            new TestConfigurationService(tempRoot),
            searchStateStore,
            new TestAppPathService(tempRoot),
            new NoopRateLimiterService());

        sut.UpsertPrefetchedWorkInfo(new Dictionary<string, Asmroner.Core.Api.WorkInfoDto>
        {
            ["RJ6002"] = new Asmroner.Core.Api.WorkInfoDto
            {
                Id = 6002,
                SourceId = "RJ6002",
                Title = "Prefetched Title",
                Release = "2026-03-16",
                HasSubtitle = false,
            },
        });

        var tasks = await sut.RunQueuedAsync();
        var task = Assert.Single(tasks);

        Assert.Equal(DownloadTaskStatus.Completed, task.Status);
        Assert.Equal("Prefetched Title", task.Title);
        Assert.Equal(0, apiClient.WorkInfoCallCount);
    }

    [Fact]
    public async Task CancelAsync_ShouldReturnFalse_WhenTaskDoesNotExist()
    {
        var tempRoot = CreateTempRoot("cancel-missing");
        var sut = CreateSut(tempRoot, new ScriptedApiClient(), new SearchStateStore());

        var canceled = await sut.CancelAsync(Guid.NewGuid());

        Assert.False(canceled);
    }

    [Fact]
    public async Task RetryFailedAsync_ShouldNotRetry_WhenTaskIsNotFailed()
    {
        var tempRoot = CreateTempRoot("retry-not-failed");
        var searchStateStore = new SearchStateStore();
        searchStateStore.EnqueueForDownload(new[] { "RJ6101" });

        var sut = CreateSut(tempRoot, new ScriptedApiClient(), searchStateStore);
        var tasks = await sut.RunQueuedAsync();
        var completedTask = Assert.Single(tasks);
        Assert.Equal(DownloadTaskStatus.Completed, completedTask.Status);

        var retried = await sut.RetryFailedAsync(completedTask.TaskId);

        Assert.Null(retried);
        Assert.Equal(DownloadTaskStatus.Completed, completedTask.Status);
        Assert.Equal(0, completedTask.RetryCount);
    }

    [Fact]
    public async Task RunQueuedAsync_ShouldContinueOtherTasks_WhenSingleTaskFails()
    {
        var tempRoot = CreateTempRoot("batch-tolerance");
        var searchStateStore = new SearchStateStore();
        searchStateStore.EnqueueForDownload(new[] { "RJ6201", "RJ6202", "RJ6203" });

        var apiClient = new ScriptedApiClient(failOnWorkInfoIds: new[] { "RJ6202" });
        var sut = CreateSut(tempRoot, apiClient, searchStateStore);

        var tasks = await sut.RunQueuedAsync();

        Assert.Equal(3, tasks.Count);
        Assert.Equal(DownloadTaskStatus.Completed, tasks.Single(static item => item.SourceId == "RJ6201").Status);
        Assert.Equal(DownloadTaskStatus.Failed, tasks.Single(static item => item.SourceId == "RJ6202").Status);
        Assert.Equal(DownloadTaskStatus.Completed, tasks.Single(static item => item.SourceId == "RJ6203").Status);
    }

    private static DownloadService CreateSut(string tempRoot, ScriptedApiClient apiClient, SearchStateStore searchStateStore)
    {
        return new DownloadService(
            apiClient,
            new TestConfigurationService(tempRoot),
            searchStateStore,
            new TestAppPathService(tempRoot),
            new NoopRateLimiterService());
    }

    private static string CreateTempRoot(string suffix)
    {
        var root = Path.Combine(Path.GetTempPath(), $"asmroner-batch01-{suffix}-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        return root;
    }
}
