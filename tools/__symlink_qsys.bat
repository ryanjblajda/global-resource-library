@echo off
set TARGET=%~dp0
set DEBUG=0
set SOURCES="path\to\global-resource-library\q-sys"

if "%DEBUG%"=="0" (
    echo Starting...
) else (
    echo Starting... ^(debug mode, no changes will be made^).
)

for %%S in (%SOURCES%) do (
    REM Recursively find all .qplug files in %%S
    for /f "delims=" %%F in ('dir /b /s "%%~S\*.qplug" 2^>nul') do (
        REM Skip files in folders starting with a dot
        echo %%F | findstr /r /c:"\\[.]" >nul || (
            REM Skip files whose name starts with a dot
            echo %%~nxF | findstr /r /c:"^[.]" >nul || (
                if exist "%TARGET%%%~nxF" (
                    REM echo CONFLICT: %%~nxF
                ) else (
                    echo LINK: "%TARGET%%%~nxF" ^<-- "%%F"
                    if "%DEBUG%"=="0" mklink "%TARGET%%%~nxF" "%%F"
                )
            )
        )
    )
)

echo.
if "%DEBUG%"=="0" (
    echo Done.
) else (
    echo Done ^(debug only, nothing was created^).
)

pause