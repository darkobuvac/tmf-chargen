using Microsoft.Extensions.DependencyInjection;
using TmfApiClients;
using TMFChar.Gen.Application.Cli;
using TMFChar.Gen.Service;
using TMFChar.Gen.Util;

namespace TMFChar.Gen;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterService(
        this IServiceCollection services,
        GeneratorArguments arguments
    )
    {
        var configuration = ConfigurationUtill.Create(arguments);
        services.AddTmfApiClients(configuration);
        services.AddSingleton(sp =>
        {
            return arguments;
        });

        services.AddScoped<ICharacteristicNameGenerator, CharacteristicNameGenerator>();

        return services;
    }
}
