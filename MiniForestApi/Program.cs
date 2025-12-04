using Microsoft.AspNetCore.Mvc;
using MiniForestApp.Models; // kendi namespace'ine göre düzelt (aşağıda anlatıyorum)

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// EF YOK: AddDbContext kullanmıyoruz
// builder.Services.AddDbContext<...>(...);

builder.Services.AddControllers();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");

app.MapControllers();

app.Run();
