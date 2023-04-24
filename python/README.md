# RoboSAPiens: SAP GUI Automation for Humans

Fully localized Robot Framework library for automating the SAP GUI using text locators.

Available localizations:

- English
- German

## Requirements

- [.NET Runtime 7.0 x86](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- Scripting must be [enabled in the Application Server](https://help.sap.com/saphelp_aii710/helpdata/en/ba/b8710932b8c64a9e8acf5b6f65e740/content.htm?no_cache=true)
- Scripting must be [enabled in the SAP GUI client](https://help.sap.com/docs/sap_gui_for_windows/63bd20104af84112973ad59590645513/7ddb7c9c4a4c43219a65eee4ca8db001.html?version=760.01&locale=en-US)

## Installation

```bash
pip install robotframework-robosapiens
```

1. Download the zip file with the .NET Runtime binaries
2. Extract the `dotnet` directory from the zip file
3. Set the environment variable `DOTNET_ROOT(x86)` to the path of the `dotnet` directory

## Usage

Consult the [Documentation](https://imbus.github.io/robotframework-robosapiens/).
