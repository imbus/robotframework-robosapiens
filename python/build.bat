@echo off

FOR /F %%n IN (..\VERSION) DO set version=%%n
echo __version__ = '%version%' > src/version.py

@REM Create .json file with the specification
pushd src
RoboSAPiens\lib\RoboSAPiens.exe --export-api %~dp0\src\api.json

@REM Update the English schema and the schema of the translations
python schemagen.py api.json
python schemagen.py api.json i18n
del api.json

@REM Generate the Python libraries
python libgen.py en
python libgen.py de

@REM Type-Check python code
mypy .
popd

@REM Generate the documentation
python localizedoc.py ../docs/RoboSAPiens.DE.html

@REM pip install build
python -m build --wheel