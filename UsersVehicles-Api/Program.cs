using Microsoft.AspNetCore.Http;
using UsersVehicles_Api.Dominio.DTOs;

var builder = WebApplication.CreateBuilder(args);
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