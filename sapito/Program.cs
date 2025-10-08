
using CSharpRepl.Services;
using Mykeels.CSharpRepl;
using Spectre.Console;
using Sapito;


await Repl.Run(
    new Configuration(
         logSuccess: (message, result) => {
            var propertyInfo = result.GetType().GetProperty("Result");
            var value = propertyInfo!.GetValue(result, null);
            Console.WriteLine($">> {value}");
        },
        logError: (message, exception, _) => {
            Console.WriteLine($">> {exception}");
        }
    ),
    commands: [
        "using sapfewse;",
        "using saprotwr.net;",
        "using static Sapito.Sapito;",
        "var session = ConnectToRunningSap();",
    ]
);
