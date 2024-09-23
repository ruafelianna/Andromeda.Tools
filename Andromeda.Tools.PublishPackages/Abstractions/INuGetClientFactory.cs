namespace Andromeda.Tools.PublishPackages.Abstractions
{
    internal interface INuGetClientFactory
    {
        INuGetClient Create();
    }
}
