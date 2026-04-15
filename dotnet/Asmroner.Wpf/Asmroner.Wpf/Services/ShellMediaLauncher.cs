using System.Diagnostics;
using Asmroner.Core.Interfaces;

namespace Asmroner.Wpf.Services;

public sealed class ShellMediaLauncher : IMediaLauncher
{
    private readonly Func<ProcessStartInfo, Process?> _processStarter;

    public ShellMediaLauncher()
        : this(startInfo => Process.Start(startInfo))
    {
    }

    public ShellMediaLauncher(Func<ProcessStartInfo, Process?> processStarter)
    {
        _processStarter = processStarter ?? throw new ArgumentNullException(nameof(processStarter));
    }

    public void Open(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        _processStarter(new ProcessStartInfo
        {
            FileName = filePath,
            UseShellExecute = true,
        });
    }
}