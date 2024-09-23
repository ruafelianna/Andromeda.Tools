using Andromeda.Tools.PublishPackages.Abstractions;
using Andromeda.Tools.PublishPackages.Assets;
using Andromeda.Tools.PublishPackages.Helpers;
using Andromeda.Tools.PublishPackages.Interactions;
using DynamicData;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Andromeda.Tools.PublishPackages.ViewModels
{
    internal class ServersViewModel : ViewModelBase
    {
        public ServersViewModel(ILogger logger)
        {
            IsServerSelectedObservable = this
                .WhenAnyValue(vm => vm.SelectedServer)
                .Select(val => !string.IsNullOrEmpty(val));

            var settings = AppInteractions.Settings
                .Handle(default).Wait();

            InitServers(out _serversCache, out _servers, settings);

            InitCmdAddServer(out _cmdAddServer, logger);

            InitCmdRemoveServer(out _cmdRemoveServer, logger);
        }

        #region Data

        private readonly SourceCache<string, string> _serversCache;
        private readonly ReadOnlyObservableCollection<string> _servers;
        public IEnumerable<string> Servers => _servers;

        [Reactive]
        public string? NewServer { get; set; }

        [Reactive]
        public string? SelectedServer { get; set; }

        #endregion

        #region Observables

        public IObservable<bool> IsServerSelectedObservable { get; }

        #endregion

        #region Commands

        private readonly ReactiveCommand<Unit, string> _cmdAddServer;
        public ReactiveCommand<Unit, string> CmdAddServer => _cmdAddServer;

        private readonly ReactiveCommand<Unit, string> _cmdRemoveServer;
        public ReactiveCommand<Unit, string> CmdRemoveServer => _cmdRemoveServer;

        #endregion

        #region Init

        private static void InitServers(
            out SourceCache<string, string> cache,
            out ReadOnlyObservableCollection<string> data,
            ISettings settings
        )
        {
            cache = new(x => x);

            cache.AddOrUpdate(settings.Servers.Cast<string>());

            cache
                .Connect()
                .SortBy(x => x)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out data)
                .Subscribe();
        }

        private void InitCmdAddServer(
            out ReactiveCommand<Unit, string> cmd,
            ILogger logger
        )
        {
            var isServerEntered = this
                .WhenAnyValue(vm => vm.NewServer)
                .Select(val => !string.IsNullOrEmpty(val));

            cmd = ReactiveCommand.Create(
                () => {
                    _serversCache.AddOrUpdate(NewServer!);

                    return NewServer!;
                },
                isServerEntered
            );

            cmd.Subscribe(_ => NewServer = null);

            cmd
                .SelectMany(async newServer => {
                    var settings = await AppInteractions.Settings
                        .Handle(default);

                    if (!settings.Servers.Contains(newServer))
                    {
                        settings.Servers.Add(newServer);
                    }

                    settings.Save();

                    return Unit.Default;
                })
                .Subscribe();

            cmd.Subscribe(newServer => logger.LogInformationFmt(
                Strings.Info_ServerAdded,
                newServer
            ));
        }

        private void InitCmdRemoveServer(
            out ReactiveCommand<Unit, string> cmd,
            ILogger logger
        )
        {
            cmd = ReactiveCommand.Create<Unit, string>(
                _ => {
                    var server = SelectedServer!;

                    _serversCache.RemoveKey(server);

                    return server;
                },
                IsServerSelectedObservable
            );

            cmd
                .SelectMany(async server => {
                    var settings = await AppInteractions.Settings
                        .Handle(default);

                    if (settings.Servers.Contains(server))
                    {
                        settings.Servers.Remove(server);
                    }

                    settings.Save();

                    return Unit.Default;
                })
                .Subscribe();

            cmd.Subscribe(server => logger.LogInformationFmt(
                Strings.Info_ServerRemoved,
                server
            ));
        }

        #endregion
    }
}
