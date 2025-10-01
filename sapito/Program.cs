
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
        }
    ),
    commands: [
        "using static Sapito.Sapito;",
        "Console.WriteLine(\"Call ConnectToRunningSap to get started!\");",
        "Console.WriteLine(\"\");"
    ]
);
