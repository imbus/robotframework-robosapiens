from typing import Callable
from schema_i18n import LocalizedRoboSAPiens

Fstr = Callable[[str], str]

sap_error = ('SAP Fehlermeldung: {0}')
no_session = 'Keine aktive SAP-Session gefunden. Das Keyword "Verbindung zum Server Herstellen" oder "Laufende SAP GUI Übernehmen" muss zuerst aufgerufen werden.'
no_sap_gui = 'Keine laufende SAP GUI gefunden. Das Keyword "SAP starten" muss zuerst aufgerufen werden.'
no_gui_scripting = 'Die Skriptunterstützung ist nicht verfügbar. Sie muss in den Einstellungen vom SAP Client aktiviert werden.'
no_connection = 'Es besteht keine Verbindung zu einem SAP Server. Versuche zuerst das Keyword "Verbindung zum Server Herstellen" aufzurufen.'
no_server_scripting = 'Das Scripting ist auf dem SAP Server nicht freigeschaltet. Siehe die Dokumentation von RoboSAPiens.'
not_found: Fstr = lambda msg: f"{msg}\nHinweis: Prüfe die Rechtschreibung"
button_or_cell_not_found: Fstr = lambda msg: f"{msg} Hinweise: Prüfe die Rechtschreibung, maximiere das SAP Fenster"
exception: Fstr = lambda msg: f"{msg}" + "\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
row_locator = 'Entweder die Zeilennummer oder der Inhalt einer Zelle in einer bestimmten Spalte im Format: Inhalt @ Spalte. Aus Gründen der Abwärtskompatibilität ist es auch möglich nur den Inhalt anzugeben und wenn die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden.'
column = "Spaltentitel oder Kurzinfo. Falls die Spaltentitel nicht eindeutig sind, siehe [#Spalten mit demselben Namen|Spalten mit demselben Namen]."
textfield_locator = "Die Lokatoren für Textfelder sind im Schlüsselwort [#Textfeld ausfüllen|Textfeld ausfüllen] dokumentiert."
path = "Rückwärtsschrägstriche müssen doppelt geschrieben werden. Ansonsten verwende die Standard RF Variable ${/} als Trennzeichen."
tooltip_hint = """Tooltips mit einem Tastenkürzel am Ende kommen oft vor. 
Der Standardwert ``exakt=False`` sorgt dafür, dass das Tastenkürzel bei der Suche vernachlässigt wird.
Für Tooltips ohne Tastenkürzel ist eher eine genaue Übereinstimmung (``exakt=True``) wünschenswert.
"""

HLabel = "::Beschriftung"
VLabel = ":@:Beschriftung oben"
HLabelVLabel = "Beschriftung:@:Beschriftung oben"
HLabelHLabel = "Beschriftung links:>>:Beschriftung"
HLabelVIndex = "Beschriftung:@:Position"
HIndexVLabel = "Position:@:Beschriftung oben"
Content = ":=:Inhalt"
ColumnContent = "Spaltentitel:=:Inhalt"

lib: LocalizedRoboSAPiens = {
    "doc": {
        "intro": ("2360204110",
        """
        RoboSAPiens: SAP GUI-Automatisierung für Menschen

        Um diese Bibliothek zu verwenden, müssen die folgenden Bedingungen erfüllt werden:

        - Das Scripting muss [https://help.sap.com/saphelp_aii710/helpdata/de/ba/b8710932b8c64a9e8acf5b6f65e740/content.htm|auf dem SAP Server aktiviert werden].
        
        - Die Skriptunterstützung muss [https://help.sap.com/docs/sap_gui_for_windows/63bd20104af84112973ad59590645513/7ddb7c9c4a4c43219a65eee4ca8db001.html?locale=de-DE|in der SAP GUI aktiviert werden].

        == Neuigkeiten in der Version 2.4 ==

        - Unterstützung für SAP Business Client
        - Dokumentation zur Automatisierung eingebetteter Browser-Steuerelemente (nur Edge) 

        == Neuigkeiten in der Version 2.0 ==

        - Unterstützung für SAP GUI 8.0 64-bit
        - Neues Schlüsselwort "Baumelement markieren"
        - Neues Schlüsselwort "Inhalte scrollen"
        - Neues Schlüsselwort "Menüeintrag auswählen"
        - Neues Schlüsselwort "Tabellenzelle abwählen"
        - Neues Schlüsselwort "Tabellenzeilen zählen"

        == Einschneidende Änderungen gegenüber der Version 1.0 ==

        - Das Schlüsselwort "Funktionsbaum exportieren" wurde in "Baumstruktur exportieren" umbenannt.
        - Das Schlüsselwort "Statusleiste auslesen" gibt ein Dictionary statt ein String zurück.
        - Das Schlüsselwort "Tabellenzelle ausfühlen" hat drei statt zwei Parameter.
        - Das Schlüsselwort "Textzeile markieren" wurde in "Text markieren" umbenannt.
        - Das Schlüsselwort "Tabellenkalkulation exportieren" wurde entfernt.
        
        == Erste Schritte ==

        Die Anmeldung bei einem SAP Server erfolgt mit der folgenden Sequenz (der Pfad muss ggf. angepasst werden):
    
        | SAP starten                         C:${/}Program Files (x86)${/}SAP${/}FrontEnd${/}SAPgui${/}saplogon.exe
        | Verbindung zum Server herstellen    Mein Testserver
        | Textfeld ausfüllen                  Benutzer           TESTUSER
        | Textfeld ausfüllen                  Kennwort           TESTPASSWORD
        | Knopf drücken                       Weiter

        Der Vortrag [https://www.youtube.com/watch?v=H7fYngdY7NI|RoboSAPiens: SAP GUI Automation for Humans] aus der Online RoboCon 2024 dient als praktisches Tutorial.

        == Umgang mit spontanen Pop-up-Fenstern ==

        Beim Drücken eines Knopfes kann u.U. ein Dialogfenster aufpoppen.
        Das folgende Schlüsselwort kann in diesem Fall hilfreich sein:

        | Knopf drücken und Pop-up-Fenster schließen
        |     [Argumente]   ${Knopf}   ${Titel}    ${Knopf Schließen}
        |  
        |     Knopf drücken     ${Knopf}
        |     ${Fenstertitel}   Fenstertitel auslesen
        |  
        |     IF   $Fenstertitel == $Titel
        |         Log                 Pop-up Fenster: ${Titel}
        |         Fenster aufnehmen   LOG
        |         Knopf drücken       ${Knopf Schließen}
        |     END

        == Automatisierung eingebetteter Browser-Steuerelemente mittels Browser Library ==

        Die folgende Umgebungsvariable muss in Windows gesetzt werden:

        | ``Name: WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS``
        | ``Wert: --enable-features=msEdgeDevToolsWdpRemoteDebugging --remote-debugging-port=4711``

        SAP Logon oder SAP Business Client starten. 
        
        In SAP Logon auf ``Optionen > Interaktionsdesign > Control-Einstellungen`` gehen. Für "Browser-Control" den Wert "Edge" auswählen.

        In SAP Business Client auf ``Einstellungen > Browser`` gehen. Für "Primäres Browser-Control" den Wert "Edge" auswählen.
        
        Beim SAP Server anmelden und eine Transaktion ausführen, in der ein oder mehrere eingebettete Browser-Steuerelemente verwendet werden.

        Einen Chromium-basierten Browser, z.B. Microsoft Edge, starten und den URL ``chrome://inspect`` abrufen.

        Auf "Configure..." klicken, den Eintrag ``localhost:4711`` hinzufügen und alle andere Einträge löschen.

        Unter der Überschrift "Remote Target" werden die Webseiten aus allen Browser-Steuerelementen aufgelistet. Der Aufruf der Entwicklertools für eine Webseite erfolgt mit einem Klick auf "inspect".

        In Robot Framework das folgende Schlüsselwort aus der [https://robotframework-browser.org/|Browser Library] aufrufen:
     
        | ``Connect To Browser   http://localhost:4711   chromium   use_cdp=True``

        Die ``id`` der zu automatisierenden Webseite mit dem folgenden Schlüsselwort ermitteln:

        | Get Page Id by Title
        |     [Arguments]    ${title}
        | 
        |     ${browsers}    Get Browser Catalog
        |     ${contexts}    Set Variable           ${browsers}[0][contexts]
        |     ${pages}       Set Variable           ${contexts}[0][pages]
        | 
        |     FOR  ${page}  IN  @{pages}
        |         IF  '${page}[title]' == '${title}'
        |             Return From Keyword    ${page}[id]
        |         END
        |     END
        |
        |     Fail    The page '${title}' is not open in the current browser.
        
        Die Webseite wie folgt aktivieren:
        
        | ``Switch Page   ${id}``

        *Hinweis*: Das gewünschte Element kann sich in einem ``iframe`` bzw. einem ``frame`` befinden. In diesem Fall muss der Lokator des (i)frame dem Lokator des Elements vorangestellt werden.
        Beispiel:

        | ``Highlight Elements    id=frameId >>> element[name=elementName]``

        == Schutz von sensiblen Daten ==
        Um zu verhindern, dass sensible Daten in das Robot Framework-Protokoll gelangen, soll die Protokollierung vor dem Aufruf sensibler Schlüsselwörter deaktiviert werden:

        | ${log_level}          Set Log Level    NONE
        | Textfeld ausfüllen    ${Lokator}       ${Kennwort}
        | Set Log Level         ${log_level}

        == Zusicherungen ==
        Das folgende Schlüsselwort unterstützt die Implementierung von Zusicherungen.
        Gültige Werte für ``${state}`` sind aktuell Found und Changeable.
        
        | Element should be ${state}
        |     [Arguments]    ${keyword}    @{args}    ${message}
        |     [Tags]         robot:flatten
        |     
        |     TRY
        |         Run Keyword    ${keyword}    @{args}
        |     EXCEPT  Not${state}: *    type=GLOB
        |         Fail    ${message}
        |     END

        Zum Beispiel, das folgende Schlüsselwort sichert zu, dass ein bestimmtes Textfeld vorhanden ist:

        | Textfeld ist vorhanden
        |     [Argumente]    ${Lokator}
        | 
        |     Element should be Found    Textfeld markieren    ${Lokator}    message=Das Textfeld '${Lokator}' ist nicht vorhanden.

        Und das folgende Schlüsselwort sichert zu, dass eine bestimmte Tabellenzelle vorhanden ist:

        | Tabellenzelle ist vorhanden
        |     [Arguments]     ${Zeile}    ${Spalte}
        | 
        |     Element should be Found    Tabellenzelle markieren    ${Zeile}    ${Spalte}    message=Die Zelle '${Zeile}, ${Spalte}' ist nicht vorhanden.

        == Spalten mit demselben Namen ==
        Wenn eine Tabelle mehrere Spalten mit demselben Namen enthält, kann eine Spalte eindeutig identifiziert werden, indem dem Namen ein numerisches Suffix hinzugefügt wird.

        Enthält eine Tabelle beispielsweise die Spalten Variante, Produkt, Variante, so können die erste "Variante" Spalte mit Variante__1 und die zweite mit Variante__2 identifiziert werden.
        
        == Tabellen als Excel-Datei exportieren ==

        Einige Tabellen können durch Anklicken eines Knopfes mit einem Kontextmenü als Excel-Datei exportiert werden. Der entsprechende Menüpunkt kann durch Aufrufen zweier Schlüsselwörter ausgewählt werden:

        | Knopf drücken                  Exportieren
        | Auswahlmenüeintrag auswählen   Exportieren   Tabellenkalkulation

        == Automatisch eine Aufnahme des Fensters machen, wenn ein Schlüsselwort fehlschlägt ==

        Robot Framework 7 bietet die Listener-Methode ``end_library_keyword``, welche die Implementierung einer Fehlerbehandlung für Bibliotheksschlüsselwörter ermöglicht.
        Im Fall von RoboSAPiens kann es sinnvoll sein, jedes Mal, wenn ein Schlüsselwort fehlschlägt, eine Aufnahme des Fensters zu machen und das Bild ins Protokoll einzubetten. 
        Um dies zu erreichen, muss die Datei Listener.py mit dem folgenden Inhalt erstellt werden:

        | from robot.api.interfaces import ListenerV3 
        | from robot.libraries.BuiltIn import BuiltIn
        | 
        | class Listener(ListenerV3):
        |     def end_library_keyword(self, data, implementation, result):
        |         library = 'RoboSAPiens.DE'
        |         if result.failed and implementation.full_name.startswith(library):
        |             robosapiens = BuiltIn().get_library_instance(library)
        |             robosapiens.save_screenshot('LOG')

        Danach muss ``robot`` wie folgt ausgeführt werden: 
        
        | ``robot -P . --listener Listener test.robot``

        == ABAP Listen ==

        Ab Version 2.21.0 funktionieren die Schlüsselwörter für den Umgang mit Tabellen auch mit dem [https://help.sap.com/docs/ABAP_PLATFORM_NEW/b1c834a22d05483b8a75710743b5ff26/4dd40b7ac2234be2e10000000a42189c.html|SAP List Viewer (Classic)].
        Dafür muss zuerst der [https://help.sap.com/docs/sap_gui_for_windows/63bd20104af84112973ad59590645513/38da185ebd1540bdbc919db7b9013c9a.html|Barrierefreiheitsmodus] in den Optionen von SAP GUI aktiviert werden.
        
        === Hierarchisch sequenzielle Listen ===

        Im Fall von einer hierarchisch sequenziellen Liste lassen sich übergeordnete (tabelle_nummer=1) und untergeordnete (tabelle_nummer=2,3,...) Tabellen anhand ihrer Farben unterscheiden.

        Das Auf- oder Zuklappen einer untergeordneten Tabelle erfolgt durch den Aufruf des Schlüsselworts "Tabellenzeile markieren". 

        Eine Zelle in einer über- oder untergeordneten Tabelle wird durch den Aufruf des Schlüsselworts "Tabellenzelle doppelklicken" angeklickt.
        
        === Baumstrukturen ===

        Ein Baumelement lässt sich durch Drücken der Taste F2 auf- oder zuklappen. Dazu muss es zunächst mit dem Schlüsselwort "Text markieren" markiert werden. Die Taste kann mit dem Schlüsselwort "Tastenkombination drücken" gedrückt werden.

        Um einen Knopf im Baum zu drücken, muss zunächst das Baumelement markiert werden. Anschließend muss die entsprechende F-Taste gedrückt werden. Hinweis: Die in der Maske verfügbaren Tastaturkürzel werden durch einen Rechtsklick auf die Maske angezeigt.
        """
        ),
        "init": ("3784687869", 
        """
        RoboSAPiens.DE hat die folgenden Initialisierungsparameter:
        
        | =Parameter= | =Beschreibung= |
        """),
    },
    "args": {
        "a1presenter_mode": {
            "name": ("781265386", "vortragsmodus"),
            "default": False,
            "desc": ("519157360", "Nach dem Aufruf eines Schlüsselworts eine halbe Sekunde warten und das betroffene GUI Element hervorheben (falls zutreffend).")
        },
        "a2x64": {
            "name": ("218858810", "x64"),
            "default": False,
            "desc": ("2623383622", "RoboSAPiens 64-bit ausführen, um SAP GUI 8 64-bit bzw. SAP Business Client zu automatisieren.")
        }
    },
    "keywords": {
        "ActivateTab": {
            "name": ("1870139227", "Reiter auswählen"),
            "args": {
              "tab": {
                "name": ("3772862467", "Reitername"),
                "desc": ("1037266374", "Name oder Kurzinfo des Reiters"),
                "spec": {}
              }
            },
            "kwargs": {},
            "result": {
              "NoSession": ("4138997384", no_session),
              "NotFound": ("1349204363", not_found("Der Reiter '{0}' wurde nicht gefunden.")),
              "Pass": ("633926115", "Der Reiter '{0}' wurde ausgewählt."),
              "Exception": ("2577256712", exception("Der Reiter konnte nicht ausgewählt werden."))
            },
            "doc": {
                "desc": ("1953544584", "Der Reiter mit dem angegebenen Namen wird ausgewählt."),
                "examples": ("439306833", 
                """
                Beispiele:

                | ``Reiter auswählen    Reitername``
                """
              )
            }
        },
        "DoubleClickTreeElement": {
            "name": ("1544352237", "Baumelement doppelklicken"),
            "args": {
                "elementPath": {
                    "name": ("1214950134", "Elementpfad"),
                    "desc": ("4061798646", "Der Pfad zum Element, mit '/' als Trennzeichen (z.B. Engineering/Bauwesen)."),
                    "spec": {},
                }
            },
            "kwargs": {},
            "result": {
                "NoSession": ("4138997384", no_session),
                "NotFound": ("3295884259", not_found("Das Baumelement '{0}' wurde nicht gefunden.")),
                "Pass": ("2589844117", "Das Baumelement '{0}' wurde doppelgeklickt."),
                "Exception": ("178748146", exception("Das Baumelement konnte nicht doppelgeklickt werden. {0}"))
            },
            "doc": {
                "desc": ("2416620933", "Das Baumelement mit dem angegebenen Pfad wird doppelgeklickt."),
                "examples": ("3491178920", 
                """
                Beispiele:
              
                | ``Baumelement doppelklicken    Elementpfad``

                Für weitere Infos zum Elementpfad siehe [#Baumelement markieren|Baumelement markieren].
                """
              )
            }
        },
        "ExpandTreeFolder": {
            "name": ("788776543", "Baumordner aufklappen"),
            "args": {
                "folderPath": {
                    "name": ("315338010", "Ordnerpfad"),
                    "desc": ("109187537", "Der Pfad zum Ordner, mit '/' als Trennzeichen (z.B. Engineering/Bauwesen)."),
                    "spec": {},
                }
            },
            "kwargs": {},
            "result": {
                "NoSession": ("4138997384", no_session),
                "NotFound": ("3001333208", not_found("Der Baumordner '{0}' wurde nicht gefunden.")),
                "Pass": ("1780102701", "Der Baumordner '{0}' wurde aufgeklappt."),
                "Exception": ("4153938705", exception("Der Baumordner konnte nicht aufgeklappt werden. {0}"))
            },
            "doc": {
                "desc": ("3001885302", "Der Ordner mit dem angegebenen Pfad in einer Baumstruktur wird aufgeklappt."),
                "examples": ("3996641622",
                """
                Beispiele:
              
                | ``Baumordner aufklappen    Ordnerpfad``

                Für weitere Infos zum Ordnerpfad siehe [#Baumelement markieren|Baumelement markieren].
                """
                )
            }
        },
        "SelectTreeElement": {
            "name": ("3234880959", "Baumelement markieren"),
            "args": {
                "elementPath": {
                    "name": ("1214950134", "Elementpfad"),
                    "desc": ("4061798646", "Der Pfad zum Element, mit '/' als Trennzeichen (z.B. Engineering/Bauwesen)."),
                    "spec": {},
                }
            },
            "kwargs": {},
            "result": {
                "NoSession": ("4138997384", no_session),
                "NotFound": ("3295884259", not_found("Das Baumelement '{0}' wurde nicht gefunden.")),
                "Pass": ("40521386", "Das Baumelement '{0}' wurde markiert."),
                "Exception": ("4212594444", exception("Das Baumelement konnte nicht markiert werden. {0}"))
            },
            "doc": {
                "desc": ("1640119296", "Das Baumelement mit dem angegebenen Pfad wird markiert."),
                "examples": ("3248861206", 
                """
                Beispiele:
              
                | ``Baumelement markieren    Elementpfad``

                *Hinweise*
                - Ein Schrägstrich, der nicht als Trennzeichen verwendet wird, muss doppelt geschrieben werden.
                - Jedes Segment des Pfades kann teilweise angegeben werden. Zum Beispiel IDoc anstelle von IDoc 1234.
                """
              )
            }
        },
        "SelectTreeElementMenuEntry": {
            "name": ("1573180260", "Menüeintrag in Baumelement auswählen"),
            "args": {
                "a1elementPath": {
                    "name": ("1214950134", "Elementpfad"),
                    "desc": ("2154399540", "Der Pfad zum Element, mit '/' als Trennzeichen (z.B. Engineering/Bauwesen)."),
                    "spec": {},
                },
                "a2menuEntry": {
                    "name": ("3129851415", "Menüeintrag"),
                    "desc": ("2845074501", "Der Menüeintrag. Bei verschachtelten Menüs der Pfad zum Eintrag mit '|' als Trennzeichen (z.B. Anlegen|Wirtschaftseinheit)."),
                    "spec": {},
                },
            },
            "kwargs": {},
            "result": {
                "NoSession": ("4138997384", no_session),
                "NotFound": ("3295884259", not_found("Das Baumelement '{0}' wurde nicht gefunden.")),
                "Pass": ("200907440", "Der Menüeintrag '{0}' wurde ausgewählt."),
                "Exception": ("3089619033", exception("Der Menüeintrag konnte nicht ausgewählt werden. {0}"))
            },
            "doc": {
                "desc": ("2300654272", "Aus dem Kontextmenü des Baumelements mit dem angegebenen Pfad wird der angebene Eintrag ausgewählt."),
                "examples": ("4134001137", 
                """
                Beispiele:
              
                | ``Menüeintrag in Baumelement auswählen    Elementpfad    Menüeintrag``

                Für weitere Infos zum Elementpfad siehe [#Baumelement markieren|Baumelement markieren].
                """
              )
            }
        },
        "OpenSap": {
            "name": ("1259182241", "SAP starten"),
            "args": {
              "path": {
                "name": ("190089999", "Pfad"),
                "desc": ("4080190508", "Der Pfad zu saplogon.exe oder NWBC.exe"),
                "spec": {},
              }
            },
            "kwargs": {
                "sapArgs": {
                  "name": ("3959597614", "SAP_Parameter"),
                  "desc": ("4215926071", "Kommandozeileparameter für den SAP Client"),
                  "default": None,
                  "type": "str",
                  "spec": {}
              }
            },
            "result": {
              "Pass": ("871506365", "SAP wurde gestartet"),
              "NoGuiScripting": ("3820273098", no_gui_scripting),
              "SAPAlreadyRunning": ("1618084077", "SAP läuft gerade. Es muss vor dem Aufruf dieses Schlüsselworts beendet werden."),
              "SAPNotStarted": ("3387725186", "SAP konnte nicht gestartet werden. Überprüfe den Pfad und ggf. die Parameter '{0}'."),
              "Exception": ("1731801036", exception("SAP konnte nicht gestartet werden. Hinweis: Um einen 64-bit SAP Client zu starten, muss RoboSAPiens.DE mit x64=True importiert werden."))
            },
            "doc": {
                "desc": ("211145309", "SAP GUI bzw. SAP Business Client wird gestartet."),
                "examples": ("1700015374", 
                rf"""
                Beispiele:

                *Den SAP Client starten*
                
                | ``SAP starten   Pfad``
 
                Der übliche Pfad für SAP Logon 32-bit ist 
                
                | ``C:\\Program Files (x86)\\SAP\\FrontEnd\\SAPgui\\saplogon.exe``
                
                Der übliche Pfad für SAP Logon 64-bit ist 
                
                | ``C:\\Program Files\\SAP\\FrontEnd\\SAPgui\\saplogon.exe``

                Der übliche Pfad für SAP Business Client ist 

                | ``C:\\Program Files\\SAP\\NWBC800\\NWBC.exe``

                *SAP Logon bereits bei einem Mandanten angemeldet starten*

                | ``SAP starten   C:\\Program Files\\SAP\\FrontEnd\\SAPgui\\sapshcut.exe -sysname=XXX -client=NNN -user=%{{username}} -pw=%{{password}}``

                *Hinweise*: 
                
                - {path}
                - 64-bit SAP-Clients erfordern, dass die Bibliothek mit ``x64=True`` importiert wird
                - sysname ist der Name der Verbindung in SAP Logon. Wenn es Leerzeichen enthält, muss es in Anführungszeichen gesetzt werden.
                """
                )
            }
        },
        "CloseConnection": {
            "name": ("938374979", "Verbindung zum Server trennen"),
            "args": {},
            "kwargs": {},
            "result": {
              "NoSession": ("4138997384", no_session),
              "Pass": ("1657006605", "Die Verbindung zum Server wurde getrennt."),
              "Exception": ("2209141929", exception("Die Verbindung zum Server konnte nicht getrennt werden."))
            },
            "doc": {
                "desc": ("3753666144", "Die aktuelle Verbindung zum SAP Server wird getrennt."),
                "examples": ("3240780684", 
                """
                Beispiele:
                
                | ``Verbindung zum Server trennen``
                """
              )
            }
        },
        "CloseSap": {
            "name": ("1795765665", "SAP beenden"),
            "args": {},
            "kwargs": {},
            "result": {
              "NoSapGui": ("2987622841", no_sap_gui),
              "Pass": ("2970606098", "Die SAP GUI wurde beendet")
            },
            "doc": {
                "desc": ("1925934178", "Schließt die SAP GUI und beendet ihren Prozess."),
                "examples": ("1734451664", 
                """
                Beispiele:
                
                | ``SAP beenden``

                *Hinweis*: Dieses Schlüsselwort funktioniert nur, wenn die SAP GUI mit dem Schlüsselwort [#SAP starten|SAP starten] gestartet wurde.
                """
              )
            }
        },
        "CloseWindow": {
            "name": ("3843607926", "Fenster schließen"),
            "args": {},
            "kwargs": {},
            "result": {
              "NoSession": ("4138997384", no_session),
              "Pass": ("320353308", "Das Fenster im Vordergrund wurde geschlossen."),
              "Exception": ("1090626587", exception("Das Fenster konnte nicht geschlossen werden."))
            },
            "doc": {
                "desc": ("3032332168", "Das Fenster im Vordergrund wird geschlossen."),
                "examples": ("3865488397", 
                """
                Beispiele:
                
                | ``Fenster schließen``
                """
              )
            }
        },
        "CountTableRows": {
            "name": ("2280342727", "Tabellenzeilen zählen"),
            "args": {},
            "kwargs": {
              "tableNumber": {
                  "name": ("3359775383", "tabelle_nummer"),
                  "desc": ("4055958951", "Spezifiziert welche Tabelle: 1, 2, ..."),
                  "default": 1,
                  "type": "int",
                  "spec": {}
              }
            },
            "result": {
                "NoSession": ("4138997384", no_session),
                "Exception": ("1934626298", exception("Die Zeilen der Tabelle konnten nicht gezählt werden.")),
                "NotFound": ("2399256699", "Die Maske enthält keine Tabelle."),
                "InvalidTable": ("2205241003", "Die Maske enthält keine Tabelle mit dem Index {0}."),
                "Pass": ("1614075368", "Die Tabellenzeillen wurden gezählt.")
            },
            "doc": {
                "desc": ("1891631161", "Die Zeilen einer Tabelle werden gezählt."),
                "examples": ("2422780087", 
                """
                Beispiele:
              
                | ``${anzahl_zeilen}    Tabellenzeilen zählen``
                """
              )
            }
        },
        "ExportTree": {
            "name": ("23518835", "Baumstruktur exportieren"),
            "args": {
              "filepath": {
                "name": ("1769741420", "Dateipfad"),
                "desc": ("1664248363", "Absoluter Pfad zu einer Datei mit Endung .json"),
                "spec": {},
              }
            },
            "kwargs": {},
            "result": {
              "NoSession": ("4138997384", no_session),
              "NotFound": ("492802637", "Die Maske enthält keine Baumstruktur"),
              "Pass": ("2272403949", "Die Baumstruktur wurde in JSON Format in der Datei '{0}' gespeichert"),
              "Exception": ("3215052656", exception("Die Baumstruktur konnte nicht exportiert werden."))
            },
            "doc": {
                "desc": ("1676608340", "Die Baumstruktur in der Maske wird in JSON Format in der angegebenen Datei gespeichert."),
                "examples": ("2602036160", 
                f"""
                Beispiele:
                
                | ``Baumstruktur exportieren     Dateipfad``

                *Hinweis*: {path}
                """
              )
            }
        },
        "ConnectToRunningSap": {
            "name": ("4126309856", "Laufende SAP GUI übernehmen"),
            "args": {},
            "kwargs": {
                "sessionNumber": {
                    "name": ("4193981709", "session_nummer"),
                    "desc": ("1605605041", "Die Nummer der SAP-Session in der rechten oberen oder unteren Ecke des Fensters"),
                    "default": 1,
                    "type": "int",
                    "spec": {}
                },
                "connectionName": {
                    "name": ("704082790", "Verbindung"),
                    "desc": ("2063424522", "Der Name der Verbindung in SAP Logon (nicht der SID)"),
                    "default": None,
                    "type": "str",
                    "spec": {}
                },
                "client": {
                    "name": ("3343123541", "Mandant"),
                    "desc": ("575196918", "Der dreistellige Mandant"),
                    "default": None,
                    "type": "str",
                    "spec": {}
                }
            },
            "result": {
              "NoSapGui": ("3729995647", "Keine laufende SAP GUI gefunden."),
              "NoGuiScripting": ("3820273098", no_gui_scripting),
              "NoConnection": ("509780556", no_connection),
              "NoSession": ("4138997384", no_session),
              "NoServerScripting": ("3495213352", no_server_scripting),
              "InvalidClient": ("2278835198", "Es gibt keinen Mandanten '{Mandant}' bei der aktuellen Verbindung."),
              "InvalidConnection": ("68035031", "Es gibt keine Verbindung mit dem Namen '{Verbindung}'."),
              "InvalidConnectionClient": ("2496471747", "Es gibt keinen Mandanten '{Mandant}' bei der Verbindung '{Verbindung}'."),
              "InvalidSession": ("3727388681", "Die aktuelle Verbindung hat keine Session '{session_nummer}'."),
              "SapError": ("3246364722", sap_error),
              "Json": ("144359828", "Der Rückgabewert ist im JSON-Format"),
              "Pass": ("2481655346", "Die laufende SAP GUI wurde erfolgreich übernommen."),
              "Exception": ("3410975181", exception("Die laufende SAP GUI konnte nicht übernommen werden. Hinweis: Für die Verbindung mit einem 64-bit SAP Client muss RoboSAPiens.DE mit x64=True importiert werden."))
            },
            "doc": {
                "desc": ("3032107536", "Nach der Ausführung dieses Schlüsselworts kann eine bereits laufende SAP GUI mit RoboSAPiens gesteuert werden."),
                "examples": ("1120000879", 
                """
                Beispiele:
                
                | ``Laufende SAP GUI übernehmen``

                Standardmäßig wird die Session Nummer 1 verwendet. Die gewünschte Session-Nummer kann als Parameter spezifiziert werden.

                | ``Laufende SAP GUI übernehmen    session_nummer=N``

                Eine Session bei einer bestimmten Verbindung kann auch spezifiert werden.

                | ``Laufende SAP GUI übernehmen    Verbindung=Test Verbindung    session_nummer=N``

                Ein Mandant bei einer bestimmten Verbindung kann auch spezifiert werden.

                | ``Laufende SAP GUI übernehmen    Verbindung=Test Verbindung    Mandant=NNN``

                Der Rückgabewert enthält Informationen über die Session wie z.B. Mandant und System-ID.
                """
              )
            }
        },
        "ConnectToServer": {
            "name": ("1377779562", "Verbindung zum Server herstellen"),
            "args": {
              "server": {
                "name": ("3584233446", "Servername"),
                "desc": ("4054806856", "Der Name der Verbindung in SAP Logon (nicht der SID)"),
                "spec": {},
              }
            },
            "kwargs": {},
            "result": {
              "NoSapGui": ("2987622841", no_sap_gui),
              "NoGuiScripting": ("3820273098", no_gui_scripting),
              "Pass": ("1441526843", "Die Verbindung '{0}' wurde erfolgreich hergestellt."),
              "Json": ("144359828", "Der Rückgabewert ist im JSON-Format"),
              "SapError": ("3246364722", sap_error),
              "NoServerScripting": ("3495213352", no_server_scripting),
              "InvalidSession": ("3314696332", "Die aktuelle Verbindung hat keine Session '{0}'."),
              "Exception": ("667377482", exception("Die Verbindung konnte nicht hergestellt werden."))
            },
            "doc": {
                "desc": ("1003283283", "Die angegebene Verbindung mit einem SAP Server wird hergestellt."),
                "examples": ("2919365748", 
                """
                Beispiele:
                
                | ``Verbindung zum Server herstellen    Servername``

                Der Rückgabewert enthält Informationen über die Session wie z.B. Mandant und System-ID.
                """
              )
            }
        },
        "DoubleClickCell": {
            "name": ("2108476291", "Tabellenzelle doppelklicken"),
            "args": {
              "a1row_locator": {
                "name": ("315353924", "Zeile"),
                "desc": ("2714474921", row_locator),
                "spec": {},
              },
              "a2column": {
                "name": ("2102626174", "Spaltentitel"),
                "desc": ("245054585", column),
                "spec": {},
              },
            },
            "kwargs": {
              "tableNumber": {
                  "name": ("3359775383", "tabelle_nummer"),
                  "desc": ("4055958951", "Spezifiziert welche Tabelle: 1, 2, ..."),
                  "default": None,
                  "type": "int",
                  "spec": {}
              }
            },
            "result": {
              "NoSession": ("4138997384", no_session),
              "NotFound": ("3279676079", button_or_cell_not_found("Die Zelle mit dem Lokator '{0}, {1}' wurde nicht gefunden.")),
              "InvalidTable": ("2205241003", "Die Maske enthält keine Tabelle mit dem Index {0}."),
              "Pass": ("4023459711", "Die Zelle mit dem Lokator '{0}, {1}' wurde doppelgeklickt."),
              "Exception": ("2384367029", exception("Die Zelle konnte nicht doppelgeklickt werden."))
            },
            "doc": {
                "desc": ("3460694629", "Führt einen Doppelklick auf die Zelle aus, die an der Schnittstelle der gegebenen Zeile und Spalte liegt."),
                "examples": ("142831630", 
                f"""
                Beispiele:
                
                | ``Tabellenzelle doppelklicken     Zeile     Spaltentitel``
                """
              )
            }
        },
        "DoubleClickTextField": {
            "name": ("3737103423", "Textfeld doppelklicken"),
            "args": {
              "locator": {
                "name": ("2051440239", "Lokator"),
                "desc": ("1257056815", textfield_locator),
                "spec": {
                  "Content": ("880934240", Content),
                  "HLabel": ("4229670492", HLabel),
                  "VLabel": ("474824962", VLabel),
                  "HLabelVLabel": ("1999142431", HLabelVLabel),
                  "HLabelHLabel": ("3678957963", HLabelHLabel),
                  "HLabelVIndex": ("509417766", HLabelVIndex),
                  "HIndexVLabel": ("3606518505", HIndexVLabel)
                },

              }
            },
            "kwargs": {},
            "result": {
              "NoSession": ("4138997384", no_session),
              "NotFound": ("1367926790", not_found("Das Textfeld mit dem Lokator '{0}' wurde nicht gefunden.")),
              "Pass": ("727284193", "Das Textfeld mit dem Lokator '{0}' wurde doppelgeklickt."),
              "Exception": ("504842288", exception("Das Textfeld konnte nicht doppelgeklickt werden."))
            },
            "doc": {
                "desc": ("4159310422", "Führt einen Doppelklick auf das angegebene Textfeld aus."),
                "examples": ("573214453", 
                """
                Beispiele:
                
                *Identifizierung des Textfeldes über einen Lokator*
                | ``Textfeld doppelklicken     Lokator``

                *Identifizierung des Textfeldes über seinen Inhalt*
                | ``Textfeld doppelklicken     = Inhalt``
                """
              )
            }
        },
        "ExecuteTransaction": {
            "name": ("2997404008", "Transaktion ausführen"),
            "args": {
              "T_Code": {
                "name": ("1795027938", "T_Code"),
                "desc": ("3654702433", "Der Code der Transaktion"),
                "spec": {},
              }
            },
            "kwargs": {},
            "result": {
              "NoSession": ("4138997384", no_session),
              "Pass": ("468573121", "Die Transaktion mit T-Code '{0}' wurde erfolgreich ausgeführt."),
              "Exception": ("3958687903", exception("Die Transaktion konnte nicht ausgeführt werden."))
            },
            "doc": {
                "desc": ("3379948688", "Die Transaktion mit dem angegebenen T-Code wird ausgeführt."),
                "examples": ("3858705015", 
                """
                Beispiele:
                
                | ``Transaktion ausführen    T-Code``
                """
              )
            }
        },
        "ExportWindow": {
            "name": ("1462144723", "Maske exportieren"),
            "args": {
              "a1name": {
                "name": ("1579384326", "Name"),
                "desc": ("216390163", "Der Name der generierten Dateien"),
                "spec": {},
              },
              "a2directory": {
                "name": ("1182287066", "Verzeichnis"),
                "desc": ("3819401787", "Der absolute Pfad des Verzeichnisses, wo die Dateien gespeichert werden."),
                "spec": {},
              }
            },
            "kwargs": {},
            "result": {
              "NoSession": ("4138997384", no_session),
              "Pass": ("620190076", "Die Maske wurde in den Dateien '{0}' und '{1}' gespeichert"),
              "Exception": ("1068065483", exception("Die Maske konnte nicht exportiert werden."))
            },
            "doc": {
                "desc": ("2766864209", "Die Inhalte der Maske werden in einer JSON-Datei geschrieben. Außerdem wird ein Bildschirmfoto automatisch in PNG-Format erstellt."),
                "examples": ("544925055", 
                f"""
                Beispiele:
                
                | ``Maske exportieren     Name     Verzeichnis``
                
                *Hinweis*: {path}

                *Anmerkung*: Aktuell werden nicht alle GUI-Elemente exportiert.
                """
              )
            }
        },
        "FillCell": {
            "name": ("1010164935", "Tabellenzelle ausfüllen"),
            "args": {
              "a1row_locator": {
                "name": ("315353924", "Zeile"),
                "desc": ("2714474921", row_locator),
                "spec": {},
              },
              "a2column": {
                "name": ("2102626174", "Spaltentitel"),
                "desc": ("245054585", column),
                "spec": {
                },
              },
              "a3content": {
                  "name": ("4274335913", "Inhalt"),
                  "desc": ("449567510", "Der neue Inhalt der Zelle"),
                  "spec": {},
              }
            },
            "kwargs": {
              "tableNumber": {
                  "name": ("3359775383", "tabelle_nummer"),
                  "desc": ("4055958951", "Spezifiziert welche Tabelle: 1, 2, ..."),
                  "default": None,
                  "type": "int",
                  "spec": {}
              } 
            },
            "result": {
              "NoSession": ("4138997384", no_session),
              "NotFound": ("807131089", button_or_cell_not_found("Die Zelle mit dem Lokator '{0}, {1}' wurde nicht gefunden.")),
              "NotChangeable": ("2520536917", "Die Zelle mit dem Lokator '{0}, {1}' ist nicht bearbeitbar."),
              "NoTable": ("2399256699", "Die Maske enthält keine Tabelle."),
              "InvalidTable": ("2205241003", "Die Maske enthält keine Tabelle mit dem Index {0}."),
              "Pass": ("2876607603", "Die Zelle mit dem Lokator '{0}, {1}' wurde ausgefüllt."),
              "Exception": ("1958379303", exception("Die Zelle konnte nicht ausgefüllt werden."))
            },
            "doc": {
                "desc": ("958131390", "Die Zelle am Schnittpunkt der Zeile und der Spalte wird mit dem angegebenen Inhalt ausgefüllt."),
                "examples": ("646876225", 
                """
                Beispiele:
                
                | ``Tabellenzelle ausfüllen     Zeile     Spaltentitel     Inhalt``
                
                *Hinweis*: Für die Migration aus dem alten Schlüsselwort mit zwei Argumenten soll eine Suche und Ersetzung mit einem regulären Ausdruck durchgeführt werden.
                """
              )
            }
        },
        "FillTextEdit": {
            "name": ("896071987", "Mehrzeiliges Textfeld ausfüllen"),
            "args": {
                "content": {
                    "name": ("4274335913", "Inhalt"),
                    "desc": ("2819333797", "Der neue Inhalt des mehrzeiligen Textfelds"),
                    "spec": {}
                }
            },
            "kwargs": {},
            "result": {
                "NoSession": ("4138997384", no_session),
                "NotFound": ("2220996720", not_found("Die Maske enthält kein mehrzeiliges Textfeld.")),
                "NotChangeable": ("1635324573", "Das mehrzeilige Textfeld ist nicht bearbeitbar."),
                "Pass": ("3073974581", "Das mehrzeilige Textfeld wurde ausgefüllt."),
                "Exception": ("3230145683", exception("Das mehrzeilige Textfeld konnte nicht ausgefüllt werden. {0}"))
            },
            "doc": {
                "desc": ("2252184307", "Das mehrzeilige Textfeld in der Maske wird mit dem angegebenen Inhalt ausgefüllt."),
                "examples": ("1933559513",
                """
                Beispiele:
                
                | ``Mehrzeiliges Textfeld ausfüllen    Ein langer Text. Mit zwei Sätzen.``
                """
                )
            }
        },
        "FillTextField": {
            "name": ("3103200585", "Textfeld ausfüllen"),
            "args": {
              "a1locator": {
                "name": ("2051440239", "Lokator"),
                "desc": ("1317994941", "Ein Lokator, um das Textfeld zu finden"),
                "spec": {
                    "HLabel": ("4229670492", HLabel),
                    "VLabel": ("474824962", VLabel),
                    "HLabelVLabel": ("1999142431", HLabelVLabel),
                    "HLabelHLabel": ("3678957963", HLabelHLabel),
                    "HLabelVIndex": ("509417766", HLabelVIndex),
                    "HIndexVLabel": ("3606518505", HIndexVLabel)
                },
              },
              "a2content": {
                "name": ("4274335913", "Inhalt"),
                "desc": ("3646718294", "Der neue Inhalt des Textfelds"),
                "spec": {},
              },
            },
            "kwargs": {
              "exact": {
                  "name": ("1775676165", "exakt"),
                  "desc": ("82873837", "Entweder eine genaue oder eine partielle Übereinstimmung mit dem Lokator."),
                  "default": True,
                  "type": "bool",
                  "spec": {}
              }
            },
            "result": {
              "NoSession": ("4138997384", no_session),
              "NotFound": ("2917845132", not_found("Das Textfeld mit dem Lokator '{0}' wurde nicht gefunden.")),
              "NotChangeable": ("3389129718", "Das Textfeld mit dem Lokator '{0}' ist nicht bearbeitbar."),
              "Pass": ("1361669956", "Das Textfeld mit dem Lokator '{0}' wurde ausgefüllt."),
              "Exception": ("3667643114", exception("Das Textfeld konnte nicht ausgefüllt werden. Möglicherweise, weil der Inhalt nicht dazu passt."))
            },
            "doc": {
                "desc": ("3165034876", "Das angegebene Textfeld wird mit dem angegebenen Inhalt ausgefüllt."),
                "examples": ("1211580635", 
                """
                Beispiele:
                
                *Textfeld mit einer Beschriftung links*
                | ``Textfeld ausfüllen    Beschriftung    Inhalt``
                
                *Hinweis*: Die Beschreibung, die durch die Auswahl eines Textfeldes und Drücken von F1 erscheint, kann ebenfalls als Beschriftung genutzt werden. Wenn sie zu lang ist, kann der Anfang verwendet werden, indem exakt=False gesetzt wird.

                *Textfeld mit einer Beschriftung oben*
                | ``Textfeld ausfüllen    @ Beschriftung    Inhalt``
                
                *Textfeld am Schnittpunkt einer Beschriftung links und einer oben (inkl. eine Kastenüberschrift)*
                | ``Textfeld ausfüllen    Beschriftung links @ Beschriftung oben    Inhalt``
                
                *Textfeld in einem vertikalen Raster unter einer Beschriftung*
                | ``Textfeld ausfüllen    Position (1,2,..) @ Beschriftung    Inhalt``
                
                *Textfeld in einem horizontalen Raster nach einer Beschriftung*
                | ``Textfeld ausfüllen    Beschriftung @ Position (1,2,..)    Inhalt``
                
                *Textfeld mit einer nicht eindeutigen Beschriftung links oder rechts von einer eindeutigen Beschriftung*
                | ``Textfeld ausfüllen    eindeutige Beschriftung >> Textfeld-Beschriftung    Inhalt``
                
                *Textfeld ohne Beschriftung links oder rechts von einer eindeutigen Beschriftung*
                | ``Textfeld ausfüllen    eindeutige Beschriftung >> F1 Hilfetext    Inhalt``

                *Als letzter Ausweg kann der mit [https://tracker.stschnell.de/|Scripting Tracker] ermittelte Name verwendet werden*
                | ``Textfeld ausfüllen    Name    Inhalt``
                """
              )
            }
        },
        "HighlightButton": {
            "name": ("2180269929", "Knopf hervorheben"),
            "args": {
              "button": {
                "name": ("2051440239", "Lokator"),
                "desc": ("2400215713", "Name oder Kurzinfo (Tooltip) des Knopfes"),
                "spec": {},
              },

            },
            "kwargs": {
              "exact": {
                "name": ("1775676165", "exakt"),
                "desc": ("4137356431", "`True` wenn der Lokator und die Kurzinfo genau übereinstimmen, sonst `False`"),
                "default": False,
                "type": "bool",
                "spec": {},
              }
            },
            "result": {
              "NoSession": ("4138997384", no_session),
              "NotFound": ("3710914902", button_or_cell_not_found("Der Knopf '{0}' wurde nicht gefunden.")),
              "Pass": ("2149046612", "Der Knopf '{0}' wurde hervorgehoben."),
              "Exception": ("1973912995", exception("Der Knopf konnte nicht hervorgehoben werden."))
            },
            "doc": {
                "desc": ("1502722601", "Der Knopf mit dem angegebenen Lokator wird hervorgehoben."),
                "examples": ("3155266330", 
                f"""
                Beispiele:
                
                | ``Knopf hervorheben    Lokator``

                *Hinweis*: {tooltip_hint}
                """
              )
            }
        },
        "PressKeyCombination": {
            "name": ("2814882499", "Tastenkombination drücken"),
            "args": {
                "keyCombination": {
                    "name": ("2238126572", "Tastenkombination"),
                    "desc": ("3473771241", "Entweder eine Taste oder mehrere Tasten mit '+' als Trennzeichen"),
                    "spec": {},
                }
            },
            "kwargs": {
                "tableNumber": {
                    "name": ("3359775383", "tabelle_nummer"),
                    "default": None,
                    "type": "int",
                    "desc": ("4139410820", "Welche Tabelle (1, 2, ...) den Tastendruck empfangen soll."),
                    "spec": {}
                }
            },
            "result": {
                "NoSession": ("4138997384", no_session),
                "Exception": ("3975296116", exception("Die Tastenkombination konnte nicht gedrückt werden.")),
                "NotFound": ("2294374604", "Die Tastenkombination '{0}' ist nicht vorhanden. Siehe die Dokumentation des Schlüsselworts für die Liste der zulässigen Tastenkombinationen."),
                "InvalidTable": ("2978869853", "Die Maske enthält keine Tabelle mit dem Index {tabelle_nummer}."),
                "Pass": ("3497306705", "Die Tastenkombination '{0}' wurde gedrückt.")
            },
            "doc": {
                "desc": ("4221864820", "Die angegebene Tastenkombination (mit englischen Tastenbezeichnungen) wird gedrückt."),
                "examples": ("2077449164",
                """
                Beispiele:
              
                | ``Tastenkombination drücken    Tastenkombination``

                Gültige Tastenkombinationen sind unter anderem die Tastenkürzel im Kontextmenü (angezeigt, wenn die rechte Maustaste gedrückt wird). 
                Die vollständige Liste der zulässigen Tastenkombinationen ist in der [https://help.sap.com/docs/sap_gui_for_windows/b47d018c3b9b45e897faf66a6c0885a8/71d8c95e9c7947ffa197523a232d8143.html?locale=de-DE|Dokumentation von SAP GUI].

                *Hinweis*: Das Drücken der Taste F2 hat die gleiche Wirkung wie ein Doppelklick.
                """
              )
            }
        },
        "PushButton": {
            "name": ("2326550334", "Knopf drücken"),
            "args": {
              "button": {
                "name": ("2051440239", "Lokator"),
                "desc": ("2400215713", "Name oder Kurzinfo (Tooltip) des Knopfes"),
                "spec": {},
              },

            },
            "kwargs": {
              "exact": {
                "name": ("1775676165", "exakt"),
                "desc": ("4137356431", "`True` wenn der Lokator und die Kurzinfo genau übereinstimmen, sonst `False`."),
                "default": False,
                "type": "bool",
                "spec": {},
              },
              "tableNumber": {
                    "name": ("3359775383", "tabelle_nummer"),
                    "desc": ("97624453", "Die Tabelle (1, 2, ...), in deren Symbolleiste sich der Knopf befindet."),
                    "default": None,
                    "type": "int",
                    "spec": {}
                }
            },
            "result": {
              "NoSession": ("4138997384", no_session),
              "NotFound": ("3710914902", button_or_cell_not_found("Der Knopf '{0}' wurde nicht gefunden.")),
              "NotChangeable": ("258793702", "Der Knopf '{0}' ist deaktiviert."),
              "Pass": ("2346783035", "Der Knopf '{0}' wurde gedrückt."),
              "Exception": ("1002997848", exception("Der Knopf konnte nicht gedrückt werden."))
            },
            "doc": {
                "desc": ("1468743868", "Der Knopf mit dem angegebenen Lokator wird gedrückt."),
                "examples": ("635984313", 
                f"""
                Beispiele:

                *Knopf mit Namen oder Kurzinfo*
                
                | ``Knopf drücken    Name oder Kurzinfo``

                *Knopf mit einem nicht eindeutigen Namen oder Kurzinfo links oder rechts von einer eindeutigen Beschriftung*

                | ``Knopf drücken    eindeutige Beschriftung >> Name oder Kurzinfo``
                
                *Hinweis*: {tooltip_hint}
                """
              )
            }
        },
        "PushButtonCell": {
            "name": ("349686496", "Tabellenzelle drücken"),
            "args": {
              "a1row_or_label": {
                "name": ("315353924", "Zeile"),
                "desc": ("2347022671", row_locator),
                "spec": {},
              },
              "a2column": {
                "name": ("2102626174", "Spaltentitel"),
                "desc": ("245054585", column),
                "spec": {},
              }
            },
            "kwargs": {
               "tableNumber": {
                  "name": ("3359775383", "tabelle_nummer"),
                  "desc": ("4055958951", "Spezifiziert welche Tabelle: 1, 2, ..."),
                  "default": None,
                  "type": "int",
                  "spec": {}
              } 
            },
            "result": {
              "NoSession": ("4138997384", no_session),
              "NotFound": ("3485811037", button_or_cell_not_found("Die Zelle mit dem Lokator '{0}, {1}' wurde nicht gefunden.")),
              "NotAButton": ("2476489266", "Die Zelle mit dem Lokator '{0}, {1}' ist kein Knopf."),
              "NotChangeable": ("333463039", "Die Zelle mit dem Lokator '{0}, {1}' ist deaktiviert."),
              "InvalidTable": ("2205241003", "Die Maske enthält keine Tabelle mit dem Index {0}."),
              "Pass": ("1801284202", "Die Zelle mit dem Lokator '{0}, {1}' wurde gedrückt."),
              "Exception": ("1751102722", exception("Die Zelle konnte nicht gedrückt werden."))
            },
            "doc": {
                "desc": ("3745182647", "Die Zelle am Schnittpunkt der Zeile und der Spalte wird gedrückt."),
                "examples": ("1386630856", 
                """
                Beispiele:
                
                | ``Tabellenzelle drücken     Zeile     Spaltentitel``
                """
              )
            }
        },
        "ReadTextField": {
            "name": ("490498248", "Textfeld auslesen"),
            "args": {
              "locator": {
                "name": ("2051440239", "Lokator"),
                "desc": ("1257056815", textfield_locator),
                "spec": {
                    "HLabel": ("4229670492", HLabel),
                    "VLabel": ("474824962", VLabel),
                    "HLabelVLabel": ("1999142431", HLabelVLabel),
                    "Content": ("880934240", Content)
                },
              }
            },
            "kwargs": {},
            "result": {
              "NoSession": ("4138997384", no_session),
              "NotFound": ("2917845132", not_found("Das Textfeld mit dem Lokator '{0}' wurde nicht gefunden.")),
              "Pass": ("2524131110", "Das Textfeld mit dem Lokator '{0}' wurde ausgelesen."),
              "Exception": ("2613451948", exception("Das Textfeld konnte nicht ausgelesen werden."))
            },
            "doc": {
                "desc": ("2269953849", "Der Inhalt des angegebenen Textfeldes wird zurückgegeben."),
                "examples": ("3815162586", 
                """
                Beispiele:
                
                | ${Inhalt}   ``Textfeld auslesen    Lokator``
                """
              )
            }
        },
        "ReadText": {
            "name": ("3879608701", "Text auslesen"),
            "args": {
              "locator": {
                "name": ("2051440239", "Lokator"),
                "desc": ("4059025231", "Ein Lokator, um den Text zu finden"),
                "spec": {
                    "HLabel": ("4229670492", HLabel),
                    "Content": ("880934240", Content)
                },
              }
            },
            "kwargs": {},
            "result": {
              "NoSession": ("4138997384", no_session),
              "NotFound": ("837183792", not_found("Der Text mit dem Lokator '{0}' wurde nicht gefunden.")),
              "Pass": ("1360175273", "Der Text mit dem Lokator '{0}' wurde ausgelesen."),
              "Exception": ("3136337781", exception("Der Text konnte nicht ausgelesen werden."))
            },
            "doc": {
                "desc": ("4185497902", "Der Inhalt des angegebenen Texts wird zurückgegeben."),
                "examples": ("2779683906", 
                """
                Beispiele:
                
                *Text beginnt mit der angegebenen Teilzeichenfolge*
                | ``${Text}   Text auslesen    = Teilzeichenfolge``
                
                *Text folgt einer Beschriftung*
                | ``${Text}   Text auslesen    Beschriftung``
                """
              )
            }
        },
        "ReadCell": {
            "name": ("389153112", "Tabellenzelle auslesen"),
            "args": {
              "a1row_locator": {
                "name": ("315353924", "Zeile"),
                "desc": ("2714474921", row_locator),
                "spec": {},
              },
              "a2column": {
                "name": ("2102626174", "Spaltentitel"),
                "desc": ("245054585", column),
                "spec": {},
              }
            },
            "kwargs": {
                "tableNumber": {
                  "name": ("3359775383", "tabelle_nummer"),
                  "desc": ("4055958951", "Spezifiziert welche Tabelle: 1, 2, ..."),
                  "default": None,
                  "type": "int",
                  "spec": {}
              } 
            },
            "result": {
              "NoSession": ("4138997384", no_session),
              "NotFound": ("3279676079", button_or_cell_not_found("Die Zelle mit dem Lokator '{0}, {1}' wurde nicht gefunden.")),
              "NoTable": ("2399256699", "Die Maske enthält keine Tabelle."),
              "InvalidTable": ("2205241003", "Die Maske enthält keine Tabelle mit dem Index {0}."),
              "Pass": ("4222878164", "Die Zelle mit dem Lokator '{0}, {1}' wurde ausgelesen."),
              "Exception": ("1272098876", exception("Die Zelle konnte nicht ausgelesen werden."))
            },
            "doc": {
                "desc": ("854195584", "Der Inhalt der Zelle am Schnittpunkt der Zeile und der Spalte wird zurückgegeben."),
                "examples": ("2051107884", 
                """
                Beispiele:
                
                | ``Tabellenzelle auslesen     Zeile     Spaltentitel``
                """
              )
            }
        },
        "SaveScreenshot": {
            "name": ("2178392450", "Fenster aufnehmen"),
            "args": {
              "filepath": {
                "name": ("1053179562", "Speicherort"),
                "desc": ("1157352758", "Entweder der absolute Pfad einer .png Datei oder LOG, um das Bild in das Protokoll einzubetten."),
                "spec": {},
              }
            },
            "kwargs": {},
            "result": {
              "NoSession": ("4138997384", no_session),
              "InvalidPath": ("2844012395", "Der Pfad '{0}' ist ungültig"),
              "UNCPath": ("2462162559", r"Ein UNC Pfad (d.h. beginnend mit \\) ist nicht erlaubt"),
              "NoAbsPath": ("2858082864", "'{0}' ist kein absoluter Pfad"),
              "Log": ("3424982757", "Der Rückgabewert wird in das Protokoll geschrieben."),
              "Pass": ("1427858469", "Eine Aufnahme des Fensters wurde in '{0}' gespeichert."),
              "Exception": ("3250735497", exception("Eine Aufnahme des Fensters konnte nicht gespeichert werden."))
            },
            "doc": {
                "desc": ("2322410957", "Eine Bildschirmaufnahme des Fensters wird im angegebenen Speicherort gespeichert."),
                "examples": ("438329203", 
                f"""
                Beispiele:
                
                | ``Fenster aufnehmen     Speicherort``
                
                *Hinweis*: {path}
                """
              )
            }
        },
        "ScrollTextFieldContents": {
            "name": ("61854466", "Inhalte scrollen"),
            "args": {
                "direction": {
                    "name": ("1045090739", "Richtung"),
                    "desc": ("1053509555", "UP, DOWN, BEGIN, END"),
                    "spec": {}
                }
            },
            "kwargs": {
                "untilTextField": {
                    "name": ("2676914944", "bis_Textfeld"),
                    "desc": ("1257056815", textfield_locator),
                    "default": None,
                    "type": "str",
                    "spec": {}
                }
            },
            "result": {
                "NoSession": ("4138997384", no_session),
                "Exception": ("1650861005", exception("Die Inhalte der Textfelder konnten nicht gescrollt werden.")),
                "NoScrollbar": ("3931385040", "Das Fenster enthält keine scrollbaren Textfelder."),
                "MaximumReached": ("3349368208", "Die Inhalte der Textfelder können nicht weiter gescrollt werden."),
                "InvalidDirection": ("2667316811", "Die angegebene Richtung ist ungültig. Gültige Richtungen sind: UP, DOWN, BEGIN, END"),
                "Pass": ("1135653175", "Die Inahlte der Textfelder wurden in die Richtung '{0}' gescrollt.")
            },
            "doc": {
                "desc": ("4279609192", "Die Inhalte der Textfelder in einem Bereich mit einer Bildlaufleiste werden gescrollt."),
                "examples": ("1685692962", 
                """
                Beispiele:
              
                | ``Inhalte scrollen    Richtung``

                Wenn der Parameter "bis_Textfeld" übergeben wird, werden die Inhalte so lange gescrollt, bis das Textfeld gefunden wird.

                | ``Inhalte scrollen    Richtung   bis_Textfeld``
                """
              )
            }
        },
        "ScrollWindowHorizontally": {
            "name": ("3189335385", "Fenster horizontal scrollen"),
            "args": {
                "direction": {
                    "name": ("1045090739", "Richtung"),
                    "desc": ("3961976030", "LEFT, RIGHT, BEGIN, END"),
                    "spec": {}
                }
            },
            "kwargs": {},
            "result": {
                "NoSession": ("4138997384", no_session),
                "Exception": ("3689319260", exception("Das Fenster konnte nicht horizontal gescrollt werden.")),
                "NoScrollbar": ("876328389", "Das Fenster enthält keine horizontale Bildlaufleiste."),
                "MaximumReached": ("136266215", "Das Fenster kann nicht weiter gescrollt werden."),
                "InvalidDirection": ("103681373", "Die angegebene Richtung ist ungültig. Gültige Richtungen sind: LEFT, RIGHT, BEGIN, END"),
                "Pass": ("2330635320", "Das Fenster wurde in die Richtung '{0}' horizontal gescrollt.")
            },
            "doc": {
                "desc": ("21072201", "Die horizontale Bildlaufleiste des Fensters wird in die angegebene Richtung bewegt."),
                "examples": ("3013891708", 
                """
                Beispiele:
              
                | ``Fenster horizontal scrollen    Richtung``
                """
              )
            }
        },
        "SelectCell": {
            "name": ("1049942265", "Tabellenzelle markieren"),
            "args": {
              "a1row_locator": {
                "name": ("315353924", "Zeile"),
                "desc": ("2714474921", row_locator),
                "spec": {},
              },
              "a2column": {
                "name": ("2102626174", "Spaltentitel"),
                "desc": ("245054585", column),
                "spec": {},
              }
            },
            "kwargs": {
               "tableNumber": {
                  "name": ("3359775383", "tabelle_nummer"),
                  "desc": ("4055958951", "Spezifiziert welche Tabelle: 1, 2, ..."),
                  "default": None,
                  "type": "int",
                  "spec": {}
              } 
            },
            "result": {
              "NoSession": ("4138997384", no_session),
              "NotFound": ("3279676079", button_or_cell_not_found("Die Zelle mit dem Lokator '{0}, {1}' wurde nicht gefunden.")),
              "NoTable": ("2399256699", "Die Maske enthält keine Tabelle."),
              "InvalidTable": ("2205241003", "Die Maske enthält keine Tabelle mit dem Index {0}."),
              "Pass": ("1239645311", "Die Zelle mit dem Lokator '{0}, {1}' wurde markiert."),
              "Exception": ("2355177759", exception("Die Zelle konnte nicht markiert werden."))
            },
            "doc": {
                "desc": ("71460282", "Die Zelle am Schnittpunkt der Zeile und der Spalte wird markiert."),
                "examples": ("791925930", 
                f"""
                Beispiele:
                
                | ``Tabellenzelle markieren     Zeile     Spaltentitel``

                *Hinweis*: Dieses Schlüsselwort kann verwendet werden, um auf einen Link (unterstrichener Text oder Symbol) oder auf ein Optionsfeld in einer Zelle zu klicken.
                """
              )
            }
        },
        "SelectCellValue": {
            "name": ("993388184", "Tabellenzellenwert auswählen"),
            "args": {
                "a1row_locator": {
                    "name": ("315353924", "Zeile"),
                    "desc": ("2714474921", row_locator),
                    "spec": {},
                },
                "a2column": {
                    "name": ("2102626174", "Spaltentitel"),
                    "desc": ("245054585", column),
                    "spec": {},
                },
                "a3entry": {
                    "name": ("494360628", "Wert"),
                    "desc": ("3335814071", "Ein Wert aus dem Auswahlmenü"),
                    "spec": {},
                }
            },
            "kwargs": {
              "tableNumber": {
                  "name": ("3359775383", "tabelle_nummer"),
                  "desc": ("4055958951", "Spezifiziert welche Tabelle: 1, 2, ..."),
                  "default": None,
                  "type": "int",
                  "spec": {}
              }
            },
            "result": {
                "NoSession": ("4138997384", no_session),
                "NotFound": ("3279676079", button_or_cell_not_found("Die Zelle mit dem Lokator '{0}, {1}' wurde nicht gefunden.")),
                "EntryNotFound": ("1900787420", not_found("Der Wert '{2}' ist in der Zelle mit dem Lokator '{0}, {1}' nicht vorhanden.")),
                "InvalidTable": ("2205241003", "Die Maske enthält keine Tabelle mit dem Index {0}."),
                "Exception": ("813375986", exception("Der Wert konnte nicht ausgewählt werden. {0}")),
                "Pass": ("1530467143", "Der Wert '{2}' wurde ausgewählt.")
            },
            "doc": {
                "desc": ("3829091703", "In der Zelle am Schnittpunkt der Zeile und Spalte wird der angegebene Wert ausgewählt."),
                "examples": ("1404195932", 
                """
                Beispiele:
                  
                | ``Tabellenzellenwert auswählen    Zeile    Spaltentitel    Wert``
                """
                )
            }
        },
        "ReadCheckBox": {
            "name": ("706816611", "Formularfeld-Status auslesen"),
            "args": {
              "locator": {
                "name": ("2051440239", "Lokator"),
                "desc": ("896463449", "Ein Lokator, um das Formularfeld zu finden"),
                "spec": {},
              }
            },
            "kwargs": {},
            "result": {
              "NoSession": ("4138997384", no_session),
              "NotFound": ("3274358834", not_found("Das Formularfeld mit dem Lokator '{0}' wurde nicht gefunden.")),
              "Pass": ("1829602114", "Der Status des Formularfelds mit dem Lokator '{0}' wurde ausgelesen."),
              "Exception": ("817866934", exception("Der Status des Formularfelds konnte nicht ausgelesen werden."))
            },
            "doc": {
                "desc": ("417984648", "Der Status des angegebenen Formularfelds wird ausgelesen."),
                "examples": ("2386772967", 
                """
                Beispiele:
                
                *Formularfeld mit einer Beschriftung links oder rechts *
                | ``Formularfeld-Status auslesen    Beschriftung``
                
                *Formularfeld mit einer Beschriftung oben*
                | ``Formularfeld-Status auslesen    @ Beschriftung``
                
                *Formularfeld am Schnittpunkt einer Beschriftung links und einer oben*
                | ``Formularfeld-Status auslesen    Beschriftung links @ Beschriftung oben``
                """
              )
            }
        },
        "ReadComboBoxEntry": {
            "name": ("1020913973", "Auswahlmenüeintrag auslesen"),
            "args": {
              "comboBox": {
                "name": ("2051440239", "Lokator"),
                "desc": ("1540323925", "Beschriftung oder Kurzinfo des Auswahlmenüs"),
                "spec": {},
              }
            },
            "kwargs": {},
            "result": {
              "NoSession": ("4138997384", no_session),
              "NotFound": ("3185471891", not_found("Das Auswahlmenü mit dem Lokator '{0}' wurde nicht gefunden.")),
              "Pass": ("1356248111", "Der aktuelle Eintrag wurde ausgelesen."),
              "Exception": ("1202236059", exception("Der Eintrag konnte nicht ausgelesen werden."))
            },
            "doc": {
                "desc": ("2874967331", "Aus dem angegebenen Auswahlmenü wird der aktuelle Eintrag ausgelesen."),
                "examples": ("2962365709", 
                """
                Beispiele:
                
                | ``${Eintrag}   Auswahlmenüeintrag auslesen    Lokator``
                """
              )
            }
        },
        "SelectComboBoxEntry": {
            "name": ("2133292945", "Auswahlmenüeintrag auswählen"),
            "args": {
              "a1comboBox": {
                "name": ("2051440239", "Lokator"),
                "desc": ("1540323925", "Beschriftung oder Kurzinfo des Auswahlmenüs"),
                "spec": {},
              },
              "a2entry": {
                "name": ("723623280", "Eintrag"),
                "desc": ("3335814071", "Ein Eintrag aus dem Auswahlmenü"),
                "spec": {},
              }
            },
            "kwargs": {},
            "result": {
              "NoSession": ("4138997384", no_session),
              "NotFound": ("3185471891", not_found("Das Auswahlmenü mit dem Lokator '{0}' wurde nicht gefunden.")),
              "EntryNotFound": ("3289775842", not_found("Der Eintrag '{1}' wurde im Auswahlmenü '{0}' nicht gefunden.")),
              "Pass": ("2235674925", "Der Eintrag '{1}' wurde ausgewählt."),
              "Exception": ("2433413970", exception("Der Eintrag konnte nicht ausgewählt werden."))
            },
            "doc": {
                "desc": ("473069895", "Aus dem angegebenen Auswahlmenü wird der angegebene Eintrag ausgewählt."),
                "examples": ("947454329", 
                """
                Beispiele:
                
                | ``Auswahlmenüeintrag auswählen    Lokator    Eintrag``

                *Hinweise*: 
                - Wenn der Eintrag nicht eindeutig ist, verwende den Schlüssel, der angezeigt wird, wenn "Schlüssel in Dropdown-Listen anzeigen" in den SAP GUI-Optionen aktiviert ist.
                - Um einen Eintrag aus einem Symbolleisten-Knopf mit Auswahlmenü auszuwählen, verwende die Beschriftung bzw. die Kurzinfo (Tooltip) des Knopfes als Lokator.
                """
              )
            }
        },
        "SelectMenuItem": {
            "name": ("2747911439", "Menüeintrag auswählen"),
            "args": {
                "itemPath": {
                    "name": ("2396735438", "Eintragspfad"),
                    "desc": ("1754638949", "Der Pfad zum Eintrag mit '/' als Trennzeichen (z.B. System/Benutzervorgaben/Eigene Daten)."),
                    "spec": {},
                }
            },
            "kwargs": {},
            "result": {
                "NoSession": ("4138997384", no_session),
                "NotFound": ("711462162", not_found("Der Menüeintrag '{0}' wurde nicht gefunden.")),
                "Pass": ("200907440", "Der Menüeintrag '{0}' wurde ausgewählt."),
                "Exception": ("3089619033", exception("Der Menüeintrag konnte nicht ausgewählt werden. {0}"))
            },
            "doc": {
                "desc": ("3980851450", "Der Menüeintrag mit dem angegebenen Pfad wird ausgewählt."),
                "examples": ("2691366329", 
                """
                Beispiele:
              
                | ``Menüeintrag auswählen    Eintragspfad``
                """
              )
            }
        },
        "SelectRadioButton": {
            "name": ("2985728785", "Optionsfeld auswählen"),
            "args": {
              "locator": {
                "name": ("2051440239", "Lokator"),
                "desc": ("4140479875", "Ein Lokator, um das Optionsfeld zu finden"),
                "spec": {
                    "HLabel": ("4229670492", HLabel),
                    "VLabel": ("474824962", VLabel),
                    "HLabelVLabel": ("1999142431", HLabelVLabel)
                },
              }
            },
            "kwargs": {},
            "result": {
              "NoSession": ("4138997384", no_session),
              "NotFound": ("2755548585", not_found("Das Optionsfeld mit dem Lokator '{0}' wurde nicht gefunden.")),
              "NotChangeable": ("2043765722", "Das Optionsfeld mit dem Lokator '{0}' ist deaktiviert."),
              "Pass": ("259379063", "Das Optionsfeld mit dem Lokator '{0}' wurde ausgewählt."),
              "Exception": ("218028187", exception("Das Optionsfeld konnte nicht ausgewählt werden."))
            },
            "doc": {
                "desc": ("3238721707", "Das angegebene Optionsfeld wird ausgewählt."),
                "examples": ("2927088619", 
                """
                Beispiele:
                
                *Optionsfeld mit einer Beschriftung links oder rechts*
                | ``Optionsfeld auswählen    Beschriftung``
                
                *Optionsfeld mit einer Beschriftung oben*
                | ``Optionsfeld auswählen    @ Beschriftung``
                
                *Optionsfeld am Schnittpunkt einer Beschriftung links (oder rechts) und einer oben*
                | ``Optionsfeld auswählen    Beschriftung links @ Beschriftung oben``
                """
              )
            }
        },
        "SelectTableColumn": {
            "name": ("3486989426", "Tabellenspalte markieren"),
            "args": {
                "column": {
                    "name": ("2102626174", "Spalte"),
                    "desc": ("2490445981", "Spaltentitel oder Kurzhilfe (Tooltip)"),
                    "spec": {},
                }
            },
            "kwargs": {
                "tableNumber": {
                  "name": ("3359775383", "tabelle_nummer"),
                  "desc": ("4055958951", "Spezifiziert welche Tabelle: 1, 2, ..."),
                  "default": 1,
                  "type": "int",
                  "spec": {}
              }
            },
            "result": {
                "NoSession": ("4138997384", no_session),
                "Exception": ("3528372356", exception("Die Spalte konnte nicht markiert werden. {0}")),
                "NoTable": ("1798632660", "Die Maske entählt keine Tabelle"),
                "InvalidTable": ("2205241003", "Die Maske enthält keine Tabelle mit dem Index {0}."),
                "NotFound": ("3613832630", "Die Tabelle enthält keine Spalte '{0}'"),
                "Pass": ("3274536988", "Die Spalte '{0}' wurde markiert")
            },
            "doc": {
                "desc": ("2661352379", "Die angegebene Tabellenspalte wird markiert."),
                "examples": ("3612584524", 
                f"""
                Beispiele:
                
                | ``Tabellenspalte markieren    Spalte``
                """
              )
            }
        },
        "SelectTableRow": {
            "name": ("1966160675", "Tabellenzeile markieren"),
            "args": {
                "row_locator": {
                    "name": ("315353924", "Zeilenlokator"),
                    "desc": ("2714474921", row_locator),
                    "spec": {},
                },
            },
            "kwargs": {
               "tableNumber": {
                  "name": ("3359775383", "tabelle_nummer"),
                  "desc": ("4055958951", "Spezifiziert welche Tabelle: 1, 2, ..."),
                  "default": 1,
                  "type": "int",
                  "spec": {}
              }
            },
            "result": {
                "NoSession": ("4138997384", no_session),
                "Exception": ("3194892480", exception("Die Zeile konnte nicht markiert werden. {0}")),
                "NoTable": ("1798632660", "Die Maske entählt keine Tabelle"),
                "InvalidTable": ("2205241003", "Die Maske enthält keine Tabelle mit dem Index {0}."),
                "InvalidIndex": ("3624573287", "Die Tabelle hat keine Zeile '{0}'"),
                "NotFound": ("3975114955", "Die Tabelle enthält keine Zelle mit dem Inhalt '{0}'"),
                "Pass": ("2631747337", "Die Zeile mit dem Lokator '{0}' wurde markiert")
            },
            "doc": {
                "desc": ("839736602", "Die angegebene(n) Tabellenzeile(n) soll(en) markiert werden und die entsprechende(n) Zeilennummer zurückgegeben."),
                "examples": ("3116133145", 
                f"""
                Beispiele:
                
                *Eine einzelne Zeile markieren*
                | ``Tabellenzeile markieren    Zeilenlokator``
                
                *Mehrere Zeilen in einem ALV-Grid (eine Tabelle mit einer Symbolleiste) markieren*
                | ``Tabellenzeile markieren    1,2,3``

                *Hinweis*: Mit der Zeilennummer 0 wird die gesamte Tabelle markiert.
                """
              )
            }
        },
        "SelectTextField": {
            "name": ("335907869", "Textfeld markieren"),
            "args": {
              "locator": {
                "name": ("2051440239", "Lokator"),
                "desc": ("1257056815", textfield_locator),
                "spec": {
                    "HLabel": ("4229670492", HLabel),
                    "VLabel": ("474824962", VLabel),
                    "HLabelVLabel": ("1999142431", HLabelVLabel),
                    "HLabelHLabel": ("3678957963", HLabelHLabel),
                    "HLabelVIndex": ("509417766", HLabelVIndex),
                    "HIndexVLabel": ("3606518505", HIndexVLabel),
                    "Content": ("880934240", Content)
                },
              }
            },
            "kwargs": {},
            "result": {
              "NoSession": ("4138997384", no_session),
              "NotFound": ("2917845132", not_found("Das Textfeld mit dem Lokator '{0}' wurde nicht gefunden.")),
              "Pass": ("3773273557", "Das Textfeld mit dem Lokator '{0}' wurde markiert."),
              "Exception": ("1228826942", exception("Das Textfeld konnte nicht markiert werden."))
            },
            "doc": {
                "desc": ("2850695645", "Das angegebene Textfeld wird markiert."),
                "examples": ("1471605922", 
                """
                Beispiele:
                
                *Identifizierung des Textfeldes über einen Lokator*
                | ``Textfeld markieren    Lokator``

                *Identifizierung des Textfeldes über seinen Inhalt*
                | ``Textfeld markieren     = Inhalt``
                """
              )
            }
        },
        "SelectText": {
            "name": ("3466131676", "Text markieren"),
            "args": {
              "locator": {
                "name": ("2051440239", "Lokator"),
                "desc": ("4059025231", "Ein Lokator, um den Text zu finden"),
                "spec": {},
              }
            },
            "kwargs": {},
            "result": {
              "NoSession": ("4138997384", no_session),
              "NotFound": ("460274007", not_found("Der Text mit dem Lokator '{0}' wurde nicht gefunden.")),
              "Pass": ("3369275832", "Der Text mit dem Lokator '{0}' wurde markiert."),
              "Exception": ("37399687", exception("Der Text konnte nicht markiert werden."))
            },
            "doc": {
                "desc": ("1016642629", "Der angegebene Text wird markiert."),
                "examples": ("680036934", 
                """
                Beispiele:
                
                *Text beginnt mit der angegebenen Teilzeichenfolge*
                | ``Text markieren    = Teilzeichenfolge``
                
                *Text folgt einer Beschriftung*
                | ``Text markieren    Beschriftung``
                """
              )
            }
        },
        "TickCheckBox": {
            "name": ("2471720243", "Formularfeld ankreuzen"),
            "args": {
              "locator": {
                "name": ("2051440239", "Lokator"),
                "desc": ("896463449", "Ein Lokator, um das Formularfeld zu finden"),
                "spec": {
                    "HLabel": ("4229670492", HLabel),
                    "VLabel": ("474824962", VLabel),
                    "HLabelVLabel": ("1999142431", HLabelVLabel)
                },
              }
            },
            "kwargs": {},
            "result": {
              "NoSession": ("4138997384", no_session),
              "NotFound": ("3274358834", not_found("Das Formularfeld mit dem Lokator '{0}' wurde nicht gefunden.")),
              "NotChangeable": ("4165781642", "Das Formularfeld mit dem Lokator '{0}' ist deaktiviert."),
              "Pass": ("999358000", "Das Formularfeld mit dem Lokator '{0}' wurde angekreuzt."),
              "Exception": ("1153105219", exception("Das Formularfeld konnte nicht angekreuzt werden."))
            },
            "doc": {
                "desc": ("2308564624", "Das angegebene Formularfeld wird angekreuzt."),
                "examples": ("785953453", 
                """
                Beispiele:
                
                *Formularfeld mit einer Beschriftung links oder rechts *
                | ``Formularfeld ankreuzen    Beschriftung``
                
                *Formularfeld mit einer Beschriftung oben*
                | ``Formularfeld ankreuzen    @ Beschriftung``
                
                *Formularfeld am Schnittpunkt einer Beschriftung links und einer oben*
                | ``Formularfeld ankreuzen    Beschriftung links @ Beschriftung oben``
                """
              )
            }
        },
        "ReadStatusbar": {
            "name": ("118752925", "Statusleiste auslesen"),
            "args": {},
            "kwargs": {},
            "result": {
                "Json": ("144359828", "Der Rückgabewert ist im JSON-Format"),
                "NoSession": ("4138997384", no_session),
                "NotFound": ("2342000252", "Keine Statusleiste gefunden."),
                "Exception": ("4016539365", exception("Die Statusleiste konnte nicht ausgelesen werden.")),
                "Pass": ("1105532895", "Die Statusleiste wurde ausgelesen.")
            },
            "doc": {
                "desc": ("192177844", "Der Inhalt der Statusleiste wird ausgelesen. Der Rückgabewert ist ein Dictionary mit den Einträgen 'status' und 'message'."),
                "examples": ("101747441", 
                """
                Beispiele:
              
                | ``${statusleiste}   Statusleiste auslesen``
                """
              )
            }
        },
        "UntickCheckBox": {
            "name": ("47381427", "Formularfeld abwählen"),
            "args": {
              "locator": {
                "name": ("2051440239", "Lokator"),
                "desc": ("896463449", "Ein Lokator, um das Formularfeld zu finden"),
                "spec": {
                    "HLabel": ("4229670492", HLabel),
                    "VLabel": ("474824962", VLabel),
                    "HLabelVLabel": ("1999142431", HLabelVLabel)
                },
              }
            },
            "kwargs": {},
            "result": {
              "NoSession": ("4138997384", no_session),
              "NotFound": ("3274358834", not_found("Das Formularfeld mit dem Lokator '{0}' wurde nicht gefunden.")),
              "NotChangeable": ("4165781642", "Das Formularfeld mit dem Lokator '{0}' ist deaktiviert."),
              "Pass": ("1077869101", "Das Formularfeld mit dem Lokator '{0}' wurde abgewählt."),
              "Exception": ("1479426504", exception("Das Formularfeld konnte nicht abgewählt werden."))
            },
            "doc": {
                "desc": ("524349813", "Das angegebene Formularfeld wird abgewählt."),
                "examples": ("2911392799", 
                """
                Beispiele:
                
                *Formularfeld mit einer Beschriftung links oder rechts*
                | ``Formularfeld abwählen    Beschriftung``
                
                *Formularfeld mit einer Beschriftung oben*
                | ``Formularfeld abwählen    @ Beschriftung``
                
                *Formularfeld am Schnittpunkt einer Beschriftung links und einer oben*
                | ``Formularfeld abwählen    Beschriftung links @ Beschriftung oben``
                """
              )
            }
        },
        "TickCheckBoxCell": {
            "name": ("3286561809", "Tabellenzelle ankreuzen"),
            "args": {
              "a1row": {
                "name": ("315353924", "Zeile"),
                "desc": ("2714474921", row_locator),
                "spec": {},
              },
              "a2column": {
                "name": ("2102626174", "Spaltentitel"),
                "desc": ("245054585", column),
                "spec": {},
              },
            },
            "kwargs": {
               "tableNumber": {
                  "name": ("3359775383", "tabelle_nummer"),
                  "desc": ("4055958951", "Spezifiziert welche Tabelle: 1, 2, ..."),
                  "default": None,
                  "type": "int",
                  "spec": {}
              }
            },
            "result": {
              "NoSession": ("4138997384", no_session),
              "NotFound": ("2297657056", button_or_cell_not_found("Die Zelle mit dem Lokator '{0}, {1}' wurde nicht gefunden.")),
              "NotChangeable": ("366722275", "Die Zelle mit dem Lokator '{0}, {1}' ist deaktiviert."),
              "InvalidTable": ("2205241003", "Die Maske enthält keine Tabelle mit dem Index {0}."),
              "Pass": ("342341552", "Die Zelle mit dem Lokator '{0}, {1}' wurde angekreuzt."),
              "Exception": ("870126097", exception("Die Zelle konnte nicht angekreuzt werden."))
            },
            "doc": {
                "desc": ("1790771617", "Die Zelle am Schnittpunkt der Zeile und der Spalte wird angekreuzt."),
                "examples": ("1019687234", 
                f"""
                Beispiele:
                
                | ``Tabellenzelle ankreuzen     Zeile     Spaltentitel``

                *Hinweis*: Um das Formularfeld in der Spalte ganz links ohne Titel anzukreuzen, markiere die Zeile und drücke die "Enter"-Taste.
                """
              )
            }
        },
        "UntickCheckBoxCell": {
            "name": ("4146679655", "Tabellenzelle abwählen"),
            "args": {
              "a1row": {
                "name": ("315353924", "Zeile"),
                "desc": ("2714474921", row_locator),
                "spec": {},
              },
              "a2column": {
                "name": ("2102626174", "Spaltentitel"),
                "desc": ("245054585", column),
                "spec": {},
              }
            },
            "kwargs": {
               "tableNumber": {
                  "name": ("3359775383", "tabelle_nummer"),
                  "desc": ("4055958951", "Spezifiziert welche Tabelle: 1, 2, ..."),
                  "default": None,
                  "type": "int",
                  "spec": {}
              }
            },
            "result": {
              "NoSession": ("4138997384", no_session),
              "NotFound": ("2297657056", button_or_cell_not_found("Die Zelle mit dem Lokator '{0}, {1}' wurde nicht gefunden.")),
              "NotChangeable": ("366722275", "Die Zelle mit dem Lokator '{0}, {1}' ist deaktiviert."),
              "InvalidTable": ("2205241003", "Die Maske enthält keine Tabelle mit dem Index {0}."),
              "Pass": ("1958589605", "Die Zelle mit dem Lokator '{0}, {1}' wurde abgewählt."),
              "Exception": ("3759601296", exception("Die Zelle konnte nicht abgewählt werden."))
            },
            "doc": {
                "desc": ("177160211", "Die Zelle am Schnittpunkt der Zeile und der Spalte wird abgewählt."),
                "examples": ("2105808389", 
                f"""
                Beispiele:
                
                | ``Tabellenzelle abwählen     Zeile     Spaltentitel``
                """
              )
            }
        },
        "GetWindowTitle": {
            "name": ("2828980154", "Fenstertitel auslesen"),
            "args": {},
            "kwargs": {},
            "result": {
              "NoSession": ("4138997384", no_session),
              "Pass": ("2852411998", "Der Fenstertitel wurde ausgelesen"),
              "Exception": ("1458775703", exception("Der Titel des Fensters konnte nicht ausgelesen werden."))
            },
            "doc": {
                "desc": ("2557939136", "Der Titel des gerade aktiven Fensters wird zurückgegeben."),
                "examples": ("2536322048", 
                """
                Beispiele:
                
                | ``${Titel}    Fenstertitel auslesen``
                """
              )
            }
        },
        "GetWindowText": {
            "name": ("1085911504", "Fenstertext auslesen"),
            "args": {},
            "kwargs": {},
            "result": {
              "NoSession": ("4138997384", no_session),
              "Pass": ("2562559050", "Der Text des Fensters wurde ausgelesen"),
              "Exception": ("293173089", exception("Der Text des Fensters konnte nicht ausgelesen werden."))
            },
            "doc": {
                "desc": ("2795776118", "Der Text des gerade aktiven Fensters wird zurückgegeben."),
                "examples": ("353358958", 
                """
                Beispiele:
                
                | ``${Text}    Fenstertext auslesen``
                """
              )
            }
        },
        "MaximizeWindow": {
            "name": ("458747722", "Fenster maximieren"),
            "args": {},
            "kwargs": {},
            "result": {
                "NoSession": ("4138997384", no_session),
                "Pass": ("3684348012", "Das Fenster im Vordergrund wurde maximiert."),
                "Exception": ("3751877905", exception("Das Fenster im Vordergrund konnte nicht maximiert werden. {0}"))
            },
            "doc": {
                "desc": ("3269397069", "Das Fenster im Vordergrund wird maximiert."),
                "examples": ("298229724",
                """
                Beispiele:

                | ``Fenster maximieren``
                """
                )
            }
        }
    },
    "specs": {}
}
