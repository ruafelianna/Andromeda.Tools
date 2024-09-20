using Andromeda.Tools.PublishPackages.Interactions;
using Andromeda.Tools.PublishPackages.Properties;
using Andromeda.Tools.PublishPackages.Services;
using BaGet.Protocol;
using BaGet.Protocol.Models;
using DynamicData;
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
    internal partial class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            var settings = Settings.Instance;

            // ----------------------------------------------

            _serversCache = new(x => x);

            _serversCache.AddOrUpdate(settings.Servers.Cast<string>());

            _serversCache
                .Connect()
                .SortBy(x => x)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _servers)
                .Subscribe();

            // ----------------------------------------------

            _searchResultsCache = new(x => x.PackageId);

            _searchResultsCache
                .Connect()
                .SortBy(x => x.PackageId)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _searchResults)
                .Subscribe();

            // ----------------------------------------------

            _foldersCache = new(x => x.Name);

            _foldersCache
                .Connect()
                .SortBy(x => x.Name)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _folders)
                .Subscribe();

            _foldersCache.AddOrUpdate(
                Settings.Instance.Folders
                    .Cast<string>()
                    .Select(x => new FolderItem(x))
            );

            // ----------------------------------------------

            var isServerSelected = this
                .WhenAnyValue(vm => vm.SelectedServer)
                .Select(val => !string.IsNullOrEmpty(val));

            var isFolderChosen = this
                .WhenAnyValue(vm => vm.SelectedFolder)
                .Select(val => !string.IsNullOrEmpty(val));

            var isServerEntered = this
                .WhenAnyValue(vm => vm.NewServer)
                .Select(val => !string.IsNullOrEmpty(val));

            var anyFoldersChosen = _foldersCache
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .AutoRefresh(folder => folder.IsSelected)
                .ToCollection()
                .Select(list => list.Any(x => x.IsSelected));

            var anyPackagesChosen = _foldersCache
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .AutoRefresh(folder => folder.IsAnyPackageSelected)
                .ToCollection()
                .Select(list => list.Any(x => x.IsAnyPackageSelected));

            var canPushPackages = Observable
                .CombineLatest(isServerSelected, anyPackagesChosen)
                .Select(list => list.All(x => x));

            // ----------------------------------------------

            CmdUpdate = ReactiveCommand
                .CreateFromTask<Unit, IReadOnlyList<SearchResult>>(
                    async _ => await NewClient.SearchAsync(),
                    isServerSelected
                );

            CmdUpdate
                .Subscribe(_ => ErrorMessage = null);

            CmdUpdate.ThrownExceptions
                .Subscribe(ex => ErrorMessage = ex.Message);

            CmdUpdate
                .Subscribe(list => {
                    var keysToRemove = _searchResultsCache.Keys
                        .Except(list.Select(x => x.PackageId));

                    _searchResultsCache.RemoveKeys(keysToRemove);

                    _searchResultsCache.AddOrUpdate(list);
                });

            // ----------------------------------------------

            CmdChooseFolder = ReactiveCommand
                .CreateFromTask<Unit, string?>(
                    async _ => {
                        var sp = await AppInteractions.StorageProvider
                            .Handle(_);

                        var folders = await sp.OpenFolderPickerAsync(new(){
                            Title = "Select containing NuGet package folder",
                        });

                        if (folders.Any())
                        {
                            return folders[0].Path.LocalPath;
                        }

                        return null;
                    }
                );

            CmdChooseFolder
                .Subscribe(_ => ErrorMessage = null);

            CmdChooseFolder.ThrownExceptions
                .Subscribe(ex => ErrorMessage = ex.Message);

            CmdChooseFolder
                .Subscribe(x => SelectedFolder = x);

            // ----------------------------------------------

            CmdAddFolder = ReactiveCommand
                .Create(
                    () => {
                        _foldersCache.AddOrUpdate(
                            new FolderItem(SelectedFolder!)
                        );

                        var settings = Settings.Instance;

                        if (!settings.Folders.Contains(SelectedFolder))
                        {
                            settings.Folders.Add(SelectedFolder);
                        }

                        settings.Save();

                        SelectedFolder = null;
                    },
                    isFolderChosen
                );

            // ----------------------------------------------

            CmdListPackages = ReactiveCommand.CreateFromTask(async () =>
            {
                foreach (var item in Folders)
                {
                    await item.CmdUpdate.Execute();
                }
            });

            // ----------------------------------------------

            CmdRemoveFolders = ReactiveCommand.Create<Unit, IEnumerable<string>>(
                _ => {
                    var selected = _folders
                        .Where(x => x.IsSelected)
                        .Select(x => x.Name)
                        .ToArray();

                    _foldersCache.RemoveKeys(selected);

                    return selected;
                },
                anyFoldersChosen
            );

            CmdRemoveFolders
                .Subscribe(list => {
                    var settings = Settings.Instance;

                    foreach (var item in list)
                    {
                        settings.Folders.Remove(item);
                    }

                    settings.Save();
                });

            // ----------------------------------------------

            CmdPushPackages = ReactiveCommand.CreateFromTask(
                async() => {
                    var settings = Settings.Instance;

                    var token = settings.ApiKey
                        ?? await DotnetService.GetDevAPIKey()
                        ?? throw new InvalidOperationException(
                            "No NuGet API key provided"
                        );

                    var pkgs = Folders
                        .SelectMany(
                            Folder => Folder.Packages
                                .Where(p => p.IsSelected)
                                .Select(
                                    File => (Folder, File)
                                )
                        );

                    var ok = true;

                    foreach (var (Folder, File) in pkgs)
                    {
                        var (result, stdout, stderr) = await DotnetService.PushPackage(
                            token,
                            SelectedServer!,
                            Folder.Name,
                            File.Name
                        );

                        ok &= result;
                    }

                    if (!ok)
                    {
                        throw new Exception("Not all packages were pushed");
                    }
                },
                canPushPackages
            );

            CmdPushPackages
                .Subscribe(_ => ErrorMessage = null);

            CmdPushPackages.ThrownExceptions
                .Subscribe(ex => ErrorMessage = ex.Message);

            // ----------------------------------------------

            CmdAddServer = ReactiveCommand.Create(
                () => {
                    _serversCache.AddOrUpdate(NewServer!);

                    var settings = Settings.Instance;

                    if (!settings.Servers.Contains(NewServer))
                    {
                        settings.Servers.Add(NewServer);
                    }

                    settings.Save();

                    NewServer = null;
                },
                isServerEntered
            );

            // ----------------------------------------------

            CmdRemoveServer = ReactiveCommand.Create<Unit, string>(
                _ => {
                    var server = SelectedServer!;

                    _serversCache.RemoveKey(server);

                    var settings = Settings.Instance;

                    if (settings.Servers.Contains(server))
                    {
                        settings.Servers.Remove(server);
                    }

                    settings.Save();

                    return server;
                },
                isServerSelected
            );

            // ----------------------------------------------

            CmdRemovePackage = ReactiveCommand.CreateFromTask<SearchResult, string>(
                async sr => {
                    var token = Settings.Instance.ApiKey
                        ?? await DotnetService.GetDevAPIKey()
                        ?? throw new InvalidOperationException(
                            "No NuGet API key provided"
                        );

                    var (result, stdout, stderr) = await DotnetService.RemovePackage(
                        token,
                        SelectedServer!,
                        sr.PackageId,
                        sr.Version
                    );

                    if (!result)
                    {
                        throw new Exception("Couldn't remove the package");
                    }

                    await CmdUpdate.Execute();

                    return $"{sr.PackageId}.{sr.Version}";
                },
                isServerSelected
            );

            CmdRemovePackage
                .Subscribe(_ => ErrorMessage = null);

            CmdRemovePackage.ThrownExceptions
                .Subscribe(ex => ErrorMessage = ex.Message);
        }

        private readonly SourceCache<string, string> _serversCache;
        private readonly ReadOnlyObservableCollection<string> _servers;
        public IEnumerable<string> Servers => _servers;

        private readonly SourceCache<SearchResult, string> _searchResultsCache;
        private readonly ReadOnlyObservableCollection<SearchResult> _searchResults;
        public IEnumerable<SearchResult> SearchResults => _searchResults;

        private readonly SourceCache<FolderItem, string> _foldersCache;
        private readonly ReadOnlyObservableCollection<FolderItem> _folders;
        public IEnumerable<FolderItem> Folders => _folders;

        [Reactive]
        public string? SelectedServer { get; set; }

        [Reactive]
        public string? SelectedFolder { get; private set; }

        [Reactive]
        public string? ErrorMessage { get; set; }

        [Reactive]
        public string? NewServer { get; set; }

        public ReactiveCommand<Unit, IReadOnlyList<SearchResult>> CmdUpdate { get; }

        public ReactiveCommand<Unit, string?> CmdChooseFolder { get; }

        public ReactiveCommand<Unit, Unit> CmdAddFolder { get; }

        public ReactiveCommand<Unit, Unit> CmdListPackages { get; }

        public ReactiveCommand<Unit, IEnumerable<string>> CmdRemoveFolders { get; }

        public ReactiveCommand<Unit, Unit> CmdPushPackages { get; }

        public ReactiveCommand<Unit, Unit> CmdAddServer { get; }

        public ReactiveCommand<Unit, string> CmdRemoveServer { get; }

        public ReactiveCommand<SearchResult, string> CmdRemovePackage { get; }

        private NuGetClient NewClient => new(SelectedServer);
    }
}
