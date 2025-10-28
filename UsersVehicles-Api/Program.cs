using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using UsersVehicles_Api.Dominio.DTOs;
using UsersVehicles_Api.Dominio.Entidades;
using UsersVehicles_Api.Dominio.Enums;
using UsersVehicles_Api.Dominio.Interfaces;
using UsersVehicles_Api.Dominio.ModelViews;
using UsersVehicles_Api.Dominio.Servicos;
using UsersVehicles_Api.Infraestrutura.Db;

#region Builder
var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration.GetSection("Jwt").GetValue<string>("SecretKey");//Deve ter pelo menos 256 bits por culpa do algoritmo q estamos usando
if (string.IsNullOrEmpty(jwtKey)) jwtKey = "123456789-123456789-123456789-123456789"; 
builder.Services.AddAuthentication(options =>
{ //Configurações de autenticação
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; 
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{  //Config p processamento do Jwt
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true, //Se o token não expirou
        ValidateIssuer = false, //verificar se o token veio de um emissor (issuer) - configure validIssuer 
        ValidateAudience = false, //verificar se é destinado a publico (audience) 
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)) //chave p verificar assinatura
    };
});

builder.Services.AddAuthorization();

//P injeção de dependência
builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorizathion",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira seu token Jwt aqui!"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    { new OpenApiSecurityScheme{
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    }, new string[]{}
    }
});
});

builder.Services.AddDbContext<DbVehiclesContext>(options =>
{
    options.UseNpgsql(builder.Configuration
    .GetConnectionString("PostgreSQLConnection")); 
});
var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous()
.WithTags("Home");

#endregion

#region Administradores
string GerarTokenJwt(Administrador administrador)
{
    if (string.IsNullOrEmpty(jwtKey)) return string.Empty;
    var securitYKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    var credentials = new SigningCredentials(securitYKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new Claim("Email", administrador.Email),
        new Claim("Perfil", administrador.Perfil)
    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddHours(3),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("/login", ([FromBody] LoginDTO loginDto, IAdministradorServico administradorServico) =>
{
    Administrador adm = administradorServico.Login(loginDto)!;
    if (adm != null)
    {
        string token = GerarTokenJwt(adm);
        return Results.Ok(new AdministradorLogadoView
        {
            Email = adm.Email,
            Perfil = adm.Perfil,
            Token = token
        });
    }
    else
    {
        return Results.Unauthorized();
    }
}).AllowAnonymous()
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
.RequireAuthorization()
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
}).RequireAuthorization()
.WithTags("Administradores");

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
}).RequireAuthorization()
.WithTags("Administradores");


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
}).RequireAuthorization()
.WithTags("Veiculos");

app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) =>
{
    var veiculos = veiculoServico.Todos(pagina);
    return Results.Ok(veiculos);
}).RequireAuthorization()
.WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);
    if (veiculo == null) return Results.NotFound("Não foi encontrado veículo com ID " + id);

    return Results.Ok(veiculo);
}).RequireAuthorization()
.WithTags("Veiculos");

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
}).RequireAuthorization()
.WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);
    if (veiculo == null) return Results.NotFound("Não foi encontrado veículo com ID " + id);
    veiculoServico.Apagar(veiculo);
    return Results.NoContent();
}).RequireAuthorization()
.WithTags("Veiculos");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication(); //Api precisar de autenticação 
app.UseAuthorization(); //Precisar de autorização

app.Run();
#endregion