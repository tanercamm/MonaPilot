@echo off
echo ===================================
echo MonaPilot - Startup Script
echo ===================================
echo.

echo 1. RabbitMQ'yu kontrol et ve baþlat...
docker-compose up -d

echo.
echo 2. RabbitMQ hazýrlanýyor (10 saniye bekle)...
timeout /t 10 /nobreak

echo.
echo 3. Web API'yi baþlat...
start "MonaPilot Web API" cmd /k "cd MonaPilot.API && dotnet run"

echo.
echo 4. Console Application'ý baþlat...
timeout /t 5 /nobreak
start "MonaPilot Console" cmd /k "cd MonaPilot.Console && dotnet run"

echo.
echo ===================================
echo Tüm servisler baþlatýlýyor...
echo.
echo Web API: https://localhost:5001
echo RabbitMQ Management: http://localhost:15672
echo API Documentation: https://localhost:5001/openapi/v1.json
echo ===================================
