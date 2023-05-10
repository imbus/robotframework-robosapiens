set windows-shell := ["pwsh.exe", "-NoProfile", "-Command"]

version := `Get-Content VERSION`

default:
    @just --list

build-exe:
    cd robosapiens; dotnet publish -c Release /property:Version={{version}}

clean:
    if (Test-Path bin) { rm -r bin }
    if (Test-Path build) { rm -r build }
    if (Test-Path obj) { rm -r obj }

build: clean build-exe
    cd python; just build
