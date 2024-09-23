namespace Andromeda.Tools.PublishPackages.Abstractions
{
    internal interface INuGetSearchResult
    {
        string PackageId { get; }

        string Version { get; }
    }
}
