from robot.api.deco import keyword
from RoboSAPiens.client import RoboSAPiensClient

class DE(RoboSAPiensClient):
    """
    RoboSAPiens: SAP GUI-Automatisierung für Menschen
    
    Um diese Bibliothek zu verwenden, müssen die folgenden Bedingungen erfüllt werden:
    
    - Das [https://help.sap.com/saphelp_aii710/helpdata/de/ba/b8710932b8c64a9e8acf5b6f65e740/content.htm|Scripting] muss auf dem SAP Server aktiviert werden.
    
    - Die [https://help.sap.com/docs/sap_gui_for_windows/63bd20104af84112973ad59590645513/7ddb7c9c4a4c43219a65eee4ca8db001.html|Skriptunterstützung] muss in der SAP GUI aktiviert werden.
    
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
    
    Die Anmeldung bei einem SAP Server erfolgt mit der folgenden Sequenz:
    
    | SAP starten                         C:${/}Program Files (x86)${/}SAP${/}FrontEnd${/}SAPgui${/}saplogon.exe
    | Verbindung zum Server herstellen    Mein Testserver
    | Textfeld ausfüllen                  Benutzer           TESTUSER
    | Textfeld ausfüllen                  Kennwort           TESTPASSWORD
    | Knopf drücken                       Weiter
    
    == Umgang mit spontanen Pop-up-Fenstern ==
    
    Beim Drücken eines Knopfes kann u.U. ein Dialogfenster aufpoppen.
    Das folgende Schlüsselwort kann in diesem Fall hilfreich sein:
    
    | Knopf drücken und Pop-up-Fenster schließen
    |   [Argumente]   ${Knopf}   ${Titel}    ${Knopf Schließen}
    |
    |   Knopf drücken     ${Knopf}
    |   ${Fenstertitel}   Fenstertitel auslesen
    |
    |   IF   $Fenstertitel == $Titel
    |       Log                 Pop-up Fenster: ${Titel}
    |       Fenster aufnehmen   LOG
    |       Knopf drücken       ${Knopf Schließen}
    |   END
    """
    
    def __init__(self, vortragsmodus: bool=False, x64: bool=False):
        """
        *vortragsmodus*: Jedes GUI Element wird vor seiner Betätigung bzw. Änderung kurz hervorgehoben
        
        *x64*: RoboSAPiens 64-bit ausführen
        """
        
        args = {
            'presenter_mode': vortragsmodus,
            'x64': x64,
        }
        
        super().__init__(args)
    

    @keyword('Reiter auswählen') # type: ignore
    def activate_tab(self, Reitername: str): # type: ignore
        """
        Der Reiter mit dem angegebenen Namen wird ausgewählt.
        
        | ``Reiter auswählen    Reitername``
        """
        
        args = [Reitername]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Der Reiter '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Der Reiter '{0}' wurde ausgewählt.",
            "Exception": "Der Reiter konnte nicht ausgewählt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ActivateTab', args, result) # type: ignore
    

    @keyword('Baumelement markieren') # type: ignore
    def select_tree_element(self, Elementpfad: str): # type: ignore
        """
        Das Baumelement mit dem angegebenen Pfad wird markiert.
        
        | ``Baumelement markieren    Elementpfad``
        
        Elementpfad: Der Pfad zum Element, mit '/' als Trennzeichen (z.B. Engineering/Bauwesen).
        """
        
        args = [Elementpfad]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Das Baumelement '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Das Baumelement '{0}' wurde markiert.",
            "Exception": "Das Baumelement konnte nicht markiert werden. {0}\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('SelectTreeElement', args, result) # type: ignore
    

    @keyword('SAP starten') # type: ignore
    def open_sap(self, Pfad: str): # type: ignore
        """
        Die SAP GUI wird gestartet. 
        
        | ``SAP starten   Pfad``
        
        Der übliche Pfad ist
        
        | ``C:\\Program Files (x86)\\SAP\\FrontEnd\\SAPgui\\saplogon.exe``
        
        *Hinweis*: Verwende ${/} als Trennzeichen. Ansonsten müssen die Rückwärtsschrägstriche geescaped werden.
        """
        
        args = [Pfad]
        
        result = {
            "Pass": "Die SAP GUI wurde gestartet",
            "NoGuiScripting": "Die Skriptunterstützung ist nicht verfügbar. Sie muss in den Einstellungen von SAP Logon aktiviert werden.",
            "SAPAlreadyRunning": "Die SAP GUI läuft gerade. Es muss vor dem Aufruf dieses Schlüsselworts beendet werden.",
            "SAPNotStarted": "Die SAP GUI konnte nicht gestartet werden. Überprüfe den Pfad '{0}'.",
            "Exception": "Die SAP GUI konnte nicht gestartet werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('OpenSap', args, result) # type: ignore
    

    @keyword('Verbindung zum Server trennen') # type: ignore
    def close_connection(self): # type: ignore
        """
        Die Verbindung mit dem SAP Server wird getrennt.
        
        | ``Verbindung zum Server trennen``
        """
        
        args = []
        
        result = {
            "NoSapGui": "Keine laufende SAP GUI gefunden. Das Keyword \"SAP starten\" muss zuerst aufgerufen werden.",
            "NoGuiScripting": "Die Skriptunterstützung ist nicht verfügbar. Sie muss in den Einstellungen von SAP Logon aktiviert werden.",
            "NoConnection": "Es besteht keine Verbindung zu einem SAP Server. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "Pass": "Die Verbindung zum Server wurde getrennt.",
            "Exception": "Die Verbindung zum Server konnte nicht getrennt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('CloseConnection', args, result) # type: ignore
    

    @keyword('SAP beenden') # type: ignore
    def close_sap(self): # type: ignore
        """
        Die SAP GUI wird beendet.
        
        | ``SAP beenden``
        
        *Hinweis*: Dieses Schlüsselwort funktioniert nur, wenn die SAP GUI mit dem Schlüsselwort "SAP starten" gestartet wurde.
        """
        
        args = []
        
        result = {
            "NoSapGui": "Keine laufende SAP GUI gefunden. Das Keyword \"SAP starten\" muss zuerst aufgerufen werden.",
            "Pass": "Die SAP GUI wurde beendet"
        }
        return super()._run_keyword('CloseSap', args, result) # type: ignore
    

    @keyword('Tabellenzeilen zählen') # type: ignore
    def count_table_rows(self): # type: ignore
        """
        Die Zeilen einer Tabelle werden gezählt.
        
        | ``${anzahl_zeilen}    Tabellenzeilen zählen``
        """
        
        args = []
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "Exception": "Die Zeilen der Tabelle konnten nicht gezählt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen.",
            "NotFound": "Die Maske enthält keine Tabelle.",
            "Pass": "Die Tabellenzeillen wurden gezählt."
        }
        return super()._run_keyword('CountTableRows', args, result) # type: ignore
    

    @keyword('Baumstruktur exportieren') # type: ignore
    def export_tree(self, Dateipfad: str): # type: ignore
        """
        Die Baumstruktur in der Maske wird in JSON Format in der angegebenen Datei gespeichert.
        
        | ``Baumstruktur exportieren     Dateipfad``
        
        Dateipfad: Absoluter Pfad zu einer Datei mit Endung .json
        
        *Hinweis*: Verwende ${/} als Trennzeichen. Ansonsten müssen die Rückwärtsschrägstriche geescaped werden.
        """
        
        args = [Dateipfad]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Die Maske enthält keine Baumstruktur",
            "Pass": "Die Baumstruktur wurde in JSON Format in der Datei '{0}' gespeichert",
            "Exception": "Die Baumstruktur konnte nicht exportiert werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ExportTree', args, result) # type: ignore
    

    @keyword('Laufende SAP GUI übernehmen') # type: ignore
    def attach_to_running_sap(self, session_nummer: str='1'): # type: ignore
        """
        Nach der Ausführung dieses Keywords kann eine laufende SAP GUI mit RoboSAPiens gesteuert werden. 
        Standardmäßig wird die Session Nummer 1 verwendet. Die gewünschte Session-Nummer kann als Parameter spezifiziert werden.
        
        | ``Laufende SAP GUI übernehmen    session_nummer``
        """
        
        args = [session_nummer]
        
        result = {
            "NoSapGui": "Keine laufende SAP GUI gefunden. Das Keyword \"SAP starten\" muss zuerst aufgerufen werden.",
            "NoGuiScripting": "Die Skriptunterstützung ist nicht verfügbar. Sie muss in den Einstellungen von SAP Logon aktiviert werden.",
            "NoConnection": "Es besteht keine Verbindung zu einem SAP Server. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NoServerScripting": "Das Scripting ist auf dem SAP Server nicht freigeschaltet. Siehe die Dokumentation von RoboSAPiens.",
            "InvalidSessionId": "Keine Session mit Nummer {0} vorhanden",
            "Pass": "Die laufende SAP GUI wurde erfolgreich übernommen.",
            "Exception": "Die laufende SAP GUI konnte nicht übernommen werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('AttachToRunningSap', args, result) # type: ignore
    

    @keyword('Verbindung zum Server herstellen') # type: ignore
    def connect_to_server(self, Servername: str): # type: ignore
        """
        Die Verbindung mit dem angegebenen SAP Server wird hergestellt.
        
        | ``Verbindung zum Server herstellen    Servername``
        
        Servername: Der Name des Servers in SAP Logon (nicht der SID).
        """
        
        args = [Servername]
        
        result = {
            "NoSapGui": "Keine laufende SAP GUI gefunden. Das Keyword \"SAP starten\" muss zuerst aufgerufen werden.",
            "NoGuiScripting": "Die Skriptunterstützung ist nicht verfügbar. Sie muss in den Einstellungen von SAP Logon aktiviert werden.",
            "Pass": "Die Verbindung mit dem Server '{0}' wurde erfolgreich hergestellt.",
            "SapError": "SAP Fehlermeldung: {0}",
            "NoServerScripting": "Das Scripting ist auf dem SAP Server nicht freigeschaltet. Siehe die Dokumentation von RoboSAPiens.",
            "Exception": "Die Verbindung konnte nicht hergestellt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ConnectToServer', args, result) # type: ignore
    

    @keyword('Tabellenzelle doppelklicken') # type: ignore
    def double_click_cell(self, Zeile: str, Spaltentitel: str): # type: ignore
        """
        Die angegebene Tabellenzelle wird doppelgeklickt.
        
        | ``Tabellenzelle doppelklicken     Zeile     Spaltentitel``
        
        Zeile: Entweder die Zeilennummer oder der Inhalt einer Zelle in der Zeile. Wenn die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden.
        """
        
        args = [Zeile, Spaltentitel]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Die Zelle mit dem Lokator '{0}, {1}' wurde nicht gefunden. Hinweise: Prüfe die Rechtschreibung, maximiere das SAP Fenster",
            "Pass": "Die Zelle mit dem Lokator '{0}, {1}' wurde doppelgeklickt.",
            "Exception": "Die Zelle konnte nicht doppelgeklickt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('DoubleClickCell', args, result) # type: ignore
    

    @keyword('Textfeld doppelklicken') # type: ignore
    def double_click_text_field(self, Lokator: str): # type: ignore
        """
        Das angegebene Textfeld wird doppelgeklickt.
        
        | ``Textfeld doppelklicken     Lokator``
        
        Die Lokatoren für Textfelder sind im Schlüsselwort "Textfeld ausfüllen" dokumentiert.
        """
        
        args = [Lokator]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Das Textfeld mit dem Lokator '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Das Textfeld mit dem Lokator '{0}' wurde doppelgeklickt.",
            "Exception": "Das Textfeld konnte nicht doppelgeklickt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('DoubleClickTextField', args, result) # type: ignore
    

    @keyword('Transaktion ausführen') # type: ignore
    def execute_transaction(self, T_Code: str): # type: ignore
        """
        Die Transaktion mit dem angegebenen T-Code wird ausgeführt.
        
        | ``Transaktion ausführen    T-Code``
        """
        
        args = [T_Code]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "Pass": "Die Transaktion mit T-Code '{0}' wurde erfolgreich ausgeführt.",
            "Exception": "Die Transaktion konnte nicht ausgeführt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ExecuteTransaction', args, result) # type: ignore
    

    @keyword('Maske exportieren') # type: ignore
    def export_window(self, Name: str, Verzeichnis: str): # type: ignore
        """
        Die Inhalte der Maske werden in einer JSON-Datei geschrieben. Außerdem wird ein Bildschirmfoto in PNG-Format erstellt.
        
        | ``Maske exportieren     Name     Verzeichnis``
        
        Verzeichnis: Der absolute Pfad des Verzeichnisses, wo die Dateien abgelegt werden.
        
        *Hinweis*: Verwende ${/} als Trennzeichen. Ansonsten müssen die Rückwärtsschrägstriche geescaped werden.
        
        *Anmerkung*: Aktuell werden nicht alle GUI-Elemente exportiert.
        """
        
        args = [Name, Verzeichnis]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "Pass": "Die Maske wurde in den Dateien '{0}' und '{1}' gespeichert",
            "Exception": "Die Maske konnte nicht exportiert werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ExportWindow', args, result) # type: ignore
    

    @keyword('Tabellenzelle ausfüllen') # type: ignore
    def fill_table_cell(self, Zeile: str, Spaltentitel: str, Inhalt: str): # type: ignore
        """
        Die Zelle am Schnittpunkt der Zeile und Spalte wird mit dem angegebenen Inhalt ausgefüllt.
        
        | ``Tabellenzelle ausfüllen     Zeile     Spaltentitel     Inhalt``
        
        Zeile: Entweder die Zeilennummer oder der Inhalt einer Zelle in der Zeile. Wenn die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden.
        
        *Hinweis*: Für die Migration aus dem alten Schlüsselwort mit zwei Argumenten soll eine Suche und Ersetzung mit einem regulären Ausdruck durchgeführt werden.
        """
        
        args = [Zeile, Spaltentitel, Inhalt]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Die Zelle mit dem Lokator '{0}, {1}' wurde nicht gefunden. Hinweise: Prüfe die Rechtschreibung, maximiere das SAP Fenster",
            "NotChangeable": "Die Zelle mit dem Lokator '{0}, {1}' ist nicht bearbeitbar.",
            "NoTable": "Die Maske enthält keine Tabelle.",
            "Pass": "Die Zelle mit dem Lokator '{0}, {1}' wurde ausgefüllt.",
            "Exception": "Die Zelle konnte nicht ausgefüllt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('FillTableCell', args, result) # type: ignore
    

    @keyword('Textfeld ausfüllen') # type: ignore
    def fill_text_field(self, Lokator: str, Inhalt: str): # type: ignore
        """
        Das angegebene Textfeld wird mit dem angegebenen Inhalt ausgefüllt.
        
        *Textfeld mit einer Beschriftung links*
        | ``Textfeld ausfüllen    Beschriftung    Inhalt``
        
        *Textfeld mit einer Beschriftung oben*
        | ``Textfeld ausfüllen    @ Beschriftung    Inhalt``
        
        *Textfeld am Schnittpunkt einer Beschriftung links und einer oben (inkl. eine Kastenüberschrift)*
        | ``Textfeld ausfüllen    Beschriftung links @ Beschriftung oben    Inhalt``
        
        *Textfeld in einem vertikalen Raster unter einer Beschriftung*
        | ``Textfeld ausfüllen    Position (1,2,..) @ Beschriftung    Inhalt``
        
        *Textfeld in einem horizontalen Raster nach einer Beschriftung*
        | ``Textfeld ausfüllen    Beschriftung @ Position (1,2,..)    Inhalt``
        
        *Textfeld mit einer nicht eindeutigen Beschriftung rechts von einem Textfeld mit einer Beschriftung*
        | ``Textfeld ausfüllen    Beschriftung des linken Textfelds >> Beschriftung    Inhalt``
        
        *Hinweise*
        
        - Normalerweise kann der Hilfetext, der mit der Taste F1 angezeigt wird, als Lokator verwendet werden.
        
        - Als letzter Ausweg kann der mit [https://tracker.stschnell.de/|Scripting Tracker] ermittelte Name als Lokator verwendet werden.
        """
        
        args = [Lokator, Inhalt]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Das Textfeld mit dem Lokator '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "NotChangeable": "Das Textfeld mit dem Lokator '{0}' ist nicht bearbeitbar.",
            "Pass": "Das Textfeld mit dem Lokator '{0}' wurde ausgefüllt.",
            "Exception": "Das Textfeld konnte nicht ausgefüllt werden. Möglicherweise, weil der Inhalt nicht dazu passt.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('FillTextField', args, result) # type: ignore
    

    @keyword('Knopf hervorheben') # type: ignore
    def highlight_button(self, Lokator: str): # type: ignore
        """
        Der Knopf mit dem angegebenen Lokator wird hervorgehoben.
        
        | ``Knopf hervorheben    Lokator``
        
        Lokator: Name oder Kurzinfo (Tooltip). 
        
        *Hinweis*: Einige Tooltips bestehen aus einem Namen, gefolgt von mehreren Leerzeichen und einem Tastaturkürzel.
        Der Name kann als Lokator verwendet werden, solange er eindeutig ist.
        Wenn der gesamte Text des Tooltips als Lokator verwendet wird, müssen die Leerzeichen gescaped werden (z.B. ``Zurück \\ \\ (F3)``).
        """
        
        args = [Lokator]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Der Knopf '{0}' wurde nicht gefunden. Hinweise: Prüfe die Rechtschreibung, maximiere das SAP Fenster",
            "Pass": "Der Knopf '{0}' wurde hervorgehoben.",
            "Exception": "Der Knopf konnte nicht hervorgehoben werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('HighlightButton', args, result) # type: ignore
    

    @keyword('Tastenkombination drücken') # type: ignore
    def press_key_combination(self, Tastenkombination: str): # type: ignore
        """
        Die angegebene Tastenkombination (mit englischen Tastenbezeichnungen) wird gedrückt.
        
        | ``Tastenkombination drücken    Tastenkombination``
        
        Die vollständige Liste der zulässigen Tastenkombinationen ist in der [https://help.sap.com/docs/sap_gui_for_windows/b47d018c3b9b45e897faf66a6c0885a8/71d8c95e9c7947ffa197523a232d8143.html?version=770.01|Dokumentation von SAP GUI].
        
        *Hinweis*: Das Drücken der Taste F2 hat die gleiche Wirkung wie ein Doppelklick.
        """
        
        args = [Tastenkombination]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "Exception": "Die Tastenkombination konnte nicht gedrückt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen.",
            "NotFound": "Die Tastenkombination '{0}' ist nicht vorhanden. Siehe die Dokumentation des Schlüsselworts für die Liste der zulässigen Tastenkombinationen.",
            "Pass": "Die Tastenkombination '{0}' wurde gedrückt."
        }
        return super()._run_keyword('PressKeyCombination', args, result) # type: ignore
    

    @keyword('Knopf drücken') # type: ignore
    def push_button(self, Lokator: str): # type: ignore
        """
        Der Knopf mit dem angegebenen Lokator wird gedrückt.
        
        | ``Knopf drücken    Lokator``
        
        Lokator: Name oder Kurzinfo (Tooltip). 
        
        *Hinweis*: Einige Tooltips bestehen aus einem Namen, gefolgt von mehreren Leerzeichen und einem Tastaturkürzel.
        Der Name kann als Lokator verwendet werden, solange er eindeutig ist.
        Wenn der gesamte Text des Tooltips als Lokator verwendet wird, müssen die Leerzeichen gescaped werden (z.B. ``Zurück \\ \\ (F3)``).
        """
        
        args = [Lokator]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Der Knopf '{0}' wurde nicht gefunden. Hinweise: Prüfe die Rechtschreibung, maximiere das SAP Fenster",
            "NotChangeable": "Der Knopf '{0}' ist deaktiviert.",
            "Pass": "Der Knopf '{0}' wurde gedrückt.",
            "Exception": "Der Knopf konnte nicht gedrückt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('PushButton', args, result) # type: ignore
    

    @keyword('Tabellenzelle drücken') # type: ignore
    def push_button_cell(self, Zeile: str, Spaltentitel: str): # type: ignore
        """
        Die angegebene Tabellenzelle wird gedrückt.
        
        | ``Tabellenzelle drücken     Zeile     Spaltentitel``
        
        Zeile: Zeilennummer, Beschriftung oder Kurzinfo (Tooltip) der Zelle, oder Inhalt einer Zelle in der Zeile. Wenn die Beschriftung, die Kurzinfo oder die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden.
        """
        
        args = [Zeile, Spaltentitel]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Die Zelle mit dem Lokator '{0}, {1}' wurde nicht gefunden. Hinweise: Prüfe die Rechtschreibung, maximiere das SAP Fenster",
            "NotChangeable": "Die Zelle mit dem Lokator '{0}, {1}' ist deaktiviert.",
            "Pass": "Die Zelle mit dem Lokator '{0}, {1}' wurde gedrückt.",
            "Exception": "Die Zelle konnte nicht gedrückt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('PushButtonCell', args, result) # type: ignore
    

    @keyword('Textfeld auslesen') # type: ignore
    def read_text_field(self, Lokator: str): # type: ignore
        """
        Der Inhalt des angegebenen Textfeldes wird zurückgegeben.
        
        | ${Inhalt}   ``Textfeld auslesen    Lokator``
        
        Die Lokatoren für Textfelder sind im Schlüsselwort "Textfeld ausfüllen" dokumentiert.
        """
        
        args = [Lokator]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Das Textfeld mit dem Lokator '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Das Textfeld mit dem Lokator '{0}' wurde ausgelesen.",
            "Exception": "Das Textfeld konnte nicht ausgelesen werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ReadTextField', args, result) # type: ignore
    

    @keyword('Text auslesen') # type: ignore
    def read_text(self, Lokator: str): # type: ignore
        """
        Der Inhalt des angegebenen Texts wird zurückgegeben.
        
        *Text beginnt mit der angegebenen Teilzeichenfolge*
        | ``${Text}   Text auslesen    = Teilzeichenfolge``
        
        *Text folgt einer Beschriftung*
        | ``${Text}   Text auslesen    Beschriftung``
        """
        
        args = [Lokator]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Der Text mit dem Lokator '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Der Text mit dem Lokator '{0}' wurde ausgelesen.",
            "Exception": "Der Text konnte nicht ausgelesen werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ReadText', args, result) # type: ignore
    

    @keyword('Tabellenzelle auslesen') # type: ignore
    def read_table_cell(self, Zeile: str, Spaltentitel: str): # type: ignore
        """
        Der Inhalt der angegebenen Tabellenzelle wird zurückgegeben.
        
        | ``Tabellenzelle auslesen     Zeile     Spaltentitel``
        
        Zeile: Entweder die Zeilennummer oder der Inhalt einer Zelle in der Zeile. Wenn die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden.
        """
        
        args = [Zeile, Spaltentitel]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Die Zelle mit dem Lokator '{0}, {1}' wurde nicht gefunden. Hinweise: Prüfe die Rechtschreibung, maximiere das SAP Fenster",
            "NoTable": "Die Maske enthält keine Tabelle.",
            "Pass": "Die Zelle mit dem Lokator '{0}, {1}' wurde ausgelesen.",
            "Exception": "Die Zelle konnte nicht ausgelesen werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ReadTableCell', args, result) # type: ignore
    

    @keyword('Fenster aufnehmen') # type: ignore
    def save_screenshot(self, Speicherort: str): # type: ignore
        """
        Eine Bildschirmaufnahme des Fensters wird im angegebenen Speicherort gespeichert.
        
        | ``Fenster aufnehmen     Speicherort``
        
        Speicherort: Entweder der absolute Pfad einer .png Datei oder LOG, um das Bild in das Protokoll einzubetten.
        
        *Hinweis*: Verwende ${/} als Trennzeichen. Ansonsten müssen die Rückwärtsschrägstriche geescaped werden.
        """
        
        args = [Speicherort]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "InvalidPath": "Der Pfad '{0}' ist ungültig",
            "UNCPath": "Ein UNC Pfad (d.h. beginnend mit \\\\) ist nicht erlaubt",
            "NoAbsPath": "'{0}' ist kein absoluter Pfad",
            "Log": "Der Rückgabewert wird in das Protokoll geschrieben.",
            "Pass": "Eine Aufnahme des Fensters wurde in '{0}' gespeichert.",
            "Exception": "Eine Aufnahme des Fensters konnte nicht gespeichert werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('SaveScreenshot', args, result) # type: ignore
    

    @keyword('Inhalte scrollen') # type: ignore
    def scroll_text_field_contents(self, Richtung: str, bis_Textfeld: str=None): # type: ignore
        """
        Die Inhalte der Textfelder in einem Bereich mit einer Bildlaufleiste werden gescrollt.
        
        | ``Inhalte scrollen    Richtung``
        
        Richtung: UP, DOWN, BEGIN, END
        
        Wenn der Parameter "bis_Textfeld" übergeben wird, werden die Inhalte so lange gescrollt, bis das Textfeld gefunden wird.
        
        | ``Inhalte scrollen    Richtung   bis_Textfeld``
        
        bis_Textfeld: Lokator, um ein Textfeld zu finden
        """
        
        args = [Richtung, bis_Textfeld]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "Exception": "Die Inhalte der Textfelder konnten nicht gescrollt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen.",
            "NoScrollbar": "Das Fenster enthält keine scrollbaren Textfelder.",
            "MaximumReached": "Die Inhalte der Textfelder können nicht weiter gescrollt werden.",
            "InvalidDirection": "Die angegebene Richtung ist ungültig. Gültige Richtungen sind: UP, DOWN, BEGIN, END",
            "Pass": "Die Inahlte der Textfelder wurden in die Richtung '{0}' gescrollt."
        }
        return super()._run_keyword('ScrollTextFieldContents', args, result) # type: ignore
    

    @keyword('Tabellenzelle markieren') # type: ignore
    def select_cell(self, Zeile: str, Spaltentitel: str): # type: ignore
        """
        Die angegebene Tabellenzelle wird markiert.
        
        | ``Tabellenzelle markieren     Zeile     Spaltentitel``
        
        Zeile: Entweder die Zeilennummer oder der Inhalt einer Zelle in der Zeile. Wenn die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden.
        """
        
        args = [Zeile, Spaltentitel]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Die Zelle mit dem Lokator '{0}, {1}' wurde nicht gefunden. Hinweise: Prüfe die Rechtschreibung, maximiere das SAP Fenster",
            "NoTable": "Die Maske enthält keine Tabelle.",
            "Pass": "Die Zelle mit dem Lokator '{0}, {1}' wurde markiert.",
            "Exception": "Die Zelle konnte nicht markiert werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('SelectCell', args, result) # type: ignore
    

    @keyword('Tabellenzellenwert auswählen') # type: ignore
    def select_cell_value(self, Zeile: str, Spaltentitel: str, Wert: str): # type: ignore
        """
        In der spezifizierten Zelle wird der angegebene Wert ausgewählt.
        
        | ``Tabellenzellenwert auswählen    Zeile    Spaltentitel    Wert``
        
        Zeile: Entweder die Zeilennummer oder der Inhalt einer Zelle in der Zeile. Wenn die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden.
        """
        
        args = [Zeile, Spaltentitel, Wert]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Die Zelle mit dem Lokator '{0}, {1}' wurde nicht gefunden. Hinweise: Prüfe die Rechtschreibung, maximiere das SAP Fenster",
            "EntryNotFound": "Der Wert '{2}' ist in der Zelle mit dem Lokator '{0}, {1}' nicht vorhanden.\nHinweis: Prüfe die Rechtschreibung",
            "Exception": "Der Wert konnte nicht ausgewählt werden. {0}\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen.",
            "Pass": "Der Wert '{2}' wurde ausgewählt."
        }
        return super()._run_keyword('SelectCellValue', args, result) # type: ignore
    

    @keyword('Auswahlmenüeintrag auswählen') # type: ignore
    def select_combo_box_entry(self, Auswahlmenü: str, Eintrag: str): # type: ignore
        """
        Aus dem angegebenen Auswahlmenü wird der angegebene Eintrag ausgewählt.
        
        | ``Auswahlmenüeintrag auswählen    Auswahlmenü    Eintrag``
        
        *Hinweise*: Der numerische Schlüssel, dass eine vereinfachte Tastaureingabe ermöglicht, ist nicht Teil des Eintragsnamens.
        
        Um einen Eintrag aus einem Symbolleisten-Knopf mit Auswahlmenü auszuwählen, drücke zuerst den Knopf und verwende danach dieses Schlüsselwort.
        """
        
        args = [Auswahlmenü, Eintrag]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Das Auswahlmenü mit dem Lokator '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "EntryNotFound": "Der Eintrag '{1}' wurde im Auswahlmenü '{0}' nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Der Eintrag '{1}' wurde ausgewählt.",
            "Exception": "Der Eintrag konnte nicht ausgewählt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('SelectComboBoxEntry', args, result) # type: ignore
    

    @keyword('Menüeintrag auswählen') # type: ignore
    def select_menu_item(self, Eintragspfad: str): # type: ignore
        """
        Der Menüeintrag mit dem angegebenen Pfad wird ausgewählt.
        
        | ``Menüeintrag auswählen    Eintragspfad``
        
        Eintragspfad: Der Pfad zum Eintrag mit '/' als Trennzeichen (z.B. System/Benutzervorgaben/Eigene Daten).
        """
        
        args = [Eintragspfad]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Der Menüeintrag '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Der Menüeintrag '{0}' wurde ausgewählt.",
            "Exception": "Der Menüeintrag konnte nicht ausgewählt werden. {0}\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('SelectMenuItem', args, result) # type: ignore
    

    @keyword('Optionsfeld auswählen') # type: ignore
    def select_radio_button(self, Lokator: str): # type: ignore
        """
        Das angegebene Optionsfeld wird ausgewählt.
        
        *Optionsfeld mit einer Beschriftung links oder rechts*
        | ``Optionsfeld auswählen    Beschriftung``
        
        *Optionsfeld mit einer Beschriftung oben*
        | ``Optionsfeld auswählen    @ Beschriftung``
        
        *Optionsfeld am Schnittpunkt einer Beschriftung links (oder rechts) und einer oben*
        | ``Optionsfeld auswählen    Beschriftung links @ Beschriftung oben``
        """
        
        args = [Lokator]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Das Optionsfeld mit dem Lokator '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "NotChangeable": "Das Optionsfeld mit dem Lokator '{0}' ist deaktiviert.",
            "Pass": "Das Optionsfeld mit dem Lokator '{0}' wurde ausgewählt.",
            "Exception": "Das Optionsfeld konnte nicht ausgewählt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('SelectRadioButton', args, result) # type: ignore
    

    @keyword('Tabellenzeile markieren') # type: ignore
    def select_table_row(self, Zeilenlokator: str): # type: ignore
        """
        Die angegebene Tabellenzeile wird markiert.
        
        | ``Tabellenzeile markieren    Zeilenlokator``
        
        Zeilenlokator: Zeile: Entweder die Zeilennummer oder der Inhalt einer Zelle in der Zeile. Wenn die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden.
        
        *Hinweis*: Mit der Zeilennummer 0 wird die gesamte Tabelle markiert.
        """
        
        args = [Zeilenlokator]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "Exception": "Die Zeile konnte nicht markiert werden. {0}\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen.",
            "NoTable": "Die Maske entählt keine Tabelle",
            "InvalidIndex": "Die Tabelle hat keine Zeile '{0}'",
            "NotFound": "Die Tabelle enthält keine Zelle mit dem Inhalt '{0}'",
            "Pass": "Die Zeile mit dem Lokator '{0}' wurde markiert"
        }
        return super()._run_keyword('SelectTableRow', args, result) # type: ignore
    

    @keyword('Textfeld markieren') # type: ignore
    def select_text_field(self, Lokator: str): # type: ignore
        """
        Das angegebene Textfeld wird markiert.
        
        | ``Textfeld markieren    Lokator``
        
        Die Lokatoren für Textfelder sind im Schlüsselwort "Textfeld ausfüllen" dokumentiert.
        """
        
        args = [Lokator]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Das Textfeld mit dem Lokator '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Das Textfeld mit dem Lokator '{0}' wurde markiert.",
            "Exception": "Das Textfeld konnte nicht markiert werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('SelectTextField', args, result) # type: ignore
    

    @keyword('Text markieren') # type: ignore
    def select_text(self, Lokator: str): # type: ignore
        """
        Der angegebene Text wird markiert.
        
        *Text beginnt mit der angegebenen Teilzeichenfolge*
        | ``Text markieren    = Teilzeichenfolge``
        
        *Text folgt einer Beschriftung*
        | ``Text markieren    Beschriftung``
        """
        
        args = [Lokator]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Der Text mit dem Lokator '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Der Text mit dem Lokator '{0}' wurde markiert.",
            "Exception": "Der Text konnte nicht markiert werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('SelectText', args, result) # type: ignore
    

    @keyword('Formularfeld ankreuzen') # type: ignore
    def tick_check_box(self, Lokator: str): # type: ignore
        """
        Das angegebene Formularfeld wird angekreuzt.
        
        *Formularfeld mit einer Beschriftung links oder rechts *
        | ``Formularfeld ankreuzen    Beschriftung``
        
        *Formularfeld mit einer Beschriftung oben*
        | ``Formularfeld ankreuzen    @ Beschriftung``
        
        *Formularfeld am Schnittpunkt einer Beschriftung links und einer oben*
        | ``Formularfeld ankreuzen    Beschriftung links @ Beschriftung oben``
        """
        
        args = [Lokator]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Das Formularfeld mit dem Lokator '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "NotChangeable": "Das Formularfeld mit dem Lokator '{0}' ist deaktiviert.",
            "Pass": "Das Formularfeld mit dem Lokator '{0}' wurde angekreuzt.",
            "Exception": "Das Formularfeld konnte nicht angekreuzt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('TickCheckBox', args, result) # type: ignore
    

    @keyword('Statusleiste auslesen') # type: ignore
    def read_statusbar(self): # type: ignore
        """
        Der Inhalt der Statusleiste wird ausgelesen.
        
        | ``${statusleiste}   Statusleiste auslesen``
        
        Der Rückgabewert ist ein Dictionary mit den Einträgen "status" und "message".
        """
        
        args = []
        
        result = {
            "Json": "Der Rückgabewert ist im JSON-Format",
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Keine Statusleiste gefunden.",
            "Exception": "Die Statusleiste konnte nicht ausgelesen werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen.",
            "Pass": "Die Statusleiste wurde ausgelesen."
        }
        return super()._run_keyword('ReadStatusbar', args, result) # type: ignore
    

    @keyword('Formularfeld abwählen') # type: ignore
    def untick_check_box(self, Lokator: str): # type: ignore
        """
        Das angegebene Formularfeld wird abgewählt.
        
        *Formularfeld mit einer Beschriftung links oder rechts*
        | ``Formularfeld abwählen    Beschriftung``
        
        *Formularfeld mit einer Beschriftung oben*
        | ``Formularfeld abwählen    @ Beschriftung``
        
        *Formularfeld am Schnittpunkt einer Beschriftung links und einer oben*
        | ``Formularfeld abwählen    Beschriftung links @ Beschriftung oben``
        """
        
        args = [Lokator]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Das Formularfeld mit dem Lokator '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "NotChangeable": "Das Formularfeld mit dem Lokator '{0}' ist deaktiviert.",
            "Pass": "Das Formularfeld mit dem Lokator '{0}' wurde abgewählt.",
            "Exception": "Das Formularfeld konnte nicht abgewählt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('UntickCheckBox', args, result) # type: ignore
    

    @keyword('Tabellenzelle ankreuzen') # type: ignore
    def tick_check_box_cell(self, Zeile: str, Spaltentitel: str): # type: ignore
        """
        Die angegebene Tabellenzelle wird angekreuzt.
        
        | ``Tabellenzelle ankreuzen     Zeile     Spaltentitel``
        
        Zeile: Entweder die Zeilennummer oder der Inhalt einer Zelle in der Zeile. Wenn die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden.
        
        *Hinweis*: Um das Formularfeld in der Spalte ganz links ohne Titel anzukreuzen, markiere die Zeile und drücke die "Enter"-Taste.
        """
        
        args = [Zeile, Spaltentitel]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Die Zelle mit dem Lokator '{0}, {1}' wurde nicht gefunden. Hinweise: Prüfe die Rechtschreibung, maximiere das SAP Fenster",
            "NotChangeable": "Die Zelle mit dem Lokator '{0}, {1}' ist deaktiviert.",
            "Pass": "Die Zelle mit dem Lokator '{0}, {1}' wurde angekreuzt.",
            "Exception": "Die Zelle konnte nicht angekreuzt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('TickCheckBoxCell', args, result) # type: ignore
    

    @keyword('Tabellenzelle abwählen') # type: ignore
    def untick_check_box_cell(self, Zeile: str, Spaltentitel: str): # type: ignore
        """
        Die angegebene Tabellenzelle wird abgewählt.
        
        | ``Tabellenzelle abwählen     Zeile     Spaltentitel``
        
        Zeile: Entweder die Zeilennummer oder der Inhalt einer Zelle in der Zeile. Wenn die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden.
        """
        
        args = [Zeile, Spaltentitel]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Die Zelle mit dem Lokator '{0}, {1}' wurde nicht gefunden. Hinweise: Prüfe die Rechtschreibung, maximiere das SAP Fenster",
            "NotChangeable": "Die Zelle mit dem Lokator '{0}, {1}' ist deaktiviert.",
            "Pass": "Die Zelle mit dem Lokator '{0}, {1}' wurde abgewählt.",
            "Exception": "Die Zelle konnte nicht abgewählt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('UntickCheckBoxCell', args, result) # type: ignore
    

    @keyword('Fenstertitel auslesen') # type: ignore
    def get_window_title(self): # type: ignore
        """
        Der Titel des Fensters im Fordergrund wird zurückgegeben.
        
        | ``${Titel}    Fenstertitel auslesen``
        """
        
        args = []
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "Pass": "Der Fenstertitel wurde ausgelesen",
            "Exception": "Der Titel des Fensters konnte nicht ausgelesen werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('GetWindowTitle', args, result) # type: ignore
    

    @keyword('Fenstertext auslesen') # type: ignore
    def get_window_text(self): # type: ignore
        """
        Der Text des Fensters im Fordergrund wird zurückgegeben.
        
        | ``${Text}    Fenstertext auslesen``
        """
        
        args = []
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "Pass": "Der Text des Fensters wurde ausgelesen",
            "Exception": "Der Text des Fensters konnte nicht ausgelesen werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('GetWindowText', args, result) # type: ignore
    
    ROBOT_LIBRARY_SCOPE = 'SUITE'
    ROBOT_LIBRARY_VERSION = '2.0.0'