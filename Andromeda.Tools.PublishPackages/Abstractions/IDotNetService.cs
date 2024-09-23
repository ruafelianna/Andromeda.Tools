using ReactiveUI;
using System.Reactive;
using System;
using System.Threading.Tasks;

namespace Andromeda.Tools.PublishPackages.Abstractions
{
    internal interface IDotNetService
    {
        Interaction<Unit, TimeSpan> TimeOut { get; }

        Task<string> GetDevAPIKey();

        Task PushPackage(
            string token,
            string server,
            string folder,
            string file
        );

        Task RemovePackage(
            string token,
            string server,
            string name,
            string version
        );
    }
}
