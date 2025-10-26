using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsersVehicles_Api.Dominio.DTOs;
using UsersVehicles_Api.Dominio.Entidades;
using UsersVehicles_Api.Dominio.Enums;
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

app.MapPost("/administradores", ([FromBody] AdministradorDTO admDto, IAdministradorServico administradorServico) =>
{
    var validacao = new ErrosDeValidacao
    {
        Mensagens = new List<string>()
    };
    if (string.IsNullOrEmpty(admDto.Email)) validacao.Mensagens.Add("O email não pode estar vazio");
    if (string.IsNullOrEmpty(admDto.Senha)) validacao.Mensagens.Add("A senha não pode estar vazio");
    if (admDto.Perfil == null) validacao.Mensagens.Add("O perfil não pode estar vazio");

    if (validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }

    var adm = new Administrador
    {
        Email = admDto.Email,
        Senha = admDto.Senha,
        Perfil = admDto.Perfil.ToString() ?? Perfil.Editor.ToString()
    };

    administradorServico.Incluir(adm);
    AdministradorModelView admView = new AdministradorModelView
    {
        Id = adm.Id,
        Email = adm.Email,
        Perfil = adm.Perfil
    };
    return Results.Created($"/administradores/{adm.Id}", admView);

})
.WithTags("Administradores");

app.MapGet("/administradores", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
{
    var admsView = new List<AdministradorModelView>();
    var administradores = administradorServico.Todos(pagina);

    foreach(var adm in administradores)
    {
        admsView.Add(new AdministradorModelView
        {
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil
        });
    }
    return Results.Ok(admsView);
}).WithTags("Administradores");

app.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) =>
{
    var administrador = administradorServico.BuscarPorId(id);
    if (administrador == null) return Results.NotFound("Não foi encontrado administrador com ID " + id);
    AdministradorModelView admView = new AdministradorModelView
    {
        Id = administrador.Id,
        Email = administrador.Email,
        Perfil = administrador.Perfil
    };
    return Results.Ok(admView);
}).WithTags("Administradores");


#endregion

#region Veiculos
ErrosDeValidacao validaDTO(VeiculoDTO veiculoDto)
{
    var validacoes = new ErrosDeValidacao{
        Mensagens= new List<string>()
    };

    if (string.IsNullOrEmpty(veiculoDto.Nome)) validacoes.Mensagens.Add("Informe o nome do veículo!");
    if (string.IsNullOrEmpty(veiculoDto.Marca)) validacoes.Mensagens.Add("Informe a marca do veículo!");
    if (veiculoDto.Ano < 1950) validacoes.Mensagens.Add("Somente são aceitos veículos novos. Com ano superior a 1949");

    return validacoes;
}

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDto, IVeiculoServico veiculoServico) =>
{
    ErrosDeValidacao validacao = validaDTO(veiculoDto);
    if(validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }
    var veiculo = new Veiculo
    {
        Nome = veiculoDto.Nome,
        Marca = veiculoDto.Marca,
        Ano = veiculoDto.Ano
    };
    veiculoServico.Incluir(veiculo);

    return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
}).WithTags("Veiculos");

app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) =>
{
    var veiculos = veiculoServico.Todos(pagina);
    return Results.Ok(veiculos);
}).WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);
    if (veiculo == null) return Results.NotFound("Não foi encontrado veículo com ID " + id);

    return Results.Ok(veiculo);
}).WithTags("Veiculos");

app.MapPut("/veiculos/{id}", ([FromRoute]int id,[FromBody] VeiculoDTO veiculoDto, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);
    if (veiculo == null) return Results.NotFound("Não foi encontrado veículo com ID " + id);

    ErrosDeValidacao validacao = validaDTO(veiculoDto);
    if(validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }
    veiculo.Nome = veiculoDto.Nome;
    veiculo.Marca = veiculoDto.Marca;
    veiculo.Ano = veiculoDto.Ano;

    veiculoServico.Atualizar(veiculo);
    return Results.Ok(veiculo);
}).WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);
    if (veiculo == null) return Results.NotFound("Não foi encontrado veículo com ID " + id);
    veiculoServico.Apagar(veiculo);
    return Results.NoContent();
}).WithTags("Veiculos");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion