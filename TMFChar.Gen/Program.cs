using Microsoft.Extensions.DependencyInjection;
using TMFChar.Gen;
using TMFChar.Gen.Application.Cli;
using TMFChar.Gen.Service;
using TMFChar.Gen.Util;

try
{
    var arguments = ArgumentParser.Parse(args);

    using var scope = new ServiceCollection()
        .RegisterService(arguments)
        .BuildServiceProvider()
        .CreateScope();

    var genService = scope.ServiceProvider.GetRequiredService<ICharacteristicNameGenerator>();

    using var cancellationTokenSource = new CancellationTokenSource();

    Console.CancelKeyPress += (sender, e) =>
    {
        Logger.LogWarning("Cancellation requested. Please wait...");
        cancellationTokenSource.Cancel();
        e.Cancel = true;
    };

    await genService.GenerateAsync(cancellationTokenSource.Token);
}
catch (Exception ex)
{
    Logger.LogError(ex.Message);
    Environment.Exit(1);
}
