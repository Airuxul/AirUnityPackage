@echo off
setlocal EnableExtensions
REM git submodule sync + update --init --recursive for packages/

cd /d "%~dp0"
git submodule sync --recursive
if errorlevel 1 exit /b 1
git submodule update --init --recursive
if errorlevel 1 (
  echo.
  echo Submodule update failed. Check .gitmodules and network, then retry.
  echo See README.md and README.zh-CN.md
  exit /b 1
)
echo.
echo Submodules ready under packages\
endlocal
exit /b 0
