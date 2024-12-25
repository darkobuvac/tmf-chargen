using Microsoft.Extensions.Configuration;
using TMFChar.Gen.Application.Cli;

namespace TMFChar.Gen.Util;
public static class ConfigurationUtill
{
    public static IConfiguration Create(GeneratorArguments arguments)
    {
        var catalogUrl = arguments.SpecificationType switch
        {
            SpecificationType.Service => "TmfApi:ServiceCatalogManagementV4BaseUrl",
            SpecificationType.Resource => "TmfApi:ResourceCatalogManagementV4BaseUrl",
            _ => throw new InvalidOperationException($"Unsupported specification type!")
        };

        Dictionary<string, string> configData = new()
        {
            [catalogUrl] = arguments.Catalog
        };


        return new ConfigurationBuilder().AddInMemoryCollection(configData!).Build();
    }
}
