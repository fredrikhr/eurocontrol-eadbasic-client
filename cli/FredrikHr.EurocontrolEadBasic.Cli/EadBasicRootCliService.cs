using System.CommandLine.Hosting;

using Microsoft.Extensions.Hosting;

namespace FredrikHr.EurocontrolEadBasic.Cli;

internal class EadBasicRootCliService(
    IHostApplicationLifetime lifetime
    ) : CliHostedService(lifetime)
{
    protected override async Task InvokeAsync(CancellationToken cancelToken)
    {
        await Task.Yield();
    }
}
