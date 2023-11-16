namespace RoboSAPiens {
    public class Status
    {
        public const string 
            FAIL = "FAIL",
            PASS = "PASS";
    }

    public record RobotResult(
        string status = Status.PASS,
        string output = "",
        string @return = "",
        string error = "",
        string traceback = "",
        bool fatal = false,
        bool continuable = false
    ) {
        public record RobotPass: RobotResult {
            public RobotPass(string output, string returnValue=""): base(
                status: Status.PASS, 
                output: output, 
                @return: returnValue, 
                error: "", 
                traceback: "", 
                continuable: false, 
                fatal: false
            ) {}
        }

        public record RobotFail: RobotResult {
            public RobotFail(string failure, string error, string output = "", bool fatal=false, string stacktrace=""): base(
                status: Status.FAIL, 
                output: output, 
                @return: "", 
                error: failure + "|" + error, 
                traceback: stacktrace, 
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
        public record EntryNotFound(string error): RobotFail("EntryNotFound", error + "\nHinweis: Prüfe die Rechtschreibung");
        public record NotFound(string error): RobotFail("NotFound", error + "\nHinweis: Prüfe die Rechtschreibung");
        public record NotChangeable(string error = "Die Komponente ist schreibgeschützt."): RobotFail("NotChangeable", error);
        public record SapError(string message): RobotFail("SapError", error: message);
        public record UIScanFail(System.Exception e): ExceptionError(e, "Scanning the GUI components failed.");
        public record HighlightFail(System.Exception e): ExceptionError(e, "The component could not be highlighted.");
    }

    public record Result {
        public record KeywordLibrary {
            public record KeywordNotFound(string kwName): RobotResult.RobotFail("KeywordNotFound", $"The keyword '{kwName}' was not found. Check the spelling.");
            public record Exception(string kwName, System.Exception e): RobotResult.ExceptionError(e, $"An error occurred when calling the keyword '{kwName}'");
        }

        public record RobotRemote {
            public record InvalidArgumentType(string arg, string rfType, string kwType): RobotResult.RobotFail("InvalidArgumentType", $"The argument '{arg}' has type '{rfType}'. It must have type '{kwType}'");
        }

        public record ActivateTab {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string theTab): RobotResult.NotFound($"Der Reiter {theTab} konnte nicht gefunden werden.");
            public record SapError(string message): RobotResult.SapError(message);
            public record Pass(string theTab): RobotResult.RobotPass($"Der Reiter {theTab} wurde ausgewählt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Der Reiter konnte nicht ausgewählt werden.");
        }

        public record OpenSap {
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

        public record CloseSap {
            public record NoSapGui(): RobotResult.NoSapGui();
            public record Pass(): RobotResult.RobotPass("Die SAP GUI wurde beendet");
        }

        public record ExportTree {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(): RobotResult.NotFound("Die Maske enthält keine Baumstruktur");
            public record Pass(string jsonFile): RobotResult.RobotPass($"Die Baumstruktur wurde in JSON Format in der Datei '{jsonFile}' gespeichert");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Baumstruktur konnte nicht exportiert werden.");
        }

        public record AttachToRunningSap {
            public record NoSapGui(): RobotResult.NoSapGui();
            public record NoGuiScripting(): RobotResult.NoGuiScripting();
            public record NoConnection(): RobotResult.NoConnection();
            public record NoServerScripting(): RobotResult.NoServerScripting();
            public record NoSession(): RobotResult.NoSession();
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

        public record ExportSpreadsheet {
            public record NoSession(): RobotResult.NoSession();
            public record Pass(): RobotResult.RobotPass("Die Export-Funktion Tabellenkalkulation wurde aufgerufen.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Export-Funktion Tabellenkalkulation konnte nicht aufgerufen werden.");
            public record NotFound(): RobotResult.NotFound("Keine Tabelle wurde gefunden, welche die Export-Funktion Tabellenkalkulation unterstützt.");
        }

        public record FillTableCell {
            public record NoSession(): RobotResult.NoSession();
            public record InvalidFormat(): RobotResult.RobotFail("InvalidFormat", "Das zweite Argument muss dem Muster `Spalte = Inhalt` entsprechen");
            public record NotFound(string locator): RobotResult.NotFound($"Die Zelle mit dem Lokator '{locator}' konnte nicht gefunden werden.");
            public record NotChangeable(string locator): RobotResult.NotChangeable($"Die Zelle mit dem Lokator '{locator}' ist schreibgeschützt.");
            public record Pass(string locator): RobotResult.RobotPass($"Die Zelle mit dem Lokator '{locator}' wurde ausgefüllt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Zelle konnte nicht ausgefüllt werden.");
        }

        public record FillTextField {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Das Textfeld mit dem Lokator '{locator}' konnte nicht gefunden werden.");
            public record Pass(string locator): RobotResult.RobotPass($"Das Textfeld mit dem Lokator '{locator}' wurde ausgefüllt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Das Textfeld konnte nicht ausgefüllt werden. Möglicherweise, weil der Inhalt nicht dazu passt.");
        }

        public record HighlightButton {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Der Knopf mit dem Lokator '{locator}' konnte nicht gefunden werden.");
            public record Pass(string locator): RobotResult.RobotPass($"Der Knopf mit dem Lokator '{locator}' wurde hervorgehoben.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Der Knopf konnte nicht hervorgehoben werden.");
        }

        public record PressKeyCombination {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string keyCombination): RobotResult.NotFound($"Die Tastenkombination '{keyCombination}' ist nicht vorhanden.");
            public record Pass(string keyCombination): RobotResult.RobotPass($"Die Tastenkombination '{keyCombination}' wurde gedrückt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Tastenkombination konnte nicht gedrückt werden.");
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

        public record ReadStatusbar {
            public record NoSession(): RobotResult.NoSession();
            public record Pass(string message): RobotResult.RobotPass("Die Statusleiste wurde ausgelesen.", returnValue: message);
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Statusleiste konnte nicht ausgelesen werden.");
            public record NotFound(): RobotResult.NotFound("Die Maske enthält keine Stastusleiste");
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
            public record InvalidPath(string path): RobotResult.RobotFail("InvalidPath", $"{path} ist kein gültiger Pfad");
            public record Log(string log_message): RobotResult.RobotPass("Eine Aufnahme des Fensters wurde B64-kodiert", returnValue: log_message);
            public record Pass(string path): RobotResult.RobotPass($"Eine Aufnahme des Fensters wurde in '{path}' gespeichert.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Eine Aufnahme des Fensters konnte nicht gespeichert werden");
        }

        public record SelectCell {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Die Zelle mit dem Lokator '{locator}' konnte nicht gefunden werden.");
            public record Pass(string locator): RobotResult.RobotPass($"Die Zelle mit dem Lokator '{locator}' wurde markiert.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Zelle konnte nicht markiert werden.");

        }

        public record SelectCellValue {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Die Zelle mit dem Lokator '{locator}' konnte nicht gefunden werden.");
            public record EntryNotFound(string value): RobotResult.EntryNotFound($"Der Wert '{value}' ist in der Zelle nicht vorhanden.");
            public record Pass(string value, string locator): RobotResult.RobotPass($"Der Wert '{value}' wurde in der Zelle mit dem Lokator '{locator}' ausgewählt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Der Wert konnte nicht ausgewählt werden.");

        }

        public record SelectComboBoxEntry {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Das Auswahlmenü mit dem Lokator '{locator}' konnte nicht gefunden werden.");
            public record EntryNotFound(string item): RobotResult.EntryNotFound($"Der Eintrag '{item}' wurde im Auswahlmenü nicht gefunden.");
            public record Pass(string item): RobotResult.RobotPass($"Der Eintrag '{item}' wurde ausgewählt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Der Eintrag konnte nicht ausgewählt werden.");
        }

        public record SelectRadioButton {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Das Optionsfeld mit dem Lokator '{locator}' konnte nicht gefunden werden.");
            public record Pass(string locator): RobotResult.RobotPass($"Das Optionsfeld mit dem Lokator '{locator}' wurde ausgewählt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Das Optionsfeld konnte nicht ausgewählt werden.");
        }

        public record SelectTableRow {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(): RobotResult.NotFound("Die Maske enthält keine Tabelle");
            public record Pass(int rowIndex): RobotResult.RobotPass($"Die Zeile mit dem Index '{rowIndex}' wurde markiert");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Zeile konnte nicht markiert werden.");
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
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Zelle konnte nicht angekreuzt werden.");
        }

        public record GetWindowText {
            public record NoSession(): RobotResult.NoSession();
            public record Pass(string text): RobotResult.RobotPass("Der Text des Fensters wurde ausgelesen", returnValue: text);
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Zelle konnte nicht angekreuzt werden.");
        }
    }
}
