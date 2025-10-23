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
    public class VeiculoServico : IVeiculoServico
    {
        private readonly DbVehiclesContext _dbContext;

        public VeiculoServico(DbVehiclesContext dbcontext)
        {
            _dbContext = dbcontext;
        }

        public void Apagar(Veiculo veiculo)
        {
            _dbContext.Veiculos.Remove(veiculo);
            _dbContext.SaveChanges();
        }

        public void Atualizar(Veiculo veiculo)
        {
            _dbContext.Veiculos.Update(veiculo);
            _dbContext.SaveChanges();
        }

        public Veiculo? BuscaPorId(int id)
        {
            return _dbContext.Veiculos.Find(id);
        }

        public void Incluir(Veiculo veiculo)
        {
            _dbContext.Veiculos.Add(veiculo);
            _dbContext.SaveChanges();
        }

        public List<Veiculo> Todos(int pagina = 1, string? nome = null, string? marca = null)
        {
            //Query baseada no IQueryable
            var query = _dbContext.Veiculos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(v => v.Nome.ToLower().Contains(nome.ToLower()));

            if (!string.IsNullOrWhiteSpace(marca))
                query = query.Where(v => v.Marca.ToLower().Contains(marca.ToLower()));
            int itensPorPagina = 5;

            var veiculos = query.OrderBy(v => v.Id) 
                .Skip((pagina - 1) * itensPorPagina)
                .Take(itensPorPagina)
                .ToList();

            return veiculos;
        }
    }
}