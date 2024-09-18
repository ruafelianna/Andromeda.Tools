using System.Collections.Specialized;
using System.Configuration;

namespace Andromeda.Tools.PublishPackages.Properties
{
    internal class Settings : ApplicationSettingsBase
    {
        public static Settings Instance
            => (Settings)Synchronized(new Settings());

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public StringCollection Folders
        {
            get => (StringCollection)this[nameof(Folders)];
            set => this[nameof(Folders)] = value;
        }
    }
}
