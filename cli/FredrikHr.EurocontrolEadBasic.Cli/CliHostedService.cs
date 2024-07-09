using Microsoft.Extensions.Hosting;

namespace System.CommandLine.Hosting;

public abstract class CliHostedService(
    IHostApplicationLifetime lifetime
    ) : BackgroundService
{
    protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await InvokeAsync(stoppingToken)
            .ConfigureAwait(continueOnCapturedContext: false);
        lifetime.StopApplication();
    }

    protected abstract Task InvokeAsync(CancellationToken cancelToken);
}
