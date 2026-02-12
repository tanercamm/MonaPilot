# MonaPilot - Tum servisleri baslatir
# Kullanim: .\start.ps1           (API + Web)
#           .\start.ps1 -All      (RabbitMQ + API + Web + Console)

param(
    [switch]$All
)

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   MonaPilot - Servisler Baslatiliyor   " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Onceki islemleri temizle (portlar mesgulse)
function Stop-PortProcess($port) {
    $proc = netstat -ano | Select-String ":$port\s+.*LISTENING" | ForEach-Object {
        ($_ -split '\s+')[-1]
    } | Select-Object -First 1
    if ($proc -and $proc -ne "0") {
        Write-Host "  Port $port mesgul (PID: $proc), durduruluyor..." -ForegroundColor Yellow
        taskkill /PID $proc /F 2>$null | Out-Null
        Start-Sleep -Seconds 1
    }
}

Stop-PortProcess 5001
Stop-PortProcess 5088

# RabbitMQ + Console (sadece -All ile)
if ($All) {
    Write-Host "[1/4] RabbitMQ baslatiliyor (Docker)..." -ForegroundColor Green
    docker-compose up -d rabbitmq
    Write-Host "       RabbitMQ hazir olana kadar bekleniyor..." -ForegroundColor Gray
    Start-Sleep -Seconds 10

    Write-Host "[2/4] Console Worker baslatiliyor..." -ForegroundColor Green
    Start-Process -NoNewWindow -FilePath "dotnet" -ArgumentList "run --project MonaPilot.Console/MonaPilot.Console.csproj" -PassThru | Out-Null
    Start-Sleep -Seconds 2
    $step = 3
    $total = 4
} else {
    $step = 1
    $total = 2
}

# API
Write-Host "[$step/$total] API baslatiliyor (http://localhost:5001)..." -ForegroundColor Green
Start-Process -NoNewWindow -FilePath "dotnet" -ArgumentList "run --project MonaPilot.API/MonaPilot.API.csproj" -PassThru | Out-Null
Start-Sleep -Seconds 3
$step++

# Web
Write-Host "[$step/$total] Web baslatiliyor (http://localhost:5088)..." -ForegroundColor Green
Start-Process -NoNewWindow -FilePath "dotnet" -ArgumentList "run --project MonaPilot.Web/MonaPilot.Web.csproj" -PassThru | Out-Null
Start-Sleep -Seconds 3

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "   Tum servisler baslatildi!            " -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "  Web UI:        http://localhost:5088" -ForegroundColor White
Write-Host "  API Explorer:  http://localhost:5001/scalar/v1" -ForegroundColor White
Write-Host "  Login:         admin / admin123" -ForegroundColor Gray
if ($All) {
    Write-Host "  RabbitMQ UI:   http://localhost:15672 (guest/guest)" -ForegroundColor White
}
Write-Host ""
Write-Host "  Durdurmak icin: .\stop.ps1" -ForegroundColor Yellow
Write-Host ""

# Tarayiciyi ac
Start-Process "http://localhost:5088"
