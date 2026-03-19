@echo off
echo ======================================
echo Creative Agency Setup Script
echo ======================================
echo.

REM Check if Docker is running
docker info >nul 2>&1
if errorlevel 1 (
    echo Error: Docker is not running. Please start Docker Desktop first.
    exit /b 1
)

REM Start Docker containers
echo Starting MySQL and phpMyAdmin containers...
docker-compose up -d

echo.
echo Waiting for MySQL to be ready...
timeout /t 10 /nobreak >nul

REM Check if .NET SDK is installed
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo Error: .NET SDK is not installed. Please install .NET 8.0 SDK first.
    exit /b 1
)

REM Restore NuGet packages
echo.
echo Restoring NuGet packages...
dotnet restore

REM Install EF Core tools if not already installed
echo.
echo Checking Entity Framework Core tools...
dotnet tool install --global dotnet-ef 2>nul
if errorlevel 1 (
    dotnet tool update --global dotnet-ef
)

REM Apply database migrations
echo.
echo Applying database migrations...
cd CreativeAgency.Web
dotnet ef database update

if %errorlevel% equ 0 (
    echo.
    echo ======================================
    echo Setup completed successfully!
    echo ======================================
    echo.
    echo Database Information:
    echo   - MySQL Server: localhost:3306
    echo   - Database: creativeagency_db
    echo   - Username: agency_user
    echo   - Password: agency_pass
    echo.
    echo phpMyAdmin Access:
    echo   - URL: http://localhost:8080
    echo   - Username: root
    echo   - Password: root123
    echo.
    echo Default Application Login:
    echo   - Username: admin
    echo   - Password: admin123
    echo.
    echo SOAP Service:
    echo   - Endpoint: https://localhost:5001/ProjectService.asmx
    echo.
    echo To start the application, run:
    echo   cd CreativeAgency.Web
    echo   dotnet run
    echo.
    echo The application will be available at:
    echo   - https://localhost:5001
    echo   - http://localhost:5000
    echo.
) else (
    echo.
    echo Error: Database migration failed!
    echo Please check the error messages above.
    exit /b 1
)
