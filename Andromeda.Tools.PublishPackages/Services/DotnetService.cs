using System.Diagnostics;
using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

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

        public static async Task<string> PushPackage(
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

            var text = await proc.StandardOutput.ReadToEndAsync();

            if (proc.ExitCode != 0)
            {
                throw CannotPushPackage(text);
            }

            return text;
        }

        private static InvalidOperationException CannotCreateProcess
            => new("Couldn't create a new process");

        private static InvalidOperationException CannotPushPackage(string log)
            => new($"Couldn't push NuGet package. Output: {log}");

        [GeneratedRegex("A valid certificate was found: ([0-9A-Fa-f]{40})")]
        private static partial Regex Regex_DevAPIKey();
    }
}
