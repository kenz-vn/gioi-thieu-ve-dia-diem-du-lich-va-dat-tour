@echo off
chcp 65001 >nul
title DUNG HE THONG TRAVELWEB DOCKER
echo ========================================================
echo          DANG DUNG HE THONG DOCKER TRAVELWEB
echo ========================================================
echo.

:: Tu dong tim duong dan Docker tren moi may tinh
where docker >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    if exist "C:\Program Files\Docker\Docker\resources\bin\docker.exe" (
        set "PATH=C:\Program Files\Docker\Docker\resources\bin;%PATH%"
    ) else if exist "%LOCALAPPDATA%\Programs\DockerDesktop\resources\bin\docker.exe" (
        set "PATH=%LOCALAPPDATA%\Programs\DockerDesktop\resources\bin;%PATH%"
    )
)

cd /d "%~dp0"

docker compose down

echo.
echo Da dung toan bo container thanh cong!
pause
