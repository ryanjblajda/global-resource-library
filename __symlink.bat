@echo off
set "TARGET=%~dp0"
set DEBUG=0
set "SOURCES=C:\path\to\library\root"

if "%DEBUG%"=="0" (
    echo Starting...
) else (
    echo Starting... ^(debug mode, no changes will be made^).
)

:: Use 'dir /ad /b /s' to recursively find EVERY subfolder down the entire chain
for /f "delims=" %%D in ('dir /b /ad /s "%SOURCES%" 2^>nul') do (
    
    :: Skip any hidden system or dot folders (like .git)
    echo %%D | findstr /r /c:"\\[.]" >nul || (
        
        :: Case 1: If a subfolder has an explicitly named '_modules' folder inside it
        if exist "%%D\_modules" (
            for %%F in ("%%D\_modules\*.ush" "%%D\_modules\*.usp" "%%D\_modules\*.umc" "%%D\_modules\*.um2" "%%D\_modules\*.clz" "%%D\_modules\*.ir" "%%D\_modules\*.pdf") do (
                echo %%~nxF | findstr /r /c:"^[.]" >nul || (
                    if exist "%TARGET%%%~nxF" (
                        REM echo CONFLICT: %%~nxF
                    ) else (
                        echo LINK: "%TARGET%%%~nxF" ^<-- "%%F"
                        if "%DEBUG%"=="0" mklink "%TARGET%%%~nxF" "%%F"
                    )
                )
            )
        ) else (
            :: Case 2: Scan for loose files inside the folder, but skip double-processing if the folder itself is named '_modules'
            if /i not "%%~nxD"==_modules (
                for %%F in ("%%D\*.ush" "%%D\*.usp" "%%D\*.umc" "%%D\*.um2" "%%D\*.clz" "%%D\*.ir" "%%D\*.pdf") do (
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
    )
)

echo.
if "%DEBUG%"=="0" (
    echo Done.
) else (
    echo Done ^(debug only, nothing was created^).
)

pause