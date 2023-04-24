from schema_i18n import LocalizedRoboSAPiens
from typing import Callable

Fstr = Callable[[str], str]

sap_error = 'SAP Fehlermeldung: {0}'
no_session = 'Keine SAP-Session vorhanden. Versuche zuerst das Keyword "Verbindung zum Server Herstellen" aufzurufen.'
no_sap_gui = 'Keine laufende SAP GUI gefunden. Das Keyword "SAP starten" muss zuerst aufgerufen werden.'
no_gui_scripting = 'Die Skriptunterstützung ist nicht verfügbar. Sie muss in den Einstellungen von SAP Logon aktiviert werden.'
no_connection = 'Es besteht keine Verbindung zu einem SAP Server. Versuche zuerst das Keyword "Verbindung zum Server Herstellen" aufzurufen.'
no_server_scripting = 'Das Scripting ist auf dem SAP Server nicht freigeschaltet. Siehe die Dokumentation von RoboSAPiens.'
not_found: Fstr = lambda msg: f"{msg}\nHinweis: Prüfe die Rechtschreibung"
exception: Fstr = lambda msg: f"{msg}" + "\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."

lib: LocalizedRoboSAPiens = {
  "doc": {
    "intro": ("2676661990", """RoboSAPiens: SAP GUI-Automatisierung für Menschen

    Um diese Bibliothek zu verwenden, müssen drei Bedigungen erfüllt werden:

    - Das .NET Runtime 7.0 x86 muss [https://dotnet.microsoft.com/en-us/download/dotnet/7.0|installiert] werden. 
    
    - Das [https://help.sap.com/saphelp_aii710/helpdata/de/ba/b8710932b8c64a9e8acf5b6f65e740/content.htm|Scripting] muss auf dem SAP Server aktiviert werden.
    
    - Die [https://help.sap.com/docs/sap_gui_for_windows/63bd20104af84112973ad59590645513/7ddb7c9c4a4c43219a65eee4ca8db001.html|Skriptunterstützung] muss in der SAP GUI aktiviert werden.

    Diese Bibliothek implementiert die [https://robotframework.org/robotframework/latest/RobotFrameworkUserGuide.html#remote-library-interface|Remote Library Interface] von Robot Framework. Das heißt, ein HTTP Server läuft im Hintergrund und Robot Framework kommuniziert mit ihm. Standardmäßig lauscht der HTTP Server auf dem Port 8270. Der Port kann beim Import der Bibliothek angepasst werden:
    | ``Library   RoboSAPiens  port=1234``
    """),
    "init": ("0", ""),
  },
  "args": {
    "a1port": {
      "name": ("1133600204", "port"),
      "default": 8270,
      "doc": ("2718491382", "Port des HTTP servers.")
    },
    "a2presenter_mode": {
      "name": ("781265386", "vortragsmodus"),
      "default": False,
      "doc": ("3421082408", "Jedes GUI Element wird vor seiner Betätigung bzw. Änderung kurz hervorgehoben")
    }
  },
  "keywords": {
    "ActivateTab": {
      "name": ("1870139227", "Reiter auswählen"),
      "args": {
        "Reitername": {
          "name": ("3665195662", "Reitername"),
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
      "doc": ("2453389726", "Der Reiter mit dem angegebenen Name wird ausgewählt.\n\n| ``Reiter auswählen    Name``")
    },
    "OpenSAP": {
      "name": ("1259182241", "SAP starten"),
      "args": {
        "Pfad": {
          "name": ("190089999", "Pfad"),
          "spec": {}
        }
      },
      "result": {
        "Pass": ("3933791589", "Die SAP GUI wurde gestartet"),
        "SAPNotStarted": ("4005776825", "Die SAP GUI konnte nicht gestartet werden. Überprüfe den Pfad '{0}'."),
        "Exception": ("2772047805", exception("Die SAP GUI konnte nicht gestartet werden."))
      },
      "doc": ("309583061", r"Die SAP GUI wird gestartet. Der übliche Pfad ist\n\n| ``C:\\Program Files (x86)\\SAP\\FrontEnd\\SAPgui\\saplogon.exe``")
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
      "doc": ("1736796211", "Die Verbindung mit dem SAP Server wird beendet.")
    },
    "CloseSAP": {
      "name": ("1795765665", "SAP beenden"),
      "args": {},
      "result": {
        "NoSapGui": ("2987622841", no_sap_gui),
        "Pass": ("2970606098", "Die SAP GUI wurde beendet")
      },
      "doc": ("1112371689", "Die SAP GUI wird beendet.")
    },
    "ExportTree": {
      "name": ("1188312707", "Funktionsbaum exportieren"),
      "args": {
        "Dateipfad": {
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
      "doc": ("1719447038", "Der Funktionsbaum wird in der angegebenen Datei gespeichert.\n\n| ``Funktionsbaum exportieren     Dateipfad``")
    },
    "AttachToRunningSAP": {
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
      "doc": ("866468958", "Nach der Ausführung dieses Keywords, kann eine laufende SAP GUI mit RoboSAPiens gesteuert werden.")
    },
    "ConnectToServer": {
      "name": ("1377779562", "Verbindung zum Server herstellen"),
      "args": {
        "Servername": {
          "name": ("763456934", "Servername"),
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
      "doc": ("287368400", "Die Verbindung mit dem angegebenen SAP Server wird hergestellt.\n\n| ``Verbindung zum Server herstellen    Servername``")
    },
    "DoubleClickCell": {
      "name": ("2108476291", "Tabellenzelle doppelklicken"),
      "args": {
        "a1Zeilennummer_oder_Zellinhalt": {
          "name": ("3668559387", "Zeilennummer_oder_Zellinhalt"),
          "spec": {}
        },
        "a2Spaltentitel": {
          "name": ("2102626174", "Spaltentitel"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("2770335633", not_found("Die Zelle mit dem Lokator '{0}, {1}' konnte nicht gefunden werden.")),
        "Pass": ("1249017752", "Die Zelle mit dem Lokator '{0}' wurde doppelgeklickt."),
        "Exception": ("2384367029", exception("Die Zelle konnte nicht doppelgeklickt werden."))
      },
      "doc": ("1790929105", "Die angegebene Tabellenzelle wird doppelgeklickt.\n\n| ``Tabellenzelle doppelklicken     Positionsgeber     Spaltentitel``\nPositionsgeber: entweder die Zeilennummer oder der Inhalt der Zelle.")
    },
    "DoubleClickTextField": {
      "name": ("3737103423", "Textfeld doppelklicken"),
      "args": {
        "Inhalt": {
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
      "doc": ("2849153724", "Das angegebene Textfeld wird doppelgeklickt.\n\n| ``Textfeld doppelklicken     Inhalt``\n")
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
      "doc": ("4152429702", "Die Transaktion mit dem angegebenen T-Code wird ausgeführt.\n\n| ``Transaktion ausführen    T-Code``")
    },
    "ExportForm": {
      "name": ("1168873090", "Maske exportieren"),
      "args": {
        "a1Name": {
          "name": ("1579384326", "Name"),
          "spec": {}
        },
        "a2Verzeichnis": {
          "name": ("1182287066", "Verzeichnis"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "Pass": ("1972246596", "Die Maske wurde in den Dateien '{0}' und '{1}' gespeichert"),
        "Exception": ("487625120", exception("Die Maske konnte nicht exportiert werden."))
      },
      "doc": ("3527465284", "Alle Texte in der aktuellen Maske werden in einer CSV-Datei gespeichert. Außerdem wird ein Bildschirmfoto in PNG-Format erstellt.\n\n| ``Maske exportieren     Name     Verzeichnis``\nVerzeichnis: Der absolute Pfad des Verzeichnisses, wo die Dateien abgelegt werden.")
    },
    "FillTableCell": {
      "name": ("1010164935", "Tabellenzelle ausfüllen"),
      "args": {
        "a1Zeilennummer_oder_Zellinhalt": {
          "name": ("3954689097", "Zeilennummer_oder_Zellinhalt"),
          "spec": {}
        },
        "a2Spaltentitel_Gleich_Inhalt": {
          "name": ("2701366812", "Spaltentitel_Gleich_Inhalt"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "InvalidFormat": ("280110049", "Das zweite Argument muss dem Muster `Spalte = Inhalt` entsprechen"),
        "NotFound": ("3381319755", not_found("Die Zelle mit dem Lokator '{0}, {1}' konnte nicht gefunden werden.")),
        "Pass": ("927017588", "Die Zelle mit dem Lokator '{0}' wurde ausgefüllt."),
        "Exception": ("1958379303", exception("Die Zelle konnte nicht ausgefüllt werden."))
      },
      "doc": ("917168571", "Die Zelle am Schnittpunkt der angegebenen Zeile und Spalte wird mit dem angegebenen Inhalt ausgefüllt.\n\n| ``Tabellenzelle ausfüllen     Zeile     Spaltentitel = Inhalt``\nZeile: entweder eine Zeilennummer oder der Inhalt einer Zelle in der Zeile.\n\n*Hinweis*: Eine Tabellenzelle hat u.U. eine Beschriftung, die man über die Hilfe (Taste F1) herausfinden kann. In diesem Fall kann man die Zelle mit dem Keyword [#Textfeld%20Ausfüllen|Textfeld ausfüllen] ausfüllen.")
    },
    "FillTextField": {
      "name": ("3103200585", "Textfeld ausfüllen"),
      "args": {
        "a1Beschriftung_oder_Positionsgeber": {
          "name": ("2051440239", "Beschriftung_oder_Positionsgeber"),
          "spec": {}
        },
        "a2Inhalt": {
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
      "doc": ("3658065712", "Das angegebene Textfeld wird mit dem angegebenen Inhalt ausgefüllt.\n\n*Textfeld mit einer Beschriftung links*\n| ``Textfeld ausfüllen    Beschriftung    Inhalt``\n*Textfeld mit einer Beschriftung oben*\n| ``Textfeld ausfüllen    @ Beschriftung    Inhalt``\n*Textfeld am Schnittpunkt einer Beschriftung links und einer oben (z.B. eine Abschnittsüberschrift)*\n| ``Textfeld ausfüllen    Beschriftung links @ Beschriftung oben    Inhalt``\n*Textfeld ohne Beschriftung unter einem Textfeld mit einer Beschriftung (z.B. eine Adresszeile)*\n| ``Textfeld ausfüllen    Position (1,2,..) @ Beschriftung    Inhalt``\n\n*Textfeld ohne Beschriftung rechts von einem Textfeld mit einer Beschriftung*\n| ``Textfeld ausfüllen    Beschriftung @ Position (1,2,..)    Inhalt``\n\n*Textfeld mit einer nicht eindeutigen Beschriftung rechts von einem Textfeld mit einer Beschriftung*\n| ``Textfeld ausfüllen    Beschriftung des linken Textfelds >> Beschriftung    Inhalt``\n\n*Hinweis*: In der Regel hat ein Textfeld eine unsichtbare Beschriftung, die man über die Hilfe (Taste F1) herausfinden kann.")
    },
    "PushButton": {
      "name": ("2326550334", "Knopf drücken"),
      "args": {
        "Name_oder_Kurzinfo": {
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
      "doc": ("1916622155", "Der Knopf mit dem angegebenen Namen oder Kurzinfo (Tooltip) wird gedrückt.\n\n| ``Knopf drücken    Name oder Kurzinfo (Tooltip)``")
    },
    "PushButtonCell": {
      "name": ("349686496", "Tabellenzelle drücken"),
      "args": {
        "a1Zeilennummer_oder_Name_oder_Kurzinfo": {
          "name": ("1299279537", "Zeilennummer_oder_Name_oder_Kurzinfo"),
          "spec": {}
        },
        "a2Spaltentitel": {
          "name": ("2102626174", "Spaltentitel"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("2199892932", not_found("Die Zelle mit dem Lokator '{0}, {1}' konnte nicht gefunden werden.")),
        "Pass": ("1649470590", "Die Zelle mit dem Lokator '{0}' wurde gedrückt."),
        "Exception": ("1751102722", exception("Die Zelle konnte nicht gedrückt werden."))
      },
      "doc": ("3042146913", "Die angegebene Tabellenzelle wird gedrückt.\n\n| ``Tabellenzelle drücken     Positionsgeber     Spaltentitel``\nPositionsgeber: Zeilennummer, Beschriftung oder Kurzinfo (Tooltip).")
    },
    "ReadTextField": {
      "name": ("490498248", "Textfeld auslesen"),
      "args": {
        "Beschriftung_oder_Positionsgeber": {
          "name": ("2051440239", "Beschriftung_oder_Positionsgeber"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("2917845132", not_found("Das Textfeld mit dem Lokator '{0}' konnte nicht gefunden werden.")),
        "Pass": ("2524131110", "Das Textfeld mit dem Lokator '{0}' wurde ausgelesen."),
        "Exception": ("2613451948", exception("Das Textfeld konnte nicht ausgelesen werden."))
      },
      "doc": ("912380966", "Der Inhalt des angegebenen Textfeldes wird zurückgegeben.\n\n*Textfeld mit einer Beschriftung links*\n| ``Textfeld auslesen    Beschriftung``\n*Textfeld mit einer Beschriftung oben*\n| ``Textfeld auslesen    @ Beschriftung``\n*Textfeld am Schnittpunkt einer Beschriftung links und einer oben*\n| ``Textfeld auslesen    Beschriftung links @ Beschriftung oben``\n*Textfeld mit dem angegebenen Inhalt*\n| ``Textfeld auslesen    = Inhalt``")
    },
    "ReadText": {
      "name": ("3879608701", "Text auslesen"),
      "args": {
        "Inhalt": {
          "name": ("4274335913", "Inhalt"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("837183792", not_found("Der Text mit dem Lokator '{0}' konnte nicht gefunden werden.")),
        "Pass": ("1360175273", "Der Text mit dem Lokator '{0}' wurde ausgelesen."),
        "Exception": ("3136337781", exception("Der Text konnte nicht ausgelesen werden."))
      },
      "doc": ("858400447", "Der Inhalt des angegebenen Texts wird zurückgegeben.\n\n*Text fängt mit der angegebenen Teilzeichenfolge an*\n| ``Text auslesen    = Teilzeichenfolge``\n*Text folgt einer Beschriftung*\n| ``Text auslesen    Beschriftung``")
    },
    "ReadTableCell": {
      "name": ("389153112", "Tabellenzelle auslesen"),
      "args": {
        "a1Zeilennummer_oder_Zellinhalt": {
          "name": ("3954689097", "Zeilennummer_oder_Zellinhalt"),
          "spec": {}
        },
        "a2Spaltentitel": {
          "name": ("2102626174", "Spaltentitel"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("2770335633", not_found("Die Zelle mit dem Lokator '{0}, {1}' konnte nicht gefunden werden.")),
        "Pass": ("2745435444", "Die Zelle mit dem Lokator '{0}' wurde ausgelesen."),
        "Exception": ("1272098876", exception("Die Zelle konnte nicht ausgelesen werden."))
      },
      "doc": ("666704855", "Der Inhalt der angegebenen Tabellenzelle wird zurückgegeben.\n\n| ``Tabellenzelle ablesen     Positionsgeber     Spaltentitel``\nPositionsgeber: Zeilennummer oder Zellinhalt.")
    },
    "SaveScreenshot": {
      "name": ("2178392450", "Fenster aufnehmen"),
      "args": {
        "Aufnahmenverzeichnis": {
          "name": ("1769741420", "Aufnahmenverzeichnis"),
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
      "doc": ("3488461470", "Eine Bildschirmaufnahme des Fensters wird im eingegebenen Dateipfad gespeichert.\n\n| ``Fenster aufnehmen     Dateipfad``\nDateifpad: Der absolute Pfad einer .png Datei bzw. eines Verzeichnisses.")
    },
    "SelectCell": {
      "name": ("1049942265", "Tabellenzelle markieren"),
      "args": {
        "a1Zeilennummer_oder_Zellinhalt": {
          "name": ("3954689097", "Zeilennummer_oder_Zellinhalt"),
          "spec": {}
        },
        "a2Spaltentitel": {
          "name": ("2102626174", "Spaltentitel"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("2770335633", not_found("Die Zelle mit dem Lokator '{0}, {1}' konnte nicht gefunden werden.")),
        "Pass": ("3085393420", "Die Zelle mit dem Lokator '{0}' wurde markiert."),
        "Exception": ("2355177759", exception("Die Zelle konnte nicht markiert werden."))
      },
      "doc": ("497443824", "Die angegebene Tabellenzelle wird markiert.\n\n| ``Tabellenzelle markieren     Positionsgeber     Spaltentitel``\nPositionsgeber: Zeilennummer oder Zellinhalt.")
    },
    "SelectComboBoxEntry": {
      "name": ("2133292945", "Auswahlmenüeintrag auswählen"),
      "args": {
        "a1Name": {
          "name": ("3378336226", "Name"),
          "spec": {}
        },
        "a2Eintrag": {
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
      "doc": ("92484869", "Aus dem angegebenen Auswahlmenü wird der angegebene Eintrag ausgewählt.\n\n| ``Auswahlmenüeintrag auswählen    Auswahlmenü    Eintrag``")
    },
    "SelectRadioButton": {
      "name": ("2985728785", "Optionsfeld auswählen"),
      "args": {
        "Beschriftung_oder_Positionsgeber": {
          "name": ("2051440239", "Beschriftung_oder_Positionsgeber"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("2755548585", not_found("Das Optionsfeld mit dem Lokator '{0}' konnte nicht gefunden werden.")),
        "Pass": ("259379063", "Das Optionsfeld mit dem Lokator '{0}' wurde ausgewählt."),
        "Exception": ("218028187", exception("Das Optionsfeld konnte nicht ausgewählt werden."))
      },
      "doc": ("2939575456", "Das angegebene Optionsfeld wird ausgewählt.\n\n*Optionsfeld mit einer Beschriftung links oder rechts*\n| ``Optionsfeld auswählen    Beschriftung``\n*Optionsfeld mit einer Beschriftung oben*\n| ``Optionsfeld auswählen    @ Beschriftung``\n*Optionsfeld am Schnittpunkt einer Beschriftung links (oder rechts) und einer oben*\n| ``Optionsfeld auswählen    Beschriftung links @ Beschriftung oben``\n")
    },
    "SelectTextField": {
      "name": ("335907869", "Textfeld markieren"),
      "args": {
        "Beschriftungen_oder_Inhalt": {
          "name": ("2051440239", "Beschriftungen_oder_Inhalt"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("2917845132", not_found("Das Textfeld mit dem Lokator '{0}' konnte nicht gefunden werden.")),
        "Pass": ("3773273557", "Das Textfeld mit dem Lokator '{0}' wurde markiert."),
        "Exception": ("1228826942", exception("Das Textfeld konnte nicht markiert werden."))
      },
      "doc": ("2308992901", "Das angegebene Textfeld wird markiert.\n\n*Textfeld mit einer Beschriftung links*\n| ``Textfeld markieren    Beschriftung``\n*Textfeld mit einer Beschriftung oben*\n| ``Textfeld markieren    @ Beschriftung``\n*Textfeld am Schnittpunkt einer Beschriftung links und einer oben*\n| ``Textfeld markieren    Beschriftung links @ Beschriftung oben``\n*Textfeld mit dem angegebenen Inhalt*\n| ``Textfeld markieren    = Inhalt``")
    },
    "SelectTextLine": {
      "name": ("4264534869", "Textzeile markieren"),
      "args": {
        "Inhalt": {
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
      "doc": ("4023168358", "Die Textzeile mit dem angegebenen Inhalt wird markiert.\n| ``Textzeile markieren    Inhalt``")
    },
    "TickCheckBox": {
      "name": ("2471720243", "Formularfeld ankreuzen"),
      "args": {
        "Beschriftung_oder_Positionsgeber": {
          "name": ("2051440239", "Beschriftung_oder_Positionsgeber"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("3274358834", not_found("Das Formularfeld mit dem Lokator '{0}' konnte nicht gefunden werden.")),
        "Pass": ("999358000", "Das Formularfeld mit dem Lokator '{0}' wurde angekreuzt."),
        "Exception": ("1153105219", exception("Das Formularfeld konnte nicht angekreuzt werden."))
      },
      "doc": ("3559298022", "Das angegebene Formularfeld wird angekreuzt.\n\n*Formularfeld mit einer Beschriftung links oder rechts *\n| ``Formularfeld ankreuzen    Beschriftung``\n*Formularfeld mit einer Beschriftung oben*\n| ``Formularfeld ankreuzen    @ Beschriftung``\n*Formularfeld am Schnittpunkt einer Beschriftung links und einer oben*\n| ``Formularfeld ankreuzen    Beschriftung links @ Beschriftung oben``")
    },
    "UntickCheckBox": {
      "name": ("47381427", "Formularfeld abwählen"),
      "args": {
        "Beschriftung_oder_Positionsgeber": {
          "name": ("2051440239", "Beschriftung_oder_Positionsgeber"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("3274358834", not_found("Das Formularfeld mit dem Lokator '{0}' konnte nicht gefunden werden.")),
        "Pass": ("1077869101", "Das Formularfeld mit dem Lokator '{0}' wurde abgewählt."),
        "Exception": ("1479426504", exception("Das Formularfeld konnte nicht abgewählt werden."))
      },
      "doc": ("2834166382", "Das angegebene Formularfeld wird abgewählt.\n\n*Formularfeld mit einer Beschriftung links oder rechts *\n| ``Formularfeld abwählen    Beschriftung``\n*Formularfeld mit einer Beschriftung oben*\n| ``Formularfeld abwählen    @ Beschriftung``\n*Formularfeld am Schnittpunkt einer Beschriftung links und einer oben*\n| ``Formularfeld abwählen    Beschriftung links @ Beschriftung oben``")
    },
    "TickCheckBoxCell": {
      "name": ("3286561809", "Tabellenzelle ankreuzen"),
      "args": {
        "a1Zeilennummer": {
          "name": ("3333868678", "Zeilennummer"),
          "spec": {}
        },
        "a2Spaltentitel": {
          "name": ("2102626174", "Spaltentitel"),
          "spec": {}
        }
      },
      "result": {
        "NoSession": ("2754484086", no_session),
        "NotFound": ("2481184945", not_found("Die Zelle mit dem Lokator '{0}, {1}' konnte nicht gefunden werden.")),
        "Pass": ("1580249093", "Die Zelle mit dem Lokator '{0}' wurde angekreuzt."),
        "Exception": ("870126097", exception("Die Zelle konnte nicht angekreuzt werden."))
      },
      "doc": ("3059587832", "Die angegebene Tabellenzelle wird angekreuzt.\n\n| ``Tabellenzelle ankreuzen     Zeilennummer     Spaltentitel``")
    },
    "GetWindowTitle": {
      "name": ("2828980154", "Fenstertitel auslesen"),
      "args": {},
      "result": {
        "NoSession": ("2754484086", no_session),
        "Pass": ("2852411998", "Der Fenstertitel wurde ausgelesen")
      },
      "doc": ("1638398427", "Der Titel des Fensters im Fordergrund wird zurückgegeben.\n\n| ``${Titel}    Fenstertitel auslesen``")
    },
    "GetWindowText": {
      "name": ("1085911504", "Fenstertext auslesen"),
      "args": {},
      "result": {
        "NoSession": ("2754484086", no_session),
        "Pass": ("2562559050", "Der Text des Fensters wurde ausgelesen")
      },
      "doc": ("2803599762", "Der Text des Fensters im Fordergrund wird zurückgegeben.\n\n| ``${Text}    Fenstertext auslesen``")
    }
  },
  "specs": {}
}