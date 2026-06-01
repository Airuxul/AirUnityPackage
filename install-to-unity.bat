@echo off
setlocal EnableExtensions
REM Install missing Air UPM packages into Unity Packages/manifest.json
REM Usage: install-to-unity.bat [UnityProject]

set "META_ROOT=%~dp0"
if "%META_ROOT:~-1%"=="\" set "META_ROOT=%META_ROOT:~0,-1%"

if not "%~1"=="" set "UNITY_PROJECT=%~1"

if defined UNITY_PROJECT (
  powershell -NoProfile -ExecutionPolicy Bypass -File "%META_ROOT%\tools\install-packages.ps1" -MetaRoot "%META_ROOT%" -UnityProject "%UNITY_PROJECT%"
) else (
  powershell -NoProfile -ExecutionPolicy Bypass -File "%META_ROOT%\tools\install-packages.ps1" -MetaRoot "%META_ROOT%"
)
set "EXIT_CODE=%ERRORLEVEL%"
if %EXIT_CODE% neq 0 (
  echo.
  echo Failed. Examples:
  echo   install-to-unity.bat
  echo   install-to-unity.bat C:\Project\GameDemo
  exit /b %EXIT_CODE%
)
endlocal
exit /b 0
