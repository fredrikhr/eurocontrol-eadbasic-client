using System.CommandLine;
using System.CommandLine.Hosting;

using FredrikHr.EurocontrolEadBasic.Cli;

CliRootCommand cliRootCommand = new("Eurocontrol EAD Basic")
{
    TreatUnmatchedTokensAsErrors = false,
    Action = new CliHostingAction<EadBasicRootCliService>(),
    Directives =
    {
        new EnvironmentVariablesDirective(),
        new CliDebugDirective()
    },
};

CliConfiguration cliConfig = new(cliRootCommand)
{ ProcessTerminationTimeout = null };
return await cliConfig.InvokeAsync(args ?? [])
    .ConfigureAwait(continueOnCapturedContext: false);
