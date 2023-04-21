using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
            return keywords.Single(keyword => keyword.name == name);
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
        public RobotResult activateTab(string Reitername) {
            return session switch {
                SAPSession session => session.activateTab(Reitername),
                _ => new NoSessionError()
            };
        }

        [Keyword("SAP starten"),
         Doc("Die SAP GUI wird gestartet. Der übliche Pfad ist\n\n" +
            @"| ``C:\Program Files (x86)\SAP\FrontEnd\SAPgui\saplogon.exe``")]
        public RobotResult openSAP(string Pfad) {
            try {
                proc = Process.Start(Pfad);

                if (proc == null) {
                    return new SAPNotStartedError();
                }

                var sapGui = new CSapROTWrapper().GetROTEntry("SAPGUI");

                while (sapGui == null) {
                    sapGui = new CSapROTWrapper().GetROTEntry("SAPGUI");
                }

                return new Success("Die SAP GUI wurde gestartet");
            }
            catch (Exception e) {
                if (e is System.ComponentModel.Win32Exception || e is System.InvalidOperationException) {
                    return new SAPNotStartedError(Pfad);
                }
                else {
                    return new ExceptionError(e, "Die SAP GUI konnte nicht gestartet werden.");
                }
            }
        }

        [Keyword("Verbindung zum Server trennen"),
         Doc("Die Verbindung mit dem SAP Server wird beendet.")]
        public RobotResult closeConnection() {
            var sapGui = new CSapROTWrapper().GetROTEntry("SAPGUI");
            if (sapGui == null) {
                return new NoSapGuiError();
            }
            
            var scriptingEngine = InvokeMethod(sapGui, "GetScriptingEngine");
            if (scriptingEngine == null) {
                return new SapError("Die Skriptunterstützung ist nicht verfügbar. "
                                  + "Sie muss in den Einstellungen von SAP Logon aktiviert werden.");
            }

            var guiApplication = (GuiApplication)scriptingEngine;

            if (guiApplication.Connections.Length == 0) {
                return new NoConnectionError();
            }

            return session switch {
                SAPSession session => session.closeConnection(),
                _ => new NoSessionError()
            };
        }

        [Keyword("SAP beenden"),
         Doc("Die SAP GUI wird beendet.")]
        public RobotResult closeSAP() {
            if (proc == null) {
                return new NoSapGuiError();
            }

            proc.Kill();
            return new Success("Die SAP GUI wurde beendet");
        }

        RobotResult createSession(GuiConnection connection) {
            var sessions = connection.Sessions;
            if (sessions.Length == 0) {
                return new NoSessionError();
            }

            var guiSession = (GuiSession)sessions.ElementAt(0);

            this.session = new SAPSession(guiSession, connection, options);

            return new Success("Die Session wurde erfolgreich erstellt");
        }

        [Keyword("Funktionsbaum exportieren"),
         Doc("Der Funktionsbaum wird in der angegebenen Datei gespeichert.\n\n" +
             "| ``Funktionsbaum exportieren     Dateipfad``")]
        public RobotResult exportTree(string Dateipfad) {
            return session switch {
                SAPSession session => session.exportTree(Dateipfad),
                _ => new NoSessionError()
            };
        }

        [Keyword("Laufende SAP GUI übernehmen"),
         Doc("Nach der Ausführung dieses Keywords, kann eine laufende SAP GUI mit RoboSAPiens gesteuert werden.")]
        public RobotResult attachToRunningSAP() {        
            try {
                var sapGui = new CSapROTWrapper().GetROTEntry("SAPGUI");
                if (sapGui == null) {
                    return new NoSapGuiError();
                }

                var scriptingEngine = InvokeMethod(sapGui, "GetScriptingEngine");
                if (scriptingEngine == null) {
                    return new SapError("Die Skriptunterstützung ist nicht verfügbar. "
                                      + "Sie muss in den Einstellungen von SAP Logon aktiviert werden.");
                }

                var guiApplication = (GuiApplication)scriptingEngine;
                var connections = guiApplication.Connections;

                if (connections.Length == 0) {
                    return new NoConnectionError();
                }
                
                var connection = (GuiConnection)connections.ElementAt(0);

                if (connection.DisabledByServer) {
                    return new NoScriptingError();
                }

                return createSession(connection) switch {
                    Error error => error,
                    _ => new Success($"Die laufende SAP GUI wurde erfolgreich übernommen."),
                    
                };
            } catch(Exception e) {
                return new ExceptionError(e, "Die laufende SAP GUI konnte nicht übernommen werden.");
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
        public RobotResult connectToServer(string Servername) {
            string theConnection = $"Die Verbindung mit dem Server '{Servername}'";

            try {
                var sapGui = new CSapROTWrapper().GetROTEntry("SAPGUI");
                if (sapGui == null) {
                    return new NoSapGuiError();
                }
                
                var scriptingEngine = InvokeMethod(sapGui, "GetScriptingEngine");
                if (scriptingEngine == null) {
                    return new SapError("Die Skriptunterstützung ist nicht verfügbar. "
                                    + "Sie muss in den Einstellungen von SAP Logon aktiviert werden.");
                }

                var guiApplication = (GuiApplication)scriptingEngine;

                var connection = getConnection(guiApplication, Servername);
                
                if (connection != null) {
                    return createSession(connection) switch {
                        Error error => error,
                        _ => new Success($"Eine Verbindung zum Server '{Servername}' besteht schon")
                    };
                }

                connection = guiApplication.OpenConnection(Servername);
                var connectionError = guiApplication.ConnectionErrorText;
                if (connectionError != "") {
                    return new SapError(connectionError);
                }

                if (connection.DisabledByServer) {
                    return new NoScriptingError();
                }

                return createSession(connection) switch {
                    Error error => error,
                    _ => new Success($"{theConnection} wurde erfolgreich hergestellt.")
                };
            } catch (Exception e) {
                return new ConnectionFailed(e, $"{theConnection} konnte nicht hergestellt werden.");
            }
        }

        [Keyword("Tabellenzelle doppelklicken"),
         Doc("Die angegebene Tabellenzelle wird doppelgeklickt.\n\n" +
             "| ``Tabellenzelle doppelklicken     Positionsgeber     Spaltentitel``\n" +
             "Positionsgeber: entweder die Zeilennummer oder der Inhalt der Zelle.")]
        public RobotResult doubleClickCell(string Zeilennummer_oder_Zellinhalt, string Spaltentitel) {
            return session switch {
                SAPSession session => session.doubleClickCell(Zeilennummer_oder_Zellinhalt, Spaltentitel),
                _ => new NoSessionError()
            };
        }

        [Keyword("Textfeld doppelklicken"),
         Doc("Das angegebene Textfeld wird doppelgeklickt.\n\n" +
             "| ``Textfeld doppelklicken     Inhalt``\n")]
        public RobotResult doubleClickTextField(string Inhalt) {
            return session switch {
                SAPSession session => session.doubleClickTextField(Inhalt),
                _ => new NoSessionError()
            };
        }

        [Keyword("Transaktion ausführen"),
         Doc("Die Transaktion mit dem angegebenen T-Code wird ausgeführt.\n\n" +
              "| ``Transaktion ausführen    T-Code``")]
        public RobotResult executeTransaction(string T_Code) {
            return session switch {
                SAPSession session => session.executeTransaction(T_Code),
                _ => new NoSessionError()
            };
        }

        [Keyword("Maske exportieren"),
         Doc("Alle Texte in der aktuellen Maske werden in einer CSV-Datei gespeichert. Außerdem wird ein Bildschirmfoto in PNG-Format erstellt.\n\n" +
             "| ``Maske exportieren     Name     Verzeichnis``\n" +
             "Verzeichnis: Der absolute Pfad des Verzeichnisses, wo die Dateien abgelegt werden.")]
        public RobotResult exportForm(string Name, string Verzeichnis) {
            return session switch {
                SAPSession session => session.exportForm(Name, Verzeichnis),
                _ => new NoSessionError()
            };
        }

        [Keyword("Tabellenzelle ausfüllen"),
         Doc("Die Zelle am Schnittpunkt der angegebenen Zeile und Spalte wird mit dem angegebenen Inhalt ausgefüllt.\n\n" +
             "| ``Tabellenzelle ausfüllen     Zeile     Spaltentitel = Inhalt``\n" +
             "Zeile: entweder eine Zeilennummer oder der Inhalt einer Zelle in der Zeile.\n\n" +
             "*Hinweis*: Eine Tabellenzelle hat u.U. eine Beschriftung, die man über die Hilfe (Taste F1) herausfinden kann. " +
             "In diesem Fall kann man die Zelle mit dem Keyword [#Textfeld%20Ausfüllen|Textfeld ausfüllen] ausfüllen.")]
        public RobotResult fillTableCell(string Zeilennummer_oder_Zellinhalt, string Spaltentitel_Gleich_Inhalt) {
            return session switch {
                SAPSession session => session.fillTableCell(Zeilennummer_oder_Zellinhalt, Spaltentitel_Gleich_Inhalt),
                _ => new NoSessionError()
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
        public RobotResult fillTextField(string Beschriftung_oder_Positionsgeber, string Inhalt) {
            return session switch {
                SAPSession session => session.fillTextField(Beschriftung_oder_Positionsgeber, Inhalt),
                _ => new NoSessionError()
            };
        }

        [Keyword("Knopf drücken"),
         Doc("Der Knopf mit dem angegebenen Namen oder Kurzinfo (Tooltip) wird gedrückt.\n\n" +
             "| ``Knopf drücken    Name oder Kurzinfo (Tooltip)``")]
        public RobotResult pushButton(string Name_oder_Kurzinfo) {
            return session switch {
                SAPSession session => session.pushButton(Name_oder_Kurzinfo),
                _ => new NoSessionError()
            };
        }

        [Keyword("Tabellenzelle drücken"),
         Doc("Die angegebene Tabellenzelle wird gedrückt.\n\n" +
             "| ``Tabellenzelle drücken     Positionsgeber     Spaltentitel``\n" +
             "Positionsgeber: Zeilennummer, Beschriftung oder Kurzinfo (Tooltip).")]
        public RobotResult pushButtonCell(string Zeilennummer_oder_Name_oder_Kurzinfo, string Spaltentitel) {
            return session switch {
                SAPSession session => session.pushButtonCell(Zeilennummer_oder_Name_oder_Kurzinfo, Spaltentitel),
                _ => new NoSessionError()
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
        public RobotResult readTextField(string Beschriftung_oder_Positionsgeber) {
            return session switch {
                SAPSession session => session.readTextField(Beschriftung_oder_Positionsgeber),
                _ => new NoSessionError()
            };
        }

        [Keyword("Text auslesen"),
         Doc("Der Inhalt des angegebenen Texts wird zurückgegeben.\n\n" +
             "*Text fängt mit der angegebenen Teilzeichenfolge an*\n" +
             "| ``Text auslesen    = Teilzeichenfolge``\n" +
             "*Text folgt einer Beschriftung*\n" +
             "| ``Text auslesen    Beschriftung``")]
        public RobotResult readText(string Inhalt) {
            return session switch {
                SAPSession session => session.readText(Inhalt),
                _ => new NoSessionError()
            };
        }

        [Keyword("Tabellenzelle auslesen"),
         Doc("Der Inhalt der angegebenen Tabellenzelle wird zurückgegeben.\n\n" +
             "| ``Tabellenzelle ablesen     Positionsgeber     Spaltentitel``\n" +
             "Positionsgeber: Zeilennummer oder Zellinhalt.")]
        public RobotResult readTableCell(string Zeilennummer_oder_Zellinhalt, string Spaltentitel) {
            return session switch {
                SAPSession session => session.readTableCell(Zeilennummer_oder_Zellinhalt, Spaltentitel),
                _ => new NoSessionError()
            };
        }

        [Keyword("Fenster aufnehmen"),
         Doc("Eine Bildschirmaufnahme des Fensters wird im eingegebenen Dateipfad gespeichert.\n\n" +
             "| ``Fenster aufnehmen     Dateipfad``\n" +
             "Dateifpad: Der absolute Pfad einer .png Datei bzw. eines Verzeichnisses.")]
        public RobotResult saveScreenshot(string Aufnahmenverzeichnis) {
            return session switch {
                SAPSession session => session.saveScreenshot(Aufnahmenverzeichnis),
                _ => new NoSessionError()
            };
        }

        [Keyword("Tabellenzelle markieren"),
         Doc("Die angegebene Tabellenzelle wird markiert.\n\n" +
             "| ``Tabellenzelle markieren     Positionsgeber     Spaltentitel``\n" +
             "Positionsgeber: Zeilennummer oder Zellinhalt.")]
        public RobotResult selectCell(string Zeilennummer_oder_Zellinhalt, string Spaltentitel) {
            return session switch {
                SAPSession session => session.selectCell(Zeilennummer_oder_Zellinhalt, Spaltentitel),
                _ => new NoSessionError()
            };
        }

        [Keyword("Auswahlmenüeintrag auswählen"),
         Doc("Aus dem angegebenen Auswahlmenü wird der angegebene Eintrag ausgewählt.\n\n" +
             "| ``Auswahlmenüeintrag auswählen    Auswahlmenü    Eintrag``")]
        public RobotResult selectComboBoxEntry(string Name, string Eintrag) {
            return session switch {
                SAPSession session => session.selectComboBoxEntry(Name, Eintrag),
                _ => new NoSessionError()
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
        public RobotResult selectRadioButton(string Beschriftung_oder_Positionsgeber) {
            return session switch {
                SAPSession session => session.selectRadioButton(Beschriftung_oder_Positionsgeber),
                _ => new NoSessionError()
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
        public RobotResult selectTextField(string Beschriftungen_oder_Inhalt) {
            return session switch {
                SAPSession session => session.selectTextField(Beschriftungen_oder_Inhalt),
                _ => new NoSessionError()
            };
        }

        [Keyword("Textzeile markieren"),
         Doc("Die Textzeile mit dem angegebenen Inhalt wird markiert.\n" +
             "| ``Textzeile markieren    Inhalt``")]
        public RobotResult selectTextLine(string Inhalt) {
            return session switch {
                SAPSession session => session.selectTextLine(Inhalt),
                _ => new NoSessionError()
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
        public RobotResult tickCheckBox(string Beschriftung_oder_Positionsgeber) {
            return session switch {
                SAPSession session => session.tickCheckBox(Beschriftung_oder_Positionsgeber),
                _ => new NoSessionError()
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
        public RobotResult untickCheckBox(string Beschriftung_oder_Positionsgeber) {
            return session switch {
                SAPSession session => session.untickCheckBox(Beschriftung_oder_Positionsgeber),
                _ => new NoSessionError()
            };
        }

        [Keyword("Tabellenzelle ankreuzen"),
         Doc("Die angegebene Tabellenzelle wird angekreuzt.\n\n" +
             "| ``Tabellenzelle ankreuzen     Zeilennummer     Spaltentitel``")]
        public RobotResult tickCheckBoxCell(string Zeilennummer, string Spaltentitel) {
            return session switch {
                SAPSession session => session.tickCheckBoxCell(Zeilennummer, Spaltentitel),
                _ => new NoSessionError()
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
