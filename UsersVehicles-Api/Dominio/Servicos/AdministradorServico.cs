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

        public Administrador Incluir(Administrador adm)
        {
            _dbContext.Administradores.Add(adm);
            _dbContext.SaveChanges();
            return adm;
        }

        public Administrador? Login(LoginDTO loginDTO)
        {

            return _dbContext.Administradores.Where(adm => adm.Email == loginDTO.Email && adm.Senha == loginDTO.Senha).FirstOrDefault();
        }

        public List<Administrador> Todos(int? pagina)
        {
            var query = _dbContext.Administradores.AsQueryable();
            int itensPorPagina = 10;

            if(pagina!=null)
            query = query.OrderBy(v => v.Id).Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);

            return query.ToList();
      
        }
    }
}