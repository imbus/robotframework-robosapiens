@echo off

dotnet publish -c release -r win-x86 -p:PublishSingleFile=true -p:PublishTrimmed=true
copy .\bin\Debug\netcoreapp5.0\RoboSAPiens.html .\bin\release\netcoreapp5.0\win-x86\publish
warp-packer.exe --arch windows-x86 --input_dir .\bin\release\netcoreapp5.0\win-x86\publish --exec RoboSAPiens.exe --output RoboSAPiens.exe
ResourceHacker.exe -open RoboSAPiens.exe -save RoboSAPiens.exe -action addskip -res RoboSAPiens.ico -mask ICONGROUP,MAIN,