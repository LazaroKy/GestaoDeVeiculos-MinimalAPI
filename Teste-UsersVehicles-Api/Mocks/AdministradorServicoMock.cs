using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersVehicles_Api.Dominio.DTOs;
using UsersVehicles_Api.Dominio.Entidades;
using UsersVehicles_Api.Dominio.Interfaces;

namespace Teste_UsersVehicles_Api.Mocks
{
    public class AdministradorServicoMock : IAdministradorServico
    {
        private static List<Administrador> administradores = new List<Administrador>()
        {
            new Administrador
            {
                Id = 1,
                Email = "meuemail@gmail.com",
                Senha = "1234567",
                Perfil = "Admin"
            },
            new Administrador
            {
                Id = 2,
                Email = "mel@gmail.com",
                Senha = "1234567",
                Perfil = "Editor"
            }
        };
        public Administrador? BuscarPorId(int id)
        {
            return administradores.Find(adm => adm.Id == id);
        }

        public Administrador Incluir(Administrador adm)
        {
            adm.Id = administradores.Count() + 1;
            administradores.Add(adm);
            return adm;
        }

        public Administrador? Login(LoginDTO loginDTO)
        {
            return administradores.Find(adm => adm.Email == loginDTO.Email && adm.Senha == loginDTO.Senha);
        }

        public List<Administrador> Todos(int? pagina)
        {
            return administradores;
        }
    }
}