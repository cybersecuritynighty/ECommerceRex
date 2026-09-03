# 🛒 E‑Commerce Rex

**A secure, high-performance e‑commerce platform** built with .NET 10, featuring a glassmorphism UI, JWT authentication, HMAC-protected ledger, Redis caching, social login (Google, GitHub, Telegram), an AI-powered chat assistant, and full Docker support.

![.NET 10](https://img.shields.io/badge/.NET-10-purple)
![Docker](https://img.shields.io/badge/Docker-✅-blue)
![License](https://img.shields.io/badge/License-MIT-green)

---

## 🚀 Key Features

- **🔐 JWT Authentication** – Stateless, signed bearer tokens.
- **📜 Tamper‑Evident Ledger** – HMAC‑SHA256 hashing on every record; any unauthorised change is flagged.
- **🔄 Social Login** – Google, GitHub, and Telegram (via Login Widget).
- **🤖 AI Chat Assistant** – Integrated with OpenAI (or mock) to answer user questions.
- **🏦 Banking Module** – Wallet, transaction history, and balance management.
- **📦 Product Management** – Full CRUD with Redis‑cached listing.
- **⏱️ Attendance Tracking** – QR‑code check‑in/out with history.
- **👑 Admin Dashboard** – User and product management, tamper alerts.
- **🎨 Glassmorphism UI** – Clean, modern, responsive design.
- **🐳 Dockerized** – Runs with SQL Server and Redis in containers.

---

## 🧰 Tech Stack

| Component | Technology |
|-----------|------------|
| **Runtime** | .NET 10.0 (ASP.NET Core MVC) |
| **Database** | Microsoft SQL Server 2022 |
| **ORM** | Entity Framework Core 10 |
| **Caching** | Redis 7.2 (Alpine) |
| **Authentication** | JWT + OAuth (Google, GitHub) + Telegram custom |
| **Security** | HMAC‑SHA256 (tamper‑evident) |
| **AI Chat** | OpenAI API (or pluggable mock) |
| **UI** | Pure CSS (glassmorphism, no Tailwind) |
| **Containerisation** | Docker & Docker Compose |

---

## 📦 Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (or Docker Engine + Compose)
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (only required if running without Docker)

---

## ⚙️ Setup & Running

### 1. Clone or download the project

```bash
git clone https://github.com/your-org/ECommerceRex.git
cd ECommerceRex
```

### 2. Configure secrets (optional)
- Before running, update appsettings.json with your own credentials

```
"Authentication": {
  "Google": {
    "ClientId": "your-google-client-id",
    "ClientSecret": "your-google-client-secret"
  },
  "GitHub": {
    "ClientId": "your-github-client-id",
    "ClientSecret": "your-github-client-secret"
  },
  "Telegram": {
    "BotToken": "your-telegram-bot-token",
    "BotUsername": "your_bot_username"
  }
},
"OpenAI": {
  "ApiKey": "your-openai-api-key"
}
```


### 3. Run with Docker Compose

```bash
docker-compose up -d
```

- This will:

Start SQL Server 2022 (port 1433)

Start Redis (port 6379)

Build and run the .NET app (port 5000)

Wait a few seconds for the database to initialise.

### 4. Access the application

Open your browser and go to:
http://localhost:5000


## 🛠️ Running Without Docker (Development)

if you prefer to run locally without containers:

1. Install .NET 10 SDK and SQL Server Express / LocalDB.

2. Update the ConnectionStrings in appsettings.Development.json to point to your local SQL Server and Redis.

3. Install Redis locally or use a cloud instance.

4. Run migrations:

```bash
dotnet ef database update
```

5. Start the app:
   
```bash
dotnet run --project src/ECommerceRex
```

The app will be available at http://localhost:5000.


## 🧪 Testing

- Unit tests can be added in a separate Tests project (not included in this release).

- The HMAC tamper detection can be tested by manually editing a row in the database; the Admin dashboard will show a warning.


## 📁 Project Structure
```
ECommerceRex/
├── context.md            # Architectural blueprint and summary
├── docker-compose.yml    # Docker services orchestration
├── Dockerfile            # Multi‑stage build for the app
├── src/
│   └── ECommerceRex/
│       ├── Controllers/  # MVC controllers
│       ├── Data/         # DbContext and HMAC interceptor
│       ├── Models/       # Entities and ViewModels
│       ├── Services/     # JWT, HMAC, Redis, AI Chat
│       ├── Views/        # Razor views (glassmorphism UI)
│       └── wwwroot/      # Static assets (CSS)
└── README.md             # This file
```

## 🤝 Contributing

Contributions are welcome! Please open an issue or submit a pull request.

1. Fork the repo.

2. Create a feature branch (git checkout -b feature/amazing-feature).

3. Commit your changes (git commit -m 'Add some amazing feature').

4. Push to the branch (git push origin feature/amazing-feature).

5. Open a Pull Request.


## 📄 License
This project is licensed under the MIT License – see the LICENSE file for details.

## 🙏 Acknowledgements

<a href="">.NET</a>

<a href="">Entity Framework Core</a>

<a href="">Redis</a>

<a href="">OpenAI</a>

<a href="">Telegram Login Widget</a>

Glassmorphism design inspired by modern UI trends.
