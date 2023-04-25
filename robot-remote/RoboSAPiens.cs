using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using sapfewse;
using saprotwr.net;

namespace RoboSAPiens {
    public class RoboSAPiens {
        CLI.Options options;
        List<RobotKeyword> keywords;
        public ISession session;
        private Process? proc = null;

        public RoboSAPiens(CLI.Options options) {
            this.options = options;
            session = new NoSAPSession();
            keywords = new List<RobotKeyword>();

            typeof(RoboSAPiens)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .ToList()
                .ForEach(methodInfo => {
                    var attrKeyword = methodInfo.GetCustomAttribute(typeof(Keyword));
                    var attrDoc = methodInfo.GetCustomAttribute(typeof(Doc));

                    if (attrKeyword != null && attrDoc != null) {
                        var name = ((Keyword)attrKeyword).Name;
                        var method = methodInfo.Name;
                        var args = methodInfo.GetParameters()
                                             .Select(param => param.Name ?? "").ToArray();
                        var types = methodInfo.GetParameters()
                                              .Select(param => param.ParameterType.ToString())
                                              .ToArray();
                        var doc = ((Doc)attrDoc).DocString;
                        keywords.Add(new RobotKeyword(name, method, args, types, doc));
                    }
                });
        }

		dynamic? InvokeMethod(object obj, string methodName, object[]? methodParams = null) {
            return obj.GetType().InvokeMember(methodName,
                                              System.Reflection.BindingFlags.InvokeMethod,
                                              null,
                                              obj,
                                              methodParams);
		}

        public RobotKeyword getKeyword(string name) {
            return keywords.Single(keyword => keyword.method == name);
        }

        public string[] getKeywordNames() {
            return keywords.Select(keyword => keyword.name).ToArray();
        }

        public static object getKeywordSpecs() {
            return typeof(RoboSAPiens)
                    .GetMethods()
                    .Where(method => 
                        method.GetCustomAttribute(typeof(Keyword)) != null &&
                        method.GetCustomAttribute(typeof(Doc)) != null
                    )
                    .ToDictionary(
                        method => method.Name,
                        method => new 
                            {
                                name = ((Keyword)method.GetCustomAttribute(typeof(Keyword))!).Name,
                                args = method.GetParameters().ToDictionary(
                                    param => param.Name!,
                                    param => new 
                                    {
                                        name = param.Name,
                                        spec = new {}
                                    }
                                ),
                                result = typeof(Result)
                                            .GetNestedType(method.Name)
                                            ?.GetNestedTypes()
                                            .ToDictionary(type => type.Name, type => type.Name)
                                            ?? new Dictionary<string, string>(),
                                doc = ((Doc)method.GetCustomAttribute(typeof(Doc))!).DocString
                            }
                    );
        }


        [Keyword("Reiter auswählen"),
         Doc("Der Reiter mit dem angegebenen Name wird ausgewählt.\n\n" +
             "| ``Reiter auswählen    Name``")]
        public RobotResult ActivateTab(string Reitername) {
            return session switch {
                SAPSession session => session.activateTab(Reitername),
                _ => new Result.ActivateTab.NoSession()
            };
        }

        [Keyword("SAP starten"),
         Doc("Die SAP GUI wird gestartet. Der übliche Pfad ist\n\n" +
            @"| ``C:\Program Files (x86)\SAP\FrontEnd\SAPgui\saplogon.exe``")]
        public RobotResult OpenSAP(string Pfad) {
            try {
                proc = Process.Start(Pfad);

                if (proc == null) {
                    return new Result.OpenSAP.SAPNotStarted();
                }

                int timeout = 20000;    // in milliseconds
                int elapsed = 0;
                int waiting_time = 100; // in milliseconds
                object? sapGui = null;

                while (sapGui == null && elapsed < timeout) {
                    Thread.Sleep(waiting_time);
                    elapsed += waiting_time;
                    sapGui = new CSapROTWrapper().GetROTEntry("SAPGUI");
                }

                if (sapGui == null) {
                    return new Result.OpenSAP.SAPNotStarted();
                }

                return new Result.OpenSAP.Pass();
            }
            catch (Exception e) {
                if (options.debug) CLI.error(e.Message, e.StackTrace ?? "");
                if (e is System.ComponentModel.Win32Exception || e is System.InvalidOperationException) {
                    return new Result.OpenSAP.SAPNotStarted(Pfad);
                }
                else {
                    return new Result.OpenSAP.Exception(e);
                }
            }
        }

        [Keyword("Verbindung zum Server trennen"),
         Doc("Die Verbindung mit dem SAP Server wird beendet.")]
        public RobotResult CloseConnection() {
            var sapGui = new CSapROTWrapper().GetROTEntry("SAPGUI");
            if (sapGui == null) {
                return new Result.CloseConnection.NoSapGui();
            }
            
            var scriptingEngine = InvokeMethod(sapGui, "GetScriptingEngine");
            if (scriptingEngine == null) {
                return new Result.CloseConnection.NoGuiScripting();
            }

            var guiApplication = (GuiApplication)scriptingEngine;

            if (guiApplication.Connections.Length == 0) {
                return new Result.CloseConnection.NoConnection();
            }

            return session switch {
                SAPSession session => session.closeConnection(),
                _ => new Result.CloseConnection.NoSession()
            };
        }

        [Keyword("SAP beenden"),
         Doc("Die SAP GUI wird beendet.")]
        public RobotResult CloseSAP() {
            if (proc == null) {
                return new Result.CloseSAP.NoSapGui();
            }

            proc.Kill();
            return new Result.CloseSAP.Pass();
        }

        RobotResult createSession(GuiConnection connection) {
            var sessions = connection.Sessions;
            if (sessions.Length == 0) {
                return new RobotResult.NoSession();
            }

            var guiSession = (GuiSession)sessions.ElementAt(0);

            this.session = new SAPSession(guiSession, connection, options);

            return new Success("Session successfully created");
        }

        [Keyword("Funktionsbaum exportieren"),
         Doc("Der Funktionsbaum wird in der angegebenen Datei gespeichert.\n\n" +
             "| ``Funktionsbaum exportieren     Dateipfad``")]
        public RobotResult ExportTree(string Dateipfad) {
            return session switch {
                SAPSession session => session.exportTree(Dateipfad),
                _ => new Result.ExportTree.NoSession()
            };
        }

        [Keyword("Laufende SAP GUI übernehmen"),
         Doc("Nach der Ausführung dieses Keywords, kann eine laufende SAP GUI mit RoboSAPiens gesteuert werden.")]
        public RobotResult AttachToRunningSAP() {        
            try {
                var sapGui = new CSapROTWrapper().GetROTEntry("SAPGUI");
                if (sapGui == null) {
                    return new Result.AttachToRunningSAP.NoSapGui();
                }

                var scriptingEngine = InvokeMethod(sapGui, "GetScriptingEngine");
                if (scriptingEngine == null) {
                    return new Result.AttachToRunningSAP.NoGuiScripting();
                }

                var guiApplication = (GuiApplication)scriptingEngine;
                var connections = guiApplication.Connections;

                if (connections.Length == 0) {
                    return new Result.AttachToRunningSAP.NoConnection();
                }
                
                var connection = (GuiConnection)connections.ElementAt(0);

                if (connection.DisabledByServer) {
                    return new Result.AttachToRunningSAP.NoServerScripting();
                }

                return createSession(connection) switch {
                    Error error => error,
                    _ => new Result.AttachToRunningSAP.Pass(),
                    
                };
            } catch(Exception e) {
                if (options.debug) CLI.error(e.Message, e.StackTrace ?? "");
                return new Result.AttachToRunningSAP.Exception(e);
            }
        }

        GuiConnection? getConnection(GuiApplication guiApplication, string serverName) {
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
        public RobotResult ConnectToServer(string Servername) {
            try {
                var sapGui = new CSapROTWrapper().GetROTEntry("SAPGUI");
                if (sapGui == null) {
                    return new Result.ConnectToServer.NoSapGui();
                }
                
                var scriptingEngine = InvokeMethod(sapGui, "GetScriptingEngine");
                if (scriptingEngine == null) {
                    return new Result.ConnectToServer.NoGuiScripting();
                }

                var guiApplication = (GuiApplication)scriptingEngine;

                var connection = getConnection(guiApplication, Servername);
                
                if (connection != null) {
                    // reuse existing connection
                    return createSession(connection) switch {
                        Error error => error,
                        _ => new Result.ConnectToServer.Pass(Servername)
                    };
                }

                connection = guiApplication.OpenConnection(Servername);
                var connectionError = guiApplication.ConnectionErrorText;
                if (connectionError != "") {
                    return new Result.ConnectToServer.SapError(connectionError);
                }

                if (connection.DisabledByServer) {
                    return new Result.ConnectToServer.NoServerScripting();
                }

                return createSession(connection) switch {
                    Error error => error,
                    _ => new Result.ConnectToServer.Pass(Servername)
                };
            } catch (Exception e) {
                if (options.debug) CLI.error(e.Message, e.StackTrace ?? "");
                return new Result.ConnectToServer.Exception(e);
            }
        }

        [Keyword("Tabellenzelle doppelklicken"),
         Doc("Die angegebene Tabellenzelle wird doppelgeklickt.\n\n" +
             "| ``Tabellenzelle doppelklicken     Positionsgeber     Spaltentitel``\n" +
             "Positionsgeber: entweder die Zeilennummer oder der Inhalt der Zelle.")]
        public RobotResult DoubleClickCell(string Zeilennummer_oder_Zellinhalt, string Spaltentitel) {
            return session switch {
                SAPSession session => session.doubleClickCell(Zeilennummer_oder_Zellinhalt, Spaltentitel),
                _ => new Result.DoubleClickCell.NoSession()
            };
        }

        [Keyword("Textfeld doppelklicken"),
         Doc("Das angegebene Textfeld wird doppelgeklickt.\n\n" +
             "| ``Textfeld doppelklicken     Inhalt``\n")]
        public RobotResult DoubleClickTextField(string Inhalt) {
            return session switch {
                SAPSession session => session.doubleClickTextField(Inhalt),
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

        [Keyword("Maske exportieren"),
         Doc("Alle Texte in der aktuellen Maske werden in einer CSV-Datei gespeichert. Außerdem wird ein Bildschirmfoto in PNG-Format erstellt.\n\n" +
             "| ``Maske exportieren     Name     Verzeichnis``\n" +
             "Verzeichnis: Der absolute Pfad des Verzeichnisses, wo die Dateien abgelegt werden.")]
        public RobotResult ExportForm(string Name, string Verzeichnis) {
            return session switch {
                SAPSession session => session.exportForm(Name, Verzeichnis),
                _ => new Result.ExportForm.NoSession()
            };
        }

        [Keyword("Tabellenzelle ausfüllen"),
         Doc("Die Zelle am Schnittpunkt der angegebenen Zeile und Spalte wird mit dem angegebenen Inhalt ausgefüllt.\n\n" +
             "| ``Tabellenzelle ausfüllen     Zeile     Spaltentitel = Inhalt``\n" +
             "Zeile: entweder eine Zeilennummer oder der Inhalt einer Zelle in der Zeile.\n\n" +
             "*Hinweis*: Eine Tabellenzelle hat u.U. eine Beschriftung, die man über die Hilfe (Taste F1) herausfinden kann. " +
             "In diesem Fall kann man die Zelle mit dem Keyword [#Textfeld%20Ausfüllen|Textfeld ausfüllen] ausfüllen.")]
        public RobotResult FillTableCell(string Zeilennummer_oder_Zellinhalt, string Spaltentitel_Gleich_Inhalt) {
            return session switch {
                SAPSession session => session.fillTableCell(Zeilennummer_oder_Zellinhalt, Spaltentitel_Gleich_Inhalt),
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
        public RobotResult FillTextField(string Beschriftung_oder_Positionsgeber, string Inhalt) {
            return session switch {
                SAPSession session => session.fillTextField(Beschriftung_oder_Positionsgeber, Inhalt),
                _ => new Result.FillTextField.NoSession()
            };
        }

        [Keyword("Knopf drücken"),
         Doc("Der Knopf mit dem angegebenen Namen oder Kurzinfo (Tooltip) wird gedrückt.\n\n" +
             "| ``Knopf drücken    Name oder Kurzinfo (Tooltip)``")]
        public RobotResult PushButton(string Name_oder_Kurzinfo) {
            return session switch {
                SAPSession session => session.pushButton(Name_oder_Kurzinfo),
                _ => new Result.PushButton.NoSession()
            };
        }

        [Keyword("Tabellenzelle drücken"),
         Doc("Die angegebene Tabellenzelle wird gedrückt.\n\n" +
             "| ``Tabellenzelle drücken     Positionsgeber     Spaltentitel``\n" +
             "Positionsgeber: Zeilennummer, Beschriftung oder Kurzinfo (Tooltip).")]
        public RobotResult PushButtonCell(string Zeilennummer_oder_Name_oder_Kurzinfo, string Spaltentitel) {
            return session switch {
                SAPSession session => session.pushButtonCell(Zeilennummer_oder_Name_oder_Kurzinfo, Spaltentitel),
                _ => new Result.PushButtonCell.NoSession()
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
        public RobotResult ReadTextField(string Beschriftung_oder_Positionsgeber) {
            return session switch {
                SAPSession session => session.readTextField(Beschriftung_oder_Positionsgeber),
                _ => new Result.ReadTextField.NoSession()
            };
        }

        [Keyword("Text auslesen"),
         Doc("Der Inhalt des angegebenen Texts wird zurückgegeben.\n\n" +
             "*Text fängt mit der angegebenen Teilzeichenfolge an*\n" +
             "| ``Text auslesen    = Teilzeichenfolge``\n" +
             "*Text folgt einer Beschriftung*\n" +
             "| ``Text auslesen    Beschriftung``")]
        public RobotResult ReadText(string Inhalt) {
            return session switch {
                SAPSession session => session.readText(Inhalt),
                _ => new Result.ReadText.NoSession()
            };
        }

        [Keyword("Tabellenzelle auslesen"),
         Doc("Der Inhalt der angegebenen Tabellenzelle wird zurückgegeben.\n\n" +
             "| ``Tabellenzelle ablesen     Positionsgeber     Spaltentitel``\n" +
             "Positionsgeber: Zeilennummer oder Zellinhalt.")]
        public RobotResult ReadTableCell(string Zeilennummer_oder_Zellinhalt, string Spaltentitel) {
            return session switch {
                SAPSession session => session.readTableCell(Zeilennummer_oder_Zellinhalt, Spaltentitel),
                _ => new Result.ReadTableCell.NoSession()
            };
        }

        [Keyword("Fenster aufnehmen"),
         Doc("Eine Bildschirmaufnahme des Fensters wird im eingegebenen Dateipfad gespeichert.\n\n" +
             "| ``Fenster aufnehmen     Dateipfad``\n" +
             "Dateifpad: Der absolute Pfad einer .png Datei bzw. eines Verzeichnisses.")]
        public RobotResult SaveScreenshot(string Aufnahmenverzeichnis) {
            return session switch {
                SAPSession session => session.saveScreenshot(Aufnahmenverzeichnis),
                _ => new Result.SaveScreenshot.NoSession()
            };
        }

        [Keyword("Tabellenzelle markieren"),
         Doc("Die angegebene Tabellenzelle wird markiert.\n\n" +
             "| ``Tabellenzelle markieren     Positionsgeber     Spaltentitel``\n" +
             "Positionsgeber: Zeilennummer oder Zellinhalt.")]
        public RobotResult SelectCell(string Zeilennummer_oder_Zellinhalt, string Spaltentitel) {
            return session switch {
                SAPSession session => session.selectCell(Zeilennummer_oder_Zellinhalt, Spaltentitel),
                _ => new Result.SelectCell.NoSession()
            };
        }

        [Keyword("Auswahlmenüeintrag auswählen"),
         Doc("Aus dem angegebenen Auswahlmenü wird der angegebene Eintrag ausgewählt.\n\n" +
             "| ``Auswahlmenüeintrag auswählen    Auswahlmenü    Eintrag``")]
        public RobotResult SelectComboBoxEntry(string Name, string Eintrag) {
            return session switch {
                SAPSession session => session.selectComboBoxEntry(Name, Eintrag),
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
        public RobotResult SelectRadioButton(string Beschriftung_oder_Positionsgeber) {
            return session switch {
                SAPSession session => session.selectRadioButton(Beschriftung_oder_Positionsgeber),
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
        public RobotResult SelectTextField(string Beschriftungen_oder_Inhalt) {
            return session switch {
                SAPSession session => session.selectTextField(Beschriftungen_oder_Inhalt),
                _ => new Result.SelectTextField.NoSession()
            };
        }

        [Keyword("Textzeile markieren"),
         Doc("Die Textzeile mit dem angegebenen Inhalt wird markiert.\n" +
             "| ``Textzeile markieren    Inhalt``")]
        public RobotResult SelectTextLine(string Inhalt) {
            return session switch {
                SAPSession session => session.selectTextLine(Inhalt),
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
        public RobotResult TickCheckBox(string Beschriftung_oder_Positionsgeber) {
            return session switch {
                SAPSession session => session.tickCheckBox(Beschriftung_oder_Positionsgeber),
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
        public RobotResult UntickCheckBox(string Beschriftung_oder_Positionsgeber) {
            return session switch {
                SAPSession session => session.untickCheckBox(Beschriftung_oder_Positionsgeber),
                _ => new Result.UntickCheckBox.NoSession()
            };
        }

        [Keyword("Tabellenzelle ankreuzen"),
         Doc("Die angegebene Tabellenzelle wird angekreuzt.\n\n" +
             "| ``Tabellenzelle ankreuzen     Zeilennummer     Spaltentitel``")]
        public RobotResult TickCheckBoxCell(string Zeilennummer, string Spaltentitel) {
            return session switch {
                SAPSession session => session.tickCheckBoxCell(Zeilennummer, Spaltentitel),
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
