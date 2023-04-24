@echo off

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
libdoc -P .\src RoboSAPiens docs/RoboSAPiens.html 
libdoc -P .\src RoboSAPiens.DE docs/RoboSAPiens.DE.html 

@REM pip install build
python -m build --wheel