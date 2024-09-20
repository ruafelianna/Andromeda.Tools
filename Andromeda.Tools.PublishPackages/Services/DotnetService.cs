using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Andromeda.Tools.PublishPackages.Services
{
    internal static partial class DotnetService
    {
        public static async Task<string?> GetDevAPIKey()
        {
            var options = new ProcessStartInfo
            {
                FileName = "dotnet",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                ArgumentList = {
                    "dev-certs",
                    "https",
                    "--check",
                },
            };
            var proc = Process.Start(options)
                ?? throw CannotCreateProcess;

            await proc.WaitForExitAsync();

            var text = await proc.StandardOutput.ReadToEndAsync();

            var match = Regex_DevAPIKey().Match(text);

            if (match.Success && match.Groups.Count > 1)
            {
                return match.Groups[1].Value;
            }

            return null;
        }

        public static async Task<(bool Result, string? StdOut, string? StdErr)> PushPackage(
            string token,
            string server,
            string folder,
            string file
        )
        {
            var options = new ProcessStartInfo()
            {
                FileName = "dotnet",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = folder,
                ArgumentList = {
                    "nuget",
                    "push",
                    "-t",
                    "3",
                    "-s",
                    server,
                    "-k",
                    token,
                    file,
                },
            };

            var proc = Process.Start(options)
                ?? throw CannotCreateProcess;

            await proc.WaitForExitAsync();

            var stdout = await proc.StandardOutput.ReadToEndAsync();

            var stderr = await proc.StandardError.ReadToEndAsync();

            if (proc.ExitCode != 0)
            {
                return (false, stdout, stderr);
            }

            return (true, stdout, stderr);
        }

        public static async Task<(bool Result, string? StdOut, string? StdErr)> RemovePackage(
            string token,
            string server,
            string name,
            string version
        )
        {
            var options = new ProcessStartInfo()
            {
                FileName = "dotnet",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                ArgumentList = {
                    "nuget",
                    "delete",
                    "-s",
                    server,
                    "-k",
                    token,
                    "--non-interactive",
                    name,
                    version,
                },
            };

            var proc = Process.Start(options)
                ?? throw CannotCreateProcess;

            var cts = new CancellationTokenSource();

            try
            {
                cts.CancelAfter(3000);

                await proc.WaitForExitAsync(cts.Token);
            }
            catch (TaskCanceledException)
            {
                throw new Exception("No answer");
            }

            var stdout = await proc.StandardOutput.ReadToEndAsync();

            var stderr = await proc.StandardError.ReadToEndAsync();

            if (proc.ExitCode != 0)
            {
                return (false, stdout, stderr);
            }

            return (true, stdout, stderr);
        }

        private static InvalidOperationException CannotCreateProcess
            => new("Couldn't create a new process");

        [GeneratedRegex("A valid certificate was found: ([0-9A-Fa-f]{40})")]
        private static partial Regex Regex_DevAPIKey();
    }
}
