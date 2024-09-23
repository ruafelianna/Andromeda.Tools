using Andromeda.Tools.PublishPackages.Abstractions;
using System;
using System.Collections.Specialized;
using System.Configuration;

namespace Andromeda.Tools.PublishPackages.Properties
{
    internal class Settings : ApplicationSettingsBase, ISettings
    {
        public static ISettings Instance
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
        [DefaultSettingValue(null)]
        public string? ApiKey
        {
            get => (string?)this[nameof(ApiKey)];
            set => this[nameof(ApiKey)] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue(null)]
        public string? Culture
        {
            get => (string?)this[nameof(Culture)];
            set => this[nameof(Culture)] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("00:00:03")]
        public TimeSpan DotnetTimeout
        {
            get => (TimeSpan)this[nameof(DotnetTimeout)];
            set => this[nameof(DotnetTimeout)] = value;
        }
    }
}
