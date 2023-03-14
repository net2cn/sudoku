@echo off
:: npm init
cd .\Puer-Project
if not exist .\node_modules\ (
    echo PuerTS development environment has not been setup. Initializing...
    call npm init -y
    call npm install
    echo PuerTS development environment has been initialized.
) else (
    echo PuerTS development environment has been setup.
)
echo Your TypeScript scripts should be located in ".\Puer-Project\src".
cd ..

echo Building TypeScript project...
call !build_puerts_scripts.bat

pause