# LinkUp — Real-Time Chat App (Backend + Frontend)

LinkUp is a real-time messaging app with chat creation, user registration/login, and live messaging via WebSockets. This is a learning project focused on clean architecture, security, and modern backend technologies.

---

## ✨ Features

- ✅ User registration with email confirmation
- ✅ JWT-based authentication & authorization
- ✅ Create/join chats by unique usernames
- ✅ Real-time messaging using SignalR
- ✅ REST API for chat and message management
- ✅ Secure architecture with token validation
- ✅ Modern layered project structure (CQRS, MediatR)

---

## 🛠️ Technologies Used

**Backend (ASP.NET Core):**
- ASP.NET Core Web API
- Entity Framework Core (PostgreSQL)
- MediatR (CQRS pattern)
- SignalR (real-time communication)
- JWT (auth)


**Frontend:**
- HTML + CSS + JavaScript
- Mobile-first layout
- Token stored in `localStorage`
- Axios for API requests
- Live message updates via SignalR

---

## 🚀 How to Run the Project

### 1. Clone the repository
```bash
git clone https://github.com/sDenisss/linkup.git
cd linkup
```

2. Configure the database
Add appsettings.json and edit 

```appsettings.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=<YOUR-PORT>;Database=<YOUR-DATABASE>;Username=<YOUR-USERNAME>;Password=<YOUR-PASSWORD>"
  },

  "Jwt": {
    "Key": "<YOUR-KEY>",  
    "Issuer": "<YOUR-ISSUER>",
    "Audience": "<YOUR-AUDIENCE>"
  }
}
```

Create .env 
```env
# Database
DB_HOST=localhost
DB_PORT=<PORT>
DB_USER=<DB_USER>
DB_PASSWORD=<PASSWORD>
DB_DB=<DB-NAME>

# JWT
JWT_KEY=<JWT-KEY>
JWT_ISSUER=<JWT_ISSUER>
JWT_AUDIENCE=<JWT_AUDIENCE>
```


3. Apply EF Core migrations
dotnet ef database update

4. Run the project
```bash
dotnet run
```
or 
```bash
dotnet run --launch-profile https
```

5. Run frontend
Just open http://localhost:5037 or https://localhost:7180 (if you run dotnet run --launch-profile https) in your browser.