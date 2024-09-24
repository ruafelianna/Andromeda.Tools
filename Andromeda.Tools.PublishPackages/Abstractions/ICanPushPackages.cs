using ReactiveUI;
using System.Reactive;

namespace Andromeda.Tools.PublishPackages.Abstractions
{
    internal interface ICanPushPackages
    {
        ReactiveCommand<Unit, int> CmdPushPackages { get; }
    }
}
