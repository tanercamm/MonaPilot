# MonaPilot - Kurulum ve Çalýþma Kýlavuzu

## ?? Hýzlý Baþlangýç (Quick Start)

### Windows'ta:
```batch
# 1. Docker'ýn kurulu olduðundan emin olun
# 2. Proje klasöründe:
start-all.bat
```

### Linux/Mac'ta:
```bash
# 1. Docker'ýn kurulu olduðundan emin olun
# 2. Proje klasöründe:
chmod +x start-all.sh
./start-all.sh
```

---

## ?? Manuel Kurulum Adýmlarý

### 1. RabbitMQ'yu Baþlat

**Docker ile (Önerilen):**
```bash
docker-compose up -d
```

**Kontrol et:**
```bash
docker ps | grep rabbitmq
```

**Yönetim Paneli:** http://localhost:15672 (guest/guest)

---

### 2. Web API'yi Çalýþtýr

Terminal 1 açýn:
```bash
cd MonaPilot.API
dotnet run
```

**Beklenen Output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to exit.
```

---

### 3. Console Application'ý Çalýþtýr

Terminal 2 açýn (Web API çalýþýrken):
```bash
cd MonaPilot.Console
dotnet run
```

**Beklenen Output:**
```
[2024-02-09 20:00:00] [Information] MonaPilot.Console: === MonaPilot Console Uygulamasý Baþlatýlýyor ===
[2024-02-09 20:00:00] [Information] MonaPilot.Console: RabbitMQ'ya baðlanýldý
[2024-02-09 20:00:00] [Information] MonaPilot.Console: Kuyruk dinleniyor: product-requests
```

---

## ?? Test Etme

### PowerShell ile Curl Ýsteði:

Terminal 3 açýn (API çalýþýrken):

```powershell
$body = @{
    personName = "Murat Aydýn"
    budget = 1500
    productType = "Electronics"
} | ConvertTo-Json

$response = Invoke-WebRequest `
    -Uri "https://localhost:5001/api/productrequest/request" `
    -Method POST `
    -ContentType "application/json" `
    -Body $body `
    -SkipCertificateCheck

Write-Output $response.Content | ConvertFrom-Json | ConvertTo-Json
```

### Curl ile (Linux/Mac):

```bash
curl -X POST https://localhost:5001/api/productrequest/request \
  -H "Content-Type: application/json" \
  -d '{
    "personName": "Murat Aydýn",
    "budget": 1500,
    "productType": "Electronics"
  }' \
  --insecure
```

### Visual Studio Code REST Client:

`requests.http` dosyasýný açýp, her test için "Send Request" butonuna týklayýn.

---

## ?? Test Senaryolarý

### Senaryo 1: Baþarýlý Satýþ
**Input:**
```json
{
  "personName": "Zeynep Koç",
  "budget": 850,
  "productType": "Electronics"
}
```

**Beklenen Çýktý (Web API):**
```
[INFO] Yeni talep kaydedildi - ID: 1, Kiþi: Zeynep Koç, Bütçe: 850, Tür: Electronics
[INFO] Event yayýnlandý - Request ID: 1
```

**Beklenen Çýktý (Console):**
```
[Event Alýndý] Request ID: 1, Kiþi: Zeynep Koç, Bütçe: 850, Tür: Electronics
[SATIÞLAÞTIRMA] Kiþi: Zeynep Koç, Ürün: Tablet, Fiyat: 500, Kalan Stok: 7
```

---

### Senaryo 2: Bütçe Yetersiz
**Input:**
```json
{
  "personName": "Emre Yýldýz",
  "budget": 200,
  "productType": "Electronics"
}
```

**Beklenen Çýktý (Console):**
```
[UYARI] Emre Yýldýz için uygun ürün bulunamadý - Bütçe: 200, Tür: Electronics
```

---

### Senaryo 3: Yanlýþ Ürün Tipi
**Input:**
```json
{
  "personName": "Selin Kaya",
  "budget": 500,
  "productType": "Sports"
}
```

**Beklenen Çýktý (Console):**
```
[UYARI] Selin Kaya için uygun ürün bulunamadý - Bütçe: 500, Tür: Sports
```

---

## ?? Hizmetleri Durdurma

### Tüm servisleri durdur:
```bash
# RabbitMQ'yu durdur
docker-compose down

# API ve Console'ý durdur: Terminal'de Ctrl+C
```

---

## ?? Sorun Giderme

### Problem: "Cannot connect to RabbitMQ"

**Çözüm:**
```bash
# RabbitMQ çalýþýp çalýþmadýðýný kontrol et
docker ps | grep rabbitmq

# Eðer çalýþmýyorsa:
docker-compose up -d

# Log'larý kontrol et:
docker logs monapilot-rabbitmq
```

---

### Problem: "Port 5672 already in use"

**Çözüm:**
```bash
# Eski container'ý kaldýr
docker-compose down -v

# Yeni baþlat
docker-compose up -d
```

---

### Problem: "SSL Certificate error"

**Çözüm (PowerShell):**
REST isteklerinde `-SkipCertificateCheck` ekleyin:
```powershell
Invoke-WebRequest ... -SkipCertificateCheck
```

**Çözüm (Curl):**
```bash
curl ... --insecure
```

---

### Problem: Console App'de "RabbitMQ.Client namespace not found"

**Çözüm:**
```bash
cd MonaPilot.Console
dotnet restore
dotnet build
```

---

## ?? Performans Testi

**50 istekten bahzýrlanacak?**

```powershell
1..50 | ForEach-Object {
    $body = @{
        personName = "TestUser$_"
        budget = (Get-Random -Minimum 100 -Maximum 2000)
        productType = @("Electronics", "Furniture", "Books") | Get-Random
    } | ConvertTo-Json

    Invoke-WebRequest `
        -Uri "https://localhost:5001/api/productrequest/request" `
        -Method POST `
        -ContentType "application/json" `
        -Body $body `
        -SkipCertificateCheck | Out-Null
    
    Write-Host "Request $_ gönderildi"
    Start-Sleep -Milliseconds 100
}
```

---

## ?? Dosya Yapýsý

```
MonaPilot/
??? MonaPilot.API/
?   ??? Controllers/
?   ?   ??? ProductRequestController.cs      (API endpoint'leri)
?   ??? Data/
?   ?   ??? ApplicationDbContext.cs          (EF Core context)
?   ?   ??? DbInitializer.cs                 (Seed data)
?   ??? Models/
?   ?   ??? BudgetRequest.cs                 (Talep modeli)
?   ?   ??? Product.cs                       (Ürün modeli)
?   ?   ??? ProductRequestEvent.cs           (Event modeli)
?   ??? Services/
?   ?   ??? IEventPublisher.cs               (Event publisher interface)
?   ?   ??? RabbitMqEventPublisher.cs        (RabbitMQ uygulamasý)
?   ??? Program.cs                           (Startup config)
?   ??? appsettings.json                     (Konfigürasyon)
?
??? MonaPilot.Console/
?   ??? Models/
?   ?   ??? Product.cs
?   ?   ??? ProductRequestEvent.cs
?   ??? Services/
?   ?   ??? ConsoleLogger.cs                 (Logging implementation)
?   ?   ??? IProductService.cs               (Service interface)
?   ?   ??? ProductService.cs                (Ürün iþlemleri)
?   ?   ??? RabbitMqEventConsumer.cs         (Event consumer)
?   ??? Program.cs                           (Startup)
?
??? docker-compose.yml                       (RabbitMQ kurulumu)
??? requests.http                            (API test istekleri)
??? start-all.bat                            (Windows baþlatma)
??? start-all.sh                             (Linux/Mac baþlatma)
??? README.md                                (Teknik dokümantasyon)
```

---

## ?? Güvenlik Notlarý

- **Production'da** RabbitMQ kimlik doðrulamasýný etkinleþtirin
- **HTTPS sertifikalarý** proper imzalý olanlarla deðiþtirin
- **API Rate Limiting** ekleyin
- **Database backup** alýnýz

---

## ?? Destek

Sorun yaþarsanýz, aþaðýdaki loglarý kontrol edin:

**Web API Log Dosyasý:**
```bash
# VS Output penceresinde ya da:
dotnet run 2>&1 | Tee-Object -FilePath api.log
```

**Console App Log Dosyasý:**
```bash
dotnet run 2>&1 | Tee-Object -FilePath console.log
```

**RabbitMQ Log:**
```bash
docker logs monapilot-rabbitmq
```

---

**Happy Coding! ??**
