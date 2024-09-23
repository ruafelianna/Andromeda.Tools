using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Andromeda.Tools.PublishPackages.Abstractions
{
    internal interface INuGetClient
    {
        Task<IReadOnlyList<INuGetSearchResult>> SearchAsync(
            string? query = null,
            CancellationToken cancellationToken = default
        );

        Task PushPackageAsync(
            string folder,
            string file,
            string apikey
        );

        Task RemovePackageAsync(
            INuGetSearchResult pkg,
            string apikey
        );
    }
}
