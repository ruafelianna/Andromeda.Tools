using Andromeda.Tools.PublishPackages.Abstractions;
using Andromeda.Tools.PublishPackages.Assets;
using BaGet.Protocol;
using System;

namespace Andromeda.Tools.PublishPackages.Services.BaGet
{
    internal class BaGetClientFactory : INuGetClientFactory
    {
        public BaGetClientFactory(
            IDotNetService dotNetService,
            Func<string?> getServer
        )
        {
            _dotNetService = dotNetService;
            _getServer = getServer;
        }

        public INuGetClient Create()
        {
            var server = _getServer()
                ?? throw new ApplicationException(
                    string.Format(
                        Strings.Error_CannotCreateNuGetClient,
                        Strings.Error_ServerNameIsNull
                    )
                );
            return new BaGetClient(
                _dotNetService,
                new NuGetClient(server),
                server
            );
        }

        private readonly IDotNetService _dotNetService;

        private readonly Func<string?> _getServer;
    }
}
