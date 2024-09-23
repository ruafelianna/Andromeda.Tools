using Andromeda.Tools.PublishPackages.Abstractions;
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
            NuGetAPIKey = new();
            Settings = new();
        }

        public static Interaction<Unit, IStorageProvider> StorageProvider { get; }

        public static Interaction<Unit, string> NuGetAPIKey { get; }

        public static Interaction<Unit, ISettings> Settings { get; }
    }
}
