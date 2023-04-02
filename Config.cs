using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text.Json;
using System.IO;

public class Config
{
    public record struct Options(bool debug, int port, bool presenterMode);

    public static Options options = default(Options) with {port = 8270};

    public class Commands
    {
        public static void exportCli(string fileName)
        {
            string fileContent = JsonSerializer.Serialize(arguments.Where(entry => entry.Value.export == true).Select(entry => new
            {
                name = entry.Key,
                type = (entry.Value.type).ToString(),
                doc = entry.Value.doc
            }));

            SaveJsonKeywordFile(fileName, fileContent);

            Environment.Exit(1);
        }

        public record Argument(string name, string type);
        public record Method(string name, Argument[] args);

        public static void exportApi(string fileName)
        {
            var methods = typeof(RoboSAPiens.RoboSAPiens).GetMethods()
                .Where(method => method.GetCustomAttribute(typeof(RoboSAPiens.Keyword)) != null)
                .Select(method => new Method(method.Name, method.GetParameters()
                    .Select(param => new Argument(param.Name!, param.ParameterType.ToString())).ToArray())).ToList();

            SaveJsonKeywordFile(fileName, JsonSerializer.Serialize(methods));

            Environment.Exit(1);
        }

        private static void SaveJsonKeywordFile(string fileName, string content)
        {
            if (!Path.HasExtension(fileName))
                fileName += ".json";

            string path =  AppDomain.CurrentDomain.BaseDirectory;

            File.WriteAllText(Path.Combine(path, fileName), content);
        }

        public static void help()
        {
            System.Console.WriteLine("The following options are available:\n");
            arguments.Select(entry => $"--{entry.Key} {getPlaceholder(entry.Value.handler)}\n  {entry.Value.doc}")
                     .ToList()
                     .ForEach(line => System.Console.WriteLine(line + "\n"));
            
            Environment.Exit(1);
        }
    }

    static String getPlaceholder(Action<object> action)
    {
        return action.Method.GetParameters().First().Name! switch
        {
            "_" => "",
            String name => name.ToUpper()
        };
    }

    private enum ArgType
    {
        Bool,
        Int,
        Str
    }

    private record Arg(ArgType type, string doc, bool export, Action<object> handler);

    private static Dictionary<string, Arg> arguments = new Dictionary<string, Arg>() {
        {"debug", new Arg(
            ArgType.Bool,
            "Print detailed information to stdout when classifying and searching GUI elements",
            false,
            (_) => options = options with {debug = true}
        )},
        {"export-api", new Arg(
            ArgType.Str,
            "Export the Robot Framework API specification in JSON format to the specified file and exit",
            false,
            (filename) => Commands.exportApi((string)filename)
        )},
        {"export-cli", new Arg(
            ArgType.Str,
            "Export the CLI specification in JSON format to the specified file and exit",
            false,
            (filename) => Commands.exportCli((string)filename)
        )},
        {"help", new Arg(
            ArgType.Bool,
            "Print the help to the console and exit",
            false,
            (_) => Commands.help()
        )},
        {"port", new Arg(
            ArgType.Int,
            "Set the port of the HTTP server implementing the Remote interface",
            true,
            (port) => options = options with {port = Int32.Parse((string)port)}
        )},
        {"presenter-mode", new Arg(
            ArgType.Bool,
            "Highlight each GUI element acted upon",
            true,
            (_) => options = options with {presenterMode = true}
        )}
    };

    public static void parseArgs(string[] args)
    {
        var optionStack = new Stack<string>(args.Reverse());
        while (optionStack.Count > 0)
        {
            var option = optionStack.Pop();

            if (option.StartsWith("--"))
                option = option.Replace("--", "");

            if (!(arguments.ContainsKey(option)))
            {
                Console.Error.WriteLine($"The Option `{option}` is invalid. Call --help to see the list of valid options");
                Environment.Exit(1);
            }

            switch (arguments[option].type)
            {
                case ArgType.Bool: 
                    arguments[option].handler(true); 
                    break;
                default:
                    if (optionStack.Count > 0) { 
                        arguments[option].handler(optionStack.Pop());
                    }
                    else {
                        System.Console.WriteLine($"Missing argument '{getPlaceholder(arguments[option].handler)}'");
                        Environment.Exit(1);
                    }
                    break;
            };
        }
    }
}