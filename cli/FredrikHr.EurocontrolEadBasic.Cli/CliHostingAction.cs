using System.CommandLine.Invocation;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace System.CommandLine.Hosting;

public class CliHostingAction<THostedService>(
    Action<IHostBuilder>? configureHost = default
    ) : AsynchronousCliAction
    where THostedService : CliHostedService
{
    private static readonly char[] equalsSeparator = ['='];

    public sealed override async Task<int> InvokeAsync(
        ParseResult parseResult,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parseResult);

        string[] args = [.. parseResult.UnmatchedTokens];
        IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args);
        hostBuilder.Properties[typeof(ParseResult)] = parseResult;
        if (parseResult.Configuration.RootCommand is CliRootCommand cliRootCommand
            && cliRootCommand.Directives.FirstOrDefault(d => d.Name.Equals("config", StringComparison.OrdinalIgnoreCase)) is { } configDirective
            && parseResult.GetResult(configDirective) is { } configResult)
        {
            hostBuilder.ConfigureHostConfiguration(hostConfig =>
            {
                hostConfig.AddInMemoryCollection(configResult.Values.Select(static s =>
                {
                    string[] parts = s.Split(equalsSeparator, count: 2);
                    string key = parts[0];
                    string? value = parts.Length > 0 ? parts[1] : null;
                    return KeyValuePair.Create(key, value);
                }).ToList());
            });
        }
        hostBuilder.ConfigureServices(services =>
        {
            services.AddSingleton(parseResult);
            services.AddHostedService<THostedService>();
        });
        configureHost?.Invoke(hostBuilder);

        await hostBuilder.RunConsoleAsync(cancellationToken)
            .ConfigureAwait(continueOnCapturedContext: false);
        return Environment.ExitCode;
    }
}
