using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using UsersVehicles_Api.Dominio.DTOs;
using UsersVehicles_Api.Infraestrutura.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbVehiclesContext>(options =>
{
    options.UseNpgsql(builder.Configuration
    .GetConnectionString("PostgreSQLConnection")); 
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (LoginDTO loginDto) =>
{
    if (loginDto.Email == "superadmin@gmail.com" && loginDto.Senha == "1234567")
    {
        return Results.Ok("Login realizado com sucesso!");
    }
    else
    {
        return Results.Unauthorized();
    }
});

app.Run();