using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UsersVehicles_Api.Dominio.Entidades;
using UsersVehicles_Api.Dominio.Servicos;
using UsersVehicles_Api.Infraestrutura.Db;

namespace Teste_UsersVehicles_Api.Domain.Servico
{
    [TestClass]
    public class VeiculoServicoTest
    {

        private DbVehiclesContext CriarContextoDeTeste(int order = 1)
        {
            var options = new DbContextOptionsBuilder<DbVehiclesContext>()
                .UseInMemoryDatabase(databaseName: $"TestDatabase{order}")
                .Options;
            return new DbVehiclesContext(options);
        }
        [TestMethod]
        [Priority(1)]
        public void TestarInclusaoDeVeiculo()
        {
            var dbContext = CriarContextoDeTeste();

            var veiculoNovo = new Veiculo
            {
                Id = 1,
                Nome = "Carrão",
                Marca = "BYD",
                Ano = 2024
            };

            var veiculoServico = new VeiculoServico(dbContext);
            veiculoServico.Incluir(veiculoNovo);

            var veiculoSalvo = veiculoServico.BuscaPorId(1);

            Assert.AreEqual(1, veiculoSalvo!.Id);
            Assert.AreEqual("Carrão", veiculoSalvo!.Nome);
            Assert.AreEqual("BYD", veiculoSalvo!.Marca);
            Assert.AreEqual(2024, veiculoSalvo!.Ano);
        }

        [TestMethod]
        [Priority(2)]
        public void TestarAtualizarVeiculo()
        {
            var dbContext = CriarContextoDeTeste();

            var veiculoNovo = new Veiculo
            {
                Id = 2,
                Nome = "Carrenho",
                Marca = "BYP",
                Ano = 2024
            };

            var veiculoServico = new VeiculoServico(dbContext);
            veiculoServico.Incluir(veiculoNovo);

            veiculoNovo.Nome = "Carrinho";
            veiculoNovo.Ano = 2030;
            veiculoServico.Atualizar(veiculoNovo);

            var veiculoAtualizado = veiculoServico.BuscaPorId(2);

            Assert.AreEqual(2, veiculoAtualizado!.Id);
            Assert.AreEqual("Carrinho", veiculoAtualizado!.Nome);
            Assert.AreEqual("BYP", veiculoAtualizado!.Marca);
            Assert.AreEqual(2030, veiculoAtualizado!.Ano);
        }

        [TestMethod]
        [Priority(3)]
        public void TestarApagarVeiculo()
        {
            var dbContext = CriarContextoDeTeste();

            var veiculoASerApagado = new Veiculo
            {
                Id = 3,
                Nome = "Care",
                Marca = "BYU",
                Ano = 2023
            };

            var veiculoServico = new VeiculoServico(dbContext);
            var quantidadeDeVeiculos = veiculoServico.Todos().Count();
            veiculoServico.Incluir(veiculoASerApagado);
            Assert.IsTrue(veiculoServico.Todos().Count() > quantidadeDeVeiculos);

            veiculoServico.Apagar(veiculoASerApagado);
            Assert.IsTrue(veiculoServico.Todos().Count() == quantidadeDeVeiculos);
        }
    }
}