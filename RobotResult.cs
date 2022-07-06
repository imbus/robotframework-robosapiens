using System;
using Horizon.XmlRpc.Core;
using System.Linq;
using System.Collections.Generic;

namespace RoboSAPiens {
    public class RobotResult {
        const string None = "";
        public const string PASS = "PASS";
        public const string FAIL = "FAIL";
        public string status = "";
        public string output = "";
        public string returnValue = None;
        public string error = "";
        public string stacktrace = "";

        public XmlRpcStruct asXmlRpcStruct() {
            var result = new XmlRpcStruct();
            result.Add("status", status);
            result.Add("output", output);
            result.Add("return", returnValue);
            result.Add("error", error);
            result.Add("traceback", stacktrace);
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

    public sealed class UnexpectedResult : RobotResult {
        public UnexpectedResult(string returnValue, string message) {
            this.output = $"*WARN* {message}";
            this.returnValue = returnValue;
            this.status = PASS;
        }
    }


    // Error comprises both technical and domain errors
    public class Error : RobotResult {}

    public sealed class ExceptionError : Error {
        const string errorDEBUG = "Für mehr Infos: robot --loglevel DEBUG datei.robot und dann in log.html schauen.";
        public ExceptionError(Exception e, string errorMessage) {
            this.output = $"*ERROR* {errorMessage}\n{errorDEBUG}";
            this.error = $"{e.Message}";
            this.stacktrace = $"{e.StackTrace}";
            this.status = FAIL;
        }
    }

    public sealed class InvalidArgumentError : Error {
        public InvalidArgumentError(string message) {
            this.output = $"*ERROR* {message}";
            this.status = FAIL;
        }
    }

    public sealed class InvalidFileError : Error {
        public InvalidFileError(string message) {
            this.output = $"*ERROR* {message}";
            this.status = FAIL;
        }
    }

    public sealed class InvalidFormatError : Error {
        public InvalidFormatError(string message) {
            this.output = $"*ERROR* {message}";
            this.status = FAIL;
        }
    }

    public sealed class InvalidValueError : Error {
        public InvalidValueError(string message) {
            this.output = $"*ERROR* {message}";
            this.status = FAIL;
        }
    }

    public sealed class NoConnectionError : Error {
        public NoConnectionError() {
            this.output = $"*ERROR* Keine Verbindung mit einem SAP Server vorhanden. Versuche zuerst das Keyword 'Verbinden mit dem SAP Server' auszuführen.";
            this.status = FAIL;
        }
    }

    public sealed class NonIdenticalFormsError : Error {
        public NonIdenticalFormsError(string message, List<string> differences) {
            this.error = message;
            this.output =
                String.Join(Environment.NewLine, 
                            differences.Select(difference => $"*ERROR* {difference}"));
            this.status = FAIL;
        }
    }

    public sealed class NoSapGuiError : Error {
        public NoSapGuiError() {
            this.output = $"*ERROR* Keine laufende SAP GUI gefunden. SAP Logon muss zuerst ausgeführt werden.";
            this.status = FAIL;
        }
    }

    public sealed class NoScriptingError : Error {
        public NoScriptingError() {
            this.output = $"*ERROR* Die Skriptunterstützung ist server-seitig nicht freigeschaltet.";
            this.status = FAIL;
        }
    }

    public sealed class NoSessionError : Error {
        public NoSessionError() {
            this.output = $"*ERROR* Keine SAP-Session vorhanden. Versuche zuerst das Keyword 'Verbinden mit dem SAP Server' auszuführen.";
            this.status = FAIL;
        }
    }

    public sealed class SapError : Error {
        public SapError(string errorMessage) {
            this.output = $"*ERROR* SAP Fehlermeldung: {errorMessage}";
            this.status = FAIL;
        }
    }

    public sealed class SpellingError : Error {
        const string errorWrongSpelling = "Prüfe die Rechtschreibung";
        public SpellingError(string message) {
            this.output = $"*ERROR* {message}\nHinweis: {errorWrongSpelling}";
            this.status = FAIL;
        }
    }
}
