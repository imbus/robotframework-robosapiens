from robot.api.deco import keyword
from RoboSAPiens.client import RoboSAPiensClient

class DE(RoboSAPiensClient):
    """
    RoboSAPiens: SAP GUI-Automatisierung für Menschen
    
    Um diese Bibliothek zu verwenden, müssen die folgenden Bedingungen erfüllt werden:
    
    - Das Scripting muss [https://help.sap.com/saphelp_aii710/helpdata/de/ba/b8710932b8c64a9e8acf5b6f65e740/content.htm|auf dem SAP Server aktiviert werden].
    
    - Die Skriptunterstützung muss [https://help.sap.com/docs/sap_gui_for_windows/63bd20104af84112973ad59590645513/7ddb7c9c4a4c43219a65eee4ca8db001.html|in der SAP GUI aktiviert werden].
    
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
    |     [Arguments]    ${keyword}    ${locator}    ${message}
    |     [Tags]         robot:flatten
    |     
    |     TRY
    |         Run Keyword    ${keyword}    ${locator}
    |     EXCEPT  Not${state}: *    type=GLOB
    |         Fail    ${message}
    |     END
    
    Zum Beispiel, das folgende Schlüsselwort sichert zu, dass ein bestimmtes Textfeld vorhanden ist.
    
    | Textfeld ist vorhanden
    |     [Argumente]    ${Lokator}
    | 
    |     Element should be Found    Textfeld markieren    ${Lokator}    Das Textfeld '${Lokator}' ist nicht vorhanden.
    """
    
    def __init__(self, vortragsmodus: bool=False, x64: bool=False):
        """
        RoboSAPiens.DE hat die folgenden Initialisierungsparameter:
        
        | =Parameter= | =Beschreibung= |
        | ``vortragsmodus`` | Nach dem Aufruf eines Schlüsselworts eine halbe Sekunde warten und das betroffene GUI Element hervorheben (falls zutreffend). |
        | ``x64`` | RoboSAPiens 64-bit ausführen, um SAP GUI 8 64-bit bzw. SAP Business Client zu automatisieren. |
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
        
        | ``Reitername`` | Name oder Kurzinfo des Reiters |
        
        
        Beispiele:
        
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
    

    @keyword('Baumelement doppelklicken') # type: ignore
    def double_click_tree_element(self, Elementpfad: str): # type: ignore
        """
        Das Baumelement mit dem angegebenen Pfad wird doppelgeklickt.
        
        | ``Elementpfad`` | Der Pfad zum Element, mit '/' als Trennzeichen (z.B. Engineering/Bauwesen). |
        
        
        Beispiele:
        
        | ``Baumelement doppelklicken    Elementpfad``
        """
        
        args = [Elementpfad]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Das Baumelement '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Das Baumelement '{0}' wurde doppelgeklickt.",
            "Exception": "Das Baumelement konnte nicht doppelgeklickt werden. {0}\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('DoubleClickTreeElement', args, result) # type: ignore
    

    @keyword('Baumelement markieren') # type: ignore
    def select_tree_element(self, Elementpfad: str): # type: ignore
        """
        Das Baumelement mit dem angegebenen Pfad wird markiert.
        
        | ``Elementpfad`` | Der Pfad zum Element, mit '/' als Trennzeichen (z.B. Engineering/Bauwesen). |
        
        
        Beispiele:
        
        | ``Baumelement markieren    Elementpfad``
        """
        
        args = [Elementpfad]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Das Baumelement '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Das Baumelement '{0}' wurde markiert.",
            "Exception": "Das Baumelement konnte nicht markiert werden. {0}\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('SelectTreeElement', args, result) # type: ignore
    

    @keyword('Menüeintrag in Baumelement auswählen') # type: ignore
    def select_tree_element_menu_entry(self, Elementpfad: str, Menüeintrag: str): # type: ignore
        """
        Aus dem Kontextmenü des Baumelements mit dem angegebenen Pfad wird der angebene Eintrag ausgewählt.
        
        | ``Elementpfad`` | Der Pfad zum Element, mit '/' als Trennzeichen (z.B. Engineering/Bauwesen). |
        | ``Menüeintrag`` | Der Menüeintrag. Bei verschachtelten Menüs der Pfad zum Eintrag mit '|' als Trennzeichen (z.B. Anlegen|Wirtschaftseinheit). |
        
        
        Beispiele:
        
        | ``Menüeintrag in Baumelement auswählen    Elementpfad    Menüeintrag``
        """
        
        args = [Elementpfad, Menüeintrag]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Das Baumelement '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Der Menüeintrag '{0}' wurde ausgewählt.",
            "Exception": "Der Menüeintrag konnte nicht ausgewählt werden. {0}\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('SelectTreeElementMenuEntry', args, result) # type: ignore
    

    @keyword('SAP starten') # type: ignore
    def open_sap(self, Pfad: str, SAP_Parameter: str=None): # type: ignore
        """
        SAP GUI bzw. SAP Business Client wird gestartet.
        
        | ``Pfad`` | Der Pfad zu saplogon.exe oder NWBC.exe |
        | ``SAP_Parameter`` | Kommandozeileparameter für den SAP Client |
        
        
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
        
        | ``SAP starten   C:\\Program Files\\SAP\\FrontEnd\\SAPgui\\sapshcut.exe -system=XXX -client=NNN -user=%{username} -pw=%{password}``
        
        *Hinweis*: Rückwärtsschrägstriche müssen doppelt geschrieben werden. Ansonsten verwende die Standard RF Variable ${/} als Trennzeichen.
        """
        
        args = [Pfad, SAP_Parameter]
        
        result = {
            "Pass": "SAP wurde gestartet",
            "NoGuiScripting": "Die Skriptunterstützung ist nicht verfügbar. Sie muss in den Einstellungen vom SAP Client aktiviert werden.",
            "SAPAlreadyRunning": "SAP läuft gerade. Es muss vor dem Aufruf dieses Schlüsselworts beendet werden.",
            "SAPNotStarted": "SAP konnte nicht gestartet werden. Überprüfe den Pfad und ggf. die Parameter '{0}'.",
            "Exception": "SAP konnte nicht gestartet werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('OpenSap', args, result) # type: ignore
    

    @keyword('Verbindung zum Server trennen') # type: ignore
    def close_connection(self): # type: ignore
        """
        Die Verbindung zum SAP Server wird getrennt.
        
        
        Beispiele:
        
        | ``Verbindung zum Server trennen``
        """
        
        args = []
        
        result = {
            "NoSapGui": "Keine laufende SAP GUI gefunden. Das Keyword \"SAP starten\" muss zuerst aufgerufen werden.",
            "NoGuiScripting": "Die Skriptunterstützung ist nicht verfügbar. Sie muss in den Einstellungen vom SAP Client aktiviert werden.",
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
        
        
        Beispiele:
        
        | ``SAP beenden``
        
        *Hinweis*: Dieses Schlüsselwort funktioniert nur, wenn die SAP GUI mit dem Schlüsselwort [#SAP starten|SAP starten] gestartet wurde.
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
        
        
        Beispiele:
        
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
        
        | ``Dateipfad`` | Absoluter Pfad zu einer Datei mit Endung .json |
        
        
        Beispiele:
        
        | ``Baumstruktur exportieren     Dateipfad``
        
        *Hinweis*: Rückwärtsschrägstriche müssen doppelt geschrieben werden. Ansonsten verwende die Standard RF Variable ${/} als Trennzeichen.
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
        
        | ``session_nummer`` | Die Nummer der SAP-Session in der rechten unteren Ecke des Fensters |
        
        
        Beispiele:
        
        | ``Laufende SAP GUI übernehmen``
        
        Standardmäßig wird die Session Nummer 1 verwendet. Die gewünschte Session-Nummer kann als Parameter spezifiziert werden.
        
        | ``Laufende SAP GUI übernehmen    session_nummer``
        
        Der Rückgabewert enthält Informationen über die Session wie z.B. Mandant und System-ID:
        
        | ``${session_info}    Laufende SAP GUI übernehmen    session_nummer``
        """
        
        args = [session_nummer]
        
        result = {
            "NoSapGui": "Keine laufende SAP GUI gefunden. Das Keyword \"SAP starten\" muss zuerst aufgerufen werden.",
            "NoGuiScripting": "Die Skriptunterstützung ist nicht verfügbar. Sie muss in den Einstellungen vom SAP Client aktiviert werden.",
            "NoConnection": "Es besteht keine Verbindung zu einem SAP Server. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NoServerScripting": "Das Scripting ist auf dem SAP Server nicht freigeschaltet. Siehe die Dokumentation von RoboSAPiens.",
            "InvalidSessionId": "Keine Session mit Nummer {0} vorhanden",
            "Json": "Der Rückgabewert ist im JSON-Format",
            "Pass": "Die laufende SAP GUI wurde erfolgreich übernommen.",
            "Exception": "Die laufende SAP GUI konnte nicht übernommen werden. Hinweis: Für die Verbindung mit einem 64-bit SAP Client muss RoboSAPiens.DE mit x64=True importiert werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('AttachToRunningSap', args, result) # type: ignore
    

    @keyword('Verbindung zum Server herstellen') # type: ignore
    def connect_to_server(self, Servername: str): # type: ignore
        """
        Die Verbindung mit dem angegebenen SAP Server wird hergestellt.
        
        | ``Servername`` | Der Name des Servers in SAP Logon (nicht der SID) |
        
        
        Beispiele:
        
        | ``Verbindung zum Server herstellen    Servername``
        """
        
        args = [Servername]
        
        result = {
            "NoSapGui": "Keine laufende SAP GUI gefunden. Das Keyword \"SAP starten\" muss zuerst aufgerufen werden.",
            "NoGuiScripting": "Die Skriptunterstützung ist nicht verfügbar. Sie muss in den Einstellungen vom SAP Client aktiviert werden.",
            "Pass": "Die Verbindung mit dem Server '{0}' wurde erfolgreich hergestellt.",
            "SapError": "SAP Fehlermeldung: {0}",
            "NoServerScripting": "Das Scripting ist auf dem SAP Server nicht freigeschaltet. Siehe die Dokumentation von RoboSAPiens.",
            "Exception": "Die Verbindung konnte nicht hergestellt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ConnectToServer', args, result) # type: ignore
    

    @keyword('Tabellenzelle doppelklicken') # type: ignore
    def double_click_cell(self, Zeile: str, Spaltentitel: str): # type: ignore
        """
        Die Zelle am Schnittpunkt der Zeile und der Spalte wird doppelgeklickt.
        
        | ``Zeile`` | Entweder die Zeilennummer oder der Inhalt einer Zelle in der Zeile. Wenn die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden. |
        | ``Spaltentitel`` | Spaltentitel oder Kurzinfo |
        
        
        Beispiele:
        
        | ``Tabellenzelle doppelklicken     Zeile     Spaltentitel``
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
        
        | ``Lokator`` | Die Lokatoren für Textfelder sind im Schlüsselwort [#Textfeld ausfüllen|Textfeld ausfüllen] dokumentiert. |
        
        
        Beispiele:
        
        | ``Textfeld doppelklicken     Lokator``
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
        
        | ``T_Code`` | Der Code der Transaktion |
        
        
        Beispiele:
        
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
        
        | ``Name`` | Der Name der generierten Dateien |
        | ``Verzeichnis`` | Der absolute Pfad des Verzeichnisses, wo die Dateien abgelegt werden. |
        
        
        Beispiele:
        
        | ``Maske exportieren     Name     Verzeichnis``
        
        *Hinweis*: Rückwärtsschrägstriche müssen doppelt geschrieben werden. Ansonsten verwende die Standard RF Variable ${/} als Trennzeichen.
        
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
        Die Zelle am Schnittpunkt der Zeile und der Spalte wird mit dem angegebenen Inhalt ausgefüllt.
        
        | ``Zeile`` | Entweder die Zeilennummer oder der Inhalt einer Zelle in der Zeile. Wenn die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden. |
        | ``Spaltentitel`` | Spaltentitel oder Kurzinfo |
        | ``Inhalt`` | Der neue Inhalt der Zelle |
        
        
        Beispiele:
        
        | ``Tabellenzelle ausfüllen     Zeile     Spaltentitel     Inhalt``
        
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
    

    @keyword('Mehrzeiliges Textfeld ausfüllen') # type: ignore
    def fill_text_edit(self, Inhalt: str): # type: ignore
        """
        Das mehrzeilige Textfeld in der Maske wird mit dem angegebenen Inhalt ausgefüllt.
        
        | ``Inhalt`` | Der neue Inhalt des mehrzeiligen Textfelds |
        
        
        Beispiele:
        
        | ``Mehrzeiliges Textfeld ausfüllen    Ein langer Text. Mit zwei Sätzen.``
        """
        
        args = [Inhalt]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Die Maske enthält kein mehrzeiliges Textfeld.\nHinweis: Prüfe die Rechtschreibung",
            "NotChangeable": "Das mehrzeilige Textfeld ist nicht bearbeitbar.",
            "Pass": "Das mehrzeilige Textfeld wurde ausgefüllt.",
            "Exception": "Das mehrzeilige Textfeld konnte nicht ausgefüllt werden. {0}\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('FillTextEdit', args, result) # type: ignore
    

    @keyword('Textfeld ausfüllen') # type: ignore
    def fill_text_field(self, Lokator: str, Inhalt: str): # type: ignore
        """
        Das angegebene Textfeld wird mit dem angegebenen Inhalt ausgefüllt.
        
        | ``Lokator`` | Ein Lokator, um das Textfeld zu finden |
        | ``Inhalt`` | Der neue Inhalt des Textfelds |
        
        
        Beispiele:
        
        *Textfeld mit einer Beschriftung links*
        | ``Textfeld ausfüllen    Beschriftung    Inhalt``
        
        *Hinweis*: Wenn ein Textfeld markiert ist, wird durch Drücken der Taste F1 ein Hilfetext angezeigt, der normalerweise als Beschriftung verwendet werden kann.
        
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
        
        | ``Lokator`` | Name oder Kurzinfo des Knopfes |
        
        
        Beispiele:
        
        | ``Knopf hervorheben    Lokator``
        
        *Hinweis*: Einige Tooltips bestehen aus einem Namen, gefolgt von mehreren Leerzeichen und einem Tastaturkürzel.
        Der Name kann als Lokator verwendet werden, solange er eindeutig ist.
        Wenn der gesamte Text des Tooltips als Lokator verwendet wird, muss lediglich ein Leerzeichen eingetippt werden (z.B. ``Zurück (F3)``).
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
        
        | ``Tastenkombination`` | Entweder eine Taste oder mehrere Tasten mit '+' als Trennzeichen |
        
        
        Beispiele:
        
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
        
        | ``Lokator`` | Ein Lokator, um den Knopf zu finden |
        
        
        Beispiele:
        
        *Knopf mit Namen oder Kurzinfo*
        
        | ``Knopf drücken    Name oder Kurzinfo``
        
        *Knopf mit einem nicht eindeutigen Namen oder Kurzinfo links oder rechts von einer eindeutigen Beschriftung*
        
        | ``Knopf drücken    eindeutige Beschriftung >> Name oder Kurzinfo``
        
        *Hinweis*: Einige Tooltips bestehen aus einem Namen, gefolgt von mehreren Leerzeichen und einem Tastaturkürzel.
        Der Name kann als Lokator verwendet werden, solange er eindeutig ist.
        Wenn der gesamte Text des Tooltips als Lokator verwendet wird, muss lediglich ein Leerzeichen eingetippt werden (z.B. ``Zurück (F3)``).
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
        Die Zelle am Schnittpunkt der Zeile und der Spalte wird gedrückt.
        
        | ``Zeile`` | Entweder die Zeilennummer oder der Inhalt einer Zelle in der Zeile. Wenn die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden. |
        | ``Spaltentitel`` | Spaltentitel oder Kurzinfo |
        
        
        Beispiele:
        
        | ``Tabellenzelle drücken     Zeile     Spaltentitel``
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
        
        | ``Lokator`` | Die Lokatoren für Textfelder sind im Schlüsselwort [#Textfeld ausfüllen|Textfeld ausfüllen] dokumentiert. |
        
        
        Beispiele:
        
        | ${Inhalt}   ``Textfeld auslesen    Lokator``
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
        
        | ``Lokator`` | Ein Lokator, um den Text zu finden |
        
        
        Beispiele:
        
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
        Der Inhalt der Zelle am Schnittpunkt der Zeile und der Spalte wird zurückgegeben.
        
        | ``Zeile`` | Entweder die Zeilennummer oder der Inhalt einer Zelle in der Zeile. Wenn die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden. |
        | ``Spaltentitel`` | Spaltentitel oder Kurzinfo |
        
        
        Beispiele:
        
        | ``Tabellenzelle auslesen     Zeile     Spaltentitel``
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
        
        | ``Speicherort`` | Entweder der absolute Pfad einer .png Datei oder LOG, um das Bild in das Protokoll einzubetten. |
        
        
        Beispiele:
        
        | ``Fenster aufnehmen     Speicherort``
        
        *Hinweis*: Rückwärtsschrägstriche müssen doppelt geschrieben werden. Ansonsten verwende die Standard RF Variable ${/} als Trennzeichen.
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
        
        | ``Richtung`` | UP, DOWN, BEGIN, END |
        | ``bis_Textfeld`` | Die Lokatoren für Textfelder sind im Schlüsselwort [#Textfeld ausfüllen|Textfeld ausfüllen] dokumentiert. |
        
        
        Beispiele:
        
        | ``Inhalte scrollen    Richtung``
        
        Wenn der Parameter "bis_Textfeld" übergeben wird, werden die Inhalte so lange gescrollt, bis das Textfeld gefunden wird.
        
        | ``Inhalte scrollen    Richtung   bis_Textfeld``
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
    

    @keyword('Fenster horizontal scrollen') # type: ignore
    def scroll_window_horizontally(self, Richtung: str): # type: ignore
        """
        Die horizontale Bildlaufleiste des Fensters wird in die angegebene Richtung bewegt.
        
        | ``Richtung`` | LEFT, RIGHT, BEGIN, END |
        
        
        Beispiele:
        
        | ``Fenster horizontal scrollen    Richtung``
        """
        
        args = [Richtung]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "Exception": "Das Fenster konnte nicht horizontal gescrollt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen.",
            "NoScrollbar": "Das Fenster enthält keine horizontale Bildlaufleiste.",
            "MaximumReached": "Das Fenster kann nicht weiter gescrollt werden.",
            "InvalidDirection": "Die angegebene Richtung ist ungültig. Gültige Richtungen sind: LEFT, RIGHT, BEGIN, END",
            "Pass": "Das Fenster wurde in die Richtung '{0}' horizontal gescrollt."
        }
        return super()._run_keyword('ScrollWindowHorizontally', args, result) # type: ignore
    

    @keyword('Tabellenzelle markieren') # type: ignore
    def select_cell(self, Zeile: str, Spaltentitel: str): # type: ignore
        """
        Die Zelle am Schnittpunkt der Zeile und der Spalte wird markiert.
        
        | ``Zeile`` | Entweder die Zeilennummer oder der Inhalt einer Zelle in der Zeile. Wenn die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden. |
        | ``Spaltentitel`` | Spaltentitel oder Kurzinfo |
        
        
        Beispiele:
        
        | ``Tabellenzelle markieren     Zeile     Spaltentitel``
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
        In der Zelle am Schnittpunkt der Zeile und der Spalte wird der angegebene Wert ausgewählt.
        
        | ``Zeile`` | Entweder die Zeilennummer oder der Inhalt einer Zelle in der Zeile. Wenn die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden. |
        | ``Spaltentitel`` | Spaltentitel oder Kurzinfo |
        | ``Wert`` | Ein Wert aus dem Auswahlmenü |
        
        
        Beispiele:
        
        | ``Tabellenzellenwert auswählen    Zeile    Spaltentitel    Wert``
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
    

    @keyword('Auswahlmenüeintrag auslesen') # type: ignore
    def read_combo_box_entry(self, Lokator: str): # type: ignore
        """
        Aus dem angegebenen Auswahlmenü wird der aktuelle Eintrag ausgelesen.
        
        | ``Lokator`` | Beschriftung oder Kurzinfo des Auswahlmenüs |
        
        
        Beispiele:
        
        | ``${Eintrag}   Auswahlmenüeintrag auslesen    Lokator``
        """
        
        args = [Lokator]
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Das Auswahlmenü mit dem Lokator '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Der aktuelle Eintrag wurde ausgelesen.",
            "Exception": "Der Eintrag konnte nicht ausgelesen werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ReadComboBoxEntry', args, result) # type: ignore
    

    @keyword('Auswahlmenüeintrag auswählen') # type: ignore
    def select_combo_box_entry(self, Lokator: str, Eintrag: str): # type: ignore
        """
        Aus dem angegebenen Auswahlmenü wird der angegebene Eintrag ausgewählt.
        
        | ``Lokator`` | Beschriftung oder Kurzinfo des Auswahlmenüs |
        | ``Eintrag`` | Ein Eintrag aus dem Auswahlmenü |
        
        
        Beispiele:
        
        | ``Auswahlmenüeintrag auswählen    Lokator    Eintrag``
        
        *Hinweise*: Der numerische Schlüssel, dass eine vereinfachte Tastaureingabe ermöglicht, ist nicht Teil des Eintragsnamens.
        
        Um einen Eintrag aus einem Symbolleisten-Knopf mit Auswahlmenü auszuwählen, drücke zuerst den Knopf und verwende danach dieses Schlüsselwort.
        """
        
        args = [Lokator, Eintrag]
        
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
        
        | ``Eintragspfad`` | Der Pfad zum Eintrag mit '/' als Trennzeichen (z.B. System/Benutzervorgaben/Eigene Daten). |
        
        
        Beispiele:
        
        | ``Menüeintrag auswählen    Eintragspfad``
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
        
        | ``Lokator`` | Ein Lokator, um das Optionsfeld zu finden |
        
        
        Beispiele:
        
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
        
        | ``Zeilenlokator`` | Entweder die Zeilennummer oder der Inhalt einer Zelle in der Zeile. Wenn die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden. |
        
        
        Beispiele:
        
        | ``Tabellenzeile markieren    Zeilenlokator``
        
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
        
        | ``Lokator`` | Die Lokatoren für Textfelder sind im Schlüsselwort [#Textfeld ausfüllen|Textfeld ausfüllen] dokumentiert. |
        
        
        Beispiele:
        
        | ``Textfeld markieren    Lokator``
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
        
        | ``Lokator`` | Ein Lokator, um den Text zu finden |
        
        
        Beispiele:
        
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
        
        | ``Lokator`` | Ein Lokator, um das Formularfeld zu finden |
        
        
        Beispiele:
        
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
        Der Inhalt der Statusleiste wird ausgelesen. Der Rückgabewert ist ein Dictionary mit den Einträgen 'status' und 'message'.
        
        
        Beispiele:
        
        | ``${statusleiste}   Statusleiste auslesen``
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
        
        | ``Lokator`` | Ein Lokator, um das Formularfeld zu finden |
        
        
        Beispiele:
        
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
        Die Zelle am Schnittpunkt der Zeile und der Spalte wird angekreuzt.
        
        | ``Zeile`` | Entweder die Zeilennummer oder der Inhalt einer Zelle in der Zeile. Wenn die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden. |
        | ``Spaltentitel`` | Spaltentitel oder Kurzinfo |
        
        
        Beispiele:
        
        | ``Tabellenzelle ankreuzen     Zeile     Spaltentitel``
        
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
        Die Zelle am Schnittpunkt der Zeile und der Spalte wird abgewählt.
        
        | ``Zeile`` | Entweder die Zeilennummer oder der Inhalt einer Zelle in der Zeile. Wenn die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden. |
        | ``Spaltentitel`` | Spaltentitel oder Kurzinfo |
        
        
        Beispiele:
        
        | ``Tabellenzelle abwählen     Zeile     Spaltentitel``
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
        
        
        Beispiele:
        
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
        
        
        Beispiele:
        
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
    ROBOT_LIBRARY_VERSION = '2.9.1'