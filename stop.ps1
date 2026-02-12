# MonaPilot - Tum servisleri durdurur

Write-Host ""
Write-Host "MonaPilot servisleri durduruluyor..." -ForegroundColor Yellow
Write-Host ""

# dotnet islemlerini durdur
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | ForEach-Object {
    $cmdLine = (Get-CimInstance Win32_Process -Filter "ProcessId = $($_.Id)" -ErrorAction SilentlyContinue).CommandLine
    if ($cmdLine -match "MonaPilot") {
        Write-Host "  Durduruluyor: PID $($_.Id) - $cmdLine" -ForegroundColor Gray
        Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue
    }
}

# Docker RabbitMQ durdur
docker-compose down 2>$null | Out-Null

Write-Host ""
Write-Host "Tum servisler durduruldu." -ForegroundColor Green
Write-Host ""
