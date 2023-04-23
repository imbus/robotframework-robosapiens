from robot.api.deco import keyword
from RoboSAPiens.client import RoboSAPiensClient

class DE(RoboSAPiensClient):
    """
    RoboSAPiens: SAP GUI-Automatisierung für Menschen
    
    Um diese Bibliothek zu verwenden, müssen drei Bedigungen erfüllt werden:
    
    - Das .NET Runtime 7.0 x86 muss [https://dotnet.microsoft.com/en-us/download/dotnet/7.0|installiert] werden. 
    
    - Das [https://help.sap.com/saphelp_aii710/helpdata/de/ba/b8710932b8c64a9e8acf5b6f65e740/content.htm|Scripting] muss auf dem SAP Server aktiviert werden.
    
    - Die [https://help.sap.com/docs/sap_gui_for_windows/63bd20104af84112973ad59590645513/7ddb7c9c4a4c43219a65eee4ca8db001.html|Skriptunterstützung] muss in der SAP GUI aktiviert werden.
    
    Diese Bibliothek implementiert die [https://robotframework.org/robotframework/latest/RobotFrameworkUserGuide.html#remote-library-interface|Remote Library Interface] von Robot Framework. Das heißt, ein HTTP Server läuft im Hintergrund und Robot Framework kommuniziert mit ihm. Standardmäßig lauscht der HTTP Server auf dem Port 8270. Der Port kann beim Import der Bibliothek angepasst werden:
    | ``Library   RoboSAPiens  port=1234``
    
    """
    
    def __init__(self, port: int=8270, vortragsmodus: bool=False):
        """
        *port*: Port des HTTP servers.
        
        *vortragsmodus*: Jedes GUI Element wird vor seiner Betätigung bzw. Änderung kurz hervorgehoben
        """
        
        args = {
            'port': port,
            'presenter_mode': vortragsmodus,
        }
        
        super().__init__(args)
    

    @keyword('Reiter auswählen') # type: ignore
    def ActivateTab(self, Reitername: str): # type: ignore
        """
        Der Reiter mit dem angegebenen Name wird ausgewählt.
        
        | ``Reiter auswählen    Name``
        """
        
        args = {
            'Reitername': Reitername,
        }
        
        result = {
            "NoSession": "Keine SAP-Session vorhanden. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "NotFound": "Der Reiter '{0}' konnte nicht gefunden werden.\nHinweis: Prüfe die Rechtschreibung",
            "SapError": "SAP Fehlermeldung: {0}",
            "Pass": "Der Reiter '{0}' wurde ausgewählt.",
            "Exception": "*ERROR* Der Reiter konnte nicht ausgewählt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ActivateTab', list(args.values()), dict(), result) # type: ignore
    

    @keyword('SAP starten') # type: ignore
    def OpenSAP(self, Pfad: str): # type: ignore
        """
        Die SAP GUI wird gestartet. Der übliche Pfad ist\n\n| ``C:\\Program Files (x86)\\SAP\\FrontEnd\\SAPgui\\saplogon.exe``
        """
        
        args = {
            'Pfad': Pfad,
        }
        
        result = {
            "Pass": "Die SAP GUI wurde gestartet",
            "SAPNotStarted": "Die SAP GUI konnte nicht gestartet werden. Überprüfe den Pfad '{0}'.",
            "Exception": "*ERROR* Die SAP GUI konnte nicht gestartet werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('OpenSAP', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Verbindung zum Server trennen') # type: ignore
    def CloseConnection(self, ): # type: ignore
        """
        Die Verbindung mit dem SAP Server wird beendet.
        """
        
        args = {
        }
        
        result = {
            "NoSapGui": "Keine laufende SAP GUI gefunden. Das Keyword \"SAP starten\" muss zuerst aufgerufen werden.",
            "NoGuiScripting": "Die Skriptunterstützung ist nicht verfügbar. Sie muss in den Einstellungen von SAP Logon aktiviert werden.",
            "NoConnection": "Es besteht keine Verbindung zu einem SAP Server. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "NoSession": "Keine SAP-Session vorhanden. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "Pass": "Die Verbindung zum Server wurde getrennt.",
            "Exception": "*ERROR* Die Verbindung zum Server konnte nicht getrennt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('CloseConnection', list(args.values()), dict(), result) # type: ignore
    

    @keyword('SAP beenden') # type: ignore
    def CloseSAP(self, ): # type: ignore
        """
        Die SAP GUI wird beendet.
        """
        
        args = {
        }
        
        result = {
            "NoSapGui": "Keine laufende SAP GUI gefunden. Das Keyword \"SAP starten\" muss zuerst aufgerufen werden.",
            "Pass": "Die SAP GUI wurde beendet"
        }
        return super()._run_keyword('CloseSAP', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Funktionsbaum exportieren') # type: ignore
    def ExportTree(self, Dateipfad: str): # type: ignore
        """
        Der Funktionsbaum wird in der angegebenen Datei gespeichert.
        
        | ``Funktionsbaum exportieren     Dateipfad``
        """
        
        args = {
            'Dateipfad': Dateipfad,
        }
        
        result = {
            "NoSession": "Keine SAP-Session vorhanden. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "NotFound": "Die Maske enthält keine Baumstruktur",
            "Pass": "Die Baumstruktur wurde in JSON Format in der Datei '{0}' gespeichert",
            "Exception": "*ERROR* Die Baumstruktur konnte nicht exportiert werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ExportTree', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Laufende SAP GUI übernehmen') # type: ignore
    def AttachToRunningSAP(self, ): # type: ignore
        """
        Nach der Ausführung dieses Keywords, kann eine laufende SAP GUI mit RoboSAPiens gesteuert werden.
        """
        
        args = {
        }
        
        result = {
            "NoSapGui": "Keine laufende SAP GUI gefunden. Das Keyword \"SAP starten\" muss zuerst aufgerufen werden.",
            "NoGuiScripting": "Die Skriptunterstützung ist nicht verfügbar. Sie muss in den Einstellungen von SAP Logon aktiviert werden.",
            "NoConnection": "Es besteht keine Verbindung zu einem SAP Server. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "NoServerScripting": "Das Scripting ist auf dem SAP Server nicht freigeschaltet. Siehe die Dokumentation von RoboSAPiens.",
            "Pass": "Die laufende SAP GUI wurde erfolgreich übernommen.",
            "Exception": "*ERROR* Die laufende SAP GUI konnte nicht übernommen werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('AttachToRunningSAP', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Verbindung zum Server herstellen') # type: ignore
    def ConnectToServer(self, Servername: str): # type: ignore
        """
        Die Verbindung mit dem angegebenen SAP Server wird hergestellt.
        
        | ``Verbindung zum Server herstellen    Servername``
        """
        
        args = {
            'Servername': Servername,
        }
        
        result = {
            "NoSapGui": "Keine laufende SAP GUI gefunden. Das Keyword \"SAP starten\" muss zuerst aufgerufen werden.",
            "NoGuiScripting": "Die Skriptunterstützung ist nicht verfügbar. Sie muss in den Einstellungen von SAP Logon aktiviert werden.",
            "Pass": "Die Verbindung mit dem Server '{0}' wurde erfolgreich hergestellt.",
            "SapError": "SAP Fehlermeldung: {0}",
            "NoServerScripting": "Das Scripting ist auf dem SAP Server nicht freigeschaltet. Siehe die Dokumentation von RoboSAPiens.",
            "Exception": "*ERROR* Die Verbindung konnte nicht hergestellt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ConnectToServer', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Tabellenzelle doppelklicken') # type: ignore
    def DoubleClickCell(self, Zeilennummer_oder_Zellinhalt: str, Spaltentitel: str): # type: ignore
        """
        Die angegebene Tabellenzelle wird doppelgeklickt.
        
        | ``Tabellenzelle doppelklicken     Positionsgeber     Spaltentitel``
        Positionsgeber: entweder die Zeilennummer oder der Inhalt der Zelle.
        """
        
        args = {
            'Zeilennummer_oder_Zellinhalt': Zeilennummer_oder_Zellinhalt,
            'Spaltentitel': Spaltentitel,
        }
        
        result = {
            "NoSession": "Keine SAP-Session vorhanden. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "NotFound": "Die Zelle mit dem Lokator '{0}, {1}' konnte nicht gefunden werden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Die Zelle mit dem Lokator '{0}' wurde doppelgeklickt.",
            "Exception": "*ERROR* Die Zelle konnte nicht doppelgeklickt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('DoubleClickCell', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Textfeld doppelklicken') # type: ignore
    def DoubleClickTextField(self, Inhalt: str): # type: ignore
        """
        Das angegebene Textfeld wird doppelgeklickt.
        
        | ``Textfeld doppelklicken     Inhalt``
        
        """
        
        args = {
            'Inhalt': Inhalt,
        }
        
        result = {
            "NoSession": "Keine SAP-Session vorhanden. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "NotFound": "Das Textfeld mit dem Lokator '{0}' konnte nicht gefunden werden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Das Textfeld mit dem Lokator '{0}' wurde doppelgeklickt.",
            "Exception": "*ERROR* Das Textfeld konnte nicht doppelgeklickt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('DoubleClickTextField', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Transaktion ausführen') # type: ignore
    def ExecuteTransaction(self, T_Code: str): # type: ignore
        """
        Die Transaktion mit dem angegebenen T-Code wird ausgeführt.
        
        | ``Transaktion ausführen    T-Code``
        """
        
        args = {
            'T_Code': T_Code,
        }
        
        result = {
            "NoSession": "Keine SAP-Session vorhanden. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "Pass": "Die Transaktion mit T-Code '{0}' wurde erfolgreich ausgeführt.",
            "Exception": "*ERROR* Die Transaktion konnte nicht ausgeführt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ExecuteTransaction', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Maske exportieren') # type: ignore
    def ExportForm(self, Name: str, Verzeichnis: str): # type: ignore
        """
        Alle Texte in der aktuellen Maske werden in einer CSV-Datei gespeichert. Außerdem wird ein Bildschirmfoto in PNG-Format erstellt.
        
        | ``Maske exportieren     Name     Verzeichnis``
        Verzeichnis: Der absolute Pfad des Verzeichnisses, wo die Dateien abgelegt werden.
        """
        
        args = {
            'Name': Name,
            'Verzeichnis': Verzeichnis,
        }
        
        result = {
            "NoSession": "Keine SAP-Session vorhanden. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "Pass": "Die Maske wurde in den Dateien '{0}' und '{1}' gespeichert",
            "Exception": "*ERROR* Die Maske konnte nicht exportiert werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ExportForm', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Tabellenzelle ausfüllen') # type: ignore
    def FillTableCell(self, Zeilennummer_oder_Zellinhalt: str, Spaltentitel_Gleich_Inhalt: str): # type: ignore
        """
        Die Zelle am Schnittpunkt der angegebenen Zeile und Spalte wird mit dem angegebenen Inhalt ausgefüllt.
        
        | ``Tabellenzelle ausfüllen     Zeile     Spaltentitel = Inhalt``
        Zeile: entweder eine Zeilennummer oder der Inhalt einer Zelle in der Zeile.
        
        *Hinweis*: Eine Tabellenzelle hat u.U. eine Beschriftung, die man über die Hilfe (Taste F1) herausfinden kann. In diesem Fall kann man die Zelle mit dem Keyword [#Textfeld%20Ausfüllen|Textfeld ausfüllen] ausfüllen.
        """
        
        args = {
            'Zeilennummer_oder_Zellinhalt': Zeilennummer_oder_Zellinhalt,
            'Spaltentitel_Gleich_Inhalt': Spaltentitel_Gleich_Inhalt,
        }
        
        result = {
            "NoSession": "Keine SAP-Session vorhanden. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "InvalidFormat": "Das zweite Argument muss dem Muster `Spalte = Inhalt` entsprechen",
            "NotFound": "Die Zelle mit dem Lokator '{0}, {1}' konnte nicht gefunden werden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Die Zelle mit dem Lokator '{0}' wurde ausgefüllt.",
            "Exception": "*ERROR* Die Zelle konnte nicht ausgefüllt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('FillTableCell', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Textfeld ausfüllen') # type: ignore
    def FillTextField(self, Beschriftung_oder_Positionsgeber: str, Inhalt: str): # type: ignore
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
        
        args = {
            'Beschriftung_oder_Positionsgeber': Beschriftung_oder_Positionsgeber,
            'Inhalt': Inhalt,
        }
        
        result = {
            "NoSession": "Keine SAP-Session vorhanden. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "NotFound": "Das Textfeld mit dem Lokator '{0}' konnte nicht gefunden werden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Das Textfeld mit dem Lokator '{0}' wurde ausgefüllt.",
            "Exception": "*ERROR* Das Textfeld konnte nicht ausgefüllt werden. Möglicherweise, weil der Inhalt nicht dazu passt.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('FillTextField', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Knopf drücken') # type: ignore
    def PushButton(self, Name_oder_Kurzinfo: str): # type: ignore
        """
        Der Knopf mit dem angegebenen Namen oder Kurzinfo (Tooltip) wird gedrückt.
        
        | ``Knopf drücken    Name oder Kurzinfo (Tooltip)``
        """
        
        args = {
            'Name_oder_Kurzinfo': Name_oder_Kurzinfo,
        }
        
        result = {
            "NoSession": "Keine SAP-Session vorhanden. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "SapError": "SAP Fehlermeldung: {0}",
            "NotFound": "Der Knopf '{0}' konnte nicht gefunden werden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Der Knopf '{0}' wurde gedrückt.",
            "Exception": "*ERROR* Der Knopf konnte nicht gedrückt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('PushButton', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Tabellenzelle drücken') # type: ignore
    def PushButtonCell(self, Zeilennummer_oder_Name_oder_Kurzinfo: str, Spaltentitel: str): # type: ignore
        """
        Die angegebene Tabellenzelle wird gedrückt.
        
        | ``Tabellenzelle drücken     Positionsgeber     Spaltentitel``
        Positionsgeber: Zeilennummer, Beschriftung oder Kurzinfo (Tooltip).
        """
        
        args = {
            'Zeilennummer_oder_Name_oder_Kurzinfo': Zeilennummer_oder_Name_oder_Kurzinfo,
            'Spaltentitel': Spaltentitel,
        }
        
        result = {
            "NoSession": "Keine SAP-Session vorhanden. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "NotFound": "Die Zelle mit dem Lokator '{0}, {1}' konnte nicht gefunden werden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Die Zelle mit dem Lokator '{0}' wurde gedrückt.",
            "Exception": "*ERROR* Die Zelle konnte nicht gedrückt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('PushButtonCell', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Textfeld auslesen') # type: ignore
    def ReadTextField(self, Beschriftung_oder_Positionsgeber: str): # type: ignore
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
        
        args = {
            'Beschriftung_oder_Positionsgeber': Beschriftung_oder_Positionsgeber,
        }
        
        result = {
            "NoSession": "Keine SAP-Session vorhanden. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "NotFound": "Das Textfeld mit dem Lokator '{0}' konnte nicht gefunden werden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Das Textfeld mit dem Lokator '{0}' wurde ausgelesen.",
            "Exception": "*ERROR* Das Textfeld konnte nicht ausgelesen werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ReadTextField', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Text auslesen') # type: ignore
    def ReadText(self, Inhalt: str): # type: ignore
        """
        Der Inhalt des angegebenen Texts wird zurückgegeben.
        
        *Text fängt mit der angegebenen Teilzeichenfolge an*
        | ``Text auslesen    = Teilzeichenfolge``
        *Text folgt einer Beschriftung*
        | ``Text auslesen    Beschriftung``
        """
        
        args = {
            'Inhalt': Inhalt,
        }
        
        result = {
            "NoSession": "Keine SAP-Session vorhanden. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "NotFound": "Der Text mit dem Lokator '{0}' konnte nicht gefunden werden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Der Text mit dem Lokator '{0}' wurde ausgelesen.",
            "Exception": "*ERROR* Der Text konnte nicht ausgelesen werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ReadText', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Tabellenzelle auslesen') # type: ignore
    def ReadTableCell(self, Zeilennummer_oder_Zellinhalt: str, Spaltentitel: str): # type: ignore
        """
        Der Inhalt der angegebenen Tabellenzelle wird zurückgegeben.
        
        | ``Tabellenzelle ablesen     Positionsgeber     Spaltentitel``
        Positionsgeber: Zeilennummer oder Zellinhalt.
        """
        
        args = {
            'Zeilennummer_oder_Zellinhalt': Zeilennummer_oder_Zellinhalt,
            'Spaltentitel': Spaltentitel,
        }
        
        result = {
            "NoSession": "Keine SAP-Session vorhanden. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "NotFound": "Die Zelle mit dem Lokator '{0}, {1}' konnte nicht gefunden werden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Die Zelle mit dem Lokator '{0}' wurde ausgelesen.",
            "Exception": "*ERROR* Die Zelle konnte nicht ausgelesen werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('ReadTableCell', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Fenster aufnehmen') # type: ignore
    def SaveScreenshot(self, Aufnahmenverzeichnis: str): # type: ignore
        """
        Eine Bildschirmaufnahme des Fensters wird im eingegebenen Dateipfad gespeichert.
        
        | ``Fenster aufnehmen     Dateipfad``
        Dateifpad: Der absolute Pfad einer .png Datei bzw. eines Verzeichnisses.
        """
        
        args = {
            'Aufnahmenverzeichnis': Aufnahmenverzeichnis,
        }
        
        result = {
            "NoSession": "Keine SAP-Session vorhanden. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "UNCPath": "Ein UNC Pfad (d.h. beginnend mit \\) ist nicht erlaubt",
            "NoAbsPath": "'{0}' ist kein absoluter Pfad",
            "Pass": "Eine Aufnahme des Fensters wurde in '{0}' gespeichert.",
            "Exception": "*ERROR* Eine Aufnahme des Fensters konnte nicht gespeichert werden\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('SaveScreenshot', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Tabellenzelle markieren') # type: ignore
    def SelectCell(self, Zeilennummer_oder_Zellinhalt: str, Spaltentitel: str): # type: ignore
        """
        Die angegebene Tabellenzelle wird markiert.
        
        | ``Tabellenzelle markieren     Positionsgeber     Spaltentitel``
        Positionsgeber: Zeilennummer oder Zellinhalt.
        """
        
        args = {
            'Zeilennummer_oder_Zellinhalt': Zeilennummer_oder_Zellinhalt,
            'Spaltentitel': Spaltentitel,
        }
        
        result = {
            "NoSession": "Keine SAP-Session vorhanden. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "NotFound": "Die Zelle mit dem Lokator '{0}, {1}' konnte nicht gefunden werden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Die Zelle mit dem Lokator '{0}' wurde markiert.",
            "Exception": "*ERROR* Die Zelle konnte nicht markiert werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('SelectCell', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Auswahlmenüeintrag auswählen') # type: ignore
    def SelectComboBoxEntry(self, Name: str, Eintrag: str): # type: ignore
        """
        Aus dem angegebenen Auswahlmenü wird der angegebene Eintrag ausgewählt.
        
        | ``Auswahlmenüeintrag auswählen    Auswahlmenü    Eintrag``
        """
        
        args = {
            'Name': Name,
            'Eintrag': Eintrag,
        }
        
        result = {
            "NoSession": "Keine SAP-Session vorhanden. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "NotFound": "Das Auswahlmenü mit dem Lokator '{0}' konnte nicht gefunden werden.\nHinweis: Prüfe die Rechtschreibung",
            "EntryNotFound": "Der Eintrag '{1}' wurde im Auswahlmenü '{0}' nicht gefunden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Der Eintrag '{1}' wurde ausgewählt.",
            "Exception": "*ERROR* Der Eintrag konnte nicht ausgewählt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('SelectComboBoxEntry', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Optionsfeld auswählen') # type: ignore
    def SelectRadioButton(self, Beschriftung_oder_Positionsgeber: str): # type: ignore
        """
        Das angegebene Optionsfeld wird ausgewählt.
        
        *Optionsfeld mit einer Beschriftung links oder rechts*
        | ``Optionsfeld auswählen    Beschriftung``
        *Optionsfeld mit einer Beschriftung oben*
        | ``Optionsfeld auswählen    @ Beschriftung``
        *Optionsfeld am Schnittpunkt einer Beschriftung links (oder rechts) und einer oben*
        | ``Optionsfeld auswählen    Beschriftung links @ Beschriftung oben``
        
        """
        
        args = {
            'Beschriftung_oder_Positionsgeber': Beschriftung_oder_Positionsgeber,
        }
        
        result = {
            "NoSession": "Keine SAP-Session vorhanden. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "NotFound": "Das Optionsfeld mit dem Lokator '{0}' konnte nicht gefunden werden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Das Optionsfeld mit dem Lokator '{0}' wurde ausgewählt.",
            "Exception": "*ERROR* Das Optionsfeld konnte nicht ausgewählt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('SelectRadioButton', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Textfeld markieren') # type: ignore
    def SelectTextField(self, Beschriftungen_oder_Inhalt: str): # type: ignore
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
        
        args = {
            'Beschriftungen_oder_Inhalt': Beschriftungen_oder_Inhalt,
        }
        
        result = {
            "NoSession": "Keine SAP-Session vorhanden. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "NotFound": "Das Textfeld mit dem Lokator '{0}' konnte nicht gefunden werden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Das Textfeld mit dem Lokator '{0}' wurde markiert.",
            "Exception": "*ERROR* Das Textfeld konnte nicht markiert werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('SelectTextField', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Textzeile markieren') # type: ignore
    def SelectTextLine(self, Inhalt: str): # type: ignore
        """
        Die Textzeile mit dem angegebenen Inhalt wird markiert.
        | ``Textzeile markieren    Inhalt``
        """
        
        args = {
            'Inhalt': Inhalt,
        }
        
        result = {
            "NoSession": "Keine SAP-Session vorhanden. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "NotFound": "Die Textzeile mit dem Inhalt '{0}' konnte nicht gefunden werden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Die Textzeile mit dem Inhalt '{0}' wurde markiert.",
            "Exception": "*ERROR* Die Textzeile konnte nicht markiert werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('SelectTextLine', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Formularfeld ankreuzen') # type: ignore
    def TickCheckBox(self, Beschriftung_oder_Positionsgeber: str): # type: ignore
        """
        Das angegebene Formularfeld wird angekreuzt.
        
        *Formularfeld mit einer Beschriftung links oder rechts *
        | ``Formularfeld ankreuzen    Beschriftung``
        *Formularfeld mit einer Beschriftung oben*
        | ``Formularfeld ankreuzen    @ Beschriftung``
        *Formularfeld am Schnittpunkt einer Beschriftung links und einer oben*
        | ``Formularfeld ankreuzen    Beschriftung links @ Beschriftung oben``
        """
        
        args = {
            'Beschriftung_oder_Positionsgeber': Beschriftung_oder_Positionsgeber,
        }
        
        result = {
            "NoSession": "Keine SAP-Session vorhanden. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "NotFound": "Das Formularfeld mit dem Lokator '{0}' konnte nicht gefunden werden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Das Formularfeld mit dem Lokator '{0}' wurde angekreuzt.",
            "Exception": "*ERROR* Das Formularfeld konnte nicht angekreuzt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('TickCheckBox', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Formularfeld abwählen') # type: ignore
    def UntickCheckBox(self, Beschriftung_oder_Positionsgeber: str): # type: ignore
        """
        Das angegebene Formularfeld wird abgewählt.
        
        *Formularfeld mit einer Beschriftung links oder rechts *
        | ``Formularfeld abwählen    Beschriftung``
        *Formularfeld mit einer Beschriftung oben*
        | ``Formularfeld abwählen    @ Beschriftung``
        *Formularfeld am Schnittpunkt einer Beschriftung links und einer oben*
        | ``Formularfeld abwählen    Beschriftung links @ Beschriftung oben``
        """
        
        args = {
            'Beschriftung_oder_Positionsgeber': Beschriftung_oder_Positionsgeber,
        }
        
        result = {
            "NoSession": "Keine SAP-Session vorhanden. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "NotFound": "Das Formularfeld mit dem Lokator '{0}' konnte nicht gefunden werden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Das Formularfeld mit dem Lokator '{0}' wurde abgewählt.",
            "Exception": "*ERROR* Das Formularfeld konnte nicht abgewählt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('UntickCheckBox', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Tabellenzelle ankreuzen') # type: ignore
    def TickCheckBoxCell(self, Zeilennummer: str, Spaltentitel: str): # type: ignore
        """
        Die angegebene Tabellenzelle wird angekreuzt.
        
        | ``Tabellenzelle ankreuzen     Zeilennummer     Spaltentitel``
        """
        
        args = {
            'Zeilennummer': Zeilennummer,
            'Spaltentitel': Spaltentitel,
        }
        
        result = {
            "NoSession": "Keine SAP-Session vorhanden. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "NotFound": "Die Zelle mit dem Lokator '{0}, {1}' konnte nicht gefunden werden.\nHinweis: Prüfe die Rechtschreibung",
            "Pass": "Die Zelle mit dem Lokator '{0}' wurde angekreuzt.",
            "Exception": "*ERROR* Die Zelle konnte nicht angekreuzt werden.\n{0}\nFür mehr Infos robot --loglevel DEBUG datei.robot ausführen und die log.html Datei durchsuchen."
        }
        return super()._run_keyword('TickCheckBoxCell', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Fenstertitel auslesen') # type: ignore
    def GetWindowTitle(self, ): # type: ignore
        """
        Der Titel des Fensters im Fordergrund wird zurückgegeben.
        
        | ``${Titel}    Fenstertitel auslesen``
        """
        
        args = {
        }
        
        result = {
            "NoSession": "Keine SAP-Session vorhanden. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "Pass": "Der Fenstertitel wurde ausgelesen"
        }
        return super()._run_keyword('GetWindowTitle', list(args.values()), dict(), result) # type: ignore
    

    @keyword('Fenstertext auslesen') # type: ignore
    def GetWindowText(self, ): # type: ignore
        """
        Der Text des Fensters im Fordergrund wird zurückgegeben.
        
        | ``${Text}    Fenstertext auslesen``
        """
        
        args = {
        }
        
        result = {
            "NoSession": "Keine SAP-Session vorhanden. Versuche zuerst das Keyword \"Verbindung zum Server Herstellen\" aufzurufen.",
            "Pass": "Der Text des Fensters wurde ausgelesen"
        }
        return super()._run_keyword('GetWindowText', list(args.values()), dict(), result) # type: ignore