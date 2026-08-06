namespace RoboSAPiens.Recorder
{
    public interface IRecordingMode;

    public static class RecordingMode
    {
        public record RoboSAPiens: IRecordingMode
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

    public record RecordedKeyword(string name, List<KeyGuiEvent> events)
    {
        public RobotKeyword toRobotKeyword(string language)
        {
            var steps = events.Select(e => e.toKeywordCall(language)).ToList();
            return new RobotKeyword(name, steps);
        }
    }

    public interface IRecorder
    {
        public IRecordingMode recordingMode { get; }
        public void save(string lang, string testCaseName);
        public void start();
        public void stop();
    }

    public abstract class BaseRecorder<B>: IRecorder where B: IRobotBuilder
    {
        protected bool debug;
        protected GuiRecorder recorder;
        public abstract IRecordingMode recordingMode { get; }
        protected B robotBuilder;

        public BaseRecorder(bool debug, B robotBuilder)
        {
            this.debug = debug;
            recorder = new GuiRecorder(debug);
            this.robotBuilder = robotBuilder;
        }

        public virtual void save(string lang, string testCaseName)
        {
            robotBuilder.build(lang, testCaseName).save();
        }

        public void start()
        {
            recorder.recordStart();
        }

        public void stop()
        {
            recorder.recordStop();
        }
    }

    public static class RobotRecorder
    {
        public class Keyword: BaseRecorder<RobotBuilder.Keyword>
        {
            public override IRecordingMode recordingMode => new RecordingMode.Keyword();

            public Keyword(bool debug): base(debug, new RobotBuilder.Keyword()) {}

            public void saveKeyword(string keywordName)
            {
                recorder.recordStop();
                var keyword = new RecordedKeyword(keywordName, recorder.getKeyGuiEvents());
                robotBuilder.addKeyword(keyword);
                Console.WriteLine($"Keyword '{keywordName}' saved.");
            }
        }

        public class RoboSAPiens: BaseRecorder<RobotBuilder.RoboSAPiens>
        {
            public override IRecordingMode recordingMode => new RecordingMode.RoboSAPiens();
            public RoboSAPiens(bool debug): base(debug, new RobotBuilder.RoboSAPiens()) {}

            public override void save(string lang, string testCaseName)
            {
                new RobotBuilder.RoboSAPiens(recorder.getKeyGuiEvents())
                .build(lang, testCaseName)
                .save();
                
                recorder.saveHtmlReport(testCaseName, lang);

                if (debug)
                {
                    var filename = testCaseName.toFileName();
                    recorder.saveEventLog(filename);
                    recorder.saveKeywordLog(filename, lang);
                    recorder.saveKeyGuiLog(filename);
                }
            }
        }
    }
}
