using System.Reflection;
using NetJinja;

namespace RoboSAPiens.Recorder
{
    public static class KeywordCallArgType
    {
        public const string ARG = "ARG";
        public const string KWARG = "KWARG";
        public const string LOCATOR = "LOCATOR";
    }

    public record KeywordCallArg(string name, string value, string type)
    {
        public override string ToString()
        {
            var rfValue = value.NullIfEmpty() ?? "${EMPTY}";

            return type switch
            {
                KeywordCallArgType.KWARG => $"{name}={rfValue}",
                _ => rfValue
            };
        }
    }

    public record KeywordCall(string name, List<KeywordCallArg> args, string? returnValue=null, string? comment=null)
    {
        public override string ToString()
        {
            if (returnValue != null)
            {
                return string.Join("    ", [returnValue, name, ..args]);
            }
            else
            {
                return string.Join("    ", [name, ..args]);
            }
        }
    }

    public record RobotKeyword(string name, List<KeywordCall> steps);

    public record RobotTestCase(string name, List<KeywordCall> steps);

    public record RobotFile(string filename, Dictionary<string, string> settings, List<RobotKeyword> keywords, List<RobotTestCase> testCases)
    {
        public override string ToString()
        {
            Dictionary<string, object?> data = new()
            {
                ["settings"] = settings,
                ["keywords"] = keywords,
                ["testCases"] = testCases
            };
            var curdir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var template = File.ReadAllText(Path.Combine(curdir!, "templates", "recording.jinja.robot"));

            return Jinja.Render(template, data);
        }

        public void save()
        {
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), filename + ".robot");
            File.WriteAllText(filepath, ToString());
            Console.WriteLine($"Test Case saved to: {filepath}");
        }
    }
}