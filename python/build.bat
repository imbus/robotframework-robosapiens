@echo off

@REM Create .json file with the specification
pushd src\RoboSAPiens\lib
RoboSAPiens.exe --export-api api.json
move api.json ..
popd

pushd src\RoboSAPiens
@REM Update the English schema and the schema of the translations
python schemagen.py api.json
python schemagen.py api.json i18n
rm api.json
popd

@REM Type-Check python code
mypy .

@REM Generate the Python libraries
pushd src\RoboSAPiens
python libgen.py en
python libgen.py de
popd

@REM pip install build
python -m build --wheel