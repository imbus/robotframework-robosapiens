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
        object? @return = null,
        string error = "",
        string traceback = "",
        bool fatal = false,
        bool continuable = false
    ) {
        public record RobotPass: RobotResult {
            public RobotPass(string output, object? returnValue=null): base(
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
        public record RobotException(string name, System.Exception e, string errorMessage): RobotFail(name, output: $"*ERROR* {errorMessage}\n{e.Message}\n{errorDEBUG}", error: e.Message, stacktrace: e.ToString() ?? "");

        public record ExceptionError(System.Exception e, string errorMessage): RobotException("Exception", e, errorMessage);
        public record InvalidSession(int sessionId): RobotFail("InvalidSession", $"Die aktuelle Verbindung hat keine Session {sessionId}.");
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
            public record NotFound(string theTab): RobotResult.NotFound($"Der Reiter {theTab} wurde nicht gefunden.");
            public record Pass(string theTab): RobotResult.RobotPass($"Der Reiter {theTab} wurde ausgewählt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Der Reiter konnte nicht ausgewählt werden.");
        }

        public record OpenSap {
            public record Pass(): RobotResult.RobotPass("Die SAP GUI wurde gestartet");
            public record SAPNotStarted(string path): RobotResult.RobotFail("SAPNotStarted", error: $"Die SAP GUI konnte nicht gestartet werden. Überprüfe den Pfad {path}.");
            public record NoGuiScripting(): RobotResult.NoGuiScripting();
            public record SAPAlreadyRunning(): RobotResult.RobotFail("SAPAlreadyRunning", error: "Die SAP GUI wird gerade ausgeführt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die SAP GUI konnte nicht gestartet werden.");
        }

        public record CloseConnection {
            public record NoSession(): RobotResult.NoSession();
            public record Pass(string server): RobotResult.RobotPass($"Die Verbindung zum Server {server} wurde getrennt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Verbindung zum Server konnte nicht getrennt werden.");
        }

        public record CloseSap {
            public record NoSapGui(): RobotResult.NoSapGui();
            public record Pass(): RobotResult.RobotPass("Die SAP GUI wurde beendet");
        }

        public record CloseWindow {
            public record NoSession(): RobotResult.NoSession();
            public record Pass(): RobotResult.RobotPass("Das Fenster im Vordergrund wurde geschlossen.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Das Fenster konnte nicht geschlossen werden.");
        }

        public record ExportTree {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(): RobotResult.NotFound("Die Maske enthält keine Baumstruktur");
            public record Pass(string jsonFile): RobotResult.RobotPass($"Die Baumstruktur wurde in JSON Format in der Datei '{jsonFile}' gespeichert");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Baumstruktur konnte nicht exportiert werden.");
        }

        public record ConnectToRunningSap {
            public record NoSapGui(): RobotResult.NoSapGui();
            public record NoGuiScripting(): RobotResult.NoGuiScripting();
            public record NoConnection(): RobotResult.NoConnection();
            public record NoServerScripting(): RobotResult.NoServerScripting();
            public record NoSession(): RobotResult.NoSession();
            public record InvalidSession(int sessionId): RobotResult.InvalidSession(sessionId);
            public record SapError(string message): RobotResult.SapError(message);
            public record InvalidConnection(string name): RobotResult.RobotFail("InvalidConnection", $"Es gibt keine Verbindung mit dem Namen '{name}'.");
            public record InvalidClient(string client): RobotResult.RobotFail("InvalidClient", $"Es besteht keine Verbindung beim Mandanten '{client}'.");
            public record InvalidConnectionClient(string connection, string client): RobotResult.RobotFail("InvalidConnectionClient", $"Es besteht keine Verbindung '{connection}' beim Mandanten '{client}'.");
            public record Json(string json): RobotResult.RobotPass("Die laufende SAP GUI wurde erfolgreich übernommen.", returnValue: json);
            public record Pass(): RobotResult.RobotPass("Die laufende SAP GUI wurde erfolgreich übernommen.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die laufende SAP GUI konnte nicht übernommen werden.");
        }

        public record ConnectToServer {
            public record NoSapGui(): RobotResult.NoSapGui();
            public record NoGuiScripting(): RobotResult.NoGuiScripting();
            public record InvalidSession(int sessionId): RobotResult.InvalidSession(sessionId);
            public record Json(string json): RobotResult.RobotPass($"Die Verbindung mit dem Server wurde erfolgreich hergestellt.", returnValue: json);
            public record Pass(string server): RobotResult.RobotPass($"Die Verbindung mit dem Server '{server}' wurde erfolgreich hergestellt.");
            public record SapError(string message): RobotResult.SapError(message);
            public record NoServerScripting(): RobotResult.NoServerScripting();
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Verbindung konnte nicht hergestellt werden.");
        }

        public record CountTableRows {
            public record InvalidTable(int tableNumber): RobotResult.RobotFail("InvalidTable", $"Die Maske enthält keine Tabelle mit Index {tableNumber}'.");
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(): RobotResult.NotFound("Die Maske enthält keine Tabelle.");
            public record Pass(int rowCount): RobotResult.RobotPass("Die Tabellenzeilen wurden gezählt", returnValue: rowCount);
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Zeilen konnten nicht gezählt werden.");
        }

        public record DoubleClickCell {
            public record InvalidTable(int tableNumber): RobotResult.RobotFail("InvalidTable", $"Die Maske enthält keine Tabelle mit Index {tableNumber}'.");
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Die Zelle mit dem Lokator '{locator}' wurde nicht gefunden.");
            public record Pass(string locator): RobotResult.RobotPass($"Die Zelle mit dem Lokator '{locator}' wurde doppelgeklickt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Zelle konnte nicht doppelgeklickt werden.");
        }

        public record DoubleClickTextField {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Das Textfeld mit dem Lokator '{locator}' wurde nicht gefunden.");
            public record Pass(string locator): RobotResult.RobotPass($"Das Textfeld mit dem Lokator '{locator}' wurde doppelgeklickt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Das Textfeld konnte nicht doppelgeklickt werden.");
        }

        public record ExecuteTransaction {
            public record NoSession(): RobotResult.NoSession();
            public record Pass(string tCode): RobotResult.RobotPass($"Die Transaktion mit T-Code '{tCode}' wurde erfolgreich ausgeführt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Transaktion konnte nicht ausgeführt werden.");
        }

        public record ExportWindow {
            public record NoSession(): RobotResult.NoSession();
            public record Pass(string csvPath, string pngPath): RobotResult.RobotPass($"Die Maske wurde in den Dateien {csvPath} und {pngPath} gespeichert");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Maske konnte nicht exportiert werden.");
        }

        public record ExpandTreeFolder {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string elementPath): RobotResult.NotFound($"Der Baumordner '{elementPath}' wurde nicht gefunden.");
            public record Pass(string elementPath): RobotResult.RobotPass($"Der Baumordner '{elementPath}' wurde aufgeklappt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Der Baumordner konnte nicht aufgeklappt werden.");
        }

        public record FillCell {
            public record InvalidTable(int tableNumber): RobotResult.RobotFail("InvalidTable", $"Die Maske enthält keine Tabelle mit Index {tableNumber}'.");
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Die Zelle mit dem Lokator '{locator}' wurde nicht gefunden.");
            public record NotChangeable(string locator): RobotResult.NotChangeable($"Die Zelle mit dem Lokator '{locator}' ist schreibgeschützt.");
            public record NoTable(): RobotResult.NotFound("Die Maske enthält keine Tabelle.");
            public record Pass(string locator): RobotResult.RobotPass($"Die Zelle mit dem Lokator '{locator}' wurde ausgefüllt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Zelle konnte nicht ausgefüllt werden.");
        }

        public record FillTextField {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Das Textfeld mit dem Lokator '{locator}' wurde nicht gefunden.");
            public record NotChangeable(string locator): RobotResult.NotChangeable($"Das Textfeld mit dem Lokator '{locator}' ist schreibgeschützt.");
            public record Pass(string locator): RobotResult.RobotPass($"Das Textfeld mit dem Lokator '{locator}' wurde ausgefüllt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Das Textfeld konnte nicht ausgefüllt werden. Möglicherweise, weil der Inhalt nicht dazu passt.");
        }

        public record FillTextEdit {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(): RobotResult.NotFound($"Die Maske enthält kein mehrzeiliges Textfeld");
            public record NotChangeable(): RobotResult.NotChangeable($"Das mehrzeilige Textfeld ist schreibgeschützt.");
            public record Pass(): RobotResult.RobotPass($"Das mehrzeilige Textfeld wurde ausgefüllt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Das mehrzeilige Textfeld konnte nicht ausgefüllt werden.");
        }

        public record HighlightButton {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Der Knopf mit dem Lokator '{locator}' wurde nicht gefunden.");
            public record Pass(string locator): RobotResult.RobotPass($"Der Knopf mit dem Lokator '{locator}' wurde hervorgehoben.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Der Knopf konnte nicht hervorgehoben werden.");
        }

        public record DoubleClickTreeElement {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string elementPath): RobotResult.NotFound($"Das Baumelement '{elementPath}' wurde nicht gefunden.");
            public record Pass(string elementPath): RobotResult.RobotPass($"Das Baumelement '{elementPath}' wurde doppelgeklickt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Das Baumelement konnte nicht doppelgeklickt werden.");
        }

        public record SelectTreeElement {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string elementPath): RobotResult.NotFound($"Das Baumelement '{elementPath}' wurde nicht gefunden.");
            public record Pass(string elementPath): RobotResult.RobotPass($"Das Baumelement '{elementPath}' wurde markiert.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Das Baumelement konnte nicht markiert werden.");
        }

        public record SelectTreeElementMenuEntry {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string elementPath): RobotResult.NotFound($"Das Baumelement '{elementPath}' wurde nicht gefunden.");
            public record Pass(string entryPath): RobotResult.RobotPass($"Der Eintrag '{entryPath}' wurde ausgewählt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Der Eintrag konnte nicht ausgewählt werden.");
        }

        public record PressKeyCombination {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string keyCombination): RobotResult.NotFound($"Die Tastenkombination '{keyCombination}' ist nicht vorhanden.");
            public record Pass(string keyCombination): RobotResult.RobotPass($"Die Tastenkombination '{keyCombination}' wurde gedrückt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Tastenkombination konnte nicht gedrückt werden.");
        }

        public record PushButton {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Der Knopf mit dem Lokator '{locator}' wurde nicht gefunden.");
            public record NotChangeable(string locator): RobotResult.NotChangeable($"Der Knopf mit dem Lokator '{locator}' ist deaktiviert.");
            public record Pass(string locator): RobotResult.RobotPass($"Der Knopf mit dem Lokator '{locator}' wurde gedrückt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Der Knopf konnte nicht gedrückt werden.");
        }

        public record PushButtonCell {
            public record InvalidTable(int tableNumber): RobotResult.RobotFail("InvalidTable", $"Die Maske enthält keine Tabelle mit Index {tableNumber}'.");
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Die Zelle mit dem Lokator '{locator}' wurde nicht gefunden.");
            public record NotAButton(string locator): RobotResult.RobotFail("NotAButton", $"Die Zelle mit dem Lokator '{locator}' enthält keinen Knopf.");
            public record NotChangeable(string locator): RobotResult.NotChangeable($"Die Zelle mit dem Lokator '{locator}' ist deaktiviert.");
            public record Pass(string locator): RobotResult.RobotPass($"Die Zelle mit dem Lokator '{locator}' wurde gedrückt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Zelle konnte nicht gedrückt werden.");
        }

        public record ReadStatusbar {
            public record NoSession(): RobotResult.NoSession();
            public record Pass(string message): RobotResult.RobotPass("Die Statusleiste wurde ausgelesen.", returnValue: message);
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Statusleiste konnte nicht ausgelesen werden.");
            public record NotFound(): RobotResult.NotFound("Die Maske enthält keine Stastusleiste");
            public record Json(string json): RobotResult.RobotPass("Die Statusleiste wurde ausgelesen.", returnValue: json);
        }

        public record ReadTextField {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Das Textfeld mit dem Lokator '{locator}' wurde nicht gefunden.");
            public record Pass(string text, string locator): RobotResult.RobotPass($"Das Textfeld mit dem Lokator '{locator}' wurde ausgelesen.", returnValue: text);
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Das Textfeld konnte nicht ausgelesen werden.");
        }

        public record ReadText {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string text): RobotResult.NotFound($"Der Text mit dem Lokator '{text}' wurde nicht gefunden.");
            public record Pass(string text): RobotResult.RobotPass($"Der Text wurde ausgelesen.", returnValue: text);
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Der Text konnte nicht ausgelesen werden.");
        }

        public record ReadCell {
            public record InvalidTable(int tableNumber): RobotResult.RobotFail("InvalidTable", $"Die Maske enthält keine Tabelle mit Index {tableNumber}'.");
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Die Zelle mit dem Lokator '{locator}' wurde nicht gefunden.");
            public record NoTable(): RobotResult.NotFound("Die Maske enthält keine Tabelle.");
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

        public record ScrollTextFieldContents {
            public record NoSession(): RobotResult.NoSession();
            public record Pass(): RobotResult.RobotPass($"Die Inhalte der Textfelder wurden gescrollt.");
            public record NoScrollbar(): RobotResult.RobotFail("NoScrollbar", "Das Fenster enthält keine scrollbaren Textfelder.");
            public record InvalidDirection(): RobotResult.RobotFail("InvalidDirection", "Die angegebene Richtung ist ungültig.");
            public record MaximumReached(): RobotResult.RobotFail("MaximumReached", "Die Inhalte der Textfelder können nicht weiter gescrollt werden.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Inhalte der Textfelder konnten nicht gescrollt werden.");
        }

        public record ScrollWindowHorizontally {
            public record NoSession(): RobotResult.NoSession();
            public record Pass(): RobotResult.RobotPass($"Das Fenster wurde horizontal gescrollt.");
            public record NoScrollbar(): RobotResult.RobotFail("NoScrollbar", "Das Fenster enthält keine horizontale Bildlaufleiste.");
            public record InvalidDirection(): RobotResult.RobotFail("InvalidDirection", "Die angegebene Richtung ist ungültig.");
            public record MaximumReached(): RobotResult.RobotFail("MaximumReached", "Das Fenster kann nicht weiter gescrollt werden.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Das Fenster konnte nicht gescrollt werden.");
        }

        public record SelectCell {
            public record InvalidTable(int tableNumber): RobotResult.RobotFail("InvalidTable", $"Die Maske enthält keine Tabelle mit Index {tableNumber}'.");
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Die Zelle mit dem Lokator '{locator}' wurde nicht gefunden.");
            public record NoTable(): RobotResult.NotFound("Die Maske enthält keine Tabelle.");
            public record Pass(string locator): RobotResult.RobotPass($"Die Zelle mit dem Lokator '{locator}' wurde markiert.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Zelle konnte nicht markiert werden.");

        }

        public record SelectCellValue {
            public record InvalidTable(int tableNumber): RobotResult.RobotFail("InvalidTable", $"Die Maske enthält keine Tabelle mit Index {tableNumber}'.");
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Die Zelle mit dem Lokator '{locator}' wurde nicht gefunden.");
            public record EntryNotFound(string value, string locator): RobotResult.EntryNotFound($"Der Wert '{value}' ist in der Zelle mit dem Lokator '{locator}' nicht vorhanden.");
            public record Pass(string value, string locator): RobotResult.RobotPass($"Der Wert '{value}' wurde in der Zelle mit dem Lokator '{locator}' ausgewählt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Der Wert konnte nicht ausgewählt werden.");

        }

        public record ReadComboBoxEntry {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Das Auswahlmenü mit dem Lokator '{locator}' wurde nicht gefunden.");
            public record Pass(string entry): RobotResult.RobotPass($"Der aktuelle Eintrag wurde ausgelesen.", returnValue: entry);
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Der Eintrag konnte nicht ausgelesen werden.");
        }

        public record SelectComboBoxEntry {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Das Auswahlmenü mit dem Lokator '{locator}' wurde nicht gefunden.");
            public record EntryNotFound(string item, string locator): RobotResult.EntryNotFound($"Der Eintrag '{item}' wurde im Auswahlmenü mit dem Lokator '{locator}' nicht gefunden.");
            public record Pass(string item): RobotResult.RobotPass($"Der Eintrag '{item}' wurde ausgewählt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Der Eintrag konnte nicht ausgewählt werden.");
        }

        public record SelectRadioButton {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Das Optionsfeld mit dem Lokator '{locator}' wurde nicht gefunden.");
            public record NotChangeable(string locator): RobotResult.NotChangeable($"Das Optionsfeld mit dem Lokator '{locator}' ist deaktiviert.");
            public record Pass(string locator): RobotResult.RobotPass($"Das Optionsfeld mit dem Lokator '{locator}' wurde ausgewählt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Das Optionsfeld konnte nicht ausgewählt werden.");
        }

        public record SelectMenuItem {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string menuEntryPath): RobotResult.NotFound($"Der Menüeintrag {menuEntryPath} wurde nicht gefunden.");
            public record Pass(string menuEntryPath): RobotResult.RobotPass($"Der Menüeintrag {menuEntryPath} wurde ausgewählt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Der Menüeintrag konnte nicht ausgewählt werden.");
        }

        public record SelectTableColumn {
            public record NoSession(): RobotResult.NoSession();
            public record NoTable(): RobotResult.NotFound("Die Maske enthält keine Tabelle");
            public record InvalidTable(int tableNumber): RobotResult.RobotFail("InvalidTable", $"Die Maske enthält keine Tabelle mit Index {tableNumber}'.");
            public record NotFound(string column): RobotResult.NotFound($"Die Spalte '{column}' wurde nicht gefunden.");
            public record Pass(string column): RobotResult.RobotPass($"Die Spalte '{column}' wurde markiert");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Spalte konnte nicht markiert werden.");
        }

        public record SelectTableRow {
            public record NoSession(): RobotResult.NoSession();
            public record NoTable(): RobotResult.NotFound("Die Maske enthält keine Tabelle");
            public record InvalidIndex(int rowIndex): RobotResult.RobotFail("InvalidIndex", $"Die Tabelle enthält keine Zeile mit Index {rowIndex}'.");
            public record InvalidTable(int tableNumber): RobotResult.RobotFail("InvalidTable", $"Die Maske enthält keine Tabelle mit Index {tableNumber}'.");
            public record NotFound(string cellContents): RobotResult.NotFound($"Die Zelle mit dem Inhalt '{cellContents}' wurde nicht gefunden.");
            public record Pass(string rowLocator, object rowIndex): RobotResult.RobotPass($"Die Zeile mit dem Lokator '{rowLocator}' wurde markiert", returnValue: rowIndex);
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Zeile konnte nicht markiert werden.");
        }

        public record SelectTextField {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Das Textfeld mit dem Lokator '{locator}' wurde nicht gefunden.");
            public record Pass(string locator): RobotResult.RobotPass($"Das Textfeld mit dem Lokator '{locator}' wurde markiert.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Das Textfeld konnte nicht markiert werden.");
        }

        public record SelectText {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string text): RobotResult.NotFound($"Die Textzeile mit dem Inhalt '{text}' wurde nicht gefunden.");
            public record Pass(string text): RobotResult.RobotPass($"Die Textzeile mit dem Inhalt '{text}' wurde markiert.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Textzeile konnte nicht markiert werden.");
        }

        public record ReadCheckBox {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Das Formularfeld mit dem Lokator '{locator}' wurde nicht gefunden.");
            public record Pass(string locator, bool selected): RobotResult.RobotPass($"Das Formularfeld mit dem Lokator '{locator}' wurde angekreuzt.", returnValue: selected);
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Das Formularfeld konnte nicht angekreuzt werden.");
        }

        public record TickCheckBox {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Das Formularfeld mit dem Lokator '{locator}' wurde nicht gefunden.");
            public record NotChangeable(string locator): RobotResult.NotChangeable($"Das Formularfeld mit dem Lokator '{locator}' ist deaktiviert.");
            public record Pass(string locator): RobotResult.RobotPass($"Das Formularfeld mit dem Lokator '{locator}' wurde angekreuzt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Das Formularfeld konnte nicht angekreuzt werden.");
        }

        public record UntickCheckBox {
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Das Formularfeld mit dem Lokator '{locator}' wurde nicht gefunden.");
            public record NotChangeable(string locator): RobotResult.NotChangeable($"Das Formularfeld mit dem Lokator '{locator}' ist deaktiviert.");
            public record Pass(string locator): RobotResult.RobotPass($"Das Formularfeld mit dem Lokator '{locator}' wurde abgewählt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Das Formularfeld konnte nicht abgewählt werden.");
        }

        public record TickCheckBoxCell {
            public record InvalidTable(int tableNumber): RobotResult.RobotFail("InvalidTable", $"Die Maske enthält keine Tabelle mit Index {tableNumber}'.");
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Die Zelle mit dem Lokator '{locator}' wurde nicht gefunden.");
            public record NotChangeable(string locator): RobotResult.NotChangeable($"Die Zelle mit dem Lokator '{locator}' ist deaktiviert.");
            public record Pass(string locator): RobotResult.RobotPass($"Die Zelle mit dem Lokator '{locator}' wurde angekreuzt.");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Die Zelle konnte nicht angekreuzt werden.");
        }

        public record UntickCheckBoxCell {
            public record InvalidTable(int tableNumber): RobotResult.RobotFail("InvalidTable", $"Die Maske enthält keine Tabelle mit Index {tableNumber}'.");
            public record NoSession(): RobotResult.NoSession();
            public record NotFound(string locator): RobotResult.NotFound($"Die Zelle mit dem Lokator '{locator}' wurde nicht gefunden.");
            public record NotChangeable(string locator): RobotResult.NotChangeable($"Die Zelle mit dem Lokator '{locator}' ist deaktiviert.");
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

        public record MaximizeWindow
        {
            public record NoSession(): RobotResult.NoSession();
            public record Pass(): RobotResult.RobotPass($"Das Fenster im Vordergrund wurde maximiert");
            public record Exception(System.Exception e): RobotResult.ExceptionError(e, "Das Fenster konnte nicht maximiert werden.");
        }
    }
}
