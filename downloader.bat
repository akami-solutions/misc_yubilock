@echo off
setlocal

if "%1" == "status=enabled" (
    rem Enable YubiLock Service
    sc.exe config "YubiLock Service" start=auto
    sc.exe start "YubiLock Service"
    echo YubiLock Service enabled successfully!
    goto end
) else if "%1" == "status=disabled" (
    rem Disable YubiLock Service
    sc.exe config "YubiLock Service" start=disabled
    sc.exe stop "YubiLock Service"
    echo YubiLock Service disabled successfully!
    goto end
) else (
    net session >nul 2>&1
    if %errorLevel% == 0 (
        sc.exe query "YubiLock Service" >nul 2>&1
        if %errorLevel% == 0 (
            echo Updating YubiLock
            sc.exe stop "YubiLock Service"
            sc.exe delete "YubiLock Service"
            del "%APPDATA%\YubiLock\YubiLock.exe"
            goto installer
        ) else (
            goto installer
        )
    ) else (
        echo Windows Services cannot be installed without administrator privileges. Please run this script as administrator.
        goto end
    )
)

:installer
cd $env:TEMP
curl -Lo YubiLock.exe https://github.com/Satowa-Network/yubiLock/releases/latest/download/YubiLock.exe
if %errorLevel% == 0 (
    md "%APPDATA%\YubiLock"
    copy /Y YubiLock.exe "%APPDATA%\YubiLock"
    sc.exe create "YubiLock Service" binpath="%APPDATA%\YubiLock\YubiLock.exe" start=delayed-auto
    del YubiLock.exe
    echo YubiLock installed successfully!
) else (
    echo Failed to download YubiLock. Please try again later.
)

:end
pause
