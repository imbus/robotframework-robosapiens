from typing import Callable
from schema_i18n import LocalizedRoboSAPiens

Fstr = Callable[[str], str]

sap_error = 'SAP Fehlermeldung: {0}'
no_session = 'Keine SAP-Session vorhanden. Versuche zuerst das Keyword "Verbindung zum Server Herstellen" aufzurufen.'
no_sap_gui = 'Keine laufende SAP GUI gefunden. Das Keyword "SAP starten" muss zuerst aufgerufen werden.'
no_gui_scripting = 'Die Skriptunterstützung ist nicht verfügbar. Sie muss in den Einstellungen von SAP Logon aktiviert werden.'
no_connection = 'Es besteht keine Verbindung zu einem SAP Server. Versuche zuerst das Keyword "Verbindung zum Server Herstellen" aufzurufen.'
no_server_scripting = 'Das Scripting ist auf dem SAP Server nicht freigeschaltet. Siehe die Dokumentation von RoboSAPiens.'
not_found: Fstr = lambda msg: f"{msg}\nHinweis: Prüfe die Rechtschreibung"
exception: Fstr = lambda msg: f"{msg}" + "\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."

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
    "intro": (
        "284571434", 
        """RoboSAPiens: SAP GUI-Automatisierung für Menschen

        Um diese Bibliothek zu verwenden, müssen die folgenden Bedingungen erfüllt werden:

        - Das [https://help.sap.com/saphelp_aii710/helpdata/de/ba/b8710932b8c64a9e8acf5b6f65e740/content.htm|Scripting] muss auf dem SAP Server aktiviert werden.
        
        - Die [https://help.sap.com/docs/sap_gui_for_windows/63bd20104af84112973ad59590645513/7ddb7c9c4a4c43219a65eee4ca8db001.html|Skriptunterstützung] muss in der SAP GUI aktiviert werden.
        """
    ),
    "init": ("0", ""),
  },
  "args": {
    "presenter_mode": {
      "name": ("781265386", "vortragsmodus"),
      "default": False,
      "doc": ("3421082408", "Jedes GUI Element wird vor seiner Betätigung bzw. Änderung kurz hervorgehoben")
    }
  },
  "keywords": {
    "ActivateTab": {
      "name": ("1870139227", "Reiter auswählen"),
      "args": {
        "tab": {
          "name": ("3772862467", "Reitername"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("2994771228", not_found("Der Reiter '{0}' konnte nicht gefunden werden.")),
        "SapError": ("3246364722", sap_error),
        "Pass": ("4294349699", "Der Reiter '{0}' wurde ausgewählt."),
        "Exception": ("2577256712", exception("Der Reiter konnte nicht ausgewählt werden."))
      },
      "doc": (
          "3801089437", 
          """Der Reiter mit dem angegebenen Namen wird ausgewählt.

          | ``Reiter auswählen    Reitername``
          """
      )
    },
    "OpenSap": {
      "name": ("1259182241", "SAP starten"),
      "args": {
        "path": {
          "name": ("190089999", "Pfad"),
          "spec": {}
        }
      },
      "result": {
        "Pass": ("3933791589", "Die SAP GUI wurde gestartet"),
        "SAPNotStarted": ("4005776825", "Die SAP GUI konnte nicht gestartet werden. Überprüfe den Pfad '{0}'."),
        "Exception": ("2772047805", exception("Die SAP GUI konnte nicht gestartet werden."))
      },
      "doc": (
          "2645300475", 
          r"""Die SAP GUI wird gestartet. Der übliche Pfad ist
          
          | ``C:\\Program Files (x86)\\SAP\\FrontEnd\\SAPgui\\saplogon.exe``
          """
      )
    },
    "CloseConnection": {
      "name": ("938374979", "Verbindung zum Server trennen"),
      "args": {},
      "result": {
        "NoSapGui": ("2987622841", no_sap_gui),
        "NoGuiScripting": ("2929771598", no_gui_scripting),
        "NoConnection": ("509780556", no_connection),
        "NoSession": ("2754484086", no_session),
        "Pass": ("1657006605", "Die Verbindung zum Server wurde getrennt."),
        "Exception": ("2209141929", exception("Die Verbindung zum Server konnte nicht getrennt werden."))
      },
      "doc": (
          "3955335875", 
          """
          Die Verbindung mit dem SAP Server wird getrennt.
          """
      )
    },
    "CloseSap": {
      "name": ("1795765665", "SAP beenden"),
      "args": {},
      "result": {
        "NoSapGui": ("2987622841", no_sap_gui),
        "Pass": ("2970606098", "Die SAP GUI wurde beendet")
      },
      "doc": (
          "3092056753", 
          """
          Die SAP GUI wird beendet.
          """
      )
    },
    "ExportSpreadsheet": {
        "name": ("3046426513", "Tabellenkalkulation exportieren"),
        "args": {
            "index": {
                "name": ("2358225529", "Tabellenindex"),
                "spec": {}
            }
        },
        "result": {
            "NoSession": ("2754484086", no_session),
            "NotFound": ("275602004", not_found("Keine Tabelle wurde gefunden, welche die Export-Funktion 'Tabellenkalkulation' unterstützt")),
            "Exception": ("1364348396", exception("Die Export-Funktion 'Tabellenkalkulation' konnte nicht aufgerufen werden")),
            "Pass": ("2225629927", "Die Export-Funktion 'Tabellenkalkulation' wurde für die Tabelle mit Index {0} aufgerufen")
        },
        "doc": (
            "2824371354", 
            """
              Die Export-Funktion 'Tabellenkalkulation' wird für die angegebene Tabelle aufgerufen, falls vorhanden.

              | ``Tabellenkalkulation exportieren   Tabellenindex``

              Tabellenindex: 1, 2,...
            """
        )
    },
    "ExportTree": {
      "name": ("1188312707", "Funktionsbaum exportieren"),
      "args": {
        "filepath": {
          "name": ("1769741420", "Dateipfad"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("811568965", "Die Maske enthält keine Baumstruktur"),
        "Pass": ("176551133", "Die Baumstruktur wurde in JSON Format in der Datei '{0}' gespeichert"),
        "Exception": ("1542087750", exception("Die Baumstruktur konnte nicht exportiert werden."))
      },
      "doc": (
          "4252712479", 
          """
          Der Funktionsbaum wird in der angegebenen Datei gespeichert.
          
          | ``Funktionsbaum exportieren     Dateipfad``
          """
      )
    },
    "AttachToRunningSap": {
      "name": ("4126309856", "Laufende SAP GUI übernehmen"),
      "args": {},
      "result": {
        "NoSapGui": ("2987622841", no_sap_gui),
        "NoGuiScripting": ("2929771598", no_gui_scripting),
        "NoConnection": ("509780556", no_connection),
        "NoServerScripting": ("3495213352", no_server_scripting),
        "Pass": ("2481655346", "Die laufende SAP GUI wurde erfolgreich übernommen."),
        "Exception": ("3120673076", exception("Die laufende SAP GUI konnte nicht übernommen werden."))
      },
      "doc": (
          "3968519172", 
          """
          Nach der Ausführung dieses Keywords, kann eine laufende SAP GUI mit RoboSAPiens gesteuert werden.
          """
      )
    },
    "ConnectToServer": {
      "name": ("1377779562", "Verbindung zum Server herstellen"),
      "args": {
        "server": {
          "name": ("3584233446", "Servername"),
          "spec": {}
        }
      },
      "result": {
        "NoSapGui": ("2987622841", no_sap_gui),
        "NoGuiScripting": ("2929771598", no_gui_scripting),
        "Pass": ("1014238539", "Die Verbindung mit dem Server '{0}' wurde erfolgreich hergestellt."),
        "SapError": ("3246364722", sap_error),
        "NoServerScripting": ("3495213352", no_server_scripting),
        "Exception": ("667377482", exception("Die Verbindung konnte nicht hergestellt werden."))
      },
      "doc": (
          "2007383915", 
          """
          Die Verbindung mit dem angegebenen SAP Server wird hergestellt.
          
          | ``Verbindung zum Server herstellen    Servername``
          """
      )
    },
    "DoubleClickCell": {
      "name": ("2108476291", "Tabellenzelle doppelklicken"),
      "args": {
        "a1row_locator": {
          "name": ("315353924", "Zeile"),
          "spec": {}
        },
        "a2column": {
          "name": ("2102626174", "Spaltentitel"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("2770335633", not_found("Die Zelle mit dem Lokator '{0}, {1}' konnte nicht gefunden werden.")),
        "Pass": ("4023459711", "Die Zelle mit dem Lokator '{0}, {1}' wurde doppelgeklickt."),
        "Exception": ("2384367029", exception("Die Zelle konnte nicht doppelgeklickt werden."))
      },
      "doc": (
          "185276928", 
          """
          Die angegebene Tabellenzelle wird doppelgeklickt.
          
          | ``Tabellenzelle doppelklicken     Zeile     Spaltentitel``
          
          Zeile: entweder die Zeilennummer oder der Inhalt der Zelle.
          """
      )
    },
    "DoubleClickTextField": {
      "name": ("3737103423", "Textfeld doppelklicken"),
      "args": {
        "content": {
          "name": ("4274335913", "Inhalt"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("3855369076", not_found("Das Textfeld mit dem Lokator '{0}' konnte nicht gefunden werden.")),
        "Pass": ("1611309101", "Das Textfeld mit dem Lokator '{0}' wurde doppelgeklickt."),
        "Exception": ("504842288", exception("Das Textfeld konnte nicht doppelgeklickt werden."))
      },
      "doc": (
          "878856351", 
          """
          Das angegebene Textfeld wird doppelgeklickt.
          
          | ``Textfeld doppelklicken     Inhalt``
          """
      )
    },
    "ExecuteTransaction": {
      "name": ("2997404008", "Transaktion ausführen"),
      "args": {
        "T_Code": {
          "name": ("1795027938", "T_Code"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "Pass": ("468573121", "Die Transaktion mit T-Code '{0}' wurde erfolgreich ausgeführt."),
        "Exception": ("3958687903", exception("Die Transaktion konnte nicht ausgeführt werden."))
      },
      "doc": (
          "1005778151", 
          """
          Die Transaktion mit dem angegebenen T-Code wird ausgeführt.
          
          | ``Transaktion ausführen    T-Code``
          """
      )
    },
    "ExportForm": {
      "name": ("1168873090", "Maske exportieren"),
      "args": {
        "a1name": {
          "name": ("1579384326", "Name"),
          "spec": {}
        },
        "a2directory": {
          "name": ("1182287066", "Verzeichnis"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "Pass": ("319055629", "Die Maske wurde in den Dateien '{0}' und '{1}' gespeichert"),
        "Exception": ("487625120", exception("Die Maske konnte nicht exportiert werden."))
      },
      "doc": (
          "4292940885", 
          """
          Alle Texte in der aktuellen Maske werden in einer JSON-Datei gespeichert. Außerdem wird ein Bildschirmfoto in PNG-Format erstellt.
          
          | ``Maske exportieren     Name     Verzeichnis``
          
          Verzeichnis: Der absolute Pfad des Verzeichnisses, wo die Dateien abgelegt werden.
          """
      )
    },
    "FillTableCell": {
      "name": ("1010164935", "Tabellenzelle ausfüllen"),
      "args": {
        "a1row_locator": {
          "name": ("315353924", "Zeile"),
          "spec": {}
        },
        "a2column_content": {
          "name": ("4122925101", "Spaltentitel_gleich_Inhalt"),
          "spec": {
              "ColumnContent": ("2284019715", ColumnContent)
          }
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "InvalidFormat": ("280110049", "Das zweite Argument muss dem Muster `Spalte = Inhalt` entsprechen"),
        "NotFound": ("3381319755", not_found("Die Zelle mit dem Lokator '{0}, {1}' konnte nicht gefunden werden.")),
        "NotChangeable": ("2078310074", "Die Zelle mit dem Lokator '{0}, {1}' ist nicht bearbeitbar."),
        "Pass": ("2876607603", "Die Zelle mit dem Lokator '{0}, {1}' wurde ausgefüllt."),
        "Exception": ("1958379303", exception("Die Zelle konnte nicht ausgefüllt werden."))
      },
      "doc": (
          "3041188501", 
          """
          Die Zelle am Schnittpunkt der angegebenen Zeile und Spalte wird mit dem angegebenen Inhalt ausgefüllt.
          
          | ``Tabellenzelle ausfüllen     Zeile     Spaltentitel = Inhalt``
          
          Zeile: entweder eine Zeilennummer oder der Inhalt einer Zelle in der Zeile.
          
          *Hinweis*: Eine Tabellenzelle hat u.U. eine Beschriftung, die man über die Hilfe (Taste F1) herausfinden kann. In diesem Fall kann man die Zelle mit dem Keyword [#Textfeld%20Ausfüllen|Textfeld ausfüllen] ausfüllen.
          """
      )
    },
    "FillTextField": {
      "name": ("3103200585", "Textfeld ausfüllen"),
      "args": {
        "a1locator": {
          "name": ("2051440239", "Beschriftung_oder_Lokator"),
          "spec": {
              "HLabel": ("4229670492", HLabel),
              "VLabel": ("474824962", VLabel),
              "HLabelVLabel": ("1999142431", HLabelVLabel),
              "HLabelHLabel": ("3678957963", HLabelHLabel),
              "HLabelVIndex": ("509417766", HLabelVIndex),
              "HIndexVLabel": ("3606518505", HIndexVLabel)
          }
        },
        "a2content": {
          "name": ("4274335913", "Inhalt"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("2917845132", not_found("Das Textfeld mit dem Lokator '{0}' konnte nicht gefunden werden.")),
        "Pass": ("1361669956", "Das Textfeld mit dem Lokator '{0}' wurde ausgefüllt."),
        "Exception": ("3667643114", exception("Das Textfeld konnte nicht ausgefüllt werden. Möglicherweise, weil der Inhalt nicht dazu passt."))
      },
      "doc": (
          "1588888798", 
          """
          Das angegebene Textfeld wird mit dem angegebenen Inhalt ausgefüllt.
          
          *Textfeld mit einer Beschriftung links*
          | ``Textfeld ausfüllen    Beschriftung    Inhalt``
          
          *Textfeld mit einer Beschriftung oben*
          | ``Textfeld ausfüllen    @ Beschriftung    Inhalt``
          
          *Textfeld am Schnittpunkt einer Beschriftung links und einer oben (z.B. eine Abschnittsüberschrift)*
          | ``Textfeld ausfüllen    Beschriftung links @ Beschriftung oben    Inhalt``
          
          *Textfeld ohne Beschriftung unter einem Textfeld mit einer Beschriftung (z.B. eine Adresszeile)*
          | ``Textfeld ausfüllen    Position (1,2,..) @ Beschriftung    Inhalt``
          
          *Textfeld ohne Beschriftung rechts von einem Textfeld mit einer Beschriftung*
          | ``Textfeld ausfüllen    Beschriftung @ Position (1,2,..)    Inhalt``
          
          *Textfeld mit einer nicht eindeutigen Beschriftung rechts von einem Textfeld mit einer Beschriftung*
          | ``Textfeld ausfüllen    Beschriftung des linken Textfelds >> Beschriftung    Inhalt``
          
          *Hinweis*: In der Regel hat ein Textfeld eine unsichtbare Beschriftung, die man über die Hilfe (Taste F1) herausfinden kann.
          """
      )
    },
    "HighlightButton": {
      "name": ("2180269929", "Knopf hervorheben"),
      "args": {
        "button": {
          "name": ("894332414", "Name_oder_Kurzinfo"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("3063247197", not_found("Der Knopf '{0}' konnte nicht gefunden werden.")),
        "Pass": ("2149046612", "Der Knopf '{0}' wurde hervorgehoben."),
        "Exception": ("1973912995", exception("Der Knopf konnte nicht hervorgehoben werden."))
      },
      "doc": (
          "782818376", 
          """
          Der Knopf mit dem angegebenen Namen oder Kurzinfo (Tooltip) wird hervorgehoben.
          
          | ``Knopf hervorheben    Name oder Kurzinfo (Tooltip)``
          """
      )
    },
    "PushButton": {
      "name": ("2326550334", "Knopf drücken"),
      "args": {
        "button": {
          "name": ("894332414", "Name_oder_Kurzinfo"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "SapError": ("3246364722", sap_error),
        "NotFound": ("3063247197", not_found("Der Knopf '{0}' konnte nicht gefunden werden.")),
        "Pass": ("2346783035", "Der Knopf '{0}' wurde gedrückt."),
        "Exception": ("1002997848", exception("Der Knopf konnte nicht gedrückt werden."))
      },
      "doc": (
          "1332516663", 
          """
          Der Knopf mit dem angegebenen Namen oder Kurzinfo (Tooltip) wird gedrückt.
          
          | ``Knopf drücken    Name oder Kurzinfo (Tooltip)``
          """
      )
    },
    "PushButtonCell": {
      "name": ("349686496", "Tabellenzelle drücken"),
      "args": {
        "a1row_or_label": {
          "name": ("2392311166", "Zeile"),
          "spec": {}
        },
        "a2column": {
          "name": ("2102626174", "Spaltentitel"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("2199892932", not_found("Die Zelle mit dem Lokator '{0}, {1}' konnte nicht gefunden werden.")),
        "Pass": ("1649470590", "Die Zelle mit dem Lokator '{0}, {1}' wurde gedrückt."),
        "Exception": ("1751102722", exception("Die Zelle konnte nicht gedrückt werden."))
      },
      "doc": (
          "2366408483", 
          """
          Die angegebene Tabellenzelle wird gedrückt.
          
          | ``Tabellenzelle drücken     Zeile     Spaltentitel``
          
          Zeile: Zeilennummer, Beschriftung oder Kurzinfo (Tooltip).
          """
      )
    },
    "ReadTextField": {
      "name": ("490498248", "Textfeld auslesen"),
      "args": {
        "locator": {
          "name": ("2051440239", "Beschriftung_oder_Lokator"),
          "spec": {
              "HLabel": ("4229670492", HLabel),
              "VLabel": ("474824962", VLabel),
              "HLabelVLabel": ("1999142431", HLabelVLabel),
              "Content": ("880934240", Content)
          }
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("2917845132", not_found("Das Textfeld mit dem Lokator '{0}' konnte nicht gefunden werden.")),
        "Pass": ("2524131110", "Das Textfeld mit dem Lokator '{0}' wurde ausgelesen."),
        "Exception": ("2613451948", exception("Das Textfeld konnte nicht ausgelesen werden."))
      },
      "doc": (
          "3457248680", 
          """
          Der Inhalt des angegebenen Textfeldes wird zurückgegeben.
          
          *Textfeld mit einer Beschriftung links*
          | ``Textfeld auslesen    Beschriftung``
          
          *Textfeld mit einer Beschriftung oben*
          | ``Textfeld auslesen    @ Beschriftung``
          
          *Textfeld am Schnittpunkt einer Beschriftung links und einer oben*
          | ``Textfeld auslesen    Beschriftung links @ Beschriftung oben``
          
          *Textfeld mit dem angegebenen Inhalt*
          | ``Textfeld auslesen    = Inhalt``
          """
      )
    },
    "ReadText": {
      "name": ("3879608701", "Text auslesen"),
      "args": {
        "locator": {
          "name": ("2051440239", "Lokator"),
          "spec": {
              "HLabel": ("4229670492", HLabel),
              "Content": ("880934240", Content)
          }
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("837183792", not_found("Der Text mit dem Lokator '{0}' konnte nicht gefunden werden.")),
        "Pass": ("1360175273", "Der Text mit dem Lokator '{0}' wurde ausgelesen."),
        "Exception": ("3136337781", exception("Der Text konnte nicht ausgelesen werden."))
      },
      "doc": (
          "1046178438", 
          """
          Der Inhalt des angegebenen Texts wird zurückgegeben.
          
          *Text beginnt mit der angegebenen Teilzeichenfolge*
          | ``Text auslesen    = Teilzeichenfolge``
          
          *Text folgt einer Beschriftung*
          | ``Text auslesen    Beschriftung``
          """
      )
    },
    "ReadTableCell": {
      "name": ("389153112", "Tabellenzelle auslesen"),
      "args": {
        "a1row_locator": {
          "name": ("315353924", "Zeile"),
          "spec": {}
        },
        "a2column": {
          "name": ("2102626174", "Spaltentitel"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("2770335633", not_found("Die Zelle mit dem Lokator '{0}, {1}' konnte nicht gefunden werden.")),
        "Pass": ("4222878164", "Die Zelle mit dem Lokator '{0}, {1}' wurde ausgelesen."),
        "Exception": ("1272098876", exception("Die Zelle konnte nicht ausgelesen werden."))
      },
      "doc": (
          "3644317529", 
          """
          Der Inhalt der angegebenen Tabellenzelle wird zurückgegeben.
          
          | ``Tabellenzelle auslesen     Zeile     Spaltentitel``
          
          Zeile: Zeilennummer oder Zellinhalt.
          """
      )
    },
    "SaveScreenshot": {
      "name": ("2178392450", "Fenster aufnehmen"),
      "args": {
        "filepath": {
          "name": ("1769741420", "Dateipfad"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "InvalidPath": ("2844012395", "Der Pfad '{0}' ist ungültig"),
        "UNCPath": ("2462162559", r"Ein UNC Pfad (d.h. beginnend mit \\) ist nicht erlaubt"),
        "NoAbsPath": ("2858082864", "'{0}' ist kein absoluter Pfad"),
        "Pass": ("1427858469", "Eine Aufnahme des Fensters wurde in '{0}' gespeichert."),
        "Exception": ("3250735497", exception("Eine Aufnahme des Fensters konnte nicht gespeichert werden."))
      },
      "doc": (
          "313859733", 
          """
          Eine Bildschirmaufnahme des Fensters wird im eingegebenen Dateipfad gespeichert.
          | ``Fenster aufnehmen     Dateipfad``
          
          Dateifpad: Der absolute Pfad einer .png Datei.
          """
      )
    },
    "SelectCell": {
      "name": ("1049942265", "Tabellenzelle markieren"),
      "args": {
        "a1row_locator": {
          "name": ("315353924", "Zeile"),
          "spec": {}
        },
        "a2column": {
          "name": ("2102626174", "Spaltentitel"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("2770335633", not_found("Die Zelle mit dem Lokator '{0}, {1}' konnte nicht gefunden werden.")),
        "Pass": ("1239645311", "Die Zelle mit dem Lokator '{0}, {1}' wurde markiert."),
        "Exception": ("2355177759", exception("Die Zelle konnte nicht markiert werden."))
      },
      "doc": (
          "219237540", 
          """
          Die angegebene Tabellenzelle wird markiert.
          
          | ``Tabellenzelle markieren     Zeile     Spaltentitel``
          
          Zeile: Zeilennummer oder Zellinhalt.
          """
      )
    },
    "SelectComboBoxEntry": {
      "name": ("2133292945", "Auswahlmenüeintrag auswählen"),
      "args": {
        "a1comboBox": {
          "name": ("3378336226", "Name"),
          "spec": {}
        },
        "a2entry": {
          "name": ("723623280", "Eintrag"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("3185471891", not_found("Das Auswahlmenü mit dem Lokator '{0}' konnte nicht gefunden werden.")),
        "EntryNotFound": ("1357582115", not_found("Der Eintrag '{1}' wurde im Auswahlmenü '{0}' nicht gefunden.")),
        "Pass": ("2235674925", "Der Eintrag '{1}' wurde ausgewählt."),
        "Exception": ("2433413970", exception("Der Eintrag konnte nicht ausgewählt werden."))
      },
      "doc": (
          "1593493126", 
          """
          Aus dem angegebenen Auswahlmenü wird der angegebene Eintrag ausgewählt.
          
          | ``Auswahlmenüeintrag auswählen    Auswahlmenü    Eintrag``
          """
      )
    },
    "SelectRadioButton": {
      "name": ("2985728785", "Optionsfeld auswählen"),
      "args": {
        "locator": {
          "name": ("2051440239", "Beschriftung_oder_Lokator"),
          "spec": {
              "HLabel": ("4229670492", HLabel),
              "VLabel": ("474824962", VLabel),
              "HLabelVLabel": ("1999142431", HLabelVLabel)
          }
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("2755548585", not_found("Das Optionsfeld mit dem Lokator '{0}' konnte nicht gefunden werden.")),
        "Pass": ("259379063", "Das Optionsfeld mit dem Lokator '{0}' wurde ausgewählt."),
        "Exception": ("218028187", exception("Das Optionsfeld konnte nicht ausgewählt werden."))
      },
      "doc": (
          "532570380", 
          """
          Das angegebene Optionsfeld wird ausgewählt.
          
          *Optionsfeld mit einer Beschriftung links oder rechts*
          | ``Optionsfeld auswählen    Beschriftung``
          
          *Optionsfeld mit einer Beschriftung oben*
          | ``Optionsfeld auswählen    @ Beschriftung``
          
          *Optionsfeld am Schnittpunkt einer Beschriftung links (oder rechts) und einer oben*
          | ``Optionsfeld auswählen    Beschriftung links @ Beschriftung oben``
          """
      )
    },
    "SelectTableRow": {
      "name": ("1966160675", "Tabellenzeile markieren"),
      "args": {
          "row_number": {
              "name": ("3333868678", "Zeilennummer"),
              "spec": {}
          }
      },
      "result": {
          "NoSession": ("2754484086", no_session),
          "Exception": ("2370701678", exception("Die Zeile '{0}' konnte nicht markiert werden")),
          "NotFound": ("2402373821", "Die Tabelle enthält keine Zeile '{0}'"),
          "Pass": ("1473660338", "Die Zeile '{0}' wurde markiert")
      },
      "doc": ("398985647", """
        Die angegebene Tabellenzeile wird markiert.
              
        | ``Tabellenzeile markieren    Zeilennummer``
        """)
    },
    "SelectTextField": {
      "name": ("335907869", "Textfeld markieren"),
      "args": {
        "locator": {
          "name": ("2051440239", "Beschriftungen_oder_Lokator"),
          "spec": {
              "HLabel": ("4229670492", HLabel),
              "VLabel": ("474824962", VLabel),
              "HLabelVLabel": ("1999142431", HLabelVLabel),
              "Content": ("880934240", Content)
          }
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("2917845132", not_found("Das Textfeld mit dem Lokator '{0}' konnte nicht gefunden werden.")),
        "Pass": ("3773273557", "Das Textfeld mit dem Lokator '{0}' wurde markiert."),
        "Exception": ("1228826942", exception("Das Textfeld konnte nicht markiert werden."))
      },
      "doc": (
          "1350596231", 
          """
          Das angegebene Textfeld wird markiert.
          
          *Textfeld mit einer Beschriftung links*
          | ``Textfeld markieren    Beschriftung``
          
          *Textfeld mit einer Beschriftung oben*
          | ``Textfeld markieren    @ Beschriftung``
          
          *Textfeld am Schnittpunkt einer Beschriftung links und einer oben*
          | ``Textfeld markieren    Beschriftung links @ Beschriftung oben``
          
          *Textfeld mit dem angegebenen Inhalt*
          | ``Textfeld markieren    = Inhalt``
          """
      )
    },
    "SelectTextLine": {
      "name": ("4264534869", "Textzeile markieren"),
      "args": {
        "content": {
          "name": ("4274335913", "Inhalt"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("1356747844", not_found("Die Textzeile mit dem Inhalt '{0}' konnte nicht gefunden werden.")),
        "Pass": ("792202299", "Die Textzeile mit dem Inhalt '{0}' wurde markiert."),
        "Exception": ("528079567", exception("Die Textzeile konnte nicht markiert werden."))
      },
      "doc": (
          "1037973034", 
          """
          Die Textzeile mit dem angegebenen Inhalt wird markiert.
          
          | ``Textzeile markieren    Inhalt``
          """
      )
    },
    "TickCheckBox": {
      "name": ("2471720243", "Formularfeld ankreuzen"),
      "args": {
        "locator": {
          "name": ("2051440239", "Beschriftung_oder_Lokator"),
          "spec": {
              "HLabel": ("4229670492", HLabel),
              "VLabel": ("474824962", VLabel),
              "HLabelVLabel": ("1999142431", HLabelVLabel)
          }
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("3274358834", not_found("Das Formularfeld mit dem Lokator '{0}' konnte nicht gefunden werden.")),
        "Pass": ("999358000", "Das Formularfeld mit dem Lokator '{0}' wurde angekreuzt."),
        "Exception": ("1153105219", exception("Das Formularfeld konnte nicht angekreuzt werden."))
      },
      "doc": (
          "3375625571", 
          """
          Das angegebene Formularfeld wird angekreuzt.
          
          *Formularfeld mit einer Beschriftung links oder rechts *
          | ``Formularfeld ankreuzen    Beschriftung``
          
          *Formularfeld mit einer Beschriftung oben*
          | ``Formularfeld ankreuzen    @ Beschriftung``
          
          *Formularfeld am Schnittpunkt einer Beschriftung links und einer oben*
          | ``Formularfeld ankreuzen    Beschriftung links @ Beschriftung oben``
          """
      )
    },
    "ReadStatusbar": {
        "name": ("118752925", "Statusleiste auslesen"),
        "args": {},
        "result": {
            "NoSession": ("2754484086", no_session),
            "NotFound": ("2342000252", "Keine Statusleiste gefunden."),
            "Exception": ("803476123", exception("Die Statusleiste konnte nicht ausgelesen werden.")),
            "Pass": ("1105532895", "Die Statusleiste wurde ausgelesen.")
        },
        "doc": ("3222829205", """
          Die Nachricht der Statusleiste wird ausgelesen.
          
          ``Statusleiste auslesen``
         """)
    },
    "UntickCheckBox": {
      "name": ("47381427", "Formularfeld abwählen"),
      "args": {
        "locator": {
          "name": ("2051440239", "Beschriftung_oder_Lokator"),
          "spec": {
              "HLabel": ("4229670492", HLabel),
              "VLabel": ("474824962", VLabel),
              "HLabelVLabel": ("1999142431", HLabelVLabel)
          }
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("3274358834", not_found("Das Formularfeld mit dem Lokator '{0}' konnte nicht gefunden werden.")),
        "Pass": ("1077869101", "Das Formularfeld mit dem Lokator '{0}' wurde abgewählt."),
        "Exception": ("1479426504", exception("Das Formularfeld konnte nicht abgewählt werden."))
      },
      "doc": (
          "2004880129", 
          """
          Das angegebene Formularfeld wird abgewählt.
          
          *Formularfeld mit einer Beschriftung links oder rechts*
          | ``Formularfeld abwählen    Beschriftung``
          
          *Formularfeld mit einer Beschriftung oben*
          | ``Formularfeld abwählen    @ Beschriftung``
          
          *Formularfeld am Schnittpunkt einer Beschriftung links und einer oben*
          | ``Formularfeld abwählen    Beschriftung links @ Beschriftung oben``
          """
      )
    },
    "TickCheckBoxCell": {
      "name": ("3286561809", "Tabellenzelle ankreuzen"),
      "args": {
        "a1row": {
          "name": ("3333868678", "Zeilennummer"),
          "spec": {}
        },
        "a2column": {
          "name": ("2102626174", "Spaltentitel"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("2481184945", not_found("Die Zelle mit dem Lokator '{0}, {1}' konnte nicht gefunden werden.")),
        "Pass": ("1580249093", "Die Zelle mit dem Lokator '{0}, {1}' wurde angekreuzt."),
        "Exception": ("870126097", exception("Die Zelle konnte nicht angekreuzt werden."))
      },
      "doc": (
          "3815558161", 
          """
          Die angegebene Tabellenzelle wird angekreuzt.
          
          | ``Tabellenzelle ankreuzen     Zeilennummer     Spaltentitel``
          """
      )
    },
    "GetWindowTitle": {
      "name": ("2828980154", "Fenstertitel auslesen"),
      "args": {},
      "result": {
        "NoSession": ("2754484086", no_session),
        "Pass": ("2852411998", "Der Fenstertitel wurde ausgelesen"),
        "Exception": ("2687794215", exception("Der Titel des Fensters konnte nicht ausgelesen werden."))
      },
      "doc": (
          "985497510", 
          """
          Der Titel des Fensters im Fordergrund wird zurückgegeben.
          
          | ``${Titel}    Fenstertitel auslesen``
          """
      )
    },
    "GetWindowText": {
      "name": ("1085911504", "Fenstertext auslesen"),
      "args": {},
      "result": {
        "NoSession": ("2754484086", no_session),
        "Pass": ("2562559050", "Der Text des Fensters wurde ausgelesen"),
        "Exception": ("922315409", exception("Der Text des Fensters konnte nicht ausgelesen werden."))
      },
      "doc": (
          "1240406406", 
          """
          Der Text des Fensters im Fordergrund wird zurückgegeben.
          
          | ``${Text}    Fenstertext auslesen``
          """
      )
    }
  },
  "specs": {}
}
