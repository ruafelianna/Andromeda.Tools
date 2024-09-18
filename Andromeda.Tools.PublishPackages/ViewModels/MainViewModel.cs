using BaGet.Protocol;
using BaGet.Protocol.Models;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Collections.ObjectModel;

namespace Andromeda.Tools.PublishPackages.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            Servers = [
                "https://localhost:7183/v3/index.json",
            ];

            _searchResultsCache = new(x => x.PackageId);

            _searchResultsCache
                .Connect()
                .SortBy(x => x.PackageId)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _searchResults)
                .Subscribe();

            var canDoThings = this
                .WhenAnyValue(vm => vm.SelectedServer)
                .Select(val => !string.IsNullOrEmpty(val));

            CmdUpdate = ReactiveCommand
                .CreateFromTask<Unit, IReadOnlyList<SearchResult>>(
                    async _ => await NewClient.SearchAsync(),
                    canDoThings
                );

            CmdUpdate.ThrownExceptions
                .Subscribe(ex => ErrorMessage = ex.Message);

            CmdUpdate
                .Subscribe(_ => ErrorMessage = null);

            CmdUpdate
                .Subscribe(list => {
                    var keysToRemove = _searchResultsCache.Keys
                        .Except(list.Select(x => x.PackageId));

                    _searchResultsCache.RemoveKeys(keysToRemove);

                    _searchResultsCache.AddOrUpdate(list);
                });
        }

        public IEnumerable<string> Servers { get; }

        private readonly SourceCache<SearchResult, string> _searchResultsCache;
        private readonly ReadOnlyObservableCollection<SearchResult> _searchResults;
        public IEnumerable<SearchResult> SearchResults => _searchResults;

        [Reactive]
        public string? SelectedServer { get; set; }

        [Reactive]
        public string? ErrorMessage { get; set; }

        public ReactiveCommand<Unit, IReadOnlyList<SearchResult>> CmdUpdate { get; }

        private NuGetClient NewClient => new(SelectedServer);
    }
}
