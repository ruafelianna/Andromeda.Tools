using Avalonia.Platform.Storage;
using ReactiveUI;
using System.Reactive;

namespace Andromeda.Tools.PublishPackages.Interactions
{
    internal static class AppInteractions
    {
        static AppInteractions()
        {
            StorageProvider = new();
        }

        public static Interaction<Unit, IStorageProvider> StorageProvider { get; }
    }
}
