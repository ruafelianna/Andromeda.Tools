using Andromeda.Tools.PublishPackages.Interactions;
using Andromeda.Tools.PublishPackages.Properties;
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
    internal class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            var settings = Settings.Instance;

            Servers = [
                "https://localhost:7183/v3/index.json",
            ];

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

            var canMakeRequest = this
                .WhenAnyValue(vm => vm.SelectedServer)
                .Select(val => !string.IsNullOrEmpty(val));

            var canAddFolder = this
                .WhenAnyValue(vm => vm.SelectedFolder)
                .Select(val => !string.IsNullOrEmpty(val));

            // ----------------------------------------------

            CmdUpdate = ReactiveCommand
                .CreateFromTask<Unit, IReadOnlyList<SearchResult>>(
                    async _ => await NewClient.SearchAsync(),
                    canMakeRequest
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
                            return folders[0].Path.AbsolutePath;
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
                    canAddFolder
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
                }
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
        }

        public IEnumerable<string> Servers { get; }

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

        public ReactiveCommand<Unit, IReadOnlyList<SearchResult>> CmdUpdate { get; }

        public ReactiveCommand<Unit, string?> CmdChooseFolder { get; }

        public ReactiveCommand<Unit, Unit> CmdAddFolder { get; }

        public ReactiveCommand<Unit, Unit> CmdListPackages { get; }

        public ReactiveCommand<Unit, IEnumerable<string>> CmdRemoveFolders { get; }

        private NuGetClient NewClient => new(SelectedServer);
    }
}
