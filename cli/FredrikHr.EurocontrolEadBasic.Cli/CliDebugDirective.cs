using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Diagnostics;

namespace System.CommandLine;

public sealed class CliDebugDirective() : CliDirective("debug")
{
    private CliAction? action;

    public override CliAction? Action
    {
        get => action ??= new CliDebugDirectiveAction(this);
        set => action = value ?? throw new ArgumentNullException(nameof(value));
    }

    private sealed class CliDebugDirectiveAction : AsynchronousCliAction
    {
        private readonly CliDebugDirective directive;

        public CliDebugDirectiveAction(
            CliDebugDirective directive
        ) : base()
        {
            this.directive = directive;
            Terminating = false;
        }

        public override async Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancelToken)
        {
            ArgumentNullException.ThrowIfNull(parseResult);
            if (parseResult.GetResult(directive) is null)
                return 0;
            await parseResult.Configuration.Output.WriteLineAsync(
                $"Please attach a debugger to process with ID {Environment.ProcessId} ({Process.GetCurrentProcess().ProcessName}) or press the ENTER key to continue . . ."
                ).ConfigureAwait(continueOnCapturedContext: false);
            await WaitForDebuggerAttached(cancelToken)
                .ConfigureAwait(continueOnCapturedContext: false);
            return 0;
        }

        private static async Task WaitForDebuggerAttached(CancellationToken cancelToken)
        {
            while (!Debugger.IsAttached)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), cancelToken)
                    .ConfigureAwait(continueOnCapturedContext: false);
                if (Console.KeyAvailable &&
                    Console.ReadKey(intercept: true).Key == ConsoleKey.Enter)
                    return;
            }
        }
    }
}
