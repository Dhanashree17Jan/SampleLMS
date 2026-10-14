using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Schoolnet.Domain;
using Schoolnet.Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Configuration.AddJsonFile("appsettings.Local.json",optional:true,reloadOnChange:false).AddEnvironmentVariables();
builder.Services.AddControllersWithViews(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
builder.Services.AddSingleton(NpgsqlDataSource.Create(new NpgsqlConnectionStringBuilder(builder.Configuration.GetConnectionString("Training")) { SearchPath="schoolnet,public", IncludeErrorDetail=false }.ConnectionString));
builder.Services.AddScoped<ITrainingRepository, TrainingRepository>();
builder.Services.AddSingleton<PasswordHasher<string>>();
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath,"../.keys"))).SetApplicationName("SchoolnetTraining");
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath="/Account/Login"; options.AccessDeniedPath="/Account/Denied";
    options.Cookie.Name="Schoolnet.Session"; options.Cookie.HttpOnly=true; options.Cookie.SameSite=SameSiteMode.Lax;
    options.ExpireTimeSpan=TimeSpan.FromHours(8); options.SlidingExpiration=true;
    options.Events.OnValidatePrincipal=async context =>
    {
        var repository=context.HttpContext.RequestServices.GetRequiredService<ITrainingRepository>();
        if (!long.TryParse(context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,out var id)) { context.RejectPrincipal(); return; }
        var user=await repository.One("SELECT active,role,updated_at FROM schoolnet.app_user WHERE id=@id",("id",id));
        if (user is null || !user.Flag() || user.Text("role")!=context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value || user.Text("updated_at")!=context.Principal?.FindFirst("user_version")?.Value) context.RejectPrincipal();
    };
});
builder.Services.AddAuthorization();
builder.Services.AddRateLimiter(o=>o.AddPolicy("login",context=>RateLimitPartition.GetFixedWindowLimiter(context.Connection.RemoteIpAddress?.ToString()??"local",_=>new FixedWindowRateLimiterOptions { PermitLimit=10,Window=TimeSpan.FromMinutes(1),QueueLimit=0 })));
var app=builder.Build();
if (args.Contains("--initialize"))
{
    var password=builder.Configuration["Bootstrap:Password"] ?? throw new InvalidOperationException("Set Bootstrap:Password in appsettings.Local.json before initialization.");
    using var scope=app.Services.CreateScope();
    var repository=scope.ServiceProvider.GetRequiredService<ITrainingRepository>();
    var hash=scope.ServiceProvider.GetRequiredService<PasswordHasher<string>>().HashPassword("admin",password);
    var path=Path.Combine(AppContext.BaseDirectory,"database/001_schema.sql");
    await repository.Initialize(await File.ReadAllTextAsync(path),hash);
    await repository.Execute(await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory,"database/003_modify_trainee.sql")));
    if(args.Contains("--seed"))
    {
        // Seed is run on a single connection so the hash is passed as a SQL parameter.
        var seed=await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory,"database/002_demo.sql"));
        await repository.Execute(seed,("hash",hash));
    }
    Console.WriteLine("Schoolnet database initialized. Login credentials are in your ignored local configuration.");
    return;
}
if(!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); app.UseHttpsRedirection(); }
app.Use(async(context,next)=> { context.Response.Headers["X-Content-Type-Options"]="nosniff"; context.Response.Headers["Referrer-Policy"]="same-origin"; context.Response.Headers["X-Frame-Options"]="DENY"; await next(); });
app.UseStaticFiles(); app.UseRouting(); app.UseRateLimiter(); app.UseAuthentication(); app.UseAuthorization();
app.MapControllerRoute("default","{controller=Home}/{action=Index}/{id?}");
app.Run();
