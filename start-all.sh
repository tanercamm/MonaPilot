#!/bin/bash

echo "==================================="
echo "MonaPilot - Startup Script"
echo "==================================="
echo ""

echo "1. RabbitMQ'yu kontrol et ve baþlat..."
docker-compose up -d

echo ""
echo "2. RabbitMQ hazýrlanýyor (10 saniye bekle)..."
sleep 10

echo ""
echo "3. Web API'yi baþlat..."
cd MonaPilot.API
dotnet run &
API_PID=$!

echo ""
echo "4. Console Application'ý baþlat..."
sleep 5
cd ../MonaPilot.Console
dotnet run &
CONSOLE_PID=$!

echo ""
echo "==================================="
echo "Tüm servisler baþlatýlýyor..."
echo ""
echo "Web API: https://localhost:5001"
echo "RabbitMQ Management: http://localhost:15672"
echo "API Documentation: https://localhost:5001/openapi/v1.json"
echo ""
echo "PID'leri: API=$API_PID, Console=$CONSOLE_PID"
echo "==================================="

wait
