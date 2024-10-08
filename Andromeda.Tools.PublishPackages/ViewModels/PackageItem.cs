using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Andromeda.Tools.PublishPackages.ViewModels
{
    internal class PackageItem : ReactiveObject
    {
        public PackageItem(string name)
        {
            Name = name;
        }

        #region Data

        public string Name { get; }

        [Reactive]
        public bool IsSelected { get; set; }

        #endregion
    }
}
