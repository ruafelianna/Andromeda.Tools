using System;
using System.Collections.Specialized;

namespace Andromeda.Tools.PublishPackages.Abstractions
{
    internal interface ISettings
    {
        public StringCollection Folders { get; }

        StringCollection Servers { get; }

        string? ApiKey { get; }

        string? Culture { get; set; }

        TimeSpan DotnetTimeout { get; set; }

        void Save();
    }
}
