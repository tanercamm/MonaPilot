# ?? MONAPILOT - TEMIZ BAÞLAT KILAVUZU

## ? Sistem Özeti

- **Web API**: ASP.NET Core 10.0 (HTTPS Port 5001)
- **Console App**: .NET 8.0 (RabbitMQ Consumer)
- **Database**: SQLite (monapilot.db)
- **Message Queue**: RabbitMQ (localhost:5672)

---

## ?? BAÞLAMAK ÝÇÝN (4 TERMINAL)

### **TERMINAL 1: RabbitMQ Baþlat**

```sh
docker-compose up -d
```

Kontrol:
```sh
docker ps | grep rabbitmq
```

? Beklenen: Container çalýþýyor

---

### **TERMINAL 2: Web API Baþlat**

```sh
cd MonaPilot.API
dotnet run
```

? Beklenen Çýktý:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to exit.
```

---

### **TERMINAL 3: Console App Baþlat**

```sh
cd MonaPilot.Console
dotnet run
```

? Beklenen Çýktý:
```
[timestamp] [Information] === MonaPilot Console Uygulamasý Baþlatýlýyor ===
[timestamp] [Information] ? RabbitMQ'ya baðlanýldý
[timestamp] [Information] Kuyruk dinleniyor: product-requests
```

**ÖNEMLÝ**: Bu terminal AÇIK TUTULMALI (kapatma!)

---

### **TERMINAL 4: Test Ýstekleri Gönder**

Proje klasörüne git:
```sh
cd C:\Users\taner\source\repos\MonaPilot
```

#### **Seçenek A: Tek Ýstek (Basit Test)**

Dosya: `test-api.ps1` - Aþaðýdaki kodu Notepad'e kopyala ve kaydet:

```powershell
# SSL/Certificate problemi çöz
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $true }

$baseUrl = "https://localhost:5001/api/productrequest/request"

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "API'YE TALEP GÖNDER" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

$body = @{
    personName = "Ahmet Yilmaz"
    budget = 900
    productType = "Electronics"
}

$jsonBody = $body | ConvertTo-Json

Write-Host "Gönderilen Veri:" -ForegroundColor Yellow
Write-Host $jsonBody
Write-Host ""

try {
    $response = Invoke-WebRequest -Uri $baseUrl -Method POST -ContentType "application/json" -Body $jsonBody

    Write-Host "? BAÞARILI! API Cevapladý:" -ForegroundColor Green
    Write-Host ""
    Write-Host ($response.Content | ConvertFrom-Json | ConvertTo-Json) -ForegroundColor Green
    Write-Host ""
    Write-Host "? 3 saniye bekle ve Console'da 'SATIÞLAÞTIRMA' yazýsýný ara!" -ForegroundColor Yellow
}
catch {
    Write-Host "? HATA: $_" -ForegroundColor Red
}
```

Çalýþtýr:
```powershell
.\test-api.ps1
```

---

#### **Seçenek B: 5 Ýstek (Hýzlý Test)**

Dosya: `test-5-istek.ps1` - Aþaðýdaki kodu Notepad'e kopyala ve kaydet:

```powershell
# SSL/Certificate problemi çöz
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $true }

$baseUrl = "https://localhost:5001/api/productrequest/request"

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "5 ÝSTEK GÖNDER" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

$testler = @(
    @{ ad = "Ali Demir"; butce = 850; tur = "Electronics" },
    @{ ad = "Ayþe Kaya"; butce = 350; tur = "Furniture" },
    @{ ad = "Murat Güzel"; butce = 100; tur = "Books" },
    @{ ad = "Zeynep Can"; butce = 500; tur = "Electronics" },
    @{ ad = "Fatih Yýldýz"; butce = 450; tur = "Furniture" }
)

foreach ($test in $testler) {
    Write-Host "?? Ýstek: $($test.ad)" -ForegroundColor Yellow
    
    $body = @{
        personName = $test.ad
        budget = $test.butce
        productType = $test.tur
    } | ConvertTo-Json

    try {
        $response = Invoke-WebRequest -Uri $baseUrl -Method POST -ContentType "application/json" -Body $body
        $data = $response.Content | ConvertFrom-Json
        Write-Host "   ? ID: $($data.id)" -ForegroundColor Green
    }
    catch {
        Write-Host "   ? Hata: $_" -ForegroundColor Red
    }
    
    Start-Sleep -Milliseconds 300
}

Write-Host ""
Write-Host "? Tüm istekler tamamlandý!" -ForegroundColor Green
```

Çalýþtýr:
```powershell
.\test-5-istek.ps1
```

---

#### **Seçenek C: Tüm Log'larý Görmek**

Dosya: `test-logs-goster.ps1` - Aþaðýdaki kodu Notepad'e kopyala ve kaydet:

```powershell
# SSL/Certificate problemi çöz
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $true }

$url = "https://localhost:5001/api/productrequest/logs/all"

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "TÜM LOG'LARI GÖRÜNTÜLE" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

try {
    $response = Invoke-WebRequest -Uri $url
    $logs = $response.Content | ConvertFrom-Json
    
    Write-Host "?? Toplam Ýþlem: $($logs.Count)" -ForegroundColor Yellow
    Write-Host ""
    
    foreach ($log in $logs) {
        Write-Host "?? $($log.personName) ? ?? $($log.productName) (?$($log.productPrice))" -ForegroundColor Cyan
    }
}
catch {
    Write-Host "? HATA: $_" -ForegroundColor Red
}
```

Çalýþtýr:
```powershell
.\test-logs-goster.ps1
```

---

## ?? RabbitMQ Management (Opsiyonel)

Browser aç: http://localhost:15672

Login: `guest` / `guest`

Queues ? product-requests ? Ýstatistiklerini gör

---

## ?? SORUN ÇÖZMEK

### Sorun: "RabbitMQ'ya baðlanamadý"

```sh
docker-compose up -d
docker ps
```

### Sorun: "Port 5001 zaten kullanýmda"

```sh
# Eski API process'ini kapat
Get-Process dotnet | Stop-Process -Force

# Yeniden baþlat
cd MonaPilot.API && dotnet run
```

### Sorun: PowerShell scripti çalýþmýyor

```powershell
# Execution policy deðiþtir
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

# Tekrar dene
.\test-api.ps1
```

---

## ?? BAÞARILI TEST AKIÞI

```
Terminal 4:  .\test-5-istek.ps1
     ?
Terminal 2:  5 istek kaydedildi, 5 event yayýnlandý
     ?
RabbitMQ:    5 mesaj kuyruða eklendi
     ?
Terminal 3:  ? 5 satýþ iþlemi baþarýlý
     ?
Terminal 4:  .\test-logs-goster.ps1
     ?
Sonuç:       5 ActivityLog kaydý görüntülenir ?
```

---

## ?? KAPAMAK

Terminal'lerde **Ctrl+C** tuþu

```sh
docker-compose down
```

---

**Baþarýlar! ??**
