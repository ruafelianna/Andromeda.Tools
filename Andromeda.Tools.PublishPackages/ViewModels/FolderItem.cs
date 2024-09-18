using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Andromeda.Tools.PublishPackages.ViewModels
{
    internal class FolderItem
    {
        public FolderItem(string name)
        {
            Name = name;

            _packagesCache = new(x => x.Name);

            _packagesCache
                .Connect()
                .SortBy(x => x.Name)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _packages)
                .Subscribe();

            CmdUpdate = ReactiveCommand
                .Create<Unit, IEnumerable<PackageItem>>(
                    _ => Directory
                        .GetFiles(Name, "*.nupkg")
                        .Select(x => new PackageItem(
                            Path.GetFileName(x))
                        )
                );

            CmdUpdate
                .Subscribe(list =>
                {
                    _packagesCache.Clear();
                    _packagesCache.AddOrUpdate(list);
                });
        }

        public string Name { get; }

        [Reactive]
        public bool IsSelected { get; set; }

        private readonly SourceCache<PackageItem, string> _packagesCache;
        private readonly ReadOnlyObservableCollection<PackageItem> _packages;
        public IEnumerable<PackageItem> Packages => _packages;

        public ReactiveCommand<Unit, IEnumerable<PackageItem>> CmdUpdate { get; }
    }
}
