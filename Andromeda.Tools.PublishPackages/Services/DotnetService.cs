using Andromeda.Tools.PublishPackages.Abstractions;
using Andromeda.Tools.PublishPackages.Assets;
using DynamicData;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Andromeda.Tools.PublishPackages.Services
{
    internal partial class DotNetService : IDotNetService
    {
        public DotNetService(ILogger logger)
        {
            _logger = logger;

            TimeOut = new();
        }

        public Interaction<Unit, TimeSpan> TimeOut { get; }

        public async Task<string> GetDevAPIKey()
        {
            var info = CreateInfo(args: _devTokenArgs);

            var (stdout, _) = await RunProcess(_logger, info);

            if (stdout is not null)
            {
                var match = Regex_DevAPIKey().Match(stdout);

                if (
                    match.Success
                    && match.Groups.Count > 1
                )
                {
                    return match.Groups[1].Value;
                }
            }

            throw CannotFindAPIKey;
        }

        public async Task PushPackage(
            string token,
            string server,
            string folder,
            string file
        )
        {
            var info = CreateInfo(
                folder,
                NuGetPushArgs(
                    server, token, file, await TimeOut.Handle(default)
                )
            );

            await RunProcess(_logger, info);
        }

        public async Task RemovePackage(
            string token,
            string server,
            string name,
            string version
        )
        {
            var info = CreateInfo(
                args: NuGetRemoveArgs(server, token, name, version)
            );

            var cts = new CancellationTokenSource();

            try
            {
                cts.CancelAfter(await TimeOut.Handle(default));

                await RunProcess(_logger, info);
            }
            catch (TaskCanceledException)
            {
                throw TimeoutException;
            }
        }

        #region Consts

        private const string _delimiter = "--> ###";

        private const string _stdoutBig = "STDOUT";

        private const string _stderrBig = "STDERR";

        private const string _start = "_START";

        private const string _stop = "_STOP";

        private const string _noStdOut = $"{_delimiter} NO {_stdoutBig}";

        private const string _noStdErr = $"{_delimiter} NO {_stderrBig}";

        private const string _stdout = $"{_delimiter} {_stdoutBig}{_start}:\n{{stdout}}\n{_delimiter} {_stdoutBig}{_stop}";

        private const string _stderr = $"{_delimiter} {_stderrBig}{_start}:\n{{stderr}}\n{_delimiter} {_stderrBig}{_stop}";

        private const string _dotnet = "dotnet";

        #endregion

        #region Args

        private static readonly string[] _devTokenArgs = [
            "dev-certs",
            "https",
            "--check",
        ];

        private static string[] NuGetPushArgs(
            string server,
            string token,
            string file,
            TimeSpan timeout
        ) => [
            "nuget",
            "push",
            "-t",
            $"{Math.Ceiling(timeout.TotalSeconds):f0}",
            "-s",
            server,
            "-k",
            token,
            file,
        ];

        private static string[] NuGetRemoveArgs(
            string server,
            string token,
            string name,
            string version
        ) => [
            "nuget",
            "delete",
            "-s",
            server,
            "-k",
            token,
            "--non-interactive",
            name,
            version,
        ];

        #endregion

        #region Exceptions

        private static InvalidOperationException CannotCreateProcess
            => new(Strings.Error_CannotCreateProcess);

        private static ApplicationException CannotFindAPIKey
            => new(Strings.Error_NoNuGetAPIKey);

        private static TimeoutException TimeoutException
            => new(Strings.Error_TimeoutException);

        #endregion

        #region Regex

        [GeneratedRegex("A valid certificate was found: ([0-9A-Fa-f]{40})")]
        private static partial Regex Regex_DevAPIKey();

        #endregion

        private readonly ILogger _logger;

        private static ProcessStartInfo CreateInfo(
            string? workingDirectory = null,
            string[]? args = null
        )
        {
            var info = new ProcessStartInfo
            {
                FileName = _dotnet,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = workingDirectory,
            };

            if (args is not null)
            {
                info.ArgumentList.AddRange(args);
            }

            return info;
        }

        private static async Task<(string? StdOut, string? StdErr)> RunProcess(
            ILogger logger,
            ProcessStartInfo info,
            [CallerMemberName] string? name = default
        )
        {
            var proc = Process.Start(info)
                ?? throw CannotCreateProcess;

            await proc.WaitForExitAsync();

            var stdout = await proc.StandardOutput.ReadToEndAsync();
            stdout = stdout.TrimEnd();

            var stderr = await proc.StandardError.ReadToEndAsync();
            stderr = stderr.TrimEnd();

            using (var _ = logger.BeginScope(name!))
            {
                if (string.IsNullOrWhiteSpace(stdout))
                {
                    logger.LogTrace(_noStdOut);
                }
                else
                {
                    logger.LogTrace(_stdout, stdout);
                }

                if (string.IsNullOrWhiteSpace(stderr))
                {
                    logger.LogDebug(_noStdErr);
                }
                else
                {
                    logger.LogDebug(_stderr, stderr);
                }
            }

            if (proc.ExitCode != 0)
            {
                throw new ApplicationException(
                    Strings.Error_ProcessReturnedCode
                );
            }

            return (stdout, stderr);
        }
    }
}
