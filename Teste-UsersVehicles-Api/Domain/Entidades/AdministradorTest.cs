using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersVehicles_Api.Dominio.Entidades;

namespace Teste_UsersVehicles_Api.Domain
{
    [TestClass]
    public class AdministradorTest
    {
        [TestMethod]
        public void TestarGetSetPropriedades()
        {
            //Arrange - Variáveis p validação
            var adm = new Administrador();

            //Action - ação que vai ser testada
            adm.Id = 1;
            adm.Email = "teste@gmail.com";
            adm.Senha = "12345";
            adm.Perfil = "Admin";

            //Assert - Validar
            Assert.AreEqual(1, adm.Id);
            Assert.AreEqual("teste@gmail.com", adm.Email);
            Assert.AreEqual("12345", adm.Senha);
            Assert.AreEqual("Admin", adm.Perfil);

        }
    }
}