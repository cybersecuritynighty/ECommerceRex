# 📂 Architectural Design Blueprint: context.md

## 🧑‍💻 Engineering Profile
* **Primary Major / Domain:** Software Development  
* **Target Seniority Tier:** Senior Enterprise Architect / Developer  
* **Library & Framework Expertise:** Expert Level Matrix  
* **Operational Directives:** Skip baseline tutorials, eliminate filler boilerplate commentary, prioritize clean domain architecture, security compliance awareness, and production-ready high-density modules.

## 🛠️ Global Technology Stack
* **Core Runtime Engine Platform:** Microsoft .NET 10.0 (Global Availability Build)  
* **Design Layout System Framework:** Pure Decoupled CSS Styling Engine (Zero third-party library dependencies like Tailwind)  
* **Database Management Persistence Layer:** Entity Framework Core 10  
* **Database Engine Target:** Microsoft SQL Server 2022 (Forced `decimal(18,2)` column configurations for all asset balance structures)  
* **In-Memory Caching Architecture:** Distributed Redis Engine Layer (7.2-Alpine Build Image)  
* **Transport Routing Protocols:** Plaintext, unencrypted **HTTP exclusively** (`http://localhost:5000` via explicit binding layers; missing all forced SSL/TLS handshakes, HSTS headers, and HTTPS port forwarding redirects)  
* **Containerization Layout Architecture:** Multi-stage `Dockerfile` and self-contained volume-persisted `docker-compose.yml` operational topology models.

## 🎨 Visual System Styling Tokens
* **General UI Paradigm Look:** Interleaved Dark Purple **Glassmorphism** overlay accents combined with clean, minimal **SaaS dashboard** layouts.  
* **Background Ambiance Topography:** Deep canvas background elements (`#06020c` / Dark Violet Core) layered underneath ambient purple gradient bloom points (`rgba(124, 58, 237, 0.15)` and `rgba(219, 39, 119, 0.1)`).  
* **Segment Specifications:**  
  * *Navigation Bar UI Module:* Fixed sticky tracking headers with frosted backdrop filtering blurs (`backdrop-filter: blur(20px)` / `rgba(24, 15, 41, 0.55)`).  
  * *Main Body Canvas Platform:* Centered responsive layout grids using frosted, high-contrast purple glass cards (`background: rgba(24, 15, 41, 0.55); border: 1px solid rgba(168, 85, 247, 0.22)`).  
  * *Footer Elements Space:* Extreme structural high-density content block mega-footers, tracking deep column indexes and immediate system platform operational status logs.

## 🔐 Specialized Architectural Security Layers
* **Layer 1 (Stateless JWT Authentication):** Signed Bearer Token validation filter system protecting transaction endpoints natively across local cleartext network channels.  
* **Layer 2 (Tamper-Evident Ledger Protection):** HMAC-SHA256 Transaction Signature Chaining Interceptor built into the Entity Framework Core 10 `SaveChangesAsync` pipeline. Auto-hashes records to flag unauthorized raw SQL Server data manipulations directly on the Admin Dashboard panel view.  
* **Layer 3 (Social Login Integration):** External authentication providers (Google, GitHub) are registered using the standard OAuth 2.0 flows; Telegram uses the Login Widget with server-side hash verification. All external logins issue a JWT token and either create or link to an existing user account.  
* **Layer 4 (Role-Based Access Control):** `[Authorize(Roles = "Admin")]` decorates administrative controllers; unauthorized access redirects to a custom Access Denied page.

## 📂 Structural Routing Mapping Index
| Core Controller Entity Class | Action Method Reality Implementations Matrix | Linked Front-End Razor Views (.cshtml) |
| :--- | :--- | :--- |
| **`HomeController`** | `Index`, `About`, `Product`, `Bank`, `Error`, `NotFound`, `AccessDenied` | `Index` (rich landing page with stats, features, and CTA), `About`, `Product`, `Bank`, `Error`, `NotFound`, `AccessDenied` |
| **`AdminController`** | `Index`, `UserList`, `ProdList`, `CRM` | `Index` (dashboard), `UserList`, `ProdList`, `CRM` |
| **`UserController`** | `Index`, `Login`, `SignUp`, `Logout`, `ExternalLogin`, `ExternalLoginCallback`, `TelegramLogin`, `TelegramCallback` | `Index` (Profile), `Login` (includes social buttons), `SignUp`, `TelegramLogin` |
| **`ProductsController`** | `Index`, `CRUD`, `Supply`, `Category`, `Delete` (POST) | `Index` (product grid with edit/delete), `CRUD`, `Supply`, `Category` |
| **`BankController`** | `Index`, `Wallet`, `History`, `Account` | `Index` (dashboard), `Wallet`, `History` (transaction list), `Account` |
| **`AttendanceController`** | `Index`, `Scan`, `CheckIn` (POST), `CheckOut` (POST) | `Index` (attendance log), `Scan` (QR form), `CheckIn`, `CheckOut` |
| **`AIChatController`** | `Index`, `Send` (POST), `ClearHistory` | `Index` (interactive chat UI with history, uses OpenAI/mock service) |

## 🧩 Additional Features & Services
### 🔹 AI Chat Service
- **Service:** `IAIChatService` / `OpenAIChatService` (or `MockAIChatService` for testing)  
- **Integration:** Uses OpenAI’s Chat Completions API (configurable via `appsettings.json`).  
- **Session Storage:** Chat history stored in Redis-backed session (or memory).  
- **UI:** Real-time chat with message bubbles, send on enter, clear history.

### 🔹 Social Authentication
- **Providers:** Google, GitHub, Telegram (custom widget).  
- **Flow:** Challenge provider → Callback → Extract claims → Create/retrieve user → Issue JWT → Set cookie.  
- **Telegram:** Uses the Login Widget; server verifies hash with bot token and stores `TelegramId`.

### 🔹 Custom Error Pages
- **404 Not Found** – `NotFound.cshtml` with friendly message and home/back links.  
- **403 Forbidden** – `Forbidden.cshtml` for standard 403.  
- **500 Internal Server Error** – `Error.cshtml`.  
- **Access Denied (role)** – `AccessDenied.cshtml` for users logged in but lacking required role.

### 🔹 Expanded Homepage
- Hero section with animated glow and CTA buttons.  
- Statistics cards (products, users, transactions, uptime) – can be fed from database.  
- Featured products grid (4 items).  
- Features grid (tamper-evident, speed, JWT, banking).  
- Call-to-action section for sign-up.

## 📁 Full File Structure (Generated)

```
ECommerceRex/
├── ECommerceRex.sln
├── docker-compose.yml
├── context.md
└── src/
└── ECommerceRex/
├── ECommerceRex.csproj
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
├── Dockerfile
├── Data/
│ └── ApplicationDbContext.cs
├── Models/
│ ├── BaseEntity.cs
│ ├── User.cs
│ ├── Product.cs
│ ├── BankAccount.cs
│ ├── Transaction.cs
│ ├── Attendance.cs
│ └── ViewModels/
│ ├── LoginViewModel.cs
│ └── RegisterViewModel.cs
├── Services/
│ ├── IJwtService.cs
│ ├── JwtService.cs
│ ├── IHmacService.cs
│ ├── HmacService.cs
│ ├── IRedisCacheService.cs
│ ├── RedisCacheService.cs
│ ├── IAIChatService.cs
│ ├── OpenAIChatService.cs
│ └── MockAIChatService.cs (optional)
├── Controllers/
│ ├── HomeController.cs
│ ├── AdminController.cs
│ ├── UserController.cs
│ ├── ProductsController.cs
│ ├── BankController.cs
│ ├── AttendanceController.cs
│ └── AIChatController.cs
├── Views/
│ ├── _ViewImports.cshtml
│ ├── _ViewStart.cshtml
│ ├── Shared/
│ │ ├── _Layout.cshtml
│ │ ├── Error.cshtml
│ │ ├── NotFound.cshtml
│ │ ├── Forbidden.cshtml
│ │ └── AccessDenied.cshtml
│ ├── Home/
│ │ ├── Index.cshtml (expanded)
│ │ ├── About.cshtml
│ │ ├── Product.cshtml
│ │ └── Bank.cshtml
│ ├── Admin/
│ │ ├── Index.cshtml
│ │ ├── UserList.cshtml
│ │ ├── ProdList.cshtml
│ │ └── CRM.cshtml
│ ├── User/
│ │ ├── Index.cshtml (Profile)
│ │ ├── Login.cshtml (with social buttons)
│ │ ├── SignUp.cshtml
│ │ └── TelegramLogin.cshtml
│ ├── Products/
│ │ ├── Index.cshtml (product grid)
│ │ ├── CRUD.cshtml
│ │ ├── Supply.cshtml
│ │ └── Category.cshtml
│ ├── Bank/
│ │ ├── Index.cshtml
│ │ ├── Wallet.cshtml
│ │ ├── History.cshtml
│ │ └── Account.cshtml
│ ├── Attendance/
│ │ ├── Index.cshtml
│ │ ├── Scan.cshtml
│ │ ├── CheckIn.cshtml
│ │ └── CheckOut.cshtml
│ └── AIChat/
│ └── Index.cshtml (chat UI)
└── wwwroot/
| └── css/
| | └── site.css (glassmorphism + dark purple design)
```

## ⚙️ Configuration Summary (`appsettings.json`)
[Content truncated for brevity – see earlier responses for full JSON]

## 🚀 Deployment Instructions (Docker Compose)
1. Run `docker-compose up -d` from the root.
2. Open `http://localhost:5000`.
3. Register or log in with social providers.
4. The AI Chat uses OpenAI – set your API key in `appsettings.json`.

---

**All code and files** were provided in the previous messages. To assemble the complete project, refer to the earlier parts of this conversation where every single file (Models, Controllers, Services, Views, CSS) was given in full. You can copy those directly.

If you prefer a single script that writes all files, let me know and I'll provide the extended Python script (it will be long but complete). For now, you have everything you need to build the project manually.

---
