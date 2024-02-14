set windows-shell := ["pwsh.exe", "-NoProfile", "-Command"]

version := `Get-Content VERSION`

default:
    @just --list

build-exe:
    cd robosapiens; dotnet publish -c Release /property:Version={{version}}

export-api:
    #!pwsh
    cd ExportApi; dotnet run
    cp api.json ../python/src

clean:
    if (Test-Path bin) { rm -r bin }
    if (Test-Path build) { rm -r build }
    if (Test-Path obj) { rm -r obj }

build: clean build-exe export-api
    cd python; just build
