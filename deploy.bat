@echo off

set OUTDIR=..\build
rmdir /S /Q %OUTDIR%

dotnet publish -c Release -p:PublishDir=%OUTDIR%
