using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UsersVehicles_Api.Dominio.Entidades;
using UsersVehicles_Api.Dominio.Servicos;
using UsersVehicles_Api.Infraestrutura.Db;

namespace Teste_UsersVehicles_Api.Domain
{
    [TestClass]
    public class AdministradorServicoTest
    {

        private DbVehiclesContext CriarContextoDeTeste(int order =1)
//Pq esse order? Pq se não ele utiliza o mesmo banco de dados em todos os métodos e acaba não tendo um comportamento que espero, assim posso mudar o banco e ter determinado comportamento
        {
        var options = new DbContextOptionsBuilder<DbVehiclesContext>()
            .UseInMemoryDatabase(databaseName: $"TestDatabase{order}") 
            .Options;
            return new DbVehiclesContext(options);
        }
        [TestMethod]
        public void TentandoSalvarAdministrador()
        {
            var dbContext = CriarContextoDeTeste();

            var adm = new Administrador();
            adm.Id = 1;
            adm.Email = "teste@gmail.com";
            adm.Senha = "12345";
            adm.Perfil = "Admin";

            var admServico = new AdministradorServico(dbContext);


            admServico.Incluir(adm);


            Assert.AreEqual(1, admServico.Todos(1).Count);
        }
        
        [TestMethod]
        public void BuscarAdministradorPorId()
        {
            var dbContext = CriarContextoDeTeste(2);
       
            var adm = new Administrador();
            adm.Id = 1;
            adm.Email = "juninhodopepei@gmail.com";
            adm.Senha = "12345";
            adm.Perfil = "Admin";
            
            var admServico = new AdministradorServico(dbContext);


            admServico.Incluir(adm);
            var admBuscado = admServico.BuscarPorId(adm.Id);


            Assert.AreEqual(1, admBuscado!.Id);
            Assert.AreEqual("juninhodopepei@gmail.com", admBuscado.Email);

        }
    }
}