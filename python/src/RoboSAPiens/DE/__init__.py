from robot.api.deco import keyword
from RoboSAPiens.client import RoboSAPiensClient

__version__ = "2.23.0"

class DE(RoboSAPiensClient):
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
    
    == Aufeinanderfolgende Spalten mit demselben Namen ==
    Wenn eine Tabelle aufeinanderfolgende Spalten mit demselben Namen enthält, kann eine Spalte eindeutig identifiziert werden, indem dem Namen ein numerisches Suffix hinzugefügt wird.
    
    Enthält eine Tabelle beispielsweise die Spalten Variante, Variante, Variante, so können diese wie folgt eindeutig identifiziert werden: Variante__1, Variante__2, Variante__3.
    
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

    def __init__(self, vortragsmodus: bool=False, x64: bool=False):
        """
        RoboSAPiens.DE hat die folgenden Initialisierungsparameter:
        
        | =Parameter= | =Beschreibung= |
        | ``vortragsmodus`` | Nach dem Aufruf eines Schlüsselworts eine halbe Sekunde warten und das betroffene GUI Element hervorheben (falls zutreffend). |
        | ``x64`` | RoboSAPiens 64-bit ausführen, um SAP GUI 8 64-bit bzw. SAP Business Client zu automatisieren. |
        """
        
        args = {
            'presenter_mode': vortragsmodus,
            'x64': x64
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

        args: list = [
            Reitername
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Der Reiter '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Der Reiter '{0}' wurde ausgewählt.",
            "Exception": "Der Reiter konnte nicht ausgewählt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ActivateTab', args, kwargs, result) # type: ignore
    
    @keyword('Baumelement doppelklicken') # type: ignore
    def double_click_tree_element(self, Elementpfad: str): # type: ignore
        """
        Das Baumelement mit dem angegebenen Pfad wird doppelgeklickt.
        
        | ``Elementpfad`` | Der Pfad zum Element, mit '/' als Trennzeichen (z.B. Engineering/Bauwesen). |
        
        Beispiele:
        
        | ``Baumelement doppelklicken    Elementpfad``
        
        Für weitere Infos zum Elementpfad siehe [#Baumelement markieren|Baumelement markieren].
        """

        args: list = [
            Elementpfad
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Das Baumelement '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Das Baumelement '{0}' wurde doppelgeklickt.",
            "Exception": "Das Baumelement konnte nicht doppelgeklickt werden. {0}\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('DoubleClickTreeElement', args, kwargs, result) # type: ignore
    
    @keyword('Baumordner aufklappen') # type: ignore
    def expand_tree_folder(self, Ordnerpfad: str): # type: ignore
        """
        Der Ordner mit dem angegebenen Pfad in einer Baumstruktur wird aufgeklappt.
        
        | ``Ordnerpfad`` | Der Pfad zum Ordner, mit '/' als Trennzeichen (z.B. Engineering/Bauwesen). |
        
        Beispiele:
        
        | ``Baumordner aufklappen    Ordnerpfad``
        
        Für weitere Infos zum Ordnerpfad siehe [#Baumelement markieren|Baumelement markieren].
        """

        args: list = [
            Ordnerpfad
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Der Baumordner '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Der Baumordner '{0}' wurde aufgeklappt.",
            "Exception": "Der Baumordner konnte nicht aufgeklappt werden. {0}\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ExpandTreeFolder', args, kwargs, result) # type: ignore
    
    @keyword('Baumelement markieren') # type: ignore
    def select_tree_element(self, Elementpfad: str): # type: ignore
        """
        Das Baumelement mit dem angegebenen Pfad wird markiert.
        
        | ``Elementpfad`` | Der Pfad zum Element, mit '/' als Trennzeichen (z.B. Engineering/Bauwesen). |
        
        Beispiele:
        
        | ``Baumelement markieren    Elementpfad``
        
        *Hinweise*
        - Ein Schrägstrich, der nicht als Trennzeichen verwendet wird, muss doppelt geschrieben werden.
        - Jedes Segment des Pfades kann teilweise angegeben werden. Zum Beispiel IDoc anstelle von IDoc 1234.
        """

        args: list = [
            Elementpfad
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Das Baumelement '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Das Baumelement '{0}' wurde markiert.",
            "Exception": "Das Baumelement konnte nicht markiert werden. {0}\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('SelectTreeElement', args, kwargs, result) # type: ignore
    
    @keyword('Menüeintrag in Baumelement auswählen') # type: ignore
    def select_tree_element_menu_entry(self, Elementpfad: str, Menüeintrag: str): # type: ignore
        """
        Aus dem Kontextmenü des Baumelements mit dem angegebenen Pfad wird der angebene Eintrag ausgewählt.
        
        | ``Elementpfad`` | Der Pfad zum Element, mit '/' als Trennzeichen (z.B. Engineering/Bauwesen). |
        | ``Menüeintrag`` | Der Menüeintrag. Bei verschachtelten Menüs der Pfad zum Eintrag mit '|' als Trennzeichen (z.B. Anlegen|Wirtschaftseinheit). |
        
        Beispiele:
        
        | ``Menüeintrag in Baumelement auswählen    Elementpfad    Menüeintrag``
        
        Für weitere Infos zum Elementpfad siehe [#Baumelement markieren|Baumelement markieren].
        """

        args: list = [
            Elementpfad,
            Menüeintrag
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Das Baumelement '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Der Menüeintrag '{0}' wurde ausgewählt.",
            "Exception": "Der Menüeintrag konnte nicht ausgewählt werden. {0}\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('SelectTreeElementMenuEntry', args, kwargs, result) # type: ignore
    
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
        
        | ``SAP starten   C:\\Program Files\\SAP\\FrontEnd\\SAPgui\\sapshcut.exe -sysname=XXX -client=NNN -user=%{username} -pw=%{password}``
        
        *Hinweise*: 
        
        - Rückwärtsschrägstriche müssen doppelt geschrieben werden. Ansonsten verwende die Standard RF Variable ${/} als Trennzeichen.
        - 64-bit SAP-Clients erfordern, dass die Bibliothek mit ``x64=True`` importiert wird
        - sysname ist der Name der Verbindung in SAP Logon. Wenn es Leerzeichen enthält, muss es in Anführungszeichen gesetzt werden.
        """

        args: list = [
            Pfad
        ]
        kwargs: dict = {
            "SAP_Parameter": SAP_Parameter
        }
        
        result = {
            "Pass": "SAP wurde gestartet",
            "NoGuiScripting": "Die Skriptunterstützung ist nicht verfügbar. Sie muss in den Einstellungen vom SAP Client aktiviert werden.",
            "SAPAlreadyRunning": "SAP läuft gerade. Es muss vor dem Aufruf dieses Schlüsselworts beendet werden.",
            "SAPNotStarted": "SAP konnte nicht gestartet werden. Überprüfe den Pfad und ggf. die Parameter '{0}'.",
            "Exception": "SAP konnte nicht gestartet werden. Hinweis: Um einen 64-bit SAP Client zu starten, muss RoboSAPiens.DE mit x64=True importiert werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('OpenSap', args, kwargs, result) # type: ignore
    
    @keyword('Verbindung zum Server trennen') # type: ignore
    def close_connection(self): # type: ignore
        """
        Die aktuelle Verbindung zum SAP Server wird getrennt.
        
        
        Beispiele:
        
        | ``Verbindung zum Server trennen``
        """

        args: list = [
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "Pass": "Die Verbindung zum Server wurde getrennt.",
            "Exception": "Die Verbindung zum Server konnte nicht getrennt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('CloseConnection', args, kwargs, result) # type: ignore
    
    @keyword('SAP beenden') # type: ignore
    def close_sap(self): # type: ignore
        """
        Schließt die SAP GUI und beendet ihren Prozess.
        
        
        Beispiele:
        
        | ``SAP beenden``
        
        *Hinweis*: Dieses Schlüsselwort funktioniert nur, wenn die SAP GUI mit dem Schlüsselwort [#SAP starten|SAP starten] gestartet wurde.
        """

        args: list = [
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSapGui": "Keine laufende SAP GUI gefunden. Das Keyword \"SAP starten\" muss zuerst aufgerufen werden.",
            "Pass": "Die SAP GUI wurde beendet"
        }
        return super()._run_keyword('CloseSap', args, kwargs, result) # type: ignore
    
    @keyword('Fenster schließen') # type: ignore
    def close_window(self): # type: ignore
        """
        Das Fenster im Vordergrund wird geschlossen.
        
        
        Beispiele:
        
        | ``Fenster schließen``
        """

        args: list = [
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "Pass": "Das Fenster im Vordergrund wurde geschlossen.",
            "Exception": "Das Fenster konnte nicht geschlossen werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('CloseWindow', args, kwargs, result) # type: ignore
    
    @keyword('Tabellenzeilen zählen') # type: ignore
    def count_table_rows(self, tabelle_nummer: int=1): # type: ignore
        """
        Die Zeilen einer Tabelle werden gezählt.
        
        | ``tabelle_nummer`` | Spezifiziert welche Tabelle: 1, 2, ... |
        
        Beispiele:
        
        | ``${anzahl_zeilen}    Tabellenzeilen zählen``
        """

        args: list = [
        ]
        kwargs: dict = {
            "tabelle_nummer": tabelle_nummer
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "Exception": "Die Zeilen der Tabelle konnten nicht gezählt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen.",
            "NotFound": "Die Maske enthält keine Tabelle.",
            "InvalidTable": "Die Maske enthält keine Tabelle mit dem Index {0}.",
            "Pass": "Die Tabellenzeillen wurden gezählt."
        }
        return super()._run_keyword('CountTableRows', args, kwargs, result) # type: ignore
    
    @keyword('Baumstruktur exportieren') # type: ignore
    def export_tree(self, Dateipfad: str): # type: ignore
        """
        Die Baumstruktur in der Maske wird in JSON Format in der angegebenen Datei gespeichert.
        
        | ``Dateipfad`` | Absoluter Pfad zu einer Datei mit Endung .json |
        
        Beispiele:
        
        | ``Baumstruktur exportieren     Dateipfad``
        
        *Hinweis*: Rückwärtsschrägstriche müssen doppelt geschrieben werden. Ansonsten verwende die Standard RF Variable ${/} als Trennzeichen.
        """

        args: list = [
            Dateipfad
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Die Maske enthält keine Baumstruktur",
            "Pass": "Die Baumstruktur wurde in JSON Format in der Datei '{0}' gespeichert",
            "Exception": "Die Baumstruktur konnte nicht exportiert werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ExportTree', args, kwargs, result) # type: ignore
    
    @keyword('Laufende SAP GUI übernehmen') # type: ignore
    def connect_to_running_sap(self, session_nummer: int=1, Verbindung: str=None, Mandant: str=None): # type: ignore
        """
        Nach der Ausführung dieses Schlüsselworts kann eine bereits laufende SAP GUI mit RoboSAPiens gesteuert werden.
        
        | ``session_nummer`` | Die Nummer der SAP-Session in der rechten oberen oder unteren Ecke des Fensters |
        | ``Verbindung`` | Der Name der Verbindung in SAP Logon (nicht der SID) |
        | ``Mandant`` | Der dreistellige Mandant |
        
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

        args: list = [
        ]
        kwargs: dict = {
            "session_nummer": session_nummer,
            "Verbindung": Verbindung,
            "Mandant": Mandant
        }
        
        result = {
            "NoSapGui": "Keine laufende SAP GUI gefunden.",
            "NoGuiScripting": "Die Skriptunterstützung ist nicht verfügbar. Sie muss in den Einstellungen vom SAP Client aktiviert werden.",
            "NoConnection": "Es besteht keine Verbindung zu einem SAP Server. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NoServerScripting": "Das Scripting ist auf dem SAP Server nicht freigeschaltet. Siehe die Dokumentation von RoboSAPiens.",
            "InvalidClient": "Es gibt keinen Mandanten '{Mandant}' bei der aktuellen Verbindung.",
            "InvalidConnection": "Es gibt keine Verbindung mit dem Namen '{Verbindung}'.",
            "InvalidConnectionClient": "Es gibt keinen Mandanten '{Mandant}' bei der Verbindung '{Verbindung}'.",
            "InvalidSession": "Die aktuelle Verbindung hat keine Session '{session_nummer}'.",
            "SapError": "SAP Fehlermeldung: {0}",
            "Json": "Der Rückgabewert ist im JSON-Format",
            "Pass": "Die laufende SAP GUI wurde erfolgreich übernommen.",
            "Exception": "Die laufende SAP GUI konnte nicht übernommen werden. Hinweis: Für die Verbindung mit einem 64-bit SAP Client muss RoboSAPiens.DE mit x64=True importiert werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ConnectToRunningSap', args, kwargs, result) # type: ignore
    
    @keyword('Verbindung zum Server herstellen') # type: ignore
    def connect_to_server(self, Servername: str): # type: ignore
        """
        Die angegebene Verbindung mit einem SAP Server wird hergestellt.
        
        | ``Servername`` | Der Name der Verbindung in SAP Logon (nicht der SID) |
        
        Beispiele:
        
        | ``Verbindung zum Server herstellen    Servername``
        
        Der Rückgabewert enthält Informationen über die Session wie z.B. Mandant und System-ID.
        """

        args: list = [
            Servername
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSapGui": "Keine laufende SAP GUI gefunden. Das Keyword \"SAP starten\" muss zuerst aufgerufen werden.",
            "NoGuiScripting": "Die Skriptunterstützung ist nicht verfügbar. Sie muss in den Einstellungen vom SAP Client aktiviert werden.",
            "Pass": "Die Verbindung '{0}' wurde erfolgreich hergestellt.",
            "Json": "Der Rückgabewert ist im JSON-Format",
            "SapError": "SAP Fehlermeldung: {0}",
            "NoServerScripting": "Das Scripting ist auf dem SAP Server nicht freigeschaltet. Siehe die Dokumentation von RoboSAPiens.",
            "InvalidSession": "Die aktuelle Verbindung hat keine Session '{0}'.",
            "Exception": "Die Verbindung konnte nicht hergestellt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ConnectToServer', args, kwargs, result) # type: ignore
    
    @keyword('Tabellenzelle doppelklicken') # type: ignore
    def double_click_cell(self, Zeile: str, Spaltentitel: str, tabelle_nummer: int=None): # type: ignore
        """
        Führt einen Doppelklick auf die Zelle aus, die an der Schnittstelle der gegebenen Zeile und Spalte liegt.
        
        | ``Zeile`` | Entweder die Zeilennummer oder der Inhalt einer Zelle in der Zeile. Wenn die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden. |
        | ``Spaltentitel`` | Spaltentitel oder Kurzinfo |
        | ``tabelle_nummer`` | Spezifiziert welche Tabelle: 1, 2, ... |
        
        Beispiele:
        
        | ``Tabellenzelle doppelklicken     Zeile     Spaltentitel``
        """

        args: list = [
            Zeile,
            Spaltentitel
        ]
        kwargs: dict = {
            "tabelle_nummer": tabelle_nummer
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Die Zelle mit dem Lokator '{0}, {1}' wurde nicht gefunden. Hinweise: Prüfe die Rechtschreibung, maximiere das SAP Fenster",
            "InvalidTable": "Die Maske enthält keine Tabelle mit dem Index {0}.",
            "Pass": "Die Zelle mit dem Lokator '{0}, {1}' wurde doppelgeklickt.",
            "Exception": "Die Zelle konnte nicht doppelgeklickt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('DoubleClickCell', args, kwargs, result) # type: ignore
    
    @keyword('Textfeld doppelklicken') # type: ignore
    def double_click_text_field(self, Lokator: str): # type: ignore
        """
        Führt einen Doppelklick auf das angegebene Textfeld aus.
        
        | ``Lokator`` | Die Lokatoren für Textfelder sind im Schlüsselwort [#Textfeld ausfüllen|Textfeld ausfüllen] dokumentiert. |
        
        Beispiele:
        
        *Identifizierung des Textfeldes über einen Lokator*
        | ``Textfeld doppelklicken     Lokator``
        
        *Identifizierung des Textfeldes über seinen Inhalt*
        | ``Textfeld doppelklicken     = Inhalt``
        """

        args: list = [
            Lokator
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Das Textfeld mit dem Lokator '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Das Textfeld mit dem Lokator '{0}' wurde doppelgeklickt.",
            "Exception": "Das Textfeld konnte nicht doppelgeklickt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('DoubleClickTextField', args, kwargs, result) # type: ignore
    
    @keyword('Transaktion ausführen') # type: ignore
    def execute_transaction(self, T_Code: str): # type: ignore
        """
        Die Transaktion mit dem angegebenen T-Code wird ausgeführt.
        
        | ``T_Code`` | Der Code der Transaktion |
        
        Beispiele:
        
        | ``Transaktion ausführen    T-Code``
        """

        args: list = [
            T_Code
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "Pass": "Die Transaktion mit T-Code '{0}' wurde erfolgreich ausgeführt.",
            "Exception": "Die Transaktion konnte nicht ausgeführt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ExecuteTransaction', args, kwargs, result) # type: ignore
    
    @keyword('Maske exportieren') # type: ignore
    def export_window(self, Name: str, Verzeichnis: str): # type: ignore
        """
        Die Inhalte der Maske werden in einer JSON-Datei geschrieben. Außerdem wird ein Bildschirmfoto automatisch in PNG-Format erstellt.
        
        | ``Name`` | Der Name der generierten Dateien |
        | ``Verzeichnis`` | Der absolute Pfad des Verzeichnisses, wo die Dateien gespeichert werden. |
        
        Beispiele:
        
        | ``Maske exportieren     Name     Verzeichnis``
        
        *Hinweis*: Rückwärtsschrägstriche müssen doppelt geschrieben werden. Ansonsten verwende die Standard RF Variable ${/} als Trennzeichen.
        
        *Anmerkung*: Aktuell werden nicht alle GUI-Elemente exportiert.
        """

        args: list = [
            Name,
            Verzeichnis
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "Pass": "Die Maske wurde in den Dateien '{0}' und '{1}' gespeichert",
            "Exception": "Die Maske konnte nicht exportiert werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ExportWindow', args, kwargs, result) # type: ignore
    
    @keyword('Tabellenzelle ausfüllen') # type: ignore
    def fill_cell(self, Zeile: str, Spaltentitel: str, Inhalt: str, tabelle_nummer: int=None): # type: ignore
        """
        Die Zelle am Schnittpunkt der Zeile und der Spalte wird mit dem angegebenen Inhalt ausgefüllt.
        
        | ``Zeile`` | Entweder die Zeilennummer oder der Inhalt einer Zelle in der Zeile. Wenn die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden. |
        | ``Spaltentitel`` | Spaltentitel oder Kurzinfo |
        | ``Inhalt`` | Der neue Inhalt der Zelle |
        | ``tabelle_nummer`` | Spezifiziert welche Tabelle: 1, 2, ... |
        
        Beispiele:
        
        | ``Tabellenzelle ausfüllen     Zeile     Spaltentitel     Inhalt``
        
        *Hinweis*: Für die Migration aus dem alten Schlüsselwort mit zwei Argumenten soll eine Suche und Ersetzung mit einem regulären Ausdruck durchgeführt werden.
        """

        args: list = [
            Zeile,
            Spaltentitel,
            Inhalt
        ]
        kwargs: dict = {
            "tabelle_nummer": tabelle_nummer
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Die Zelle mit dem Lokator '{0}, {1}' wurde nicht gefunden. Hinweise: Prüfe die Rechtschreibung, maximiere das SAP Fenster",
            "NotChangeable": "Die Zelle mit dem Lokator '{0}, {1}' ist nicht bearbeitbar.",
            "NoTable": "Die Maske enthält keine Tabelle.",
            "InvalidTable": "Die Maske enthält keine Tabelle mit dem Index {0}.",
            "Pass": "Die Zelle mit dem Lokator '{0}, {1}' wurde ausgefüllt.",
            "Exception": "Die Zelle konnte nicht ausgefüllt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('FillCell', args, kwargs, result) # type: ignore
    
    @keyword('Mehrzeiliges Textfeld ausfüllen') # type: ignore
    def fill_text_edit(self, Inhalt: str): # type: ignore
        """
        Das mehrzeilige Textfeld in der Maske wird mit dem angegebenen Inhalt ausgefüllt.
        
        | ``Inhalt`` | Der neue Inhalt des mehrzeiligen Textfelds |
        
        Beispiele:
        
        | ``Mehrzeiliges Textfeld ausfüllen    Ein langer Text. Mit zwei Sätzen.``
        """

        args: list = [
            Inhalt
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Die Maske enthält kein mehrzeiliges Textfeld.\nHinweis: Prüfe die Rechtschreibung",
            "NotChangeable": "Das mehrzeilige Textfeld ist nicht bearbeitbar.",
            "Pass": "Das mehrzeilige Textfeld wurde ausgefüllt.",
            "Exception": "Das mehrzeilige Textfeld konnte nicht ausgefüllt werden. {0}\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('FillTextEdit', args, kwargs, result) # type: ignore
    
    @keyword('Textfeld ausfüllen') # type: ignore
    def fill_text_field(self, Lokator: str, Inhalt: str, exakt: bool=True): # type: ignore
        """
        Das angegebene Textfeld wird mit dem angegebenen Inhalt ausgefüllt.
        
        | ``Lokator`` | Ein Lokator, um das Textfeld zu finden |
        | ``Inhalt`` | Der neue Inhalt des Textfelds |
        | ``exakt`` | Entweder eine genaue oder eine partielle Übereinstimmung mit dem Lokator. |
        
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

        args: list = [
            Lokator,
            Inhalt
        ]
        kwargs: dict = {
            "exakt": exakt
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Das Textfeld mit dem Lokator '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "NotChangeable": "Das Textfeld mit dem Lokator '{0}' ist nicht bearbeitbar.",
            "Pass": "Das Textfeld mit dem Lokator '{0}' wurde ausgefüllt.",
            "Exception": "Das Textfeld konnte nicht ausgefüllt werden. Möglicherweise, weil der Inhalt nicht dazu passt.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('FillTextField', args, kwargs, result) # type: ignore
    
    @keyword('Knopf hervorheben') # type: ignore
    def highlight_button(self, Lokator: str, exakt: bool=False): # type: ignore
        """
        Der Knopf mit dem angegebenen Lokator wird hervorgehoben.
        
        | ``Lokator`` | Name oder Kurzinfo (Tooltip) des Knopfes |
        | ``exakt`` | `True` wenn der Lokator und die Kurzinfo genau übereinstimmen, sonst `False` |
        
        Beispiele:
        
        | ``Knopf hervorheben    Lokator``
        
        *Hinweis*: Tooltips mit einem Tastenkürzel am Ende kommen oft vor. 
        Der Standardwert ``exakt=False`` sorgt dafür, dass das Tastenkürzel bei der Suche vernachlässigt wird.
        Für Tooltips ohne Tastenkürzel ist eher eine genaue Übereinstimmung (``exakt=True``) wünschenswert.
        
        """

        args: list = [
            Lokator
        ]
        kwargs: dict = {
            "exakt": exakt
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Der Knopf '{0}' wurde nicht gefunden. Hinweise: Prüfe die Rechtschreibung, maximiere das SAP Fenster",
            "Pass": "Der Knopf '{0}' wurde hervorgehoben.",
            "Exception": "Der Knopf konnte nicht hervorgehoben werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('HighlightButton', args, kwargs, result) # type: ignore
    
    @keyword('Tastenkombination drücken') # type: ignore
    def press_key_combination(self, Tastenkombination: str, tabelle_nummer: int=None): # type: ignore
        """
        Die angegebene Tastenkombination (mit englischen Tastenbezeichnungen) wird gedrückt.
        
        | ``Tastenkombination`` | Entweder eine Taste oder mehrere Tasten mit '+' als Trennzeichen |
        | ``tabelle_nummer`` | Welche Tabelle (1, 2, ...) den Tastendruck empfangen soll. |
        
        Beispiele:
        
        | ``Tastenkombination drücken    Tastenkombination``
        
        Gültige Tastenkombinationen sind unter anderem die Tastenkürzel im Kontextmenü (angezeigt, wenn die rechte Maustaste gedrückt wird). 
        Die vollständige Liste der zulässigen Tastenkombinationen ist in der [https://help.sap.com/docs/sap_gui_for_windows/b47d018c3b9b45e897faf66a6c0885a8/71d8c95e9c7947ffa197523a232d8143.html?locale=de-DE|Dokumentation von SAP GUI].
        
        *Hinweis*: Das Drücken der Taste F2 hat die gleiche Wirkung wie ein Doppelklick.
        """

        args: list = [
            Tastenkombination
        ]
        kwargs: dict = {
            "tabelle_nummer": tabelle_nummer
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "Exception": "Die Tastenkombination konnte nicht gedrückt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen.",
            "NotFound": "Die Tastenkombination '{0}' ist nicht vorhanden. Siehe die Dokumentation des Schlüsselworts für die Liste der zulässigen Tastenkombinationen.",
            "InvalidTable": "Die Maske enthält keine Tabelle mit dem Index {0}.",
            "Pass": "Die Tastenkombination '{0}' wurde gedrückt."
        }
        return super()._run_keyword('PressKeyCombination', args, kwargs, result) # type: ignore
    
    @keyword('Knopf drücken') # type: ignore
    def push_button(self, Lokator: str, exakt: bool=False, tabelle_nummer: int=None): # type: ignore
        """
        Der Knopf mit dem angegebenen Lokator wird gedrückt.
        
        | ``Lokator`` | Name oder Kurzinfo (Tooltip) des Knopfes |
        | ``exakt`` | `True` wenn der Lokator und die Kurzinfo genau übereinstimmen, sonst `False`. |
        | ``tabelle_nummer`` | Die Tabelle (1, 2, ...), in deren Symbolleiste sich der Knopf befindet. |
        
        Beispiele:
        
        *Knopf mit Namen oder Kurzinfo*
        
        | ``Knopf drücken    Name oder Kurzinfo``
        
        *Knopf mit einem nicht eindeutigen Namen oder Kurzinfo links oder rechts von einer eindeutigen Beschriftung*
        
        | ``Knopf drücken    eindeutige Beschriftung >> Name oder Kurzinfo``
        
        *Hinweis*: Tooltips mit einem Tastenkürzel am Ende kommen oft vor. 
        Der Standardwert ``exakt=False`` sorgt dafür, dass das Tastenkürzel bei der Suche vernachlässigt wird.
        Für Tooltips ohne Tastenkürzel ist eher eine genaue Übereinstimmung (``exakt=True``) wünschenswert.
        
        """

        args: list = [
            Lokator
        ]
        kwargs: dict = {
            "exakt": exakt,
            "tabelle_nummer": tabelle_nummer
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Der Knopf '{0}' wurde nicht gefunden. Hinweise: Prüfe die Rechtschreibung, maximiere das SAP Fenster",
            "NotChangeable": "Der Knopf '{0}' ist deaktiviert.",
            "Pass": "Der Knopf '{0}' wurde gedrückt.",
            "Exception": "Der Knopf konnte nicht gedrückt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('PushButton', args, kwargs, result) # type: ignore
    
    @keyword('Tabellenzelle drücken') # type: ignore
    def push_button_cell(self, Zeile: str, Spaltentitel: str, tabelle_nummer: int=None): # type: ignore
        """
        Die Zelle am Schnittpunkt der Zeile und der Spalte wird gedrückt.
        
        | ``Zeile`` | Entweder die Zeilennummer oder der Inhalt einer Zelle in der Zeile. Wenn die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden. |
        | ``Spaltentitel`` | Spaltentitel oder Kurzinfo |
        | ``tabelle_nummer`` | Spezifiziert welche Tabelle: 1, 2, ... |
        
        Beispiele:
        
        | ``Tabellenzelle drücken     Zeile     Spaltentitel``
        """

        args: list = [
            Zeile,
            Spaltentitel
        ]
        kwargs: dict = {
            "tabelle_nummer": tabelle_nummer
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Die Zelle mit dem Lokator '{0}, {1}' wurde nicht gefunden. Hinweise: Prüfe die Rechtschreibung, maximiere das SAP Fenster",
            "NotAButton": "Die Zelle mit dem Lokator '{0}, {1}' ist kein Knopf.",
            "NotChangeable": "Die Zelle mit dem Lokator '{0}, {1}' ist deaktiviert.",
            "InvalidTable": "Die Maske enthält keine Tabelle mit dem Index {0}.",
            "Pass": "Die Zelle mit dem Lokator '{0}, {1}' wurde gedrückt.",
            "Exception": "Die Zelle konnte nicht gedrückt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('PushButtonCell', args, kwargs, result) # type: ignore
    
    @keyword('Textfeld auslesen') # type: ignore
    def read_text_field(self, Lokator: str): # type: ignore
        """
        Der Inhalt des angegebenen Textfeldes wird zurückgegeben.
        
        | ``Lokator`` | Die Lokatoren für Textfelder sind im Schlüsselwort [#Textfeld ausfüllen|Textfeld ausfüllen] dokumentiert. |
        
        Beispiele:
        
        | ${Inhalt}   ``Textfeld auslesen    Lokator``
        """

        args: list = [
            Lokator
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Das Textfeld mit dem Lokator '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Das Textfeld mit dem Lokator '{0}' wurde ausgelesen.",
            "Exception": "Das Textfeld konnte nicht ausgelesen werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ReadTextField', args, kwargs, result) # type: ignore
    
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

        args: list = [
            Lokator
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Der Text mit dem Lokator '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Der Text mit dem Lokator '{0}' wurde ausgelesen.",
            "Exception": "Der Text konnte nicht ausgelesen werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ReadText', args, kwargs, result) # type: ignore
    
    @keyword('Tabellenzelle auslesen') # type: ignore
    def read_cell(self, Zeile: str, Spaltentitel: str, tabelle_nummer: int=None): # type: ignore
        """
        Der Inhalt der Zelle am Schnittpunkt der Zeile und der Spalte wird zurückgegeben.
        
        | ``Zeile`` | Entweder die Zeilennummer oder der Inhalt einer Zelle in der Zeile. Wenn die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden. |
        | ``Spaltentitel`` | Spaltentitel oder Kurzinfo |
        | ``tabelle_nummer`` | Spezifiziert welche Tabelle: 1, 2, ... |
        
        Beispiele:
        
        | ``Tabellenzelle auslesen     Zeile     Spaltentitel``
        """

        args: list = [
            Zeile,
            Spaltentitel
        ]
        kwargs: dict = {
            "tabelle_nummer": tabelle_nummer
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Die Zelle mit dem Lokator '{0}, {1}' wurde nicht gefunden. Hinweise: Prüfe die Rechtschreibung, maximiere das SAP Fenster",
            "NoTable": "Die Maske enthält keine Tabelle.",
            "InvalidTable": "Die Maske enthält keine Tabelle mit dem Index {0}.",
            "Pass": "Die Zelle mit dem Lokator '{0}, {1}' wurde ausgelesen.",
            "Exception": "Die Zelle konnte nicht ausgelesen werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ReadCell', args, kwargs, result) # type: ignore
    
    @keyword('Fenster aufnehmen') # type: ignore
    def save_screenshot(self, Speicherort: str): # type: ignore
        """
        Eine Bildschirmaufnahme des Fensters wird im angegebenen Speicherort gespeichert.
        
        | ``Speicherort`` | Entweder der absolute Pfad einer .png Datei oder LOG, um das Bild in das Protokoll einzubetten. |
        
        Beispiele:
        
        | ``Fenster aufnehmen     Speicherort``
        
        *Hinweis*: Rückwärtsschrägstriche müssen doppelt geschrieben werden. Ansonsten verwende die Standard RF Variable ${/} als Trennzeichen.
        """

        args: list = [
            Speicherort
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "InvalidPath": "Der Pfad '{0}' ist ungültig",
            "UNCPath": "Ein UNC Pfad (d.h. beginnend mit \\) ist nicht erlaubt",
            "NoAbsPath": "'{0}' ist kein absoluter Pfad",
            "Log": "Der Rückgabewert wird in das Protokoll geschrieben.",
            "Pass": "Eine Aufnahme des Fensters wurde in '{0}' gespeichert.",
            "Exception": "Eine Aufnahme des Fensters konnte nicht gespeichert werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('SaveScreenshot', args, kwargs, result) # type: ignore
    
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

        args: list = [
            Richtung
        ]
        kwargs: dict = {
            "bis_Textfeld": bis_Textfeld
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "Exception": "Die Inhalte der Textfelder konnten nicht gescrollt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen.",
            "NoScrollbar": "Das Fenster enthält keine scrollbaren Textfelder.",
            "MaximumReached": "Die Inhalte der Textfelder können nicht weiter gescrollt werden.",
            "InvalidDirection": "Die angegebene Richtung ist ungültig. Gültige Richtungen sind: UP, DOWN, BEGIN, END",
            "Pass": "Die Inahlte der Textfelder wurden in die Richtung '{0}' gescrollt."
        }
        return super()._run_keyword('ScrollTextFieldContents', args, kwargs, result) # type: ignore
    
    @keyword('Fenster horizontal scrollen') # type: ignore
    def scroll_window_horizontally(self, Richtung: str): # type: ignore
        """
        Die horizontale Bildlaufleiste des Fensters wird in die angegebene Richtung bewegt.
        
        | ``Richtung`` | LEFT, RIGHT, BEGIN, END |
        
        Beispiele:
        
        | ``Fenster horizontal scrollen    Richtung``
        """

        args: list = [
            Richtung
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "Exception": "Das Fenster konnte nicht horizontal gescrollt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen.",
            "NoScrollbar": "Das Fenster enthält keine horizontale Bildlaufleiste.",
            "MaximumReached": "Das Fenster kann nicht weiter gescrollt werden.",
            "InvalidDirection": "Die angegebene Richtung ist ungültig. Gültige Richtungen sind: LEFT, RIGHT, BEGIN, END",
            "Pass": "Das Fenster wurde in die Richtung '{0}' horizontal gescrollt."
        }
        return super()._run_keyword('ScrollWindowHorizontally', args, kwargs, result) # type: ignore
    
    @keyword('Tabellenzelle markieren') # type: ignore
    def select_cell(self, Zeile: str, Spaltentitel: str, tabelle_nummer: int=None): # type: ignore
        """
        Die Zelle am Schnittpunkt der Zeile und der Spalte wird markiert.
        
        | ``Zeile`` | Entweder die Zeilennummer oder der Inhalt einer Zelle in der Zeile. Wenn die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden. |
        | ``Spaltentitel`` | Spaltentitel oder Kurzinfo |
        | ``tabelle_nummer`` | Spezifiziert welche Tabelle: 1, 2, ... |
        
        Beispiele:
        
        | ``Tabellenzelle markieren     Zeile     Spaltentitel``
        
        *Hinweis*: Dieses Schlüsselwort kann verwendet werden, um auf einen Link (unterstrichener Text oder Symbol) oder auf ein Optionsfeld in einer Zelle zu klicken.
        """

        args: list = [
            Zeile,
            Spaltentitel
        ]
        kwargs: dict = {
            "tabelle_nummer": tabelle_nummer
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Die Zelle mit dem Lokator '{0}, {1}' wurde nicht gefunden. Hinweise: Prüfe die Rechtschreibung, maximiere das SAP Fenster",
            "NoTable": "Die Maske enthält keine Tabelle.",
            "InvalidTable": "Die Maske enthält keine Tabelle mit dem Index {0}.",
            "Pass": "Die Zelle mit dem Lokator '{0}, {1}' wurde markiert.",
            "Exception": "Die Zelle konnte nicht markiert werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('SelectCell', args, kwargs, result) # type: ignore
    
    @keyword('Tabellenzellenwert auswählen') # type: ignore
    def select_cell_value(self, Zeile: str, Spaltentitel: str, Wert: str, tabelle_nummer: int=None): # type: ignore
        """
        In der Zelle am Schnittpunkt der Zeile und Spalte wird der angegebene Wert ausgewählt.
        
        | ``Zeile`` | Entweder die Zeilennummer oder der Inhalt einer Zelle in der Zeile. Wenn die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden. |
        | ``Spaltentitel`` | Spaltentitel oder Kurzinfo |
        | ``Wert`` | Ein Wert aus dem Auswahlmenü |
        | ``tabelle_nummer`` | Spezifiziert welche Tabelle: 1, 2, ... |
        
        Beispiele:
        
        | ``Tabellenzellenwert auswählen    Zeile    Spaltentitel    Wert``
        """

        args: list = [
            Zeile,
            Spaltentitel,
            Wert
        ]
        kwargs: dict = {
            "tabelle_nummer": tabelle_nummer
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Die Zelle mit dem Lokator '{0}, {1}' wurde nicht gefunden. Hinweise: Prüfe die Rechtschreibung, maximiere das SAP Fenster",
            "EntryNotFound": "Der Wert '{2}' ist in der Zelle mit dem Lokator '{0}, {1}' nicht vorhanden.\nHinweis: Prüfe die Rechtschreibung",
            "InvalidTable": "Die Maske enthält keine Tabelle mit dem Index {0}.",
            "Exception": "Der Wert konnte nicht ausgewählt werden. {0}\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen.",
            "Pass": "Der Wert '{2}' wurde ausgewählt."
        }
        return super()._run_keyword('SelectCellValue', args, kwargs, result) # type: ignore
    
    @keyword('Formularfeld-Status auslesen') # type: ignore
    def read_check_box(self, Lokator: str): # type: ignore
        """
        Der Status des angegebenen Formularfelds wird ausgelesen.
        
        | ``Lokator`` | Ein Lokator, um das Formularfeld zu finden |
        
        Beispiele:
        
        *Formularfeld mit einer Beschriftung links oder rechts *
        | ``Formularfeld-Status auslesen    Beschriftung``
        
        *Formularfeld mit einer Beschriftung oben*
        | ``Formularfeld-Status auslesen    @ Beschriftung``
        
        *Formularfeld am Schnittpunkt einer Beschriftung links und einer oben*
        | ``Formularfeld-Status auslesen    Beschriftung links @ Beschriftung oben``
        """

        args: list = [
            Lokator
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Das Formularfeld mit dem Lokator '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Der Status des Formularfelds mit dem Lokator '{0}' wurde ausgelesen.",
            "Exception": "Der Status des Formularfelds konnte nicht ausgelesen werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ReadCheckBox', args, kwargs, result) # type: ignore
    
    @keyword('Auswahlmenüeintrag auslesen') # type: ignore
    def read_combo_box_entry(self, Lokator: str): # type: ignore
        """
        Aus dem angegebenen Auswahlmenü wird der aktuelle Eintrag ausgelesen.
        
        | ``Lokator`` | Beschriftung oder Kurzinfo des Auswahlmenüs |
        
        Beispiele:
        
        | ``${Eintrag}   Auswahlmenüeintrag auslesen    Lokator``
        """

        args: list = [
            Lokator
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Das Auswahlmenü mit dem Lokator '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Der aktuelle Eintrag wurde ausgelesen.",
            "Exception": "Der Eintrag konnte nicht ausgelesen werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ReadComboBoxEntry', args, kwargs, result) # type: ignore
    
    @keyword('Auswahlmenüeintrag auswählen') # type: ignore
    def select_combo_box_entry(self, Lokator: str, Eintrag: str): # type: ignore
        """
        Aus dem angegebenen Auswahlmenü wird der angegebene Eintrag ausgewählt.
        
        | ``Lokator`` | Beschriftung oder Kurzinfo des Auswahlmenüs |
        | ``Eintrag`` | Ein Eintrag aus dem Auswahlmenü |
        
        Beispiele:
        
        | ``Auswahlmenüeintrag auswählen    Lokator    Eintrag``
        
        *Hinweise*: 
        - Wenn der Eintrag nicht eindeutig ist, verwende den Schlüssel, der angezeigt wird, wenn "Schlüssel in Dropdown-Listen anzeigen" in den SAP GUI-Optionen aktiviert ist.
        - Um einen Eintrag aus einem Symbolleisten-Knopf mit Auswahlmenü auszuwählen, verwende die Beschriftung bzw. die Kurzinfo (Tooltip) des Knopfes als Lokator.
        """

        args: list = [
            Lokator,
            Eintrag
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Das Auswahlmenü mit dem Lokator '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "EntryNotFound": "Der Eintrag '{1}' wurde im Auswahlmenü '{0}' nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Der Eintrag '{1}' wurde ausgewählt.",
            "Exception": "Der Eintrag konnte nicht ausgewählt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('SelectComboBoxEntry', args, kwargs, result) # type: ignore
    
    @keyword('Menüeintrag auswählen') # type: ignore
    def select_menu_item(self, Eintragspfad: str): # type: ignore
        """
        Der Menüeintrag mit dem angegebenen Pfad wird ausgewählt.
        
        | ``Eintragspfad`` | Der Pfad zum Eintrag mit '/' als Trennzeichen (z.B. System/Benutzervorgaben/Eigene Daten). |
        
        Beispiele:
        
        | ``Menüeintrag auswählen    Eintragspfad``
        """

        args: list = [
            Eintragspfad
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Der Menüeintrag '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Der Menüeintrag '{0}' wurde ausgewählt.",
            "Exception": "Der Menüeintrag konnte nicht ausgewählt werden. {0}\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('SelectMenuItem', args, kwargs, result) # type: ignore
    
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

        args: list = [
            Lokator
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Das Optionsfeld mit dem Lokator '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "NotChangeable": "Das Optionsfeld mit dem Lokator '{0}' ist deaktiviert.",
            "Pass": "Das Optionsfeld mit dem Lokator '{0}' wurde ausgewählt.",
            "Exception": "Das Optionsfeld konnte nicht ausgewählt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('SelectRadioButton', args, kwargs, result) # type: ignore
    
    @keyword('Tabellenspalte markieren') # type: ignore
    def select_table_column(self, Spalte: str, tabelle_nummer: int=1): # type: ignore
        """
        Die angegebene Tabellenspalte wird markiert.
        
        | ``Spalte`` | Spaltentitel oder Kurzhilfe (Tooltip) |
        | ``tabelle_nummer`` | Spezifiziert welche Tabelle: 1, 2, ... |
        
        Beispiele:
        
        | ``Tabellenspalte markieren    Spalte``
        """

        args: list = [
            Spalte
        ]
        kwargs: dict = {
            "tabelle_nummer": tabelle_nummer
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "Exception": "Die Spalte konnte nicht markiert werden. {0}\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen.",
            "NoTable": "Die Maske entählt keine Tabelle",
            "InvalidTable": "Die Maske enthält keine Tabelle mit dem Index {0}.",
            "NotFound": "Die Tabelle enthält keine Spalte '{0}'",
            "Pass": "Die Spalte '{0}' wurde markiert"
        }
        return super()._run_keyword('SelectTableColumn', args, kwargs, result) # type: ignore
    
    @keyword('Tabellenzeile markieren') # type: ignore
    def select_table_row(self, Zeilenlokator: str, tabelle_nummer: int=1): # type: ignore
        """
        Die angegebene(n) Tabellenzeile(n) soll(en) markiert werden und die entsprechende(n) Zeilennummer zurückgegeben.
        
        | ``Zeilenlokator`` | Entweder die Zeilennummer oder der Inhalt einer Zelle in der Zeile. Wenn die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden. |
        | ``tabelle_nummer`` | Spezifiziert welche Tabelle: 1, 2, ... |
        
        Beispiele:
        
        *Eine einzelne Zeile markieren*
        | ``Tabellenzeile markieren    Zeilenlokator``
        
        *Mehrere Zeilen in einem ALV-Grid (eine Tabelle mit einer Symbolleiste) markieren*
        | ``Tabellenzeile markieren    1,2,3``
        
        *Hinweis*: Mit der Zeilennummer 0 wird die gesamte Tabelle markiert.
        """

        args: list = [
            Zeilenlokator
        ]
        kwargs: dict = {
            "tabelle_nummer": tabelle_nummer
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "Exception": "Die Zeile konnte nicht markiert werden. {0}\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen.",
            "NoTable": "Die Maske entählt keine Tabelle",
            "InvalidTable": "Die Maske enthält keine Tabelle mit dem Index {0}.",
            "InvalidIndex": "Die Tabelle hat keine Zeile '{0}'",
            "NotFound": "Die Tabelle enthält keine Zelle mit dem Inhalt '{0}'",
            "Pass": "Die Zeile mit dem Lokator '{0}' wurde markiert"
        }
        return super()._run_keyword('SelectTableRow', args, kwargs, result) # type: ignore
    
    @keyword('Textfeld markieren') # type: ignore
    def select_text_field(self, Lokator: str): # type: ignore
        """
        Das angegebene Textfeld wird markiert.
        
        | ``Lokator`` | Die Lokatoren für Textfelder sind im Schlüsselwort [#Textfeld ausfüllen|Textfeld ausfüllen] dokumentiert. |
        
        Beispiele:
        
        *Identifizierung des Textfeldes über einen Lokator*
        | ``Textfeld markieren    Lokator``
        
        *Identifizierung des Textfeldes über seinen Inhalt*
        | ``Textfeld markieren     = Inhalt``
        """

        args: list = [
            Lokator
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Das Textfeld mit dem Lokator '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Das Textfeld mit dem Lokator '{0}' wurde markiert.",
            "Exception": "Das Textfeld konnte nicht markiert werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('SelectTextField', args, kwargs, result) # type: ignore
    
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

        args: list = [
            Lokator
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Der Text mit dem Lokator '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Der Text mit dem Lokator '{0}' wurde markiert.",
            "Exception": "Der Text konnte nicht markiert werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('SelectText', args, kwargs, result) # type: ignore
    
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

        args: list = [
            Lokator
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Das Formularfeld mit dem Lokator '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "NotChangeable": "Das Formularfeld mit dem Lokator '{0}' ist deaktiviert.",
            "Pass": "Das Formularfeld mit dem Lokator '{0}' wurde angekreuzt.",
            "Exception": "Das Formularfeld konnte nicht angekreuzt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('TickCheckBox', args, kwargs, result) # type: ignore
    
    @keyword('Statusleiste auslesen') # type: ignore
    def read_statusbar(self): # type: ignore
        """
        Der Inhalt der Statusleiste wird ausgelesen. Der Rückgabewert ist ein Dictionary mit den Einträgen 'status' und 'message'.
        
        
        Beispiele:
        
        | ``${statusleiste}   Statusleiste auslesen``
        """

        args: list = [
        ]
        kwargs: dict = {
        }
        
        result = {
            "Json": "Der Rückgabewert ist im JSON-Format",
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Keine Statusleiste gefunden.",
            "Exception": "Die Statusleiste konnte nicht ausgelesen werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen.",
            "Pass": "Die Statusleiste wurde ausgelesen."
        }
        return super()._run_keyword('ReadStatusbar', args, kwargs, result) # type: ignore
    
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

        args: list = [
            Lokator
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Das Formularfeld mit dem Lokator '{0}' wurde nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "NotChangeable": "Das Formularfeld mit dem Lokator '{0}' ist deaktiviert.",
            "Pass": "Das Formularfeld mit dem Lokator '{0}' wurde abgewählt.",
            "Exception": "Das Formularfeld konnte nicht abgewählt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('UntickCheckBox', args, kwargs, result) # type: ignore
    
    @keyword('Tabellenzelle ankreuzen') # type: ignore
    def tick_check_box_cell(self, Zeile: str, Spaltentitel: str, tabelle_nummer: int=None): # type: ignore
        """
        Die Zelle am Schnittpunkt der Zeile und der Spalte wird angekreuzt.
        
        | ``Zeile`` | Entweder die Zeilennummer oder der Inhalt einer Zelle in der Zeile. Wenn die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden. |
        | ``Spaltentitel`` | Spaltentitel oder Kurzinfo |
        | ``tabelle_nummer`` | Spezifiziert welche Tabelle: 1, 2, ... |
        
        Beispiele:
        
        | ``Tabellenzelle ankreuzen     Zeile     Spaltentitel``
        
        *Hinweis*: Um das Formularfeld in der Spalte ganz links ohne Titel anzukreuzen, markiere die Zeile und drücke die "Enter"-Taste.
        """

        args: list = [
            Zeile,
            Spaltentitel
        ]
        kwargs: dict = {
            "tabelle_nummer": tabelle_nummer
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Die Zelle mit dem Lokator '{0}, {1}' wurde nicht gefunden. Hinweise: Prüfe die Rechtschreibung, maximiere das SAP Fenster",
            "NotChangeable": "Die Zelle mit dem Lokator '{0}, {1}' ist deaktiviert.",
            "InvalidTable": "Die Maske enthält keine Tabelle mit dem Index {0}.",
            "Pass": "Die Zelle mit dem Lokator '{0}, {1}' wurde angekreuzt.",
            "Exception": "Die Zelle konnte nicht angekreuzt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('TickCheckBoxCell', args, kwargs, result) # type: ignore
    
    @keyword('Tabellenzelle abwählen') # type: ignore
    def untick_check_box_cell(self, Zeile: str, Spaltentitel: str, tabelle_nummer: int=None): # type: ignore
        """
        Die Zelle am Schnittpunkt der Zeile und der Spalte wird abgewählt.
        
        | ``Zeile`` | Entweder die Zeilennummer oder der Inhalt einer Zelle in der Zeile. Wenn die Zelle nur eine Zahl enthält, muss diese in Anführungszeichen gesetzt werden. |
        | ``Spaltentitel`` | Spaltentitel oder Kurzinfo |
        | ``tabelle_nummer`` | Spezifiziert welche Tabelle: 1, 2, ... |
        
        Beispiele:
        
        | ``Tabellenzelle abwählen     Zeile     Spaltentitel``
        """

        args: list = [
            Zeile,
            Spaltentitel
        ]
        kwargs: dict = {
            "tabelle_nummer": tabelle_nummer
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "NotFound": "Die Zelle mit dem Lokator '{0}, {1}' wurde nicht gefunden. Hinweise: Prüfe die Rechtschreibung, maximiere das SAP Fenster",
            "NotChangeable": "Die Zelle mit dem Lokator '{0}, {1}' ist deaktiviert.",
            "InvalidTable": "Die Maske enthält keine Tabelle mit dem Index {0}.",
            "Pass": "Die Zelle mit dem Lokator '{0}, {1}' wurde abgewählt.",
            "Exception": "Die Zelle konnte nicht abgewählt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('UntickCheckBoxCell', args, kwargs, result) # type: ignore
    
    @keyword('Fenstertitel auslesen') # type: ignore
    def get_window_title(self): # type: ignore
        """
        Der Titel des gerade aktiven Fensters wird zurückgegeben.
        
        
        Beispiele:
        
        | ``${Titel}    Fenstertitel auslesen``
        """

        args: list = [
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "Pass": "Der Fenstertitel wurde ausgelesen",
            "Exception": "Der Titel des Fensters konnte nicht ausgelesen werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('GetWindowTitle', args, kwargs, result) # type: ignore
    
    @keyword('Fenstertext auslesen') # type: ignore
    def get_window_text(self): # type: ignore
        """
        Der Text des gerade aktiven Fensters wird zurückgegeben.
        
        
        Beispiele:
        
        | ``${Text}    Fenstertext auslesen``
        """

        args: list = [
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "Pass": "Der Text des Fensters wurde ausgelesen",
            "Exception": "Der Text des Fensters konnte nicht ausgelesen werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('GetWindowText', args, kwargs, result) # type: ignore
    
    @keyword('Fenster maximieren') # type: ignore
    def maximize_window(self): # type: ignore
        """
        Das Fenster im Vordergrund wird maximiert.
        
        
        Beispiele:
        
        | ``Fenster maximieren``
        """

        args: list = [
        ]
        kwargs: dict = {
        }
        
        result = {
            "NoSession": "Keine aktive SAP-Session gefunden. Das Keyword \"Verbindung zum Server Herstellen\" oder \"Laufende SAP GUI Übernehmen\" muss zuerst aufgerufen werden.",
            "Pass": "Das Fenster im Vordergrund wurde maximiert.",
            "Exception": "Das Fenster im Vordergrund konnte nicht maximiert werden. {0}\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('MaximizeWindow', args, kwargs, result) # type: ignore
    
    ROBOT_LIBRARY_SCOPE = 'GLOBAL'
    ROBOT_LIBRARY_VERSION = '2.23.0'