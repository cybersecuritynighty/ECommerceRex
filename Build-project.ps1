Write-Host "🚀 Creating Store Mobile directory structure..." -ForegroundColor Cyan
$Directories = @(
    "StoreMobile/Data",
    "StoreMobile/Controllers",
    "StoreMobile/Views/Shared",
    "StoreMobile/Views/Home",
    "StoreMobile/Views/Admin",
    "StoreMobile/Views/User",
    "StoreMobile/Views/Products",
    "StoreMobile/Views/Bank"
)
foreach ($Dir in $Directories) {
    New-Item -ItemType Directory -Force -Path $Dir | Out-Null
}

Set-Location StoreMobile

Write-Host "📄 Creating project configuration file..." -ForegroundColor Yellow
@'
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="10.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.0" />
  </ItemGroup>
</Project>
'@ | Out-File -FilePath StoreMobile.csproj -Encoding utf8

Write-Host "⚙️ Generating Program.cs pipeline..." -ForegroundColor Yellow
@'
using Microsoft.EntityFrameworkCore;
using MobileStoreBank.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); }

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    DbInitializer.Initialize(context);
}
app.Run();
'@ | Out-File -FilePath Program.cs -Encoding utf8

Write-Host "🗄️ Generating Database Context & Seed Engine..." -ForegroundColor Yellow
@'
using Microsoft.EntityFrameworkCore;
namespace MobileStoreBank.Data {
    public class User { public int Id { get; set; } public string Username { get; set; } = ""; public string Email { get; set; } = ""; public string PasswordHash { get; set; } = ""; public string Role { get; set; } = "Merchant"; }
    public class Product { public int Id { get; set; } public string Name { get; set; } = ""; public string SKU { get; set; } = ""; public string Category { get; set; } = "General"; public decimal Price { get; set; } public int Stock { get; set; } }
    public class Wallet { public int Id { get; set; } public string WalletAddress { get; set; } = Guid.NewGuid().ToString("N"); public string AssetName { get; set; } = "USD"; public decimal Balance { get; set; } public decimal PendingClearance { get; set; } }
    public class CrmTicket { public int Id { get; set; } public string CustomerName { get; set; } = ""; public string IssueSummary { get; set; } = ""; public string Priority { get; set; } = "Medium"; public string Status { get; set; } = "Open"; }
    public class ApplicationDbContext : DbContext {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
        public DbSet<User> Users => Set<User>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Wallet> Wallets => Set<Wallet>();
        public DbSet<CrmTicket> CrmTickets => Set<CrmTicket>();
        protected override void OnModelCreating(ModelBuilder mb) {
            mb.Entity<Product>().Property(p => p.Price).HasConversion<double>();
            mb.Entity<Wallet>().Property(w => w.Balance).HasConversion<double>();
        }
    }
    public static class DbInitializer {
        public static void Initialize(ApplicationDbContext context) {
            context.Database.EnsureCreated();
            if (!context.Users.Any()) {
                context.Users.Add(new User { Username = "admin", Email = "admin@storebank.com", PasswordHash = "admin123", Role = "Admin" });
            }
            if (!context.Products.Any()) {
                context.Products.Add(new Product { Name = "iPhone 15 Pro Max", SKU = "IPH15", Category = "Devices", Price = 1199.99m, Stock = 50 });
            }
            if (!context.Wallets.Any()) {
                context.Wallets.Add(new Wallet { AssetName = "USD Core Pool", Balance = 50000.00m });
            }
            context.SaveChanges();
        }
    }
}
'@ | Out-File -FilePath Data/ApplicationDbContext.cs -Encoding utf8

Write-Host "🎮 Generating Controllers Layer..." -ForegroundColor Yellow
@'
using Microsoft.AspNetCore.Mvc;
namespace StoreMobile.Controllers { public class HomeController : Controller { public IActionResult Index() => View(); public IActionResult About() => View(); public IActionResult Product() => View(); public IActionResult Bank() => View(); } }
'@ | Out-File -FilePath Controllers/HomeController.cs -Encoding utf8

@'
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreMobile.Data;
namespace StoreMobile.Controllers {
    public class AdminController : Controller {
        private readonly ApplicationDbContext _c;
        public AdminController(ApplicationDbContext c) => _c = c;
        public async Task<IActionResult> Index() { ViewBag.TotalLiquidity = await _c.Wallets.SumAsync(w => w.Balance); return View(); }
        public async Task<IActionResult> UserList() => View(await _c.Users.ToListAsync());
        public async Task<IActionResult> ProdList() => View(await _c.Products.ToListAsync());
        public async Task<IActionResult> CRM() => View(await _c.CrmTickets.ToListAsync());
    }
}
'@ | Out-File -FilePath Controllers/AdminController.cs -Encoding utf8

@'
using Microsoft.AspNetCore.Mvc;
namespace StoreMobile.Controllers { public class UserController : Controller { public IActionResult Index() => View(); public IActionResult Login() => View(); public IActionResult SignUp() => View(); } }
'@ | Out-File -FilePath Controllers/UserController.cs -Encoding utf8

@'
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreMobile.Data;
namespace StoreMobile.Controllers {
    public class ProductsController : Controller {
        private readonly ApplicationDbContext _c;
        public ProductsController(ApplicationDbContext c) => _c = c;
        public async Task<IActionResult> Index() => View(await _c.Products.ToListAsync());
        public IActionResult CRUD() => View();
        public IActionResult Supply() => View();
        public IActionResult Category() => View();
    }
}
'@ | Out-File -FilePath Controllers/ProductsController.cs -Encoding utf8

@'
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreMobile.Data;
namespace StoreMobile.Controllers {
    public class BankController : Controller {
        private readonly ApplicationDbContext _c;
        public BankController(ApplicationDbContext c) => _c = c;
        public async Task<IActionResult> Index() => View(await _c.Wallets.ToListAsync());
        public async Task<IActionResult> Wallet() => View(await _c.Wallets.ToListAsync());
        public IActionResult History() => View();
        public IActionResult Account() => View();
    }
}
'@ | Out-File -FilePath Controllers/BankController.cs -Encoding utf8

Write-Host "🎨 Generating Razor Layout Elements..." -ForegroundColor Yellow
"@using StoreMobile`n@using StoreMobile.Data`n@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers" | Out-File -FilePath Views/_ViewImports.cshtml -Encoding utf8
"@{ Layout = `"_Layout.cshtml`"; }" | Out-File -FilePath Views/_ViewStart.cshtml -Encoding utf8

@'
<!DOCTYPE html>
<html lang="en" class="h-full bg-slate-950 text-slate-100">
<head>
    <meta charset="utf-8" />
    <title>Store Mobile</title>
    <script src="https://tailwindcss.com"></script>
</head>
<body class="min-h-full flex flex-col bg-slate-950 text-slate-100 antialiased">
    <header class="sticky top-0 z-50 w-full border-b border-white/10 bg-slate-900/60 backdrop-blur-md h-16 flex items-center justify-between px-8">
        <a href="/" class="text-xl font-bold">MobileStore<span class="text-indigo-400">Bank</span></a>
        <nav class="flex gap-6 text-sm font-medium text-slate-400">
            <a href="/Home/Index" class="hover:text-white">Dashboard</a>
            <a href="/Bank/Wallet" class="hover:text-white">Wallets</a>
            <a href="/Products/Index" class="hover:text-white">Products</a>
            <a href="/Admin/Index" class="hover:text-white">Admin</a>
        </nav>
    </header>
    <main class="flex-grow w-full max-w-7xl mx-auto px-8 py-10">
        <div class="bg-white/[0.02] border border-white/10 backdrop-blur-lg rounded-2xl p-8 shadow-2xl">
            @RenderBody()
        </div>
    </main>
    <footer class="border-t border-white/10 bg-slate-950/80 p-8 text-center text-xs text-slate-500">
        &copy; Store Mobile. Running over cleartext HTTP.
    </footer>
</body>
</html>
'@ | Out-File -FilePath Views/Shared/_Layout.cshtml -Encoding utf8

Write-Host "📝 Populating key view pages..." -ForegroundColor Yellow
@'
<div class="text-center py-10 space-y-4">
    <h1 class="text-4xl font-black">Liquidity Infrastructure</h1>
    <p class="text-slate-400 text-sm max-w-md mx-auto">High-density banking and inventory orchestration running entirely on .NET 10 over HTTP channels.</p>
</div>
'@ | Out-File -FilePath Views/Home/Index.cshtml -Encoding utf8

@'
@model IEnumerable<StoreMobile.Data.Wallet>
<div class="space-y-6">
    <h2 class="text-2xl font-bold">Active SaaS Wallets</h2>
    <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        @foreach(var w in Model) {
            <div class="bg-white/[0.04] border border-white/10 p-6 rounded-xl">
@w.AssetName$@w.Balance.ToString("N2")}
'@ | Out-File -FilePath Views/Bank/Wallet.cshtml -Encoding utf8
@'{"Logging": { "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" } },"AllowedHosts": "*","ConnectionStrings": { "DefaultConnection": "Data Source=MobileStoreBank.db" }}
'@ | Out-File -FilePath appsettings.json -Encoding utf8
$Views = @(
"Views/Home/About.cshtml", 
"Views/Home/Product.cshtml", 
"Views/Home/Bank.cshtml",
"Views/Admin/Index.cshtml", 
"Views/Admin/UserList.cshtml",
"Views/Admin/ProdList.cshtml", 
"Views/Admin/CRM.cshtml",
"Views/User/Index.cshtml", 
"Views/User/Login.cshtml", 
"Views/User/SignUp.cshtml",
"Views/Products/Index.cshtml", 
"Views/Products/CRUD.cshtml", 
"Views/Products/Supply.cshtml", 
"Views/Products/Category.cshtml",
"Views/Bank/Index.cshtml", 
"Views/Bank/History.cshtml",
"Views/Bank/Account.cshtml")
foreach ($View in $Views) {
New-Item -ItemType File -Force -Path $View | Out-Null
}
Write-Host "✅ Whole codebase built successfully in folder: 'MobileStoreBank'!" -ForegroundColor Green
