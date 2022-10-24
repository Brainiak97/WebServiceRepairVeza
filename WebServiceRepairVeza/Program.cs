using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using BLL.SignalR;
using Core.Models;
using DAL.Repository;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WebEssentials.AspNetCore.Pwa;
using WebService.Data;
using WebService.Models.Mapper;
using WebService.Quartz;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddDbContext<DAL.AppContext>(options => options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(8, 0, 28))));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services
    .AddIdentity<User, IdentityRole<int>>(opts =>
    {
        opts.User.AllowedUserNameCharacters = "абвгдеёжзийколмнопрстуфхцчшъщьэюяАБВГДЕЁЖЗИЙКОЛМНОПРСТУФХЦЧШЪЩЬЭЮЯabcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-.,_";
        opts.Password.RequiredLength = 4;   // минимальная длина
        opts.Password.RequireNonAlphanumeric = false;   // требуются ли не алфавитно-цифровые символы
        opts.Password.RequireLowercase = false; // требуются ли символы в нижнем регистре
        opts.Password.RequireUppercase = false; // требуются ли символы в верхнем регистре
        opts.Password.RequireDigit = true; // требуются ли цифры
    })
    .AddEntityFrameworkStores<DAL.AppContext>();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling
         = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

builder.Services.AddAutoMapper(config =>
{
    config.AddProfiles(new Profile[] { new BLL.Mapper() });
    config.AddExpressionMapping();
});

var bl = Assembly.Load("BLL");

builder.Services
    .AddTransient(typeof(IRepository<>), typeof(Repository<>))
    .Scan(scan => scan
    .FromAssemblies(bl)
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
    .AsImplementedInterfaces()
    .AsSelf()
    .WithTransientLifetime())
    .AddAutoMapper(typeof(BLL.Mapper), typeof(PresentationProfile))
    .AddTransient<ArchivingFactory>()
    .AddScoped<ArchivingJob>();

builder.Services.AddValidatorsFromAssembly(bl);

builder.Services.AddSignalR();

builder.Services.AddProgressiveWebApp(new PwaOptions
{
    CacheId = "Cache_WebServiceRepairVeza",
    Strategy = ServiceWorkerStrategy.NetworkFirst,
    RegisterServiceWorker = true,
    RegisterWebmanifest = true,
    AllowHttp = true,
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<User>>();
        var rolesManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
        await RoleInitializer.InitializeAsync(userManager, rolesManager);
        await ArchivingScheduler.Start(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<CommentHub>("/comments");
    endpoints.MapHub<NotificationHub>("/notifications");
    endpoints.MapHub<RepairLogIndexHub>("/repairlogindex");
    endpoints.MapHub<RepairLogDetailsHub>("/repairlogdetails");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=RepairLog}/{action=Index}/");
app.MapRazorPages();

app.Run();
