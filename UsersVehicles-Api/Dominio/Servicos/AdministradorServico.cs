using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersVehicles_Api.Dominio.DTOs;
using UsersVehicles_Api.Dominio.Entidades;
using UsersVehicles_Api.Dominio.Interfaces;
using UsersVehicles_Api.Infraestrutura.Db;

namespace UsersVehicles_Api.Dominio.Servicos
{
    public class AdministradorServico : IAdministradorServico
    {
        private readonly DbVehiclesContext _dbContext;

        public AdministradorServico(DbVehiclesContext dbcontext)
        {
            _dbContext = dbcontext;
        }
        public Administrador? Login(LoginDTO loginDTO)
        {

            return _dbContext.Administradores.Where(adm => adm.Email == loginDTO.Email && adm.Senha == loginDTO.Senha).FirstOrDefault();
        }
    }
}