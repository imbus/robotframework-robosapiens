using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using sapfewse;
using saprotwr.net;

namespace RoboSAPiens 
{
    public class KeywordLibrary 
    {
        private readonly Dictionary<string, Func<string[], RobotResult>> keywords;
        private ILogger logger;
        private Options options;
        private Process? proc = null;
        private ISession session;

        public KeywordLibrary(Options options, ILogger logger) 
        {
            this.logger = logger;
            this.options = options;
            session = new NoSAPSession();
            keywords = new Dictionary<string, Func<string[], RobotResult>>()
            {
                {"ActivateTab", args => ActivateTab(args[0])},
                {"AttachToRunningSap", args => AttachToRunningSap()},
                {"CloseConnection", args => CloseConnection()},
                {"CloseSap", args => CloseSap()},
                {"ConnectToServer", args => ConnectToServer(args[0])},
                {"DoubleClickCell", args => DoubleClickCell(args[0], args[1])},
                {"DoubleClickTextField", args => DoubleClickTextField(args[0])},
                {"ExecuteTransaction", args => ExecuteTransaction(args[0])},
                {"ExportForm", args => ExportForm(args[0], args[1])},
                {"ExportSpreadsheet", args => ExportSpreadsheet(args[0])},
                {"ExportTree", args => ExportTree(args[0])},
                {"FillTableCell", args => FillTableCell(args[0], args[1], args[2])},
                {"FillTextField", args => FillTextField(args[0], args[1])},
                {"GetWindowText", args => GetWindowText()},
                {"GetWindowTitle", args => GetWindowTitle()},
                {"HighlightButton", args => HighlightButton(args[0])},
                {"OpenSap", args => OpenSap(args[0])},
                {"PressKeyCombination", args => PressKeyCombination(args[0])},
                {"PushButton", args => PushButton(args[0])},
                {"PushButtonCell", args => PushButtonCell(args[0], args[1])},
                {"ReadStatusbar", args => ReadStatusbar()},
                {"ReadTableCell", args => ReadTableCell(args[0], args[1])},
                {"ReadText", args => ReadText(args[0])},
                {"ReadTextField", args => ReadTextField(args[0])},
                {"SaveScreenshot", args => SaveScreenshot(args[0])},
                {"SelectCell", args => SelectCell(args[0], args[1])},
                {"SelectCellValue", args => SelectCellValue(args[0], args[1], args[2])},
                {"SelectComboBoxEntry", args => SelectComboBoxEntry(args[0], args[1])},
                {"SelectRadioButton", args => SelectRadioButton(args[0])},
                {"SelectTableRow", args => SelectTableRow(args[0])},
                {"SelectTextField", args => SelectTextField(args[0])},
                {"SelectTextLine", args => SelectTextLine(args[0])},
                {"TickCheckBox", args => TickCheckBox(args[0])},
                {"TickCheckBoxCell", args => TickCheckBoxCell(args[0], args[1])},
                {"UntickCheckBox", args => UntickCheckBox(args[0])},
            };
        }

        // TODO: Implement a test to verify that this function handles all keywords
        // That is, the function should not return KeywordNotFound.
        public RobotResult callKeyword(string methodName, string[] args) 
        {
            if (!keywords.ContainsKey(methodName)) {
                return new Result.KeywordLibrary.KeywordNotFound(methodName);
            }

            try
            {
                return keywords[methodName].Invoke(args);
            }
            catch (Exception e)
            {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.KeywordLibrary.Exception(methodName, e);   
            }
        }

        public List<string> getKeywordNames()
        {
            return keywords.Keys.ToList();
        }

        record EitherSapGui {
            public record Ok(GuiApplication sapGui): EitherSapGui;
            public record Err(RobotResult.RobotFail robotFail): EitherSapGui;
        };

        EitherSapGui getSapGui() {
            try
            {
                var sapGui = new CSapROTWrapper().GetROTEntry("SAPGUI");
                if (sapGui == null) {
                    return new EitherSapGui.Err(new RobotResult.NoSapGui());
                }
                
                var scriptingEngine = sapGui.GetType().InvokeMember(
                    "GetScriptingEngine",
                    BindingFlags.InvokeMethod,
                    null,
                    sapGui,
                    null
                );
                
                if (scriptingEngine == null) {
                    return new EitherSapGui.Err(new RobotResult.NoGuiScripting());
                }

                return new EitherSapGui.Ok((GuiApplication)scriptingEngine);
            }
            catch (System.Exception e)
            {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new EitherSapGui.Err(new RobotResult.ExceptionError(e, "An unexpected error occurred."));
            }
        }

        [Keyword("Reiter auswählen"),
         Doc("Der Reiter mit dem angegebenen Name wird ausgewählt.\n\n" +
             "| ``Reiter auswählen    Name``")]
        public RobotResult ActivateTab(string tab) {
            return session switch {
                SAPSession session => session.activateTab(tab),
                _ => new Result.ActivateTab.NoSession()
            };
        }

        [Keyword("SAP starten"),
         Doc("Die SAP GUI wird gestartet. Der übliche Pfad ist\n\n" +
            @"| ``C:\Program Files (x86)\SAP\FrontEnd\SAPgui\saplogon.exe``")]
        public RobotResult OpenSap(string path)
        {
            try
            {
                proc = Process.Start(path);

                Thread.Sleep(500);
                if (proc.HasExited)
                {
                    return new Result.OpenSap.SAPAlreadyRunning();
                }

                int timeout = 10000;    // in milliseconds
                int elapsed = 0;
                int waiting_time = 100; // in milliseconds
                object? sapGui = null;

                while (sapGui == null && elapsed < timeout)
                {
                    Thread.Sleep(waiting_time);
                    elapsed += waiting_time;
                    sapGui = new CSapROTWrapper().GetROTEntry("SAPGUI");
                }

                if (sapGui == null) {
                    return new Result.OpenSap.NoGuiScripting();
                }

                return new Result.OpenSap.Pass();
            }
            catch (Exception e)
            {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");

                if (e is System.ComponentModel.Win32Exception)
                {
                    return new Result.OpenSap.SAPNotStarted(path);
                }
                else {
                    return new Result.OpenSap.Exception(e);
                }
            }
        }

        [Keyword("Verbindung zum Server trennen"),
         Doc("Die Verbindung mit dem SAP Server wird beendet.")]
        public RobotResult CloseConnection() {
            switch(getSapGui()) 
            {
                case EitherSapGui.Err(RobotResult.RobotFail.NoSapGui):
                    return new Result.CloseConnection.NoSapGui();
                
                case EitherSapGui.Err(RobotResult.RobotFail.NoGuiScripting):
                    return new Result.CloseConnection.NoGuiScripting();

                case EitherSapGui.Err(RobotResult.ExceptionError(System.Exception e, string errorMessage)):
                    return new Result.CloseConnection.Exception(e);

                case EitherSapGui.Ok(GuiApplication guiApplication):
                    if (guiApplication.Connections.Length == 0) {
                        return new Result.CloseConnection.NoConnection();
                    }
                    break;
            }

            return session switch {
                SAPSession session => session.closeConnection(),
                _ => new Result.CloseConnection.NoSession()
            };
        }

        [Keyword("SAP beenden"),
         Doc("Die SAP GUI wird beendet.")]
        public RobotResult CloseSap() {
            if (proc == null) {
                return new Result.CloseSap.NoSapGui();
            }

            proc.Kill();
            return new Result.CloseSap.Pass();
        }


        [Keyword("Funktionsbaum exportieren"),
         Doc("Der Funktionsbaum wird in der angegebenen Datei gespeichert.\n\n" +
             "| ``Funktionsbaum exportieren     Dateipfad``")]
        public RobotResult ExportTree(string filepath) {
            return session switch {
                SAPSession session => session.exportTree(filepath),
                _ => new Result.ExportTree.NoSession()
            };
        }

        [Keyword("Laufende SAP GUI übernehmen"),
         Doc("Nach der Ausführung dieses Keywords, kann eine laufende SAP GUI mit RoboSAPiens gesteuert werden.")]
        public RobotResult AttachToRunningSap() {
            GuiApplication? guiApplication = null;

            switch(getSapGui()) 
            {
                case EitherSapGui.Err(RobotResult.RobotFail.NoSapGui):
                    return new Result.AttachToRunningSap.NoSapGui();
                
                case EitherSapGui.Err(RobotResult.RobotFail.NoGuiScripting):
                    return new Result.AttachToRunningSap.NoGuiScripting();

                case EitherSapGui.Err(RobotResult.ExceptionError(System.Exception e, string errorMessage)):
                    return new Result.AttachToRunningSap.Exception(e);

                case EitherSapGui.Ok(GuiApplication sapGui):
                    guiApplication = sapGui;
                    break;
            }

            try
            {
                var connections = guiApplication!.Connections;

                if (connections.Length == 0) {
                    return new Result.AttachToRunningSap.NoConnection();
                }
                
                var result = new RobotResult();

                for (int c = 0; c < connections.Length; c++)
                {
                    var connection = (GuiConnection)connections.ElementAt(c);

                    if (connection.DisabledByServer) 
                    {
                        result = new Result.AttachToRunningSap.NoServerScripting();
                    }
                    else if (connection.Sessions.Length == 0) 
                    {
                        result = new Result.AttachToRunningSap.NoSession();
                    }
                    else 
                    {
                        var guiSession = (GuiSession)connection.Sessions.ElementAt(0);
                        this.session = new SAPSession(guiSession, connection, options, logger);
                        result = new Result.AttachToRunningSap.Pass();
                        break;
                    }
                }

                return result;
            } 
            catch(Exception e) 
            {
                if (options.debug) 
                {
                    Console.WriteLine();
                    logger.error(e.Message, e.StackTrace ?? "");
                }
                return new Result.AttachToRunningSap.Exception(e);
            }
        }

        GuiConnection? getConnection(GuiApplication guiApplication, string serverName) 
        {
            var connections = guiApplication.Connections;

            for (int i=0; i < connections.Length; i++) {
                var connection = (GuiConnection)connections.ElementAt(i);
                if (connection.Description.Equals(serverName)) {
                    return connection;
                }
            }

            return null;
        }

        [Keyword("Verbindung zum Server herstellen"),
         Doc("Die Verbindung mit dem angegebenen SAP Server wird hergestellt.\n\n" +
             "| ``Verbindung zum Server herstellen    Servername``")]
        public RobotResult ConnectToServer(string server) {
            GuiApplication? guiApplication = null;

            switch(getSapGui()) 
            {
                case EitherSapGui.Err(RobotResult.RobotFail.NoSapGui):
                    return new Result.ConnectToServer.NoSapGui();
                
                case EitherSapGui.Err(RobotResult.RobotFail.NoGuiScripting):
                    return new Result.ConnectToServer.NoGuiScripting();

                case EitherSapGui.Err(RobotResult.ExceptionError(System.Exception e, string errorMessage)):
                    return new Result.ConnectToServer.Exception(e);

                case EitherSapGui.Ok(GuiApplication sapGui):
                    guiApplication = sapGui;
                    break;
            }

            try 
            {
                // reuse existing connection
                var connection = getConnection(guiApplication!, server);

                if (connection != null && connection.Sessions.Length != 0) 
                {
                    var currentSession = (GuiSession)connection.Sessions.ElementAt(0);
                    this.session = new SAPSession(currentSession, connection, options, logger);

                    return new Result.ConnectToServer.Pass(server);
                }

                connection = guiApplication!.OpenConnection(server);
                var connectionError = guiApplication.ConnectionErrorText;
                if (connectionError != "")
                    return new Result.ConnectToServer.SapError(connectionError);

                if (connection.DisabledByServer)
                    return new Result.ConnectToServer.NoServerScripting();

                var guiSession = (GuiSession)connection.Sessions.ElementAt(0);
                this.session = new SAPSession(guiSession, connection, options, logger);

                return new Result.ConnectToServer.Pass(server);
            
            } 
            catch (Exception e) 
            {
                if (options.debug) logger.error(e.Message, e.StackTrace ?? "");
                return new Result.ConnectToServer.Exception(e);
            }
        }

        [Keyword("Tabellenzelle doppelklicken"),
         Doc("Die angegebene Tabellenzelle wird doppelgeklickt.\n\n" +
             "| ``Tabellenzelle doppelklicken     Positionsgeber     Spaltentitel``\n" +
             "Positionsgeber: entweder die Zeilennummer oder der Inhalt der Zelle.")]
        public RobotResult DoubleClickCell(string row_locator, string column) {
            return session switch {
                SAPSession session => session.doubleClickCell(row_locator, column),
                _ => new Result.DoubleClickCell.NoSession()
            };
        }

        [Keyword("Textfeld doppelklicken"),
         Doc("Das angegebene Textfeld wird doppelgeklickt.\n\n" +
            "*Textfeld mit einer Beschriftung links*\n" +
             "| ``Textfeld doppelklicken    Beschriftung``\n" +
             "*Textfeld mit einer Beschriftung oben*\n" +
             "| ``Textfeld doppelklicken    @ Beschriftung``\n" +
             "*Textfeld am Schnittpunkt einer Beschriftung links und einer oben (z.B. eine Abschnittsüberschrift)*\n" +
             "| ``Textfeld doppelklicken    Beschriftung links @ Beschriftung oben``\n" +
             "*Textfeld ohne Beschriftung unter einem Textfeld mit einer Beschriftung (z.B. eine Adresszeile)*\n" +
             "| ``Textfeld doppelklicken    Position (1,2,..) @ Beschriftung``\n\n" +
             "*Textfeld ohne Beschriftung rechts von einem Textfeld mit einer Beschriftung*\n" +
             "| ``Textfeld doppelklicken    Beschriftung @ Position (1,2,..)``\n\n" +
             "*Textfeld mit einer nicht eindeutigen Beschriftung rechts von einem Textfeld mit einer Beschriftung*\n" +
             "| ``Textfeld doppelklicken    Beschriftung des linken Textfelds >> Beschriftung``\n\n" +
            "*Textfeld mit dem angegebenen Inhalt*\n" +
             "| ``Textfeld doppelklicken    = Inhalt``"
             )]
        public RobotResult DoubleClickTextField([Locator(Loc.HLabel, Loc.VLabel, Loc.HLabelVLabel, Loc.HIndexVLabel, Loc.HLabelVIndex, Loc.HLabelHLabel, Loc.Content)] string locator) {
            return session switch {
                SAPSession session => session.doubleClickTextField(locator),
                _ => new Result.DoubleClickTextField.NoSession()
            };
        }

        [Keyword("Transaktion ausführen"),
         Doc("Die Transaktion mit dem angegebenen T-Code wird ausgeführt.\n\n" +
              "| ``Transaktion ausführen    T-Code``")]
        public RobotResult ExecuteTransaction(string T_Code) {
            return session switch {
                SAPSession session => session.executeTransaction(T_Code),
                _ => new Result.ExecuteTransaction.NoSession()
            };
        }

        [Keyword("Tabellenkalkulation exportieren"),
         Doc("Die Export-Funktion 'Tabellenkalkulation' wird für die angegebene Tabelle aufgerufen, falls vorhanden.\n\n" +
             "| ``Tabellenkalkulation exportieren   Index``\n" + 
             "Index: 1, 2,...")]
        public RobotResult ExportSpreadsheet(string index) {
            return session switch {
                SAPSession session => session.exportSpreadsheet(index),
                _ => new Result.ExportSpreadsheet.NoSession()
            };
        }

        [Keyword("Maske exportieren"),
         Doc("Alle Texte in der aktuellen Maske werden in einer CSV-Datei gespeichert. Außerdem wird ein Bildschirmfoto in PNG-Format erstellt.\n\n" +
             "| ``Maske exportieren     Name     Verzeichnis``\n" +
             "Verzeichnis: Der absolute Pfad des Verzeichnisses, wo die Dateien abgelegt werden.")]
        public RobotResult ExportForm(string name, string directory) {
            return session switch {
                SAPSession session => session.exportForm(name, directory),
                _ => new Result.ExportForm.NoSession()
            };
        }

        [Keyword("Tabellenzelle ausfüllen"),
         Doc("Die Zelle am Schnittpunkt der angegebenen Zeile und Spalte wird mit dem angegebenen Inhalt ausgefüllt.\n\n" +
             "| ``Tabellenzelle ausfüllen     Zeile     Spaltentitel = Inhalt``\n" +
             "Zeile: entweder eine Zeilennummer oder der Inhalt einer Zelle in der Zeile.\n\n" +
             "*Hinweis*: Eine Tabellenzelle hat u.U. eine Beschriftung, die man über die Hilfe (Taste F1) herausfinden kann. " +
             "In diesem Fall kann man die Zelle mit dem Keyword [#Textfeld%20Ausfüllen|Textfeld ausfüllen] ausfüllen.")]
        public RobotResult FillTableCell(string row_locator, [Locator(Loc.ColumnContent)] string column_content, string? content) {
            return session switch {
                SAPSession session => session.fillTableCell(row_locator, column_content, content),
                _ => new Result.FillTableCell.NoSession()
            };
        }

        [Keyword("Textfeld ausfüllen"),
         Doc("Das angegebene Textfeld wird mit dem angegebenen Inhalt ausgefüllt.\n\n" +
             "*Textfeld mit einer Beschriftung links*\n" +
             "| ``Textfeld ausfüllen    Beschriftung    Inhalt``\n" +
             "*Textfeld mit einer Beschriftung oben*\n" +
             "| ``Textfeld ausfüllen    @ Beschriftung    Inhalt``\n" +
             "*Textfeld am Schnittpunkt einer Beschriftung links und einer oben (z.B. eine Abschnittsüberschrift)*\n" +
             "| ``Textfeld ausfüllen    Beschriftung links @ Beschriftung oben    Inhalt``\n" +
             "*Textfeld ohne Beschriftung unter einem Textfeld mit einer Beschriftung (z.B. eine Adresszeile)*\n" +
             "| ``Textfeld ausfüllen    Position (1,2,..) @ Beschriftung    Inhalt``\n\n" +
             "*Textfeld ohne Beschriftung rechts von einem Textfeld mit einer Beschriftung*\n" +
             "| ``Textfeld ausfüllen    Beschriftung @ Position (1,2,..)    Inhalt``\n\n" +
             "*Textfeld mit einer nicht eindeutigen Beschriftung rechts von einem Textfeld mit einer Beschriftung*\n" +
             "| ``Textfeld ausfüllen    Beschriftung des linken Textfelds >> Beschriftung    Inhalt``\n\n" +
             "*Hinweis*: In der Regel hat ein Textfeld eine unsichtbare Beschriftung, " +
             "die man über die Hilfe (Taste F1) herausfinden kann.")]
        public RobotResult FillTextField([Locator(Loc.HLabel, Loc.VLabel, Loc.HLabelVLabel, Loc.HIndexVLabel, Loc.HLabelVIndex, Loc.HLabelHLabel)] string locator, string content) {
            return session switch {
                SAPSession session => session.fillTextField(locator, content),
                _ => new Result.FillTextField.NoSession()
            };
        }

        [Keyword("Knopf drücken"),
         Doc("Der Knopf mit dem angegebenen Namen oder Kurzinfo (Tooltip) wird gedrückt.\n\n" +
             "| ``Knopf drücken    Name oder Kurzinfo (Tooltip)``")]
        public RobotResult PushButton(string button) {
            return session switch {
                SAPSession session => session.pushButton(button),
                _ => new Result.PushButton.NoSession()
            };
        }

        [Keyword("Knopf hervorheben"),
         Doc("Der Knopf mit dem angegebenen Namen oder Kurzinfo (Tooltip) wird hervorgehoben.\n\n" +
             "| ``Knopf hervorheben    Name oder Kurzinfo (Tooltip)``")]
        public RobotResult HighlightButton(string button) {
            return session switch {
                SAPSession session => session.highlightButton(button),
                _ => new Result.HighlightButton.NoSession()
            };
        }

        [Keyword("Statusleiste auslesen"),
         Doc("Die Statusleiste wird ausgelesen.\n\n" +
             "| ``Statusleiste auslesen``")]
        public RobotResult ReadStatusbar() {
            return session switch {
                SAPSession session => session.readStatusbar(),
                _ => new Result.ReadStatusbar.NoSession()
            };
        }

        [Keyword("Tabellenzelle drücken"),
         Doc("Die angegebene Tabellenzelle wird gedrückt.\n\n" +
             "| ``Tabellenzelle drücken     Positionsgeber     Spaltentitel``\n" +
             "Positionsgeber: Zeilennummer, Beschriftung oder Kurzinfo (Tooltip).")]
        public RobotResult PushButtonCell(string row_or_label, string column) {
            return session switch {
                SAPSession session => session.pushButtonCell(row_or_label, column),
                _ => new Result.PushButtonCell.NoSession()
            };
        }

        [Keyword("Tabellenzeile markieren"),
         Doc("Die angegebene Tabellenzeile wird markiert.\n\n" +
             "| ``Tabellenzeile markieren     Zeilennummer``")]
        public RobotResult SelectTableRow(string row_number) {
            return session switch {
                SAPSession session => session.selectTableRow(row_number),
                _ => new Result.SelectTableRow.NoSession()
            };
        }

        [Keyword("Tastenkombination drücken"),
         Doc("Die angegebene Tastenkombination wird gedrückt.\n\n" +
             "| ``Tastenkombination drücken    Tastenkombination``")]
        public RobotResult PressKeyCombination(string keyCombination) {
            return session switch {
                SAPSession session => session.pressKeyCombination(keyCombination),
                _ => new Result.PressKeyCombination.NoSession()
            };
        }

        [Keyword("Textfeld auslesen"),
         Doc("Der Inhalt des angegebenen Textfeldes wird zurückgegeben.\n\n" +
             "*Textfeld mit einer Beschriftung links*\n" +
             "| ``Textfeld auslesen    Beschriftung``\n" +
             "*Textfeld mit einer Beschriftung oben*\n" +
             "| ``Textfeld auslesen    @ Beschriftung``\n" +
             "*Textfeld am Schnittpunkt einer Beschriftung links und einer oben*\n" +
             "| ``Textfeld auslesen    Beschriftung links @ Beschriftung oben``\n" +
             "*Textfeld mit dem angegebenen Inhalt*\n" +
             "| ``Textfeld auslesen    = Inhalt``")]
        public RobotResult ReadTextField([Locator(Loc.HLabel, Loc.VLabel, Loc.HLabelVLabel, Loc.Content)] string locator) {
            return session switch {
                SAPSession session => session.readTextField(locator),
                _ => new Result.ReadTextField.NoSession()
            };
        }

        [Keyword("Text auslesen"),
         Doc("Der Inhalt des angegebenen Texts wird zurückgegeben.\n\n" +
             "*Text fängt mit der angegebenen Teilzeichenfolge an*\n" +
             "| ``Text auslesen    = Teilzeichenfolge``\n" +
             "*Text folgt einer Beschriftung*\n" +
             "| ``Text auslesen    Beschriftung``")]
        public RobotResult ReadText([Locator(Loc.Content, Loc.HLabel)] string locator) {
            return session switch {
                SAPSession session => session.readText(locator),
                _ => new Result.ReadText.NoSession()
            };
        }

        [Keyword("Tabellenzelle auslesen"),
         Doc("Der Inhalt der angegebenen Tabellenzelle wird zurückgegeben.\n\n" +
             "| ``Tabellenzelle ablesen     Positionsgeber     Spaltentitel``\n" +
             "Positionsgeber: Zeilennummer oder Zellinhalt.")]
        public RobotResult ReadTableCell(string row_locator, string column) {
            return session switch {
                SAPSession session => session.readTableCell(row_locator, column),
                _ => new Result.ReadTableCell.NoSession()
            };
        }

        [Keyword("Fenster aufnehmen"),
         Doc("Eine Bildschirmaufnahme des Fensters wird im eingegebenen Dateipfad gespeichert.\n\n" +
             "| ``Fenster aufnehmen     Dateipfad``\n" +
             "Dateifpad: Der absolute Pfad einer .png Datei bzw. eines Verzeichnisses.")]
        public RobotResult SaveScreenshot(string filepath) {
            return session switch {
                SAPSession session => session.saveScreenshot(filepath),
                _ => new Result.SaveScreenshot.NoSession()
            };
        }

        [Keyword("Tabellenzelle markieren"),
         Doc("Die angegebene Tabellenzelle wird markiert.\n\n" +
             "| ``Tabellenzelle markieren     Positionsgeber     Spaltentitel``\n" +
             "Positionsgeber: Zeilennummer oder Zellinhalt.")]
        public RobotResult SelectCell(string row_locator, string column) {
            return session switch {
                SAPSession session => session.selectCell(row_locator, column),
                _ => new Result.SelectCell.NoSession()
            };
        }

        [Keyword("Tabellenzellenwert auswählen"),
         Doc("In der spezifizierten Zelle wird der angegebene Wert ausgewählt.\n\n" +
             "| ``Tabellenzellenwert auswählen    Zeilennummer    Spaltentitel    Eintrag``")]
        public RobotResult SelectCellValue(string row_locator, string column, string entry) {
            return session switch {
                SAPSession session => session.selectCellValue(row_locator, column, entry),
                _ => new Result.SelectCellValue.NoSession()
            };
        }

        [Keyword("Auswahlmenüeintrag auswählen"),
         Doc("Aus dem angegebenen Auswahlmenü wird der angegebene Eintrag ausgewählt.\n\n" +
             "| ``Auswahlmenüeintrag auswählen    Auswahlmenü    Eintrag``")]
        public RobotResult SelectComboBoxEntry(string comboBox, string entry) {
            return session switch {
                SAPSession session => session.selectComboBoxEntry(comboBox, entry),
                _ => new Result.SelectComboBoxEntry.NoSession()
            };
        }

        [Keyword("Optionsfeld auswählen"),
         Doc("Das angegebene Optionsfeld wird ausgewählt.\n\n" +
             "*Optionsfeld mit einer Beschriftung links oder rechts*\n" +
             "| ``Optionsfeld auswählen    Beschriftung``\n" +
            "*Optionsfeld mit einer Beschriftung oben*\n" +
             "| ``Optionsfeld auswählen    @ Beschriftung``\n" +
             "*Optionsfeld am Schnittpunkt einer Beschriftung links (oder rechts) und einer oben*\n" +
             "| ``Optionsfeld auswählen    Beschriftung links @ Beschriftung oben``\n")]
        public RobotResult SelectRadioButton([Locator(Loc.HLabel, Loc.VLabel, Loc.HLabelVLabel)] string locator) {
            return session switch {
                SAPSession session => session.selectRadioButton(locator),
                _ => new Result.SelectRadioButton.NoSession()
            };
        }

        [Keyword("Textfeld markieren"),
         Doc("Das angegebene Textfeld wird markiert.\n\n" +
             "*Textfeld mit einer Beschriftung links*\n" +
             "| ``Textfeld markieren    Beschriftung``\n" +
             "*Textfeld mit einer Beschriftung oben*\n" +
             "| ``Textfeld markieren    @ Beschriftung``\n" +
             "*Textfeld am Schnittpunkt einer Beschriftung links und einer oben*\n" +
             "| ``Textfeld markieren    Beschriftung links @ Beschriftung oben``\n" +
            "*Textfeld mit dem angegebenen Inhalt*\n" +
             "| ``Textfeld markieren    = Inhalt``")]
        public RobotResult SelectTextField([Locator(Loc.HLabel, Loc.VLabel, Loc.HLabelVLabel, Loc.Content)] string locator) {
            return session switch {
                SAPSession session => session.selectTextField(locator),
                _ => new Result.SelectTextField.NoSession()
            };
        }

        [Keyword("Textzeile markieren"),
         Doc("Die Textzeile mit dem angegebenen Inhalt wird markiert.\n" +
             "| ``Textzeile markieren    Inhalt``")]
        public RobotResult SelectTextLine(string content) {
            return session switch {
                SAPSession session => session.selectTextLine(content),
                _ => new Result.SelectTextLine.NoSession()
            };
        }

        [Keyword("Formularfeld ankreuzen"),
         Doc("Das angegebene Formularfeld wird angekreuzt.\n\n" +
             "*Formularfeld mit einer Beschriftung links oder rechts *\n" +
             "| ``Formularfeld ankreuzen    Beschriftung``\n" +
            "*Formularfeld mit einer Beschriftung oben*\n" +
             "| ``Formularfeld ankreuzen    @ Beschriftung``\n" +
             "*Formularfeld am Schnittpunkt einer Beschriftung links und einer oben*\n" +
             "| ``Formularfeld ankreuzen    Beschriftung links @ Beschriftung oben``")]
        public RobotResult TickCheckBox([Locator(Loc.HLabel, Loc.VLabel, Loc.HLabelVLabel)] string locator) {
            return session switch {
                SAPSession session => session.tickCheckBox(locator),
                _ => new Result.TickCheckBox.NoSession()
            };
        }

        [Keyword("Formularfeld abwählen"),
         Doc("Das angegebene Formularfeld wird abgewählt.\n\n" +
             "*Formularfeld mit einer Beschriftung links oder rechts *\n" +
             "| ``Formularfeld abwählen    Beschriftung``\n" +
            "*Formularfeld mit einer Beschriftung oben*\n" +
             "| ``Formularfeld abwählen    @ Beschriftung``\n" +
             "*Formularfeld am Schnittpunkt einer Beschriftung links und einer oben*\n" +
             "| ``Formularfeld abwählen    Beschriftung links @ Beschriftung oben``")]
        public RobotResult UntickCheckBox([Locator(Loc.HLabel, Loc.VLabel, Loc.HLabelVLabel)] string locator) {
            return session switch {
                SAPSession session => session.untickCheckBox(locator),
                _ => new Result.UntickCheckBox.NoSession()
            };
        }

        [Keyword("Tabellenzelle ankreuzen"),
         Doc("Die angegebene Tabellenzelle wird angekreuzt.\n\n" +
             "| ``Tabellenzelle ankreuzen     Zeilennummer     Spaltentitel``")]
        public RobotResult TickCheckBoxCell(string row, string column) {
            return session switch {
                SAPSession session => session.tickCheckBoxCell(row, column),
                _ => new Result.TickCheckBoxCell.NoSession()
            };
        }

        [Keyword("Fenstertitel auslesen"),
         Doc("Der Titel des Fensters im Fordergrund wird zurückgegeben.\n\n" +
             "| ``${Titel}    Fenstertitel auslesen``")]
        public RobotResult GetWindowTitle() {
            return session switch {
                SAPSession session => session.getWindowTitle(),
                _ => new Result.GetWindowTitle.NoSession()
            };
        }

        [Keyword("Fenstertext auslesen"),
         Doc("Der Text des Fensters im Fordergrund wird zurückgegeben.\n\n" +
             "| ``${Text}    Fenstertext auslesen``")]
        public RobotResult GetWindowText() {
            return session switch {
                SAPSession session => session.getWindowText(),
                _ => new Result.GetWindowText.NoSession()
            };
        }
    }
}
