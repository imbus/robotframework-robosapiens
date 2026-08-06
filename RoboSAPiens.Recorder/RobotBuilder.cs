using System.Reflection;
using NetJinja;

namespace RoboSAPiens.Recorder
{
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

    public interface IRobotBuilder
    {
        RobotFile build(string lang, string testCaseName);
    }

    public abstract class BaseRobotBuilder: IRobotBuilder
    {
        public RobotFile build(string lang, string testCaseName)
        {
            return new RobotFile(
                filename: testCaseName.toFileName(),
                settings: settings(lang),
                keywords: keywords(lang),
                testCases: [testCase(lang, testCaseName)]
            );
        }

        protected abstract List<RobotKeyword> keywords(string lang);

        protected virtual Dictionary<string, string> settings(string lang)
        {
            var robosapiens = new Robosapiens(lang);
            var x64 = true;
            var sapPath = x64 switch
            {
                true => @"C:\Program Files\SAP\FrontEnd\SAPGUI\saplogon.exe",
                false => @"C:\Program Files (x86)\SAP\FrontEnd\SAPGUI\saplogon.exe"
            };
            
            return new Dictionary<string, string>
            {
                ["Library"] = robosapiens.libraryImport(x64).ToString(),
                ["Test Setup"] = robosapiens.OpenSap(sapPath).ToString(),
                ["Test Teardown"] = robosapiens.CloseSap().ToString()
            };
        }

        protected RobotTestCase testCase(string lang, string testCaseName)
        {
            return new RobotTestCase(name: testCaseName, steps: testSteps(lang));
        }

        protected abstract List<KeywordCall> testSteps(string lang);
    }

    public static class RobotBuilder
    {
        public class Keyword: BaseRobotBuilder
        {
            List<RecordedKeyword> keywords_ = [];

            public void addKeyword(RecordedKeyword keyword)
            {
                keywords_.Add(keyword);
            }

            protected override List<RobotKeyword> keywords(string lang)
            {
                return [.. keywords_.Select(k => k.toRobotKeyword(lang))];
            }

            protected override List<KeywordCall> testSteps(string lang)
            {
                return [.. keywords_.Select(k => new KeywordCall(k.name, []))];
            }
        }

        public class RoboSAPiens: BaseRobotBuilder
        {
            List<KeyGuiEvent> keyGuiEventLog = [];

            public RoboSAPiens(List<KeyGuiEvent> keyGuiEventLog)
            {
                this.keyGuiEventLog = keyGuiEventLog;
            }

            protected override List<RobotKeyword> keywords(string lang)
            {
                return [];
            }
            
            protected override List<KeywordCall> testSteps(string lang)
            {
                return [.. keyGuiEventLog.Select(e => e.toKeywordCall(lang))];
            }
        }
    }
}
