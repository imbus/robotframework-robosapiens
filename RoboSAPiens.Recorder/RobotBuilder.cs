namespace RoboSAPiens.Recorder
{
    public interface IRobotBuilder
    {
        RobotFile build(string lang, string testCaseName);
    }

    public record WindowCommenter(Dictionary<long, Window> windows)
    {
        string currentWindowTitle = "";

        bool similar(string title1, string title2)
        {
            var words1 = title1.Split(" ").ToHashSet();
            var words2 = title2.Split(" ").ToHashSet();
            var commonWords = words1.Intersect(words2);

            return words1.Except(commonWords).Union(words2.Except(commonWords)).Count() <= 1;
        }

        public KeywordCall addWindowComment(KeywordCall step, long timestamp)
        {
            var window = windows[timestamp];

            if (!similar(currentWindowTitle, window.title))
            {
                currentWindowTitle = window.title;
                return step with {comment=$"Window: {window.title}"};
            }
            
            return step;
        }
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
            List<RecordedKeyword> recordedKeywords = [];
            Dictionary<long, Window> windows = [];

            public void addKeyword(RecordedKeyword keyword)
            {
                recordedKeywords.Add(keyword);
            }

            public void addWindows(Dictionary<long, Window> windows)
            {
                windows.ToList().ForEach(kv => this.windows[kv.Key] = kv.Value);
            }

            protected override List<RobotKeyword> keywords(string lang)
            {
                return [.. recordedKeywords.Select(k => k.toRobotKeyword(lang, new WindowCommenter(windows)))];
            }

            protected override List<KeywordCall> testSteps(string lang)
            {
                return [.. recordedKeywords.Select(k => new KeywordCall(k.name, []))];
            }
        }

        public class RoboSAPiens: BaseRobotBuilder
        {
            List<KeyGuiEvent> keyGuiEventLog = [];
            Dictionary<long, Window> windows = [];

            public RoboSAPiens(List<KeyGuiEvent> keyGuiEventLog, Dictionary<long, Window> windows)
            {
                this.keyGuiEventLog = keyGuiEventLog;
                this.windows = windows;
            }

            protected override List<RobotKeyword> keywords(string lang)
            {
                return [];
            }
            
            protected override List<KeywordCall> testSteps(string lang)
            {
                var windowCommenter = new WindowCommenter(windows);
                var connectEvent = keyGuiEventLog.First();
                return [
                    connectEvent.toKeywordCall(lang), 
                    .. keyGuiEventLog.Skip(1).Select(e => windowCommenter.addWindowComment(e.toKeywordCall(lang), e.window))
                ];
            }
        }
    }
}
