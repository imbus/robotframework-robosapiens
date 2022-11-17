# RoboSAPiens: SAP GUI Automation for Humans

## Users

### Requirements

- [.NET Runtime 6.0 x86](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)


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
