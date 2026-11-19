@echo off
chcp 65001 >nul
echo ========================================================
echo  KHOI CHAY TRAVELWEB VOI DOCKER DESKTOP
echo ========================================================
echo.

:: Kiem tra Docker daemon co dang chay khong
docker info >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo [!] Docker Desktop chua bat hoac dang khoi dong.
    echo [*] Dang tim va kich hoat Docker Desktop...
    if exist "%LOCALAPPDATA%\Programs\DockerDesktop\Docker Desktop.exe" (
        start "" "%LOCALAPPDATA%\Programs\DockerDesktop\Docker Desktop.exe"
    ) else if exist "C:\Program Files\Docker\Docker\Docker Desktop.exe" (
        start "" "C:\Program Files\Docker\Docker\Docker Desktop.exe"
    ) else (
        echo [!] Vui long tu mo Docker Desktop tu Start Menu...
    )

    echo [*] Dang cho Docker Desktop khoi dong hoan tat...
    :wait_docker
    timeout /t 3 >nul
    docker info >nul 2>&1
    if errorlevel 1 (
        echo [*] Docker van dang khoi dong, vui long doi giay lat...
        goto wait_docker
    )
    echo [*] Docker Desktop da san sang!
)

echo.
echo [*] Dang build va khoi chay cac container (Web + SQL Server)...
docker compose up -d --build

if %ERRORLEVEL% equ 0 (
    echo.
    echo ========================================================
    echo  THANH CONG! He thong da san sang:
    echo  - Trang web TravelWeb:  http://localhost:5000
    echo  - SQL Server Container: localhost,14333 (sa / TravelWeb@2026!)
    echo ========================================================
    echo.
    echo [*] Dang doi ung dung khoi tao CSDL va mo trinh duyet...
    timeout /t 5 >nul
    start http://localhost:5000
) else (
    echo.
    echo [X] Co loi xay ra khi chay docker compose.
)
pause
