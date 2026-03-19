#!/bin/bash

echo "======================================"
echo "Creative Agency Setup Script"
echo "======================================"
echo ""

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "Error: Docker is not running. Please start Docker Desktop first."
    exit 1
fi

# Start Docker containers
echo "Starting MySQL and phpMyAdmin containers..."
docker-compose up -d

echo ""
echo "Waiting for MySQL to be ready..."
sleep 10

# Check if .NET SDK is installed
if ! command -v dotnet &> /dev/null; then
    echo "Error: .NET SDK is not installed. Please install .NET 8.0 SDK first."
    exit 1
fi

# Restore NuGet packages
echo ""
echo "Restoring NuGet packages..."
dotnet restore

# Install EF Core tools if not already installed
echo ""
echo "Checking Entity Framework Core tools..."
dotnet tool install --global dotnet-ef 2>/dev/null || dotnet tool update --global dotnet-ef

# Apply database migrations
echo ""
echo "Applying database migrations..."
cd CreativeAgency.Web
dotnet ef database update

if [ $? -eq 0 ]; then
    echo ""
    echo "======================================"
    echo "Setup completed successfully!"
    echo "======================================"
    echo ""
    echo "Database Information:"
    echo "  - MySQL Server: localhost:3306"
    echo "  - Database: creativeagency_db"
    echo "  - Username: agency_user"
    echo "  - Password: agency_pass"
    echo ""
    echo "phpMyAdmin Access:"
    echo "  - URL: http://localhost:8080"
    echo "  - Username: root"
    echo "  - Password: root123"
    echo ""
    echo "Default Application Login:"
    echo "  - Username: admin"
    echo "  - Password: admin123"
    echo ""
    echo "SOAP Service:"
    echo "  - Endpoint: https://localhost:5001/ProjectService.asmx"
    echo ""
    echo "To start the application, run:"
    echo "  cd CreativeAgency.Web"
    echo "  dotnet run"
    echo ""
    echo "The application will be available at:"
    echo "  - https://localhost:5001"
    echo "  - http://localhost:5000"
    echo ""
else
    echo ""
    echo "Error: Database migration failed!"
    echo "Please check the error messages above."
    exit 1
fi
