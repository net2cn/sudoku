@echo off
:: Generate code
echo Generating PuerTS wrappers...
"C:\Program Files\Unity\Hub\Editor\2018.4.36f1\Editor\Unity.exe" -batchmode -quit -projectPath . -executeMethod Puerts.Editor.Generator.UnityMenu.ClearAll
"C:\Program Files\Unity\Hub\Editor\2018.4.36f1\Editor\Unity.exe" -batchmode -quit -projectPath . -executeMethod Puerts.Editor.Generator.UnityMenu.GenV1