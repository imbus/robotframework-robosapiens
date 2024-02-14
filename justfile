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
    if (Test-Path robosapiens/bin) { rm -r robosapiens/bin }
    if (Test-Path robosapiens/build) { rm -r robosapiens/build }
    if (Test-Path robosapiens/obj) { rm -r robosapiens/obj }

build: clean build-exe export-api
    cd python; just build
