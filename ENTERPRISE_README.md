# ?? MONAPILOT - ENTERPRISE SYSTEM (UPGRADED)

## ?? **COMPLETE SYSTEM ARCHITECTURE**

```
???????????????????????????????????????????????????????????????
?                    MONAPILOT ENTERPRISE                     ?
???????????????????????????????????????????????????????????????
?                                                             ?
?  Frontend (Blazor WebAssembly)                            ?
?  ?? ?? Login Page                                         ?
?  ?? ?? Dashboard (Real-time Stats)                        ?
?  ?? ?? Admin Panel (Product Management)                   ?
?                                                             ?
?  Backend (ASP.NET Core 10.0)                              ?
?  ?? ?? JWT Authentication                                ?
?  ?? ?? SignalR Hub (Real-time Updates)                   ?
?  ?? ?? RESTful API Endpoints                             ?
?  ?? ??? SQLite Database                                   ?
?  ?? ?? RabbitMQ Event Publisher                          ?
?                                                             ?
?  Message Queue (RabbitMQ)                                 ?
?  ?? product-requests Queue                               ?
?  ?? activity-logs Queue                                  ?
?                                                             ?
?  Consumer (Console App .NET 8.0)                          ?
?  ?? Event Processing                                     ?
?  ?? Product Selection                                    ?
?  ?? Stock Management                                     ?
?  ?? Activity Logging                                     ?
?                                                             ?
?  Testing & Deployment                                     ?
?  ?? ? Unit Tests (xUnit)                                ?
?  ?? ?? Docker Containers                                 ?
?  ?? ?? CI/CD Ready                                       ?
?                                                             ?
???????????????????????????????????????????????????????????????
```

---

## ? **YENÝ ÖZELLIKLER**

### **1. Frontend (Blazor WebAssembly)**
- ? **Login Page** - JWT Authentication
- ? **Dashboard** - Real-time Statistics
- ? **Admin Panel** - Product & User Management
- ? **Activity Viewer** - Log Display
- ? **Responsive Design** - Mobile-friendly

### **2. Backend Enhancements**
- ? **JWT Tokens** - Secure API Access
- ? **SignalR** - Real-time Communication
- ? **CORS** - Cross-Origin Support
- ? **Better Logging** - Detailed Error Tracking
- ? **Advanced Error Handling** - Try-catch patterns

### **3. Testing & Quality**
- ? **Unit Tests** - xUnit Framework
- ? **Authentication Tests** - JWT Validation
- ? **Activity Log Tests** - Data Integrity

### **4. Deployment**
- ? **Docker** - Container Images
- ? **Docker Compose** - Multi-container Setup
- ? **Production Config** - Optimized Settings

---

## ?? **BAÞLAMA KODU**

### **Development Mode (Local)**

**Terminal 1: RabbitMQ**
```sh
docker-compose up -d
```

**Terminal 2: Web API**
```sh
cd MonaPilot.API
dotnet run
```
Açýlýþ: `https://localhost:5001`

**Terminal 3: Console Consumer**
```sh
cd MonaPilot.Console
dotnet run
```

**Terminal 4: Blazor Web (Front-end)**
```sh
cd MonaPilot.Web
dotnet run
```
Açýlýþ: `http://localhost:5281`

---

### **Production Mode (Docker)**

```sh
docker-compose -f docker-compose.prod.yml up -d
```

Eriþim:
- Frontend: `http://localhost:5281`
- API: `http://localhost:5001`
- RabbitMQ: `http://localhost:15672`

---

## ?? **TEST KOMUTU**

```sh
cd MonaPilot.Tests
dotnet test
```

**Çalýþan Testler:**
- ? JWT Token Generation
- ? Token Validation
- ? Invalid Token Rejection
- ? Activity Log Creation
- ? Timestamp Validation

---

## ?? **LOGIN BÝLGÝLERÝ**

| Field | Value |
|-------|-------|
| **Username** | admin |
| **Password** | admin123 |

---

## ?? **API ENDPOINTS**

### **Authentication**
```
POST /api/auth/login
POST /api/auth/refresh
```

### **Product Requests** (Requires Auth)
```
POST /api/productrequest/request
GET /api/productrequest/{id}
GET /api/productrequest/all
GET /api/productrequest/logs/all
GET /api/productrequest/logs/person/{name}
```

### **Real-time (SignalR)**
```
/hubs/notifications
```

---

## ?? **DATA FLOW**

```
1. User Login (Frontend)
   ? GET JWT Token
   ?
2. Create Request (API)
   ? Save to Database
   ? Publish Event
   ?
3. RabbitMQ Queue
   ?
4. Console Consumer
   ? Process Event
   ? Update Stock
   ? Log Activity
   ?
5. SignalR Broadcast
   ?
6. Frontend Update (Real-time)
```

---

## ?? **PROJECT STRUCTURE**

```
MonaPilot/
??? MonaPilot.API/              # Backend API (.NET 10)
?   ??? Controllers/
?   ??? Models/
?   ??? Services/
?   ??? Hubs/                   # SignalR Hubs
?   ??? Data/                   # EF Core
?   ??? Dockerfile
?
??? MonaPilot.Console/          # Event Consumer (.NET 8)
?   ??? Services/
?   ??? Models/
?   ??? Program.cs
?
??? MonaPilot.Web/              # Frontend (Blazor)
?   ??? Components/
?   ?   ??? Pages/
?   ?   ?   ??? Login.razor
?   ?   ?   ??? Dashboard.razor
?   ?   ?   ??? Admin.razor
?   ?   ??? Layout/
?   ??? Dockerfile
?
??? MonaPilot.Tests/            # Unit Tests
?   ??? AuthenticationTests.cs
?   ??? ActivityLogTests.cs
?
??? docker-compose.yml          # Development
??? docker-compose.prod.yml     # Production
??? README.md
```

---

## ?? **FEATURES CHECKLIST**

- [x] Event-Driven Architecture
- [x] RabbitMQ Integration
- [x] SQLite Database
- [x] JWT Authentication
- [x] SignalR Real-time Updates
- [x] Blazor Frontend
- [x] Admin Panel
- [x] Activity Logging
- [x] Unit Tests
- [x] Docker Support
- [x] Error Handling
- [x] CORS Support

---

## ?? **NEXT STEPS (FUTURE)**

1. Database Migrations (EF Core)
2. User Management (Roles & Permissions)
3. Email Notifications
4. Payment Integration
5. Analytics Dashboard
6. API Rate Limiting
7. Caching (Redis)
8. Kubernetes Deployment
9. Load Balancing
10. Monitoring & Alerting (Prometheus)

---

## ?? **TROUBLESHOOTING**

### Issue: Port Already in Use
```sh
Get-Process -Name dotnet | Stop-Process -Force
```

### Issue: RabbitMQ Connection Failed
```sh
docker-compose down -v
docker-compose up -d
```

### Issue: CORS Error
Check `appsettings.json` for `AllowBlazor` policy

### Issue: JWT Token Invalid
Ensure `Jwt:Key` is at least 32 characters

---

## ?? **SUPPORT**

For issues, check:
1. API Logs: Terminal 2
2. Consumer Logs: Terminal 3
3. RabbitMQ Dashboard: `http://localhost:15672`

---

**?? MonaPilot - Enterprise Ready!**

Last Updated: 2024
Version: 1.0.0
