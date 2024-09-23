using Andromeda.Tools.PublishPackages.Abstractions;
using BaGet.Protocol.Models;

namespace Andromeda.Tools.PublishPackages.Services.BaGet
{
    internal class BaGetSearchResult : INuGetSearchResult
    {
        public BaGetSearchResult(SearchResult result)
        {
            _result = result;
        }

        public string PackageId => _result.PackageId;

        public string Version => _result.Version;

        private readonly SearchResult _result;
    }
}
