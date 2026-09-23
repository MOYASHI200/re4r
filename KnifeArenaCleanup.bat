@echo off
setlocal EnableExtensions EnableDelayedExpansion
cd /d "%~dp0"

set "RE4DIR=C:\Program Files (x86)\Steam\steamapps\common\RESIDENT EVIL 4  BIOHAZARD RE4"

for /f "usebackq delims=" %%I in (`powershell -NoProfile -STA -Command ^
  "Add-Type -AssemblyName System.Windows.Forms; $d=New-Object System.Windows.Forms.OpenFileDialog; $d.Title='Select the BioRand generated RE4 patch PAK'; $d.Filter='RE4 patch PAK (*.pak)|*.pak|All files (*.*)|*.*'; if (Test-Path '%RE4DIR%') { $d.InitialDirectory='%RE4DIR%' }; if ($d.ShowDialog() -eq 'OK') { $d.FileName }"`) do set "INPUT=%%I"

if not defined INPUT (
  echo.
  echo No file was selected.
  pause
  exit /b 1
)

for %%I in ("%INPUT%") do (
  set "INDIR=%%~dpI"
  set "INNAME=%%~nxI"
)

set "PATCHNUM="
for /f "tokens=2 delims=_" %%N in ('echo %INNAME%^| findstr /r /c:"patch_[0-9][0-9][0-9]\.pak$"') do set "PATCHNUM=%%N"

if not defined PATCHNUM (
  for /f "tokens=6 delims=._" %%N in ("%INNAME%") do set "PATCHNUM=%%N"
)

set /a NEXT=1%PATCHNUM%-1000+1 2>nul
if errorlevel 1 (
  echo.
  echo Could not determine the patch number from:
  echo %INNAME%
  echo.
  echo Expected a name like re_chunk_000.pak.patch_007.pak
  pause
  exit /b 1
)

:find_free
set "PADDED=00%NEXT%"
set "PADDED=!PADDED:~-3!"
set "OUTPUT=%INDIR%re_chunk_000.pak.patch_!PADDED!.pak"
if exist "!OUTPUT!" (
  set /a NEXT+=1
  goto find_free
)

echo.
echo Input :
echo   %INPUT%
echo.
echo Output:
echo   !OUTPUT!
echo.
echo Building and creating Knife Arena cleanup PAK...
echo.

dotnet run --project ".\src\biorand-re4r\biorand-re4r.csproj" -c Release -- strip-arena-extras -i "%INPUT%" -o "!OUTPUT!"
if errorlevel 1 (
  echo.
  echo Cleanup failed.
  pause
  exit /b 1
)

echo.
echo ============================================
echo Done.
echo Created:
echo !OUTPUT!
echo ============================================
echo.
pause
