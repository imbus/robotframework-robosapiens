namespace RoboSAPiens.Recorder
{
    public interface IRecordingMode;

    public static class RecordingMode
    {
        public record Robosapiens: IRecordingMode
        {
            public override string ToString()
            {
                return "RoboSAPiens recording mode: the Test Case consists of RoboSAPiens keywords.";
            }
        }
        
        public record Keyword: IRecordingMode
        {
            public override string ToString()
            {
                return "Keyword recording mode: the Test Case consists of the recorded keywords.";
            }
        }
    }

    public static class Recorder
    {
        static class Api
        {
            public interface Initialized
            {
                public IRecordingMode recordingMode { get; }
                public State.Recording record(string lang, string testCaseName);
            }

            public interface Recording
            {
                public void addKeywordCall(long timestamp, string[] keywordCall);
                public void save();
                public void start();
                public void stop();
            }
        }

        public static class State
        {
            public abstract record Initialized(bool debug): Api.Initialized
            {
                public abstract IRecordingMode recordingMode { get; }

                public abstract Recording record(string lang, string testCaseName);
            }

            public abstract record Recording(bool debug): Api.Recording
            {
                protected GuiRecorder recorder = new(debug);

                protected Dictionary<long, string[]> keywordCalls = [];

                public void addKeywordCall(long timestamp, string[] keywordCall)
                {
                    keywordCalls[timestamp] = keywordCall;
                }

                public abstract void save();

                public virtual void start()
                {
                    recorder.recordStart();
                }

                public void stop()
                {
                    recorder.recordStop();
                }
            }
        }
    
        public static class Keyword
        {
            public record Initialized(bool debug): State.Initialized(debug)
            {
                public override IRecordingMode recordingMode => new RecordingMode.Keyword();

                public override State.Recording record(string lang, string testCaseName)
                {
                    return new Recording(debug, lang, testCaseName);
                }
            }

            public record Recording(bool debug, string lang, string testCaseName): State.Recording(debug)
            {
                RobotBuilder.Keyword robotBuilder = new(lang, testCaseName);

                public bool hasEvents()
                {
                    return recorder.getKeyGuiEvents().Count > 0;
                }

                public override void save()
                {
                    robotBuilder.build().save();
                }

                public void saveKeyword(string keywordName)
                {
                    var keywordBuilder = new KeywordBuilder(keywordName, recorder.getKeyGuiEvents(), keywordCalls);
                    robotBuilder.addKeywordBuilder(keywordBuilder);
                    robotBuilder.addWindows(recorder.getWindows());
                }

                public override void start()
                {
                    recorder = new GuiRecorder(debug);
                    recorder.recordStart();
                }
            }
        }

        public static class Robosapiens
        {
            public record Initialized(bool debug): State.Initialized(debug)
            {
                public override IRecordingMode recordingMode => new RecordingMode.Robosapiens();

                public override State.Recording record(string lang, string testCaseName)
                {
                    return new Recording(debug, lang, testCaseName);
                }
            }

            public record Recording(bool debug, string lang, string testCaseName): State.Recording(debug)
            {
                public override void save()
                {
                    var robosapiensBuilder = new RobotBuilder.RoboSAPiens(lang, testCaseName, recorder.getKeyGuiEvents(), keywordCalls);
                    robosapiensBuilder.addWindows(recorder.getWindows());
                    robosapiensBuilder.build().save();
                    recorder.saveHtmlReport(testCaseName, lang);

                    if (debug)
                    {
                        var filename = testCaseName.toFileName();
                        recorder.saveEventLog(filename);
                        recorder.saveKeywordLog(filename, lang);
                        recorder.saveKeyGuiLog(filename);
                    }
                }

                public override void start()
                {
                    recorder.recordStart();
                    recorder.addConnectEvent();
                }
            }
        }
    }
}