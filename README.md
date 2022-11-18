# RoboSAPiens: SAP GUI Automation for Humans

## Users

### Requirements

- [.NET Runtime 6.0 x86](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- Scripting must be enabled both [in the Application Server](https://help.sap.com/saphelp_aii710/helpdata/en/ba/b8710932b8c64a9e8acf5b6f65e740/content.htm?no_cache=true) and [in the SAP GUI client](https://help.sap.com/docs/sap_gui_for_windows/63bd20104af84112973ad59590645513/7ddb7c9c4a4c43219a65eee4ca8db001.html?version=760.01&locale=en-US)


### Installation

1. Download the zip file with the .NET Runtime binaries
2. Extract the dotnet directory from the zip file
3. Assign the path to this directory to the environment variable `DOTNET_ROOT(x86)`
4. Install the Python wheel

    ```powershell
    pip install python\dist\robotframework_robosapiens-xxx.whl
    ```


## Developers

### Requirements

- [.NET SDK 6.0 x64](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

### Compile RoboSAPiens.exe

```powershell
build.bat
```

### Build the Python wheel

```powershell
python\build.bat
```

## References

- [SAP GUI Scripting API Documentation](https://help.sap.com/docs/sap_gui_for_windows/b47d018c3b9b45e897faf66a6c0885a8/babdf65f4d0a4bd8b40f5ff132cb12fa.html?locale=en-US)
