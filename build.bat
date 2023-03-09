@echo off

FOR /F %%n IN (VERSION) DO set version=%%n

dotnet publish -c Release /property:Version=%version%