set windows-shell := ["pwsh.exe", "-NoProfile", "-Command"]

version := `Get-Content VERSION`

default:
    @just --list

build32:
    cd robosapiens; dotnet publish RoboSAPiensx86.csproj -c Release /property:Version={{version}}

build64:
    cd robosapiens; dotnet publish RoboSAPiensx64.csproj -c Release /property:Version={{version}}

build: build32 build64
    cd python; just build
