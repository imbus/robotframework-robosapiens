@echo off

@REM Create .json-File with Informations for Keyword-Interface
pushd src\RoboSAPiens\lib
RoboSAPiens.exe --export-cli cli.json
move cli.json ..
popd

pushd src\RoboSAPiens
@REM Generate Keyword-Interface
python cligen.py

@REM Type-Check python code
mypy --ignore-missing-imports .
popd

@REM pip install build
python -m build --wheel