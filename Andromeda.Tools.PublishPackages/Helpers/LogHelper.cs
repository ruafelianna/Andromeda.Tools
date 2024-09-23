using Microsoft.Extensions.Logging;
using System;

namespace Andromeda.Tools.PublishPackages.Helpers
{
    internal static class LogHelper
    {
        public static void LogInformationFmt(
            this ILogger logger,
            string message,
            params object?[] args
        ) => logger.LogInformation(
            _infoTpl,
            string.Format(message, args)
        );

        public static void LogErrorNonFmt(
            this ILogger logger,
            Exception ex,
            string message
        ) => logger.LogError(ex, _errorTpl, message);

        private const string _infoTpl = "{message}";

        private const string _errorTpl = "{error}";
    }
}
