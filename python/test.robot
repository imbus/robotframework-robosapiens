*** Settings ***
Library    RoboSAPiens.DE    x64=True

*** Test Cases ***
Erster Test
    SAP starten                         C:\\Program Files\\SAP\\FrontEnd\\SAPGUI\\saplogon.exe
    Verbindung Zum Server Herstellen    Demo Server
