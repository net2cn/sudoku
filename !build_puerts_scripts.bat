@echo off
call !generate_puerts_wrappers.bat

:: npm run build
cd ./Puer-Project
call npm run build
cd ..

pause