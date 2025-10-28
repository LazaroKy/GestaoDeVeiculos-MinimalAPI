using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersVehicles_Api.Dominio.Entidades;

namespace Teste_UsersVehicles_Api.Domain
{
    [TestClass]
    public class VeiculoTest
    {
        [TestMethod]
        public void TestarGetSetPropriedades()
        {
            var veiculo = new Veiculo();

            veiculo.Id = 1;
            veiculo.Nome = "FastFa";
            veiculo.Marca = "BM";
            veiculo.Ano = 2000;

            Assert.AreEqual(1, veiculo.Id);
            Assert.AreEqual("FastFa", veiculo.Nome);
            Assert.AreEqual("BM", veiculo.Marca);
            Assert.AreEqual(2000, veiculo.Ano);
        }
    }
}