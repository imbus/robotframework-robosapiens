# RoboSAPiens: SAP GUI-Automatisierung für Menschen

## Voraussetzungen für die Entwicklung

| Programm                  | Quelle                                                                                              | Wofür               |
|---------------------------|------------------------------------------                                                           |---------------------|
| .NET 6.0 für x86 (32-bit) | https://dotnet.microsoft.com/en-us/download                                                         | dotnet build/run    |
| SAP GUI Scripting API     | C:\Program Files (x86)\SAP\FrontEnd\SapGui                                                          | SAP Automatisierung |
| tlbimp.exe                | [.NET Framework 4.8 Developer Pack](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks) | COM -> .NET         |
| Robot Framework           | pip install robotframework                                                                          | libdoc              |
| warp-packer               | warp-packer.exe                                                                                     | Selbstständige .exe |
| ResourceHacker            | ResourceHacker.exe                                                                                  | Icon                |

**Hinweis**

Die [SAP GUI Scripting API](https://help.sap.com/viewer/b47d018c3b9b45e897faf66a6c0885a8/770.01/en-US/babdf65f4d0a4bd8b40f5ff132cb12fa.html) wird von SAP als COM-Bibliothek geliefert. Um diese API mit .NET benutzen zu können, muss eine .NET-Bibliothek aus der COM-Bibliothek generiert werden:

1. PowerShell im Verzeichnis `robotframework-robosapiens` ausführen.
2. `tblimp C:\Program Files (x86)\SAP\FrontEnd\SapGui\sapfewse.ocx /out:SAPFEWSELib.dll` ausführen
3. `tblimp C:\Program Files (x86)\SAP\FrontEnd\SapGui\SAPROTWR.DLL /out:SAPROTWR.NET.DLL` ausführen

Wenn die SAP GUI über diese API gesteuert wird, können die GUI Elementen mit dem SAP Scripting Tracker untersucht werden.


## Herstellung des RoboSAPiens.exe

Zuerst muss die Dokumentation in HTML-Format generiert werden:

1. PowerShell im Verzeichnis `robotframework-robosapiens` ausführen.
2. `dotnet run` aufrufen.
3. Ein zweites PowerShell im Verzeichnis `robotframework-robosapiens` ausführen.
4. `.\docgen` ausführen. Wenn der Prozess fertig ist PowerShell beenden.

### Kompilierung und Verpackung

1. Auf dem ersten PowerShell `.\deploy` ausführen.
2. Nach ein paar Minuten wird **RoboSAPiens.exe** im Verzeichnis `robotframework-robosapiens` abgelegt.
