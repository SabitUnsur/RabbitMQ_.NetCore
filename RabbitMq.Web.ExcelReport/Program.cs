using Microsoft.AspNetCore.Identity;
using RabbitMq.Web.ExcelReport.Models;
using RabbitMq.Web.ExcelReport.Services;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllersWithViews();

builder.Services.AddEntityFrameworkNpgsql()
  .AddDbContext<AppDbContext>()
  .BuildServiceProvider();

builder.Services.AddDbContext<AppDbContext>();

// UserManager baðýmlýlýðýný ekleyin
builder.Services.AddScoped<UserManager<IdentityUser>>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(opt =>
{
    opt.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();


var configuration = builder.Configuration;
builder.Services.AddSingleton(sp => new ConnectionFactory()
{
    Uri = new Uri(configuration.GetConnectionString("RabbitMQ"))
});

builder.Services.AddSingleton<RabbitMQClientService>();
builder.Services.AddSingleton<RabbitMQPublisher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
