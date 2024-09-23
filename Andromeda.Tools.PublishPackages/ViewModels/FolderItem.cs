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
    internal class FolderItem : ReactiveObject
    {
        public FolderItem(string name)
        {
            Name = name;

            InitPackages(out _packagesCache, out _packages);

            InitCmdUpdate(out _cmdUpdate);
        }

        #region Data

        private readonly SourceCache<PackageItem, string> _packagesCache;
        private readonly ReadOnlyObservableCollection<PackageItem> _packages;
        public IEnumerable<PackageItem> Packages => _packages;

        public string Name { get; }

        [Reactive]
        public bool IsSelected { get; set; }

        [ObservableAsProperty]
        public bool IsAnyPackageSelected { get; }

        #endregion

        #region Commands

        private readonly ReactiveCommand<Unit, IEnumerable<PackageItem>> _cmdUpdate;
        public ReactiveCommand<Unit, IEnumerable<PackageItem>> CmdUpdate => _cmdUpdate;

        #endregion

        #region Init

        private void InitPackages(
            out SourceCache<PackageItem, string> cache,
            out ReadOnlyObservableCollection<PackageItem> data
        )
        {
            cache = new(x => x.Name);

            cache
                .Connect()
                .SortBy(x => x.Name)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out data)
                .Subscribe();

            cache
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .AutoRefresh(file => file.IsSelected)
                .ToCollection()
                .Select(list => list.Any(x => x.IsSelected))
                .ToPropertyEx(this, vm => vm.IsAnyPackageSelected);
        }

        private void InitCmdUpdate(
            out ReactiveCommand<Unit, IEnumerable<PackageItem>> cmd
        )
        {
            cmd = ReactiveCommand.Create<Unit, IEnumerable<PackageItem>>(
                _ => Directory
                    .GetFiles(Name, "*.nupkg")
                    .Select(x => new PackageItem(
                        Path.GetFileName(x))
                    )
            );

            cmd.Subscribe(list => {
                _packagesCache.Clear();
                _packagesCache.AddOrUpdate(list);
            });
        }

        #endregion
    }
}
