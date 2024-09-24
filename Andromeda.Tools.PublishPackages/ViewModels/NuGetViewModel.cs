using Andromeda.Tools.PublishPackages.Abstractions;
using Andromeda.Tools.PublishPackages.Assets;
using Andromeda.Tools.PublishPackages.Helpers;
using Andromeda.Tools.PublishPackages.Interactions;
using DynamicData;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Andromeda.Tools.PublishPackages.ViewModels
{
    internal class NuGetViewModel : ViewModelBase, ICanPushPackages
    {
        public NuGetViewModel(
            ILogger logger,
            INuGetClientFactory clientFactory,
            IObservable<bool> isServerSelectedObservable,
            IObservable<bool> canPushPackagesObservable
        )
        {
            InitInteractions(out _foldersInteraction);

            InitSearchResults(out _searchResultsCache, out _searchResults);

            InitCmdUpdate(
                out _cmdUpdate,
                logger,
                clientFactory,
                isServerSelectedObservable
            );

            InitCmdRemovePackage(
                out _cmdRemovePackage,
                logger,
                clientFactory,
                isServerSelectedObservable
            );

            InitCmdPushPackages(
                out _cmdPushPackages,
                logger,
                clientFactory,
                canPushPackagesObservable
            );
        }

        #region Data

        private readonly SourceCache<INuGetSearchResult, string> _searchResultsCache;
        private readonly ReadOnlyObservableCollection<INuGetSearchResult> _searchResults;
        public IEnumerable<INuGetSearchResult> SearchResults => _searchResults;

        #endregion

        #region Commands

        private readonly ReactiveCommand<Unit, IReadOnlyList<INuGetSearchResult>> _cmdUpdate;
        public ReactiveCommand<Unit, IReadOnlyList<INuGetSearchResult>> CmdUpdate => _cmdUpdate;

        private readonly ReactiveCommand<INuGetSearchResult, string> _cmdRemovePackage;
        public ReactiveCommand<INuGetSearchResult, string> CmdRemovePackage => _cmdRemovePackage;

        private readonly ReactiveCommand<Unit, int> _cmdPushPackages;
        public ReactiveCommand<Unit, int> CmdPushPackages => _cmdPushPackages;

        #endregion

        #region Interactions

        private readonly Interaction<Unit, IEnumerable<FolderItem>> _foldersInteraction;
        public Interaction<Unit, IEnumerable<FolderItem>> FoldersInteraction => _foldersInteraction;

        #endregion

        #region Init

        private static void InitInteractions(
            out Interaction<Unit, IEnumerable<FolderItem>> foldersInteraction
        ) => foldersInteraction = new();

        private static void InitSearchResults(
            out SourceCache<INuGetSearchResult, string> cache,
            out ReadOnlyObservableCollection<INuGetSearchResult> data
        )
        {
            cache = new(x => x.PackageId);

            cache
                .Connect()
                .SortBy(x => x.PackageId)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out data)
                .Subscribe();
        }

        private void InitCmdUpdate(
            out ReactiveCommand<Unit, IReadOnlyList<INuGetSearchResult>> cmd,
            ILogger logger,
            INuGetClientFactory clientFactory,
            IObservable<bool> isServerSelectedObservable
        )
        {
            cmd = ReactiveCommand
                .CreateFromTask<Unit, IReadOnlyList<INuGetSearchResult>>(
                    async _ => await clientFactory.Create()
                        .SearchAsync(),
                    isServerSelectedObservable
                );

            cmd.Subscribe(list =>
            {
                _searchResultsCache.Clear();
                _searchResultsCache.AddOrUpdate(list);
            });

            cmd.Subscribe(list => logger.LogInformationFmt(
                Strings.Info_PackagesListUpdated,
                $"{list.Count}{Environment.NewLine}{string.Join(
                    Environment.NewLine,
                    list.Select(x => x.PackageId).Order()
                )}"
            ));

            cmd.ThrownExceptions.Subscribe(ex => logger.LogErrorNonFmt(
                ex, Strings.Error_UpdatePackagesList
            ));
        }

        private void InitCmdRemovePackage(
            out ReactiveCommand<INuGetSearchResult, string> cmd,
            ILogger logger,
            INuGetClientFactory clientFactory,
            IObservable<bool> isServerSelectedObservable
        )
        {
            cmd = ReactiveCommand.CreateFromTask<INuGetSearchResult, string>(
                async sr => {
                    var token = await AppInteractions.NuGetAPIKey
                        .Handle(default);

                    await clientFactory.Create()
                        .RemovePackageAsync(sr, token);

                    return $"{sr.PackageId}.{sr.Version}";
                },
                isServerSelectedObservable
            );

            cmd.Subscribe(async _ => await CmdUpdate.Execute());

            cmd.Subscribe(pkgName => logger.LogInformationFmt(
                Strings.Info_PackageRemoved,
                pkgName
            ));

            cmd.ThrownExceptions
                .Subscribe(ex => logger.LogErrorNonFmt(
                    ex, Strings.Error_RemovePackage
                ));
        }

        private void InitCmdPushPackages(
            out ReactiveCommand<Unit, int> cmd,
            ILogger logger,
            INuGetClientFactory clientFactory,
            IObservable<bool> canPushPackagesObservable
        )
        {
            cmd = ReactiveCommand.CreateFromTask(
                async () => {
                    var token = await AppInteractions.NuGetAPIKey
                        .Handle(default);

                    var client = clientFactory.Create();

                    var pkgs = (await FoldersInteraction.Handle(default))
                        .SelectMany(
                            Folder => Folder.Packages
                                .Where(p => p.IsSelected)
                                .Select(
                                    File => (Folder, File)
                                )
                        );

                    var count = 0;

                    foreach (var (Folder, File) in pkgs)
                    {
                        try
                        {
                            await client.PushPackageAsync(
                                Folder.Name,
                                File.Name,
                                token
                            );

                            count++;
                        }
                        catch (Exception)
                        {
                        }
                    }

                    return count;
                },
                canPushPackagesObservable
            );

            cmd.Subscribe(count => logger.LogInformationFmt(
                Strings.Info_PackagesPushed,
                count
            ));

            cmd.ThrownExceptions
                .Subscribe(ex => logger.LogErrorNonFmt(
                    ex, Strings.Error_CannotPushPackages
                ));
        }

        #endregion
    }
}
