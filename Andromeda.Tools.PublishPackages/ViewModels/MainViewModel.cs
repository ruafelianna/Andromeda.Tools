using Andromeda.Tools.PublishPackages.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Andromeda.Tools.PublishPackages.ViewModels
{
    internal partial class MainViewModel : ViewModelBase
    {
        public MainViewModel(
            ILoggerFactory loggerFactory,
            Func<Func<string?>, INuGetClientFactory> createClientFactory
        )
        {
            ServersViewModel = new(
                loggerFactory.CreateLogger<ServersViewModel>()
            );

            string? getSelectedServer() => ServersViewModel.SelectedServer;

            var clientFactory = createClientFactory(getSelectedServer);

            FoldersViewModel = new(
                loggerFactory.CreateLogger<FoldersViewModel>()
            );

            var canPushPackages = Observable
                .CombineLatest(
                    ServersViewModel.IsServerSelectedObservable,
                    FoldersViewModel.IsAnyPackageChosenObservable
                )
                .Select(list => list.All(x => x));

            NuGetViewModel = new(
                loggerFactory.CreateLogger<NuGetViewModel>(),
                clientFactory,
                ServersViewModel.IsServerSelectedObservable,
                canPushPackages
            );

            NuGetViewModel.FoldersInteraction
                .RegisterHandler(ctx => ctx.SetOutput(
                    FoldersViewModel.Folders
                ));

            FoldersViewModel.PackagesPusher = NuGetViewModel;
        }

        public ServersViewModel ServersViewModel { get; }

        public NuGetViewModel NuGetViewModel { get; }

        public FoldersViewModel FoldersViewModel { get; }
    }
}
