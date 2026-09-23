@echo off
setlocal EnableExtensions
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

for /f "usebackq delims=" %%O in (`powershell -NoProfile -Command ^
  "$p=$env:INPUT; $dir=Split-Path -LiteralPath $p; $name=Split-Path -Leaf $p; $m=[regex]::Match($name,'^(?<prefix>.+\.patch_)(?<n>\d{3})(?<suffix>\.pak)$'); if (-not $m.Success) { exit 2 }; $n=[int]$m.Groups['n'].Value + 1; do { $candidate=Join-Path $dir ($m.Groups['prefix'].Value + ('{0:D3}' -f $n) + $m.Groups['suffix'].Value); $n++ } while (Test-Path -LiteralPath $candidate); $candidate"`) do set "OUTPUT=%%O"

if not defined OUTPUT (
  echo.
  echo Could not determine the next patch number.
  echo Expected a name like:
  echo re_chunk_000.pak.patch_007.pak
  pause
  exit /b 1
)

echo.
echo Input:
echo   %INPUT%
echo.
echo Output:
echo   %OUTPUT%
echo.
echo Creating Knife Arena cleanup PAK...
echo.

dotnet run --project ".\src\biorand-re4r\biorand-re4r.csproj" -c Release -- strip-arena-extras -i "%INPUT%" -o "%OUTPUT%"
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
echo %OUTPUT%
echo ============================================
echo.
pause
