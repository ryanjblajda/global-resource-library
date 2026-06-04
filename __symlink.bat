@echo off
set TARGET=%~dp0
set DEBUG=0
set SOURCES=D:\WORK\global-resource-library\crestron\module-library\customized-developed D:\WORK\global-resource-library\crestron\module-library\vendor-provided D:\PERSONAL\OneDrive\_PROGRAMMING\crestron\_modules

if "%DEBUG%"=="0" (
    echo Starting...
) else (
    echo Starting... ^(debug mode, no changes will be made^).
)

for %%S in (%SOURCES%) do (
    for /d %%D in ("%%S\*") do (
        echo %%D | findstr /r /c:"\\[.]" >nul || (
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

    REM Recursively find all .clz files in %%S
    for /f "delims=" %%F in ('dir /b /s "%%S\*.clz" 2^>nul') do (
        REM Skip files starting with a dot in any folder
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