using TMFChar.Gen.Util;

namespace TMFChar.Gen.Application.Cli;

public enum SpecificationType
{
    Service,
    Resource,
}

public record GeneratorArguments(
    SpecificationType SpecificationType,
    string Host,
    string Port,
    string Namespace,
    string Catalog,
    List<string> SpecificationNames,
    bool All = false
);

public static class ArgumentParser
{
    private static readonly Dictionary<SpecificationType, string> _specUrls = new()
    {
        [SpecificationType.Service] = "http://{0}:{1}/tmf-api/serviceCatalogManagement/v4",
        [SpecificationType.Resource] = "http://{0}:{1}/tmf-api/resourceCatalog/v4",
    };

    private static string UsageText =>
        """"
            dotnet-tmfchargen

            Usage: dotnet-tmfchargen [options] [

            The following arguments are required to run:
                --type              Specifies the type of specification to scaffold characteristics for. Supported values are:  
                                    - `service`  
                                    - `resource`
                --host              Hostname to use when constructing URLs for catalog management services. Defaults to `127.0.0.1`.
                --port              Port number of the service. Defaults to `40207`.
                --spec-name         The name of the specification(s) whose characteristics will be used for class generation. Multiple specifications can be separated by commas.
                --namespace         The namespace for the generated class file.
            The following arguments are optional:
                --catalog           Absolute URL for the catalog management service. If not provided, a URL is constructed using `--host` and `--port`.
            """";

    public static GeneratorArguments Parse(string[] args)
    {
        try
        {
            var argList = args.ToList();

            if (
                !TryGetArgumentValue("--type", argList, out var type)
                || (type != "service" && type != "resource")
            )
                throw new InvalidOperationException(
                    $"provide --type, supported values are 'service' and 'resource'"
                );

            var specType = type switch
            {
                "service" => SpecificationType.Service,
                "resource" => SpecificationType.Resource,
                _ => throw new InvalidOperationException("Invalid type specified"),
            };

            var specUrl = _specUrls[specType];

            if (!TryGetArgumentValue("--host", argList, out var host))
                host = "127.0.0.1";

            if (!TryGetArgumentValue("--port", argList, out var port))
                port = "40207";

            if (
                !TryGetArgumentValue("--catalog", argList, out var catalog)
                && !string.IsNullOrEmpty(catalog)
                && !Uri.IsWellFormedUriString(catalog, UriKind.Absolute)
            )
                throw new InvalidOperationException(
                    $"Provided catalog url is in invalid URL format."
                );
            else if (string.IsNullOrEmpty(catalog))
                catalog = string.Format(specUrl, host, port);

            if (!TryGetArgumentValue("--namespace", argList, out var @namespace))
                throw new InvalidOperationException($"namespace needs to be provided.");

            var all = false;

            if (!TryGetArgumentValue("--spec-name", argList, out var specName))
            {
                Logger.LogInfo(
                    "No specific specification name provided. All specifications will be fetched, and characteristics name will be generated."
                );
                all = true;
            }

            var specs = specName.Split(",").ToList();

            return new GeneratorArguments(specType, host, port, @namespace, catalog, specs, all);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message);
            Logger.LogInfo(UsageText);
            throw;
        }
    }

    private static bool TryGetArgumentValue(
        string argumentName,
        List<string> args,
        out string argumentValue
    )
    {
        argumentValue = "";

        var argumentIndex = args.IndexOf(argumentName);
        if (argumentIndex < 0)
            return false;

        var valueIndex = argumentIndex + 1;
        if (valueIndex >= args.Count)
            return false;

        var value = args[valueIndex];
        if (string.IsNullOrEmpty(value) || value.StartsWith("--"))
            return false;

        argumentValue = value;
        return true;
    }
}
