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

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public StringCollection Servers
        {
            get => (StringCollection)this[nameof(Servers)];
            set => this[nameof(Servers)] = value;
        }

        [UserScopedSetting]
        public string? ApiKey
        {
            get => (string?)this[nameof(ApiKey)];
            set => this[nameof(ApiKey)] = value;
        }
    }
}
