using Andromeda.Tools.PublishPackages.Abstractions;
using BaGet.Protocol;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Andromeda.Tools.PublishPackages.Services.BaGet
{
    internal class BaGetClient : INuGetClient
    {
        public BaGetClient(
            IDotNetService dotNetService,
            NuGetClient client,
            string server
        )
        {
            _dotNetService = dotNetService;
            _client = client;
            _server = server;
        }

        public async Task<IReadOnlyList<INuGetSearchResult>> SearchAsync(
            string? query,
            CancellationToken cancellationToken
        ) => (await _client.SearchAsync(query, cancellationToken))
            .Select(sr => new BaGetSearchResult(sr))
            .ToImmutableList();

        public Task PushPackageAsync(
            string folder,
            string file,
            string apikey
        ) => _dotNetService.PushPackage(
            apikey,
            _server,
            folder,
            file
        );

        public Task RemovePackageAsync(
            INuGetSearchResult pkg,
            string apikey
        ) => _dotNetService.RemovePackage(
            apikey,
            _server,
            pkg.PackageId,
            pkg.Version
        );

        private readonly IDotNetService _dotNetService;

        private readonly NuGetClient _client;

        private readonly string _server;
    }
}
