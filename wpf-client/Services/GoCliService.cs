using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AsmrDownloader.Client.Services
{
    /// <summary>
    /// 调用本地 Go 二进制（假定可执行名为 asmroner 或 asmroner.exe）来执行 search/download/sync 等命令
    /// </summary>
    public class GoCliService
    {
        private readonly string _exeName;

        public GoCliService(string exeName = "asmroner")
        {
            _exeName = exeName;
        }

        private string ResolveExe()
        {
            // 在 PATH 中查找或当前工作目录
            // First try project-local C# backend locations then fall back to configured name and PATH
            var probe = _exeName;
            // try backend project output locations
            var repoRoot = System.IO.Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, ".."));
            var candidates = new[] {
                // prefer published self-contained release in repo dist
                System.IO.Path.Combine(repoRoot, "dist", "asmroner-win", probe + ".exe"),
                System.IO.Path.Combine(Environment.CurrentDirectory, "dist", "asmroner-win", probe + ".exe"),
                // fallback to project build outputs
                System.IO.Path.Combine(Environment.CurrentDirectory, "backend", "asmroner", "bin", "Debug", "net7.0", probe + ".exe"),
                System.IO.Path.Combine(Environment.CurrentDirectory, "backend", "asmroner", "bin", "Release", "net7.0", probe + ".exe"),
                System.IO.Path.Combine(repoRoot, "backend", "asmroner", "bin", "Debug", "net7.0", probe + ".exe"),
                System.IO.Path.Combine(repoRoot, "backend", "asmroner", "bin", "Release", "net7.0", probe + ".exe"),
                System.IO.Path.Combine(Environment.CurrentDirectory, probe + ".exe"),
            };
            foreach (var c in candidates)
            {
                try { if (System.IO.File.Exists(c)) return c; } catch { }
            }
#if Windows
            if (!probe.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)) probe += ".exe";
#endif
            if (File.Exists(probe)) return probe;
            var path = Environment.GetEnvironmentVariable("PATH") ?? "";
            foreach (var p in path.Split(Path.PathSeparator))
            {
                var candidate = Path.Combine(p, probe);
                if (File.Exists(candidate)) return candidate;
            }
            return _exeName; // let process resolution attempt
        }

        public async Task<(bool ok, string output)> RunCommandAsync(string args)
        {
            try
            {
                var exe = ResolveExe();
                var psi = new ProcessStartInfo(exe, args)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                var p = Process.Start(psi)!;
                var sb = new StringBuilder();
                // Read output and error fully
                sb.AppendLine(await p.StandardOutput.ReadToEndAsync());
                sb.AppendLine(await p.StandardError.ReadToEndAsync());
                await p.WaitForExitAsync();
                return (p.ExitCode == 0, sb.ToString());
            }
            catch (Exception ex)
            {
                return (false, ex.ToString());
            }
        }

        /// <summary>
        /// 启动命令并实时回调每一行输出（stdout/stderr）。
        /// 返回进程退出后的退出码和收集到的文本。
        /// </summary>
        public async Task<(int exitCode, string output)> RunCommandStreamAsync(string args, Action<string> onOutput)
        {
            try
            {
                var exe = ResolveExe();
                var psi = new ProcessStartInfo(exe, args)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                var p = new Process();
                p.StartInfo = psi;
                var sb = new StringBuilder();
                p.OutputDataReceived += (s, e) => { if (e.Data != null) { sb.AppendLine(e.Data); onOutput?.Invoke(e.Data); } };
                p.ErrorDataReceived += (s, e) => { if (e.Data != null) { sb.AppendLine(e.Data); onOutput?.Invoke(e.Data); } };
                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                await p.WaitForExitAsync();
                var exit = p.ExitCode;
                try { p.Kill(); } catch { }
                p.Dispose();
                return (exit, sb.ToString());
            }
            catch (Exception ex)
            {
                onOutput?.Invoke(ex.ToString());
                return (-1, ex.ToString());
            }
        }

        /// <summary>
        /// 启动进程并返回 Process 实例和一个 Task，该 Task 在进程退出后完成并返回退出码。
        /// 调用方可以通过返回的 Process 来取消（Kill）进程。
        /// </summary>
        public (Process process, Task<int> exitTask) StartProcessWithCallbacks(string args, Action<string>? onOutput)
        {
            var exe = ResolveExe();
            var psi = new ProcessStartInfo(exe, args)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var p = new Process();
            p.StartInfo = psi;
            p.OutputDataReceived += (s, e) => { if (e.Data != null) onOutput?.Invoke(e.Data); };
            p.ErrorDataReceived += (s, e) => { if (e.Data != null) onOutput?.Invoke(e.Data); };
            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();

            var t = Task.Run(async () =>
            {
                await p.WaitForExitAsync();
                var exit = p.ExitCode;
                try { p.Close(); } catch { }
                return exit;
            });

            return (p, t);
        }

        public async Task<(bool ok, string output)> StartProcessDetachedAsync(string args)
        {
            try
            {
                var exe = ResolveExe();
                var psi = new ProcessStartInfo(exe, args)
                {
                    UseShellExecute = true,
                    CreateNoWindow = false,
                };
                Process.Start(psi);
                return (true, "Started process");
            }
            catch (Exception ex)
            {
                return (false, ex.ToString());
            }
        }
    }
}
