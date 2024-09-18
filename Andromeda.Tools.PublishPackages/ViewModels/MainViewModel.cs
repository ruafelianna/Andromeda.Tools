using Andromeda.Tools.PublishPackages.Interactions;
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

            _foldersCache = new(x => x);

            _foldersCache
                .Connect()
                .SortBy(x => x)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _folders)
                .Subscribe();

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
                        _foldersCache.AddOrUpdate(SelectedFolder!);
                        SelectedFolder = null;
                    },
                    canAddFolder
                );
        }

        public IEnumerable<string> Servers { get; }

        private readonly SourceCache<SearchResult, string> _searchResultsCache;
        private readonly ReadOnlyObservableCollection<SearchResult> _searchResults;
        public IEnumerable<SearchResult> SearchResults => _searchResults;

        private readonly SourceCache<string, string> _foldersCache;
        private readonly ReadOnlyObservableCollection<string> _folders;
        public IEnumerable<string> Folders => _folders;

        [Reactive]
        public string? SelectedServer { get; set; }

        [Reactive]
        public string? SelectedFolder { get; private set; }

        [Reactive]
        public string? ErrorMessage { get; set; }

        public ReactiveCommand<Unit, IReadOnlyList<SearchResult>> CmdUpdate { get; }

        public ReactiveCommand<Unit, string?> CmdChooseFolder { get; }

        public ReactiveCommand<Unit, Unit> CmdAddFolder { get; }

        private NuGetClient NewClient => new(SelectedServer);
    }
}
