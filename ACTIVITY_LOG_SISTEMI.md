# ?? MonaPilot - Activity Log Sistemi (YENÝ)

## ?? Nedir?

"Ufak iletiþimlerde bulunabilen log tutan sistem" olarak istediðiniz þey **tamamen uygulandý**:

? **Her iþlem kaydediliyor** - Kimin ne ürün aldýðý  
? **Veritabanýnda saklanýyor** - SQLite'da kalýcý  
? **Sorgulanabiliyor** - API endpoints aracýlýðýyla  
? **RabbitMQ'ya gönderiliyor** - Realtime iletiþim  
? **Zaman damgasý var** - Ne zaman yapýldýðý belli  

---

## ?? Akýþ (Flow)

```
1. API'ye talep gönder
        ?
2. Talep DB'ye kaydedilir
        ?
3. Event RabbitMQ'ya gönderilir
        ?
4. Console App Event'i alýr
        ?
5. Ürün satýþý yapýlýr
        ?
6. ? ActivityLog oluþturulur
        ?
7. ActivityLog DB'ye kaydedilir
        ?
8. ActivityLog RabbitMQ'ya gönderilir
        ?
9. ?? API'den sorgulanabilir!
```

---

## ?? Yeni API Endpoints

### 1. TÜM LOG'LAR
```http
GET /api/productrequest/logs/all
```

**Response:**
```json
[
  {
    "id": 1,
    "budgetRequestId": 1,
    "personName": "Fatih Kaplan",
    "productName": "Smartphone Pro",
    "productPrice": 800,
    "remainingStock": 9,
    "logMessage": "Satýþ baþarýlý",
    "status": "Success",
    "createdAt": "2024-02-09T20:15:31.123Z"
  }
]
```

### 2. KÝÞÝYE AÝT LOG'LAR
```http
GET /api/productrequest/logs/person/{personName}
```

**Örnek:**
```http
GET /api/productrequest/logs/person/Fatih%20Kaplan
```

**Response:**
```json
[
  {
    "id": 1,
    "budgetRequestId": 1,
    "productName": "Smartphone Pro",
    "productPrice": 800,
    "logMessage": "Satýþ baþarýlý",
    "status": "Success",
    "createdAt": "2024-02-09T20:15:31.123Z"
  }
]
```

---

## ?? Veritabaný Yapýsý

### ActivityLogs Tablosu

```sql
CREATE TABLE ActivityLogs (
    Id INT PRIMARY KEY,
    BudgetRequestId INT,
    PersonName VARCHAR(255),
    ProductName VARCHAR(255),
    ProductPrice DECIMAL(10,2),
    RemainingStock INT,
    LogMessage VARCHAR(500),
    Status VARCHAR(50),  -- "Success", "Warning", "Error"
    CreatedAt DATETIME
)
```

---

## ?? Test Senaryolarý

### Senaryo 1: Baþarýlý Satýþ ve Log Kaydý

**1. Talep Gönder:**
```bash
curl -X POST https://localhost:5001/api/productrequest/request \
  -H "Content-Type: application/json" \
  -d '{
    "personName": "Ali Yýldýz",
    "budget": 1200,
    "productType": "Electronics"
  }' --insecure
```

**2. Tüm Log'larý Kontrol Et:**
```bash
curl https://localhost:5001/api/productrequest/logs/all --insecure
```

**3. Sadece Bu Kiþinin Log'larýný Gör:**
```bash
curl https://localhost:5001/api/productrequest/logs/person/Ali%20Yýldýz --insecure
```

---

### Senaryo 2: Bütçe Yetersiz (Warning Log)

**Talep:**
```json
{
  "personName": "Zeynep Kaya",
  "budget": 100,
  "productType": "Electronics"
}
```

**Beklenen Log:**
```json
{
  "personName": "Zeynep Kaya",
  "logMessage": "Uygun ürün bulunamadý - Bütçe: 100, Tür: Electronics",
  "status": "Warning"
}
```

---

## ?? Hangi Bilgiler Kaydediliyor?

| Bilgi | Açýklama |
|-------|----------|
| **BudgetRequestId** | Hangi talepten kaynaklanmýþ |
| **PersonName** | Kiþi adý |
| **ProductName** | Satýlan ürün adý |
| **ProductPrice** | Ürünün fiyatý |
| **RemainingStock** | Satýþ sonrasý kalan stok |
| **LogMessage** | Detaylý mesaj |
| **Status** | Success/Warning/Error |
| **CreatedAt** | Yapýldýðý saat |

---

## ?? Log Status Türleri

```
? "Success"  ? Satýþ baþarýlý
??  "Warning" ? Bütçe yetersiz veya stok yok
? "Error"    ? Hata oluþtu
```

---

## ?? Veri Güvenliði

- ? Tüm iþlemler **kaydediliyor**
- ? **Deðiþtirilemez** kayýtlar (append-only)
- ? **Zaman damgasý** ile audit trail
- ? Kiþisel bilgiler **þifreli** depolanabilir (future)

---

## ?? Kullaným Örnekleri

### Özellik 1: Kiþinin Geçmiþ Alýþveriþ Raporu
```
GET /api/productrequest/logs/person/Ali%20Yýldýz

Sonuç: Ali Yýldýz'ýn ne zaman ne ürün aldýðý tam liste
```

### Özellik 2: Belirli Bir Gün Ýçindeki Tüm Satýþlar
```
GET /api/productrequest/logs/all

Sonuç: Tüm satýþlarý filtreleyip analiz edebilirsiniz
```

### Özellik 3: Hata Ayýklama
```
Eðer müþteri "Ýsim çýkmazlýk" yaþarsa, 
log'lara bakarak ne olduðu görülebilir
```

---

## ?? Kullanmaya Baþla

### 1. Sistem baþlat
```bash
start-all.bat  # Windows
./start-all.sh # Linux/Mac
```

### 2. Ýstekler gönder
```bash
# requests.http'deki Test 7, 8, 9 kullan
```

### 3. Log'larý görüntüle
```bash
# GET /api/productrequest/logs/all
# GET /api/productrequest/logs/person/{isim}
```

---

## ?? Sonraki Geliþtirmeler

- [ ] Log'larý Elasticsearch'e kaydetme
- [ ] Dashboard oluþturma
- [ ] Export fonksiyonu (CSV/PDF)
- [ ] Zaman aralýðý filtreleme
- [ ] Ýstatistik raporlarý

---

**Sisteminiz þimdi tamamen auditible ve traceable! ??**
