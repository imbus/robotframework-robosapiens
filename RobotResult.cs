using System;
using Horizon.XmlRpc.Core;
using System.Linq;
using System.Collections.Generic;

namespace RoboSAPiens {
    public class RobotResult {
        const string None = "";
        public const string PASS = "PASS";
        public const string FAIL = "FAIL";
        public string status = PASS;
        public string output = None;
        public string returnValue = None;
        public string error = None;
        public string stacktrace = None;
        public bool continuable = false;
        public bool fatal = false;

        public XmlRpcStruct asXmlRpcStruct() {
            var result = new XmlRpcStruct();
            result.Add("status", status);
            result.Add("output", output);
            result.Add("return", returnValue);
            result.Add("error", error);
            result.Add("traceback", stacktrace);
            result.Add("continuable", continuable);
            result.Add("fatal", fatal);
            return result;
        }
    }

    public sealed class Success : RobotResult {
        public Success(string returnValue, string message) {
            this.returnValue = returnValue;
            this.output = $"*INFO* {message}";
            this.status = PASS;
        }

        public Success(string message, List<string> positiveTests) {
            this.output =
                String.Join(Environment.NewLine, 
                            positiveTests.Select(test => $"*INFO* {test}"));
            this.status = PASS;
        }

        public Success(string message) {
            this.output = $"*INFO* {message}";
            this.status = PASS;
        }
    }

    public sealed class WrongWindow : RobotResult {
        public WrongWindow(string expectedTitle, string message) {
            this.output = $"*WARN* Die Überschrift der Maske ist nicht '{expectedTitle}'. {message}";
            this.returnValue = "FALSE";
            this.status = PASS;
        }
    }

    public class Error : RobotResult {
        public Error(): base() {
            this.status = FAIL;
        }
    }

    public class FatalError : Error {
        public FatalError(): base() {
            this.fatal = true;
        }
    }

    public sealed class ExceptionError : Error {
        const string errorDEBUG = "Für mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen.";

        public ExceptionError(Exception e, string errorMessage) {
            this.output = $"*ERROR* {errorMessage}\n{e.Message}\n{errorDEBUG}";
            this.error = e.Message;
            this.stacktrace = e.StackTrace ?? "";
        }
    }

    public sealed class InvalidArgumentError : Error {
        public InvalidArgumentError(string message) {
            this.error = message;
        }
    }

    public sealed class InvalidFileError : Error {
        public InvalidFileError(string message) {
            this.error = message;
        }
    }

    public sealed class InvalidFormatError : Error {
        public InvalidFormatError(string message) {
            this.error = message;
        }
    }

    public sealed class InvalidValueError : Error {
        public InvalidValueError(string message) {
            this.error = message;
        }
    }

    public sealed class ConnectionFailed : FatalError {
        public ConnectionFailed(Exception e, String message) {
            this.output = $"*ERROR* {message}";
            this.error = e.Message;
            this.stacktrace = e.StackTrace ?? "";
        }
    }

    public sealed class NoConnectionError : FatalError {
        public NoConnectionError() {
            this.error = "Es besteht keine Verbindung zu einem SAP Server.";
        }
    }

    public sealed class NoSapGuiError : Error {
        public NoSapGuiError() {
            this.error = $"*ERROR* Keine laufende SAP GUI gefunden. SAP Logon muss zuerst ausgeführt werden.";
        }
    }

    public sealed class SapGuiAlreadyOpen : FatalError {
        public SapGuiAlreadyOpen() {
            this.error = "Die SAP GUI ist bereits geöffnet. Die Anwendung 'SAP Logon' muss beendet werden.";
        }
    }

    public sealed class NoScriptingError : FatalError {
        public NoScriptingError() {
            this.error = $"*ERROR* Die Skriptunterstützung ist server-seitig nicht freigeschaltet.";
        }
    }

    public sealed class NoSessionError : FatalError {
        public NoSessionError() {
            this.error = "Keine SAP-Session vorhanden. Versuche zuerst das Keyword 'Verbinden mit dem SAP Server' auszuführen.";
        }
    }

    public sealed class SapError : Error {
        public SapError(string errorMessage) {
            this.error = $"SAP Fehlermeldung: {errorMessage}";
        }
    }

    public sealed class SpellingError : Error {
        public SpellingError(string message) {
            this.error = $"*ERROR* {message}\nHinweis: Prüfe die Rechtschreibung";
        }
    }
}
