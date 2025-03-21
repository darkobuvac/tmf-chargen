﻿using TmfApiClients.ResourceCatalogManagement.v4;
using TmfApiClients.ServiceCatalogManagement.v4;
using TMFChar.Gen.Application.Cli;
using TMFChar.Gen.Util;

namespace TMFChar.Gen.Service;

public interface ICharacteristicNameGenerator
{
    Task GenerateAsync(CancellationToken cancellationToken);
}

public class CharacteristicNameGenerator(
    GeneratorArguments generatorArguments,
    IServiceCatalogManagement4ApiClient serviceCatalog,
    IResourceCatalogManagement4ApiClient resourceCatalog
) : ICharacteristicNameGenerator
{
    private readonly GeneratorArguments _generatorArguments = generatorArguments;
    private readonly IServiceCatalogManagement4ApiClient _serviceCatalog = serviceCatalog;
    private readonly IResourceCatalogManagement4ApiClient _resourceCatalog = resourceCatalog;

    public async Task GenerateAsync(CancellationToken cancellationToken)
    {
        try
        {
            var charList = await GetCharacteristicNamesAsync(cancellationToken);

            var className = _generatorArguments.SpecificationType switch
            {
                SpecificationType.Service => "ServiceCharacteristics",
                SpecificationType.Resource => "ResourceCharacteristics",
                _ => throw new InvalidOperationException($"Unsupported specification type"),
            };

            var classGenerator = new CharacteristicClassGenerator(
                _generatorArguments.Namespace,
                className,
                charList!
            );

            var content = classGenerator.Build();
            var fileName = $"{className}.cs";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            await File.WriteAllTextAsync(filePath, content, cancellationToken);

            ConsoleLogger.LogInfo(
                $"File '{fileName}' has been successfully created at: {filePath}"
            );
        }
        catch (Exception ex)
        {
            ConsoleLogger.LogError(ex.Message);
            throw;
        }
    }

    private async Task<List<string>> GetCharacteristicNamesAsync(
        CancellationToken cancellationToken
    )
    {
        if (_generatorArguments.SpecificationType == SpecificationType.Service)
        {
            var qp = new ListServiceSpecificationsQueryParams();

            if (!_generatorArguments.All)
                qp.NameList = _generatorArguments.SpecificationNames;

            var specs = await _serviceCatalog.ListServiceSpecificationsAsync(qp, cancellationToken);

            var svcChars = (specs.Items ?? [])
                .SelectMany(s => s.SpecCharacteristic ?? [])
                .DistinctBy(c => c.Name)
                .Select(c => c.Name!)
                .ToList();

            CheckCharacteristics(svcChars);

            return svcChars;
        }

        if (_generatorArguments.SpecificationType == SpecificationType.Resource)
        {
            var qp = new ListResourceSpecificationsQueryParams();

            if (!_generatorArguments.All)
                qp.NameList = _generatorArguments.SpecificationNames;

            var specs = await _resourceCatalog.ListResourceSpecificationsAsync(
                qp,
                cancellationToken
            );

            var rscChars = (specs.Items ?? [])
                .SelectMany(s => s.ResourceSpecCharacteristic ?? [])
                .DistinctBy(c => c.Name)
                .Select(c => c.Name!)
                .ToList();

            CheckCharacteristics(rscChars);

            return rscChars;
        }

        throw new InvalidOperationException($"Unsupported specification type.");
    }

    private static void CheckCharacteristics(List<string> characteristics)
    {
        var charsWithSpecialCharacters = characteristics.Where(c =>
            c.Any(ch => !char.IsLetterOrDigit(ch))
        );

        if (charsWithSpecialCharacters.Any())
            ConsoleLogger.LogWarning(
                $"Some characteristic names contain special characters: {string.Join(",", charsWithSpecialCharacters)}."
            );

        var charGroup = characteristics.GroupBy(c => c.ToLowerInvariant());
        var charDict = charGroup.ToDictionary(g => g.Key, g => g.ToList());

        foreach (var v in charDict.Values)
        {
            if (v.Count == 1)
                continue;

            ConsoleLogger.LogWarning(
                $"Multiple characteristics with the same name but different casing were found: {string.Join(",", v)}."
            );
        }
    }
}
