set windows-shell := ["pwsh.exe", "-nop", "-c"]

version := `Get-Content VERSION`

default:
    @just --list

build-exe:
    dotnet publish -c Release /property:Version={{version}}

clean:
    if (Test-Path bin) { rm -r bin }
    if (Test-Path build) { rm -r build }
    if (Test-Path obj) { rm -r obj }
    cd python; just clean

build: build-exe
    cd python; just build
