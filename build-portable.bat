@echo off
setlocal
cd /d "%~dp0"
dotnet publish PersianScreenTranslator.csproj -c Release -r win-x64 --self-contained true ^
  /p:PublishSingleFile=true ^
  /p:IncludeNativeLibrariesForSelfExtract=true ^
  /p:EnableCompressionInSingleFile=true ^
  -o "%~dp0publish"
if errorlevel 1 (
  echo.
  echo Build failed. Install .NET 8 SDK on the build computer and try again.
  pause
  exit /b 1
)
echo.
echo Portable build created in the publish folder.
pause
