@echo off
call !generate_puerts_wrappers.bat

echo Please be aware that this batch script ONLY watch your changes located in ".\Puer-Project\src"!
echo You may need to manually invoke "Generate (all in one)" from file menu bar "PuerTS" in Unity Editor if you found some bindings missing.

:: npm run develop
cd ./Puer-Project
call npm run develop
cd ..

pause