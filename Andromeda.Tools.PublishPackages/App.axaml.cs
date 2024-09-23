using Andromeda.Tools.PublishPackages.Assets;
using Andromeda.Tools.PublishPackages.Interactions;
using Andromeda.Tools.PublishPackages.Properties;
using Andromeda.Tools.PublishPackages.Services;
using Andromeda.Tools.PublishPackages.Services.BaGet;
using Andromeda.Tools.PublishPackages.ViewModels;
using Andromeda.Tools.PublishPackages.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Globalization;
using System.Reactive.Linq;

namespace Andromeda.Tools.PublishPackages
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            AppInteractions.Settings
                .RegisterHandler(ctx => ctx.SetOutput(
                    Settings.Instance
                ));

            var settings = AppInteractions.Settings.Handle(default).Wait();

            Strings.Culture = settings.Culture is null
                ? CultureInfo.InvariantCulture
                : new CultureInfo(settings.Culture);

            var loggerFactory = LoggerFactory
                .Create(builder => builder
                    .AddFilter(logLevel => true)
                    .AddSimpleConsole(options => {
                        options.TimestampFormat = "dd.MM.yyyy HH:mm:ss ";
                        options.IncludeScopes = true;
                        options.SingleLine = false;
                        options.ColorBehavior = LoggerColorBehavior.Enabled;
                        options.UseUtcTimestamp = false;
                    }));

            var dotnetService = new DotNetService(
                loggerFactory.CreateLogger<DotNetService>()
            );

            dotnetService.TimeOut.RegisterHandler(async ctx => ctx.SetOutput(
                (await AppInteractions.Settings.Handle(default)).DotnetTimeout
            ));

            AppInteractions.NuGetAPIKey
                .RegisterHandler(async ctx => ctx.SetOutput(
                    (await AppInteractions.Settings.Handle(default)).ApiKey
                        ?? await dotnetService.GetDevAPIKey()
                        ?? throw new InvalidOperationException(
                            Strings.Error_NoNuGetAPIKey
                        )
                ));

            if (Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime cdl)
            {
                cdl.MainWindow = new MainWindow()
                {
                    DataContext = new MainViewModel(
                        loggerFactory,
                        getServer => new BaGetClientFactory(
                            dotnetService,
                            getServer
                        )
                    ),
                };

                AppInteractions.StorageProvider
                    .RegisterHandler(ctx => ctx.SetOutput(
                        cdl.MainWindow.StorageProvider
                    ));
            }
        }
    }
}
