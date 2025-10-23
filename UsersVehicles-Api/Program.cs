using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsersVehicles_Api.Dominio.DTOs;
using UsersVehicles_Api.Dominio.Entidades;
using UsersVehicles_Api.Dominio.Interfaces;
using UsersVehicles_Api.Dominio.ModelViews;
using UsersVehicles_Api.Dominio.Servicos;
using UsersVehicles_Api.Infraestrutura.Db;

#region Builder
var builder = WebApplication.CreateBuilder(args);

//P injeção de dependência
builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbVehiclesContext>(options =>
{
    options.UseNpgsql(builder.Configuration
    .GetConnectionString("PostgreSQLConnection")); 
});
var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home()))
.WithTags("Home");
#endregion

#region Administradores
app.MapPost("/login", ([FromBody] LoginDTO loginDto, IAdministradorServico administradorServico) =>
{
    if (administradorServico.Login(loginDto) != null)
    {
        return Results.Ok("Login realizado com sucesso!");
    }
    else
    {
        return Results.Unauthorized();
    }
})
.WithTags("Administradores"); //Divisão no swagger
#endregion

#region Veiculos
app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDto, IVeiculoServico veiculoServico) =>
{
    var veiculo = new Veiculo
    {
        Nome = veiculoDto.Nome,
        Marca = veiculoDto.Marca,
        Ano = veiculoDto.Ano
    };
    veiculoServico.Incluir(veiculo);

    return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
}).WithTags("Veiculos");

app.MapGet("/veiculos", ([FromQuery]int? pagina, IVeiculoServico veiculoServico) =>
{
    var veiculos = veiculoServico.Todos(pagina);
    return Results.Ok(veiculos);
}).WithTags("Veiculos");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion