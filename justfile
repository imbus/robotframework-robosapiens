set windows-shell := ["pwsh.exe", "-NoProfile", "-Command"]

version := `Get-Content VERSION`

default:
    @just --list

build32:
    cd robosapiens; dotnet publish -c Release -r win-x86 /property:Version={{version}}

build64:
    cd robosapiens; dotnet publish -c Release -r win-x64 /property:Version={{version}}

build: build32 build64
    cd python; just build
