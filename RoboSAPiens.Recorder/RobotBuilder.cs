namespace RoboSAPiens.Recorder
{
    public record KeywordBuilder(string name, List<KeyGuiEvent> events, Dictionary<long, string[]> keywordCalls)
    {
        public RobotKeyword build(string lang, Dictionary<long, Window> windows)
        {
            var keywordCallMerger = new KeywordCallMerger(lang, windows);
            var steps = keywordCallMerger.merge([.. events], keywordCalls);
            return new RobotKeyword(name, steps);
        }
    }

    public record KeywordCallMerger(string lang, Dictionary<long, Window> windows)
    {
        public List<KeywordCall> merge(List<KeyGuiEvent> events, Dictionary<long, string[]> keywordCalls)
        {
            var robosapiens = new Robosapiens(lang);
            var steps = new SortedList<long, KeywordCall>();
            var windowCommenter = new WindowCommenter(windows);

            foreach (var e in events)
            {
                steps[e.window] = windowCommenter.addWindowComment(e.toKeywordCall(lang), e.window);
            }

            foreach (var (timestamp, keywordCallArr) in keywordCalls)
            {
                (string returnValue, string keyword, var args) = keywordCallArr;
                var keywordCall = robosapiens.callKeyword(returnValue, keyword, [.. args]);

                if (keywordCall != null)
                {
                    steps[timestamp] = keywordCall;
                }
            }

            return [.. steps.Values];
        }
    }

    public static class RobotBuilder
    {
        public interface IRobotBuilder
        {
            RobotFile build();
        }

        public abstract record BaseRobotBuilder(string lang, string testCaseName): IRobotBuilder
        {
            protected Dictionary<long, Window> windows = [];

            public void addWindows(Dictionary<long, Window> windows)
            {
                windows.ToList().ForEach(kv => this.windows[kv.Key] = kv.Value);
            }

            public RobotFile build()
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

        public record Keyword(string lang, string testCaseName): BaseRobotBuilder(lang, testCaseName)
        {
            List<KeywordBuilder> keywordBuilders = [];

            public void addKeywordBuilder(KeywordBuilder keyword)
            {
                keywordBuilders.Add(keyword);
            }

            protected override List<RobotKeyword> keywords(string lang)
            {
                return [.. keywordBuilders.Select(kb => kb.build(lang, windows))];
            }

            protected override List<KeywordCall> testSteps(string lang)
            {
                return [.. keywordBuilders.Select(kb => new KeywordCall(kb.name, []))];
            }
        }

        public record RoboSAPiens(string lang, string testCaseName, List<KeyGuiEvent> keyGuiEventLog, Dictionary<long, string[]> keywordCalls): BaseRobotBuilder(lang, testCaseName)
        {
            protected override List<RobotKeyword> keywords(string lang)
            {
                return [];
            }
            
            protected override List<KeywordCall> testSteps(string lang)
            {
                var keywordCallMerger = new KeywordCallMerger(lang, windows);
                var (connectEvent, events) = keyGuiEventLog;
                return [connectEvent.toKeywordCall(lang), .. keywordCallMerger.merge([.. events], keywordCalls)];
            }
        }
    }

    public record WindowCommenter(Dictionary<long, Window> windows)
    {
        string currentWindowId = "";
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

            if (currentWindowId != window.id)
            {
                currentWindowId = window.id;
                currentWindowTitle = window.title;
                return step with {comment=$"Window: {window.title}"};
            }

            if (!similar(currentWindowTitle, window.title))
            {
                currentWindowTitle = window.title;
                return step with {comment=$"Window: {window.title}"};
            }
            
            return step;
        }
    }
}