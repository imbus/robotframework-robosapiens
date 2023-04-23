using System;
using Horizon.XmlRpc.Core;
using System.Linq;
using System.Collections.Generic;

namespace RoboSAPiens {
    public record RobotResult(
        string status = "PASS",
        string output = "",
        string returnValue = "",
        string error = "",
        string stacktrace = "",
        bool continuable = false,
        bool fatal = false
    ) {
        public const string PASS = "PASS";
        public const string FAIL = "FAIL";

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
        
        public record RobotPass: RobotResult {
            public RobotPass(string output, string returnValue=""): base(
                status: "PASS", 
                output: "*INFO* " + "Pass|" + output, 
                returnValue: returnValue, 
                error: "", 
                stacktrace: "", 
                continuable: false, 
                fatal: false
            ) {}
        }

        public record RobotFail: RobotResult {
            public RobotFail(string failure, string error, string output = "", bool fatal=false, string stacktrace=""): base(
                status: "FAIL", 
                output: output, 
                returnValue: "", 
                error: failure + "|" + error, 
                stacktrace: stacktrace, 
                continuable: false, 
                fatal: fatal
            ) {}
        }

        const string errorDEBUG = "Für mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen.";
        public record RobotException(string name, System.Exception e, string errorMessage): RobotFail(name, output: $"*ERROR* {errorMessage}\n{e.Message}\n{errorDEBUG}", error: e.Message, stacktrace: e.StackTrace ?? "");

        public record ExceptionError(System.Exception e, string errorMessage): RobotException("Exception", e, errorMessage);
        public record NoConnection(): RobotFail("NoConnection", "Es besteht keine Verbindung zu einem SAP Server. Versuche zuerst das Keyword 'Verbindung zum Server Herstellen' aufzurufen.");
        public record NoGuiScripting(): RobotFail("NoGuiScripting", "Die Skriptunterstützung ist nicht verfügbar. Sie muss in den Einstellungen von SAP Logon aktiviert werden.");
        public record NoServerScripting(): RobotFail("NoServerScripting", "Das Scripting ist auf dem SAP Server nicht freigeschaltet. Siehe die Dokumentation von RoboSAPiens.");
        public record NoSapGui(): RobotFail("NoSapGui", "Keine laufende SAP GUI gefunden. Das Keyword 'SAP starten' muss zuerst aufgerufen werden.");
        public record NoSession(): RobotFail("NoSession", "Keine SAP-Session vorhanden. Versuche zuerst das Keyword 'Verbindung zum Server Herstellen' aufzurufen.");
        public record NotFound(string error): RobotFail("NotFound", error + "\nHinweis: Prüfe die Rechtschreibung");
        public record SapError(string message): RobotFail("SapError", error: message);
        public record UIScanFail(System.Exception e): RobotException("UIScanFail", e, "Scanning the GUI elements failed.");
        public record HighlightFail(System.Exception e): RobotException("HighlightFail", e, "The element could not be highlighted");
    }

    public record Result {
        public record ActivateTab {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string theTab): RobotResult.NotFound($"Der Reiter {theTab} konnte nicht gefunden werden.");
            public record SapError(string message): RobotResult.SapError(message);
            public record Pass(string theTab): RobotResult.RobotPass($"Der Reiter {theTab} wurde ausgewählt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Der Reiter konnte nicht ausgewählt werden.");
        }

        public record OpenSAP {
            public record Pass(): RobotResult.RobotPass("Die SAP GUI wurde gestartet");
            public record SAPNotStarted(string path=""): RobotResult.RobotFail("SAPNotStarted", error: $"Die SAP GUI konnte nicht gestartet werden. Überprüfe den Pfad {path}.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die SAP GUI konnte nicht gestartet werden.");
        }

        public record CloseConnection {
            public record NoSapGui(): RobotResult.NoSapGui();
            public record NoGuiScripting(): RobotResult.NoGuiScripting();
            public record NoConnection(): RobotResult.NoConnection();
            public record NoSession(): RobotResult.NoSession();
            public record Pass(string server): RobotResult.RobotPass($"Die Verbindung zum Server {server} wurde getrennt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Verbindung zum Server konnte nicht getrennt werden.");
        }

        public record CloseSAP {
            public record NoSapGui(): RobotResult.NoSapGui();
            public record Pass(): RobotResult.RobotPass("Die SAP GUI wurde beendet");
        }

        public record ExportTree {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(): RobotResult.NotFound("Die Maske enthält keine Baumstruktur");
            public record Pass(string jsonFile): RobotResult.RobotPass($"Die Baumstruktur wurde in JSON Format in der Datei '{jsonFile}' gespeichert");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Baumstruktur konnte nicht exportiert werden.");
        }

        public record AttachToRunningSAP {
            public record NoSapGui(): RobotResult.NoSapGui();
            public record NoGuiScripting(): RobotResult.NoGuiScripting();
            public record NoConnection(): RobotResult.NoConnection();
            public record NoServerScripting(): RobotResult.NoServerScripting();
            public record Pass(): RobotResult.RobotPass("Die laufende SAP GUI wurde erfolgreich übernommen.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die laufende SAP GUI konnte nicht übernommen werden.");
        }

        public record ConnectToServer {
            public record NoSapGui(): RobotResult.NoSapGui();
            public record NoGuiScripting(): RobotResult.NoGuiScripting();
            public record Pass(string server): RobotResult.RobotPass($"Die Verbindung mit dem Server '{server}' wurde erfolgreich hergestellt.");
            public record SapError(string message): RobotResult.SapError(message);
            public record NoServerScripting(): RobotResult.NoServerScripting();
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Verbindung konnte nicht hergestellt werden.");
        }

        public record DoubleClickCell {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Die Zelle mit dem Lokator '{locator}' konnte nicht gefunden werden.");
            public record Pass(string locator): RobotResult.RobotPass($"Die Zelle mit dem Lokator '{locator}' wurde doppelgeklickt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Zelle konnte nicht doppelgeklickt werden.");
        }

        public record DoubleClickTextField {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Das Textfeld mit dem Lokator '{locator}' konnte nicht gefunden werden.");
            public record Pass(string locator): RobotResult.RobotPass($"Das Textfeld mit dem Lokator '{locator}' wurde doppelgeklickt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Das Textfeld konnte nicht doppelgeklickt werden.");
        }

        public record ExecuteTransaction {
            public record NoSession(): RobotResult.NoSession();
            public record Pass(string tCode): RobotResult.RobotPass($"Die Transaktion mit T-Code '{tCode}' wurde erfolgreich ausgeführt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Transaktion konnte nicht ausgeführt werden.");
        }

        public record ExportForm {
            public record NoSession(): RobotResult.NoSession();
            public record Pass(string csvPath, string pngPath): RobotResult.RobotPass($"Die Maske wurde in den Dateien {csvPath} und {pngPath} gespeichert");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Maske konnte nicht exportiert werden.");
        }

        public record FillTableCell {
            public record NoSession(): RobotResult.NoSession();
            public record InvalidFormat(): RobotResult.RobotFail("InvalidFormat", "Das zweite Argument muss dem Muster `Spalte = Inhalt` entsprechen");
            public record NotFound(string locator): RobotResult.NotFound($"Die Zelle mit dem Lokator '{locator}' konnte nicht gefunden werden.");
            public record Pass(string locator): RobotResult.RobotPass($"Die Zelle mit dem Lokator '{locator}' wurde ausgefüllt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Zelle konnte nicht ausgefüllt werden.");
        }

        public record FillTextField {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Das Textfeld mit dem Lokator '{locator}' konnte nicht gefunden werden.");
            public record Pass(string locator): RobotResult.RobotPass($"Das Textfeld mit dem Lokator '{locator}' wurde ausgefüllt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Das Textfeld konnte nicht ausgefüllt werden. Möglicherweise, weil der Inhalt nicht dazu passt.");
        }

        public record PushButton {
            public record NoSession(): RobotResult.NoSession();
            public record SapError(string message): RobotResult.SapError(message);
            public record NotFound(string locator): RobotResult.NotFound($"Der Knopf mit dem Lokator '{locator}' konnte nicht gefunden werden.");
            public record Pass(string locator): RobotResult.RobotPass($"Der Knopf mit dem Lokator '{locator}' wurde gedrückt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Der Knopf konnte nicht gedrückt werden.");
        }

        public record PushButtonCell {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Die Zelle mit dem Lokator '{locator}' konnte nicht gefunden werden.");
            public record Pass(string locator): RobotResult.RobotPass($"Die Zelle mit dem Lokator '{locator}' wurde gedrückt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Zelle konnte nicht gedrückt werden.");
        }

        public record ReadTextField {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Das Textfeld mit dem Lokator '{locator}' konnte nicht gefunden werden.");
            public record Pass(string text, string locator): RobotResult.RobotPass($"Das Textfeld mit dem Lokator '{locator}' wurde ausgelesen.", returnValue: text);
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Das Textfeld konnte nicht ausgelesen werden.");
        }

        public record ReadText {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string text): RobotResult.NotFound($"Der Text mit dem Lokator '{text}' konnte nicht gefunden werden.");
            public record Pass(string text): RobotResult.RobotPass($"Der Text wurde ausgelesen.", returnValue: text);
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Der Text konnte nicht ausgelesen werden.");
        }

        public record ReadTableCell {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Die Zelle mit dem Lokator '{locator}' konnte nicht gefunden werden.");
            public record Pass(string text, string locator): RobotResult.RobotPass($"Die Zelle mit dem Lokator '{locator}' wurde ausgelesen.", returnValue: text);
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Zelle konnte nicht ausgelesen werden.");
        }

        public record SaveScreenshot {
            public record NoSession(): RobotResult.NoSession();
            public record UNCPath(): RobotResult.RobotFail("UNCPath", "Ein UNC Pfad ist nicht erlaubt");
            public record NoAbsPath(string path): RobotResult.RobotFail("NoAbsPath", $"{path} ist kein absoluter Pfad");
            public record Pass(string path): RobotResult.RobotPass($"Eine Aufnahme des Fensters wurde in '{path}' gespeichert.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Eine Aufnahme des Fensters konnte nicht gespeichert werden");
        }

        public record SelectCell {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Die Zelle mit dem Lokator '{locator}' konnte nicht gefunden werden.");
            public record Pass(string locator): RobotResult.RobotPass($"Die Zelle mit dem Lokator '{locator}' wurde markiert.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Zelle konnte nicht markiert werden.");

        }

        public record SelectComboBoxEntry {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Das Auswahlmenü mit dem Lokator '{locator}' konnte nicht gefunden werden.");
            public record EntryNotFound(string item): RobotResult.NotFound($"Der Eintrag '{item}' wurde im Auswahlmenü nicht gefunden.");
            public record Pass(string item): RobotResult.RobotPass($"Der Eintrag '{item}' wurde ausgewählt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Der Eintrag konnte nicht ausgewählt werden.");
        }

        public record SelectRadioButton {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Das Optionsfeld mit dem Lokator '{locator}' konnte nicht gefunden werden.");
            public record Pass(string locator): RobotResult.RobotPass($"Das Optionsfeld mit dem Lokator '{locator}' wurde ausgewählt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Das Optionsfeld konnte nicht ausgewählt werden.");
        }

        public record SelectTextField {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Das Textfeld mit dem Lokator '{locator}' konnte nicht gefunden werden.");
            public record Pass(string locator): RobotResult.RobotPass($"Das Textfeld mit dem Lokator '{locator}' wurde markiert.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Das Textfeld konnte nicht markiert werden.");
        }

        public record SelectTextLine {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string text): RobotResult.NotFound($"Die Textzeile mit dem Inhalt '{text}' konnte nicht gefunden werden.");
            public record Pass(string text): RobotResult.RobotPass($"Die Textzeile mit dem Inhalt '{text}' wurde markiert.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Textzeile konnte nicht markiert werden.");
        }

        public record TickCheckBox {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Das Formularfeld mit dem Lokator '{locator}' konnte nicht gefunden werden.");
            public record Pass(string locator): RobotResult.RobotPass($"Das Formularfeld mit dem Lokator '{locator}' wurde angekreuzt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Das Formularfeld konnte nicht angekreuzt werden.");
        }

        public record UntickCheckBox {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Das Formularfeld mit dem Lokator '{locator}' konnte nicht gefunden werden.");
            public record Pass(string locator): RobotResult.RobotPass($"Das Formularfeld mit dem Lokator '{locator}' wurde abgewählt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Das Formularfeld konnte nicht abgewählt werden.");
        }

        public record TickCheckBoxCell {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Die Zelle mit dem Lokator '{locator}' konnte nicht gefunden werden.");
            public record Pass(string locator): RobotResult.RobotPass($"Die Zelle mit dem Lokator '{locator}' wurde angekreuzt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Zelle konnte nicht angekreuzt werden.");
        }

        public record GetWindowTitle {
            public record NoSession(): RobotResult.NoSession();
            public record Pass(string title): RobotResult.RobotPass("Der Fenstertitel wurde ausgelesen", returnValue: title);
        }

        public record GetWindowText {
            public record NoSession(): RobotResult.NoSession();
            public record Pass(string text): RobotResult.RobotPass("Der Text des Fensters wurde ausgelesen", returnValue: text);
        }
    }

    public sealed record Success : RobotResult {
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

    public sealed record WrongWindow : RobotResult {
        public WrongWindow(string expectedTitle, string message) {
            this.output = $"*WARN* Die Überschrift der Maske ist nicht '{expectedTitle}'. {message}";
            this.returnValue = "FALSE";
            this.status = PASS;
        }
    }

    public record Error : RobotResult {
        public Error(): base() {
            this.status = FAIL;
        }
    }

    public record FatalError : Error {
        public FatalError(): base() {
            this.fatal = true;
        }
    }

    public sealed record ExceptionError : Error {
        const string errorDEBUG = "Für mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen.";

        public ExceptionError(Exception e, string errorMessage) {
            this.output = $"*ERROR* {errorMessage}\n{e.Message}\n{errorDEBUG}";
            this.error = e.Message;
            this.stacktrace = e.StackTrace ?? "";
        }
    }

    public sealed record InvalidArgumentError : Error {
        public InvalidArgumentError(string message) {
            this.error = message;
        }
    }

    public sealed record InvalidFileError : Error {
        public InvalidFileError(string message) {
            this.error = message;
        }
    }

    public sealed record InvalidFormatError : Error {
        public InvalidFormatError(string message) {
            this.error = message;
        }
    }

    public sealed record InvalidValueError : Error {
        public InvalidValueError(string message) {
            this.error = message;
        }
    }

    public sealed record ConnectionFailed : FatalError {
        public ConnectionFailed(Exception e, String message) {
            this.output = $"*ERROR* {message}";
            this.error = e.Message;
            this.stacktrace = e.StackTrace ?? "";
        }
    }

    public sealed record NoConnectionError : FatalError {
        public NoConnectionError() {
            this.error = "Es besteht keine Verbindung zu einem SAP Server. Versuche zuerst das Keyword 'Verbindung zum Server Herstellen' aufzurufen.";
        }
    }

    public sealed record SAPNotStartedError : FatalError {
        public SAPNotStartedError(string path) {
            this.error = $"Die SAP GUI konnte nicht gestartet werden. Überprüfe den Pfad '{path}'.";
        }

        public SAPNotStartedError() {
            this.error = $"Die SAP GUI konnte nicht gestartet werden.";
        }
    }


    public sealed record NoSapGuiError : Error {
        public NoSapGuiError() {
            this.error = "Keine laufende SAP GUI gefunden. Das Keyword 'SAP starten' muss zuerst aufgerufen werden.";
        }
    }

    public sealed record NoScriptingError : FatalError {
        public NoScriptingError() {
            this.error = "Das Scripting ist auf dem SAP Server nicht freigeschaltet. Siehe die Dokumentation von RoboSAPiens.";
        }
    }

    public sealed record NoSessionError : FatalError {
        public NoSessionError() {
            this.error = "Keine SAP-Session vorhanden. Versuche zuerst das Keyword 'Verbindung zum Server Herstellen' aufzurufen.";
        }
    }

    public sealed record SapError : Error {
        public SapError(string errorMessage) {
            this.error = $"SAP Fehlermeldung: {errorMessage}";
        }
    }

    public sealed record SpellingError : Error {
        public SpellingError(string message) {
            this.error = $"{message}\nHinweis: Prüfe die Rechtschreibung";
        }
    }
}
