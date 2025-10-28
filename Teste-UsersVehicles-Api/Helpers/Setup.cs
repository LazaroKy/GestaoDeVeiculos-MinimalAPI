using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Teste_UsersVehicles_Api.Mocks;
using UsersVehicles_Api.Dominio.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using UsersVehicles_Api.Dominio.DTOs;
using System.Text.Json;
using System.Text;
using UsersVehicles_Api.Dominio.ModelViews; //p reconhecer o AddScoped

namespace Teste_UsersVehicles_Api.Helpers
{
    [TestClass]
    public class Setup
    {
        public const string PORT = "5027";
        public static WebApplicationFactory<Startup> http = default!;
        public static HttpClient client = default!;

        [ClassInitialize]
        public static void ClassInit(TestContext _)
        {
            Setup.http = new WebApplicationFactory<Startup>();

            Setup.http = Setup.http.WithWebHostBuilder(builder =>
            {
                builder.UseSetting("https_port", Setup.PORT).UseEnvironment("Testing");

                builder.ConfigureServices(services =>
                {
                    services.AddScoped<IAdministradorServico, AdministradorServicoMock>();
                });

            });

            Setup.client = Setup.http.CreateClient();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Setup.http.Dispose();
        }

        [TestMethod]
        public async Task TestarLogin()
        {
            var loginDto = new LoginDTO
            {
                Email = "meuemail@gmail.com",
                Senha = "1234567"
            };


            var requestConteudo = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");


            var response = await Setup.client.PostAsync("/administradores/login", requestConteudo);
            Assert.AreEqual(200, (int)response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            var admLogado = JsonSerializer.Deserialize<AdministradorLogadoView>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            Assert.IsNotNull(admLogado);
            Assert.IsNotNull(admLogado.Token);
        }

        [TestMethod]
        public async Task TestarBuscarAdministradorPorIdAsync()
        {
            var token = await ObterTokenDoAdministrador(new LoginDTO
            {
                Email = "meuemail@gmail.com",
                Senha = "1234567"
            });
            var request = new HttpRequestMessage(HttpMethod.Get, "/administradores/1");
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await Setup.client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            var admBuscado = JsonSerializer.Deserialize<AdministradorModelView>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            Assert.IsNotNull(admBuscado);
            Assert.AreEqual("meuemail@gmail.com", admBuscado.Email);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task BuscarTodosAdministradores()
        {
            var token = await ObterTokenDoAdministrador(new LoginDTO
            {
                Email = "meuemail@gmail.com",
                Senha = "1234567"
            });
            var request = new HttpRequestMessage(HttpMethod.Get, "/administradores");
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await Setup.client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            
            List<AdministradorModelView> administradores = JsonSerializer.Deserialize<List<AdministradorModelView>>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;
            Assert.IsNotNull(administradores);
            Assert.IsTrue(administradores.Count() >0);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        private async Task<string> ObterTokenDoAdministrador(LoginDTO loginDto)
        {
        
            var conteudo = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");
            var response = await Setup.client.PostAsync("/administradores/login", conteudo);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var admLogado = JsonSerializer.Deserialize<AdministradorLogadoView>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return admLogado!.Token;
        }
    }
}