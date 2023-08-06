using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMqWeb.WaterMark.BackgroundServices;
using RabbitMqWeb.WaterMark.Models;
using RabbitMqWeb.WaterMark.Services;
using System;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase(databaseName: "productDb");
});

var configuration = builder.Configuration;
builder.Services.AddSingleton(sp => new ConnectionFactory()
{
    Uri = new Uri(configuration.GetConnectionString("RabbitMQ"))
});

builder.Services.AddHostedService<ImageWatermarkProcessBackgroundService>();

builder.Services.AddSingleton<RabbitMQClientService>();
builder.Services.AddSingleton<RabbitMQPublisher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();