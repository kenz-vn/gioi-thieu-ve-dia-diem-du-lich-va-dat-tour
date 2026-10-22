@echo off
chcp 65001 >nul
title KHOI DONG HE THONG TRAVELWEB BANG DOCKER
echo ========================================================
echo       DANG KHOI DONG TRAVELWEB VA SQL SERVER BANG DOCKER
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

echo [1/3] Kiem tra Docker Engine...
docker info >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [Loi] May tinh cua ban chua mo Docker Desktop!
    echo Vui long:
    echo  1. Cai dat Docker Desktop neu chua co.
    echo  2. Bat Docker Desktop len cho den khi bieu tuong Docker chuyen sang mau xanh la.
    echo  3. Sau do chay lai file nay.
    pause
    exit /b 1
)

echo [2/3] Dang build va chay cac container (Website + SQL Server)...
docker compose up -d

echo.
echo [3/3] He thong dang hoat dong tai dia chi:
echo   --^> Website:    http://localhost:5230
echo   --^> SQL Server: localhost,1433 (sa / YourStrong@Password123)
echo.
echo ========================================================
echo Hoan tat! Nhan phim bat ky de mo trinh duyet...
pause >nul
start http://localhost:5230
