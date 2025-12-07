using Microsoft.AspNetCore.Mvc;
using MiniForestApp.Models; 
using Microsoft.EntityFrameworkCore;
using MiniForestApp.Models; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MiniForestDbContext>(options =>
{
    // SQL Server'ı kullanması için yapılandırıyoruz
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


builder.Services.AddControllers();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");

app.MapControllers();

app.Run();