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
    internal class FoldersViewModel : ViewModelBase
    {
        public FoldersViewModel(ILogger logger)
        {
            InitFolders(out _foldersCache, out _folders);

            IsAnyPackageChosenObservable = _foldersCache
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .AutoRefresh(folder => folder.IsAnyPackageSelected)
                .ToCollection()
                .Select(list => list.Any(x => x.IsAnyPackageSelected));

            InitCmdAddFolder(out _cmdAddFolder, logger);

            InitCmdChooseFolder(out _cmdChooseFolder, logger);

            InitCmdRemoveFolders(out _cmdRemoveFolders, logger);

            InitCmdListPackages(out _cmdListPackages, logger);
        }

        #region Data

        private readonly SourceCache<FolderItem, string> _foldersCache;
        private readonly ReadOnlyObservableCollection<FolderItem> _folders;
        public IEnumerable<FolderItem> Folders => _folders;

        [Reactive]
        public string? SelectedFolder { get; private set; }

        #endregion

        #region Observables

        public IObservable<bool> IsAnyPackageChosenObservable { get; }

        #endregion

        #region Commands

        private readonly ReactiveCommand<Unit, (string Folder, bool Exists)> _cmdAddFolder;
        public ReactiveCommand<Unit, (string Folder, bool Exists)> CmdAddFolder => _cmdAddFolder;

        private readonly ReactiveCommand<Unit, string?> _cmdChooseFolder;
        public ReactiveCommand<Unit, string?> CmdChooseFolder => _cmdChooseFolder;

        private readonly ReactiveCommand<Unit, IEnumerable<string>> _cmdRemoveFolders;
        public ReactiveCommand<Unit, IEnumerable<string>> CmdRemoveFolders => _cmdRemoveFolders;

        private readonly ReactiveCommand<Unit, int> _cmdListPackages;
        public ReactiveCommand<Unit, int> CmdListPackages => _cmdListPackages;

        #endregion

        #region Init

        private static void InitFolders(
            out SourceCache<FolderItem, string> cache,
            out ReadOnlyObservableCollection<FolderItem> data
        )
        {
            cache = new(x => x.Name);

            cache
                .Connect()
                .SortBy(x => x.Name)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out data)
                .Subscribe();

            cache.AddOrUpdate(
                AppInteractions.Settings.Handle(default).Wait().Folders
                    .Cast<string>()
                    .Select(x => new FolderItem(x))
            );
        }

        private void InitCmdAddFolder(
            out ReactiveCommand<Unit, (string Folder, bool Exists)> cmd,
            ILogger logger
        )
        {
            var isFolderChosen = this
                .WhenAnyValue(vm => vm.SelectedFolder)
                .Select(val => !string.IsNullOrEmpty(val));

            cmd = ReactiveCommand.Create<Unit, (string Folder, bool Exists)>(
                _ => {
                    if (_foldersCache.Keys.Contains(SelectedFolder))
                    {
                        return new(SelectedFolder!, true);
                    }

                    _foldersCache.AddOrUpdate(
                        new FolderItem(SelectedFolder!)
                    );

                    return new(SelectedFolder!, false);
                },
                isFolderChosen
            );

            cmd.Subscribe(_ => SelectedFolder = null);

            cmd
                .SelectMany(async folder => {
                    var settings = await AppInteractions.Settings
                        .Handle(default);

                    if (!settings.Folders.Contains(folder.Folder))
                    {
                        settings.Folders.Add(folder.Folder);
                    }

                    settings.Save();

                    return Unit.Default;
                })
                .Subscribe();

            cmd.Subscribe(folder => {
                if (folder.Exists)
                {
                    logger.LogInformationFmt(
                        Strings.Info_FolderExists,
                        folder
                    );
                }
                else
                {
                    logger.LogInformationFmt(
                        Strings.Info_FolderAdded,
                        folder
                    );
                }
            });
        }

        private void InitCmdChooseFolder(
            out ReactiveCommand<Unit, string?> cmd,
            ILogger logger
        )
        {
            cmd = ReactiveCommand.CreateFromTask<Unit, string?>(
                async _ => {
                    var storage = await AppInteractions.StorageProvider
                        .Handle(default);

                    var folders = await storage.OpenFolderPickerAsync(new()
                    {
                        Title = Strings.Dialog_SelectPackageFolder,
                    });

                    if (folders.Any())
                    {
                        return folders[0].Path.LocalPath;
                    }

                    return null;
                }
            );

            cmd.Subscribe(folder => {
                if (folder is not null)
                {
                    SelectedFolder = folder;
                }
            });

            cmd.Subscribe(folder => {
                if (folder is not null)
                {
                    logger.LogInformationFmt(
                        Strings.Info_DirectoryChosen,
                        folder
                    );
                }
            });

            cmd.ThrownExceptions
                .Subscribe(ex => logger.LogErrorNonFmt(
                    ex, Strings.Error_ChooseFolder
                ));
        }

        private void InitCmdRemoveFolders(
            out ReactiveCommand<Unit, IEnumerable<string>> cmd,
            ILogger logger
        )
        {
            var anyFoldersChosen = _foldersCache
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .AutoRefresh(folder => folder.IsSelected)
                .ToCollection()
                .Select(list => list.Any(x => x.IsSelected));

            cmd = ReactiveCommand.Create<Unit, IEnumerable<string>>(
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

            cmd
                .SelectMany(async list => {
                    var settings = await AppInteractions.Settings
                        .Handle(default);

                    foreach (var item in list)
                    {
                        settings.Folders.Remove(item);
                    }

                    settings.Save();

                    return Unit.Default;
                })
                .Subscribe();

            cmd.Subscribe(list => logger.LogInformationFmt(
                Strings.Info_FoldersRemoved,
                $"{list.Count()}{Environment.NewLine}{string.Join(
                    Environment.NewLine,
                    list
                )}"
            ));
        }

        private void InitCmdListPackages(
            out ReactiveCommand<Unit, int> cmd,
            ILogger logger
        )
        {
            cmd = ReactiveCommand.CreateFromTask<Unit, int>(async _ =>
            {
                var count = 0;

                foreach (var item in Folders)
                {
                    await item.CmdUpdate.Execute();
                    count++;
                }

                return count;
            });

            cmd.Subscribe(count => logger.LogInformationFmt(
                Strings.Info_PackageFilesListed,
                count
            ));
        }

        #endregion
    }
}
