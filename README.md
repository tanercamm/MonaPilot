# MonaPilot - Event-Driven Budget & Product Management System

## ?? Proje Yapýsý

Bu proje **Event-Driven Architecture** kullanarak iki baðýmsýz bileþenden oluþur:

### 1. **Web API** (MonaPilot.API)
- **Teknoloji**: ASP.NET Core 10.0
- **Veritabaný**: SQLite (Entity Framework Core)
- **Message Queue**: RabbitMQ

**Sorumluluklar:**
- Kullanýcýdan bütçe ve ürün tipini API endpoint'i aracýlýðýyla alýr
- Verileri SQLite veritabanýna kaydeder
- Event'i RabbitMQ kuyruðuna gönderir

### 2. **Console Application** (MonaPilot.Console)
- **Teknoloji**: .NET Framework 4.7.2
- **Message Queue Consumer**: RabbitMQ
- **Logging**: Microsoft.Extensions.Logging

**Sorumluluklar:**
- RabbitMQ kuyruðundan event'leri okur
- Ýlgili ürün tipinde, bütçeye uygun olan ürünü bulur
- Ürünün stokunu 1 adet azaltýr
- Ýþlem detaylarýný konsola loglar

---

## ?? Kurulum

### Ön Koþullar
- .NET 10.0 SDK (Web API için)
- .NET Framework 4.7.2 (Console App için)
- RabbitMQ Server (kurulu ve çalýþan)

### RabbitMQ Kurulumu

Windows'ta RabbitMQ'yu Docker ile çalýþtýrmak en kolay yoldur:

```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:4-management
```

Yönetim paneli: `http://localhost:15672`
- Username: guest
- Password: guest

### Proje Kurulumu

1. **Baðýmlýlýklarý Yükle**
   ```bash
   dotnet restore
   ```

2. **Web API'yi Çalýþtýr**
   ```bash
   cd MonaPilot.API
   dotnet run
   ```
   API þu adreste eriþilebilir: `https://localhost:5001`

3. **Console Application'ý Çalýþtýr** (ayrý terminal'de)
   ```bash
   cd MonaPilot.Console
   dotnet run
   ```

---

## ?? API Endpoints

### 1. Ürün Talebini Oluþtur
```http
POST /api/productrequest/request
Content-Type: application/json

{
  "personName": "Ahmet Yýlmaz",
  "budget": 1500,
  "productType": "Electronics"
}
```

**Response (201 Created):**
```json
{
  "id": 1,
  "personName": "Ahmet Yýlmaz",
  "budget": 1500,
  "productType": "Electronics",
  "createdAt": "2024-02-09T19:00:00Z",
  "status": "Pending"
}
```

### 2. Talebi Görüntüle
```http
GET /api/productrequest/{id}
```

### 3. Tüm Talepleri Listele
```http
GET /api/productrequest/all
```

---

## ?? Veri Modelleri

### BudgetRequest (Web API - Database)
```csharp
public class BudgetRequest
{
    public int Id { get; set; }
    public string PersonName { get; set; }
    public decimal Budget { get; set; }
    public string ProductType { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } // Pending, Completed, Failed
}
```

### Product
```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
```

### Mevcut Ürünler (Seed Data)

| ID | Adý | Tür | Fiyat | Stok |
|----|-----|-----|-------|------|
| 1 | Laptop Deluxe | Electronics | 1200 | 5 |
| 2 | Smartphone Pro | Electronics | 800 | 10 |
| 3 | Tablet | Electronics | 500 | 8 |
| 4 | Office Chair | Furniture | 300 | 15 |
| 5 | Desk | Furniture | 400 | 7 |
| 6 | Book Bundle | Books | 50 | 100 |

---

## ?? Event Flow

```
1. POST /api/productrequest/request
          ?
2. API verileri DB'ye kaydeder
          ?
3. API, ProductRequestEvent'ini RabbitMQ'ya yayýnlar
          ?
4. Console App kuyruktan event'i okur
          ?
5. Ürün tipine göre filtreler, bütçeye uygun olanýný seçer
          ?
6. Stoktan 1 adet düþer
          ?
7. Ýþlemi konsola loglar
```

---

## ?? Örnek Senaryo

**1. Ýstek Gönder:**
```json
POST /api/productrequest/request
{
  "personName": "Fatih Kaplan",
  "budget": 900,
  "productType": "Electronics"
}
```

**2. Web API Loglarý:**
```
[2024-02-09 20:15:30] [Information] MonaPilot.API.Controllers.ProductRequestController: 
Yeni talep kaydedildi - ID: 1, Kiþi: Fatih Kaplan, Bütçe: 900, Tür: Electronics

[2024-02-09 20:15:30] [Information] MonaPilot.API.Controllers.ProductRequestController:
Event yayýnlandý - Request ID: 1
```

**3. Console App Loglarý:**
```
[2024-02-09 20:15:31] [Information] MonaPilot.Console: 
[Event Alýndý] Request ID: 1, Kiþi: Fatih Kaplan, Bütçe: 900, Tür: Electronics

[2024-02-09 20:15:31] [Information] MonaPilot.Console:
[SATIÞLAÞTIRMA] Kiþi: Fatih Kaplan, Ürün: Smartphone Pro, Fiyat: 800, Kalan Stok: 9
```

---

## ??? Mimarinin Avantajlarý

? **Loose Coupling**: Web API ve Console App baðýmsýz çalýþabilir  
? **Scalability**: Birden fazla consumer eklenebilir  
? **Asynchronous Processing**: Hýzlý API response'u  
? **Reliability**: Event'ler baþarýsýz olursa yeniden denenebilir  
? **Auditability**: Tüm iþlemler loglanýr  

---

## ?? Ýleri Konfigurasyonlar

### RabbitMQ Kullanýcý Oluþtur
```bash
docker exec rabbitmq rabbitmqctl add_user monapilot password123
docker exec rabbitmq rabbitmqctl set_permissions -p / monapilot ".*" ".*" ".*"
```

Sonra appsettings.json'da güncelleyin:
```json
"RabbitMq": {
  "Host": "localhost",
  "Username": "monapilot",
  "Password": "password123"
}
```

---

## ?? Notlar

- Veritabaný ilk çalýþtýrýldýðýnda otomatik oluþturulur (SQLite)
- Ürün stok bilgisi þu anda memory'de tutulur (production'da DB'ye taþýnmalý)
- RabbitMQ localhost'ta çalýþmak üzere konfigüre edilmiþtir

---

## ?? Geliþtirme

Herhangi bir sorun veya özellik isteði için lütfen issue açýnýz.
