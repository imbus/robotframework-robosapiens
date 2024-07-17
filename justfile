set windows-shell := ["pwsh.exe", "-NoProfile", "-Command"]

version := `Get-Content VERSION`

default:
    @just --list

build-exe:
    cd robosapiens; dotnet publish RoboSAPiensx86.csproj -c Release /property:Version={{version}}
    cd robosapiens; dotnet publish RoboSAPiensx64.csproj -c Release /property:Version={{version}}

export-api:
    #!pwsh
    cd ExportApi; dotnet run
    cp api.json ../python/src

build: build-exe export-api
    cd python; just build
