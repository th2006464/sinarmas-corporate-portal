@echo off
echo Killing leftover processes...
taskkill /f /im Portal.exe 2>nul
taskkill /f /im dotnet.exe 2>nul
echo Done
pause
