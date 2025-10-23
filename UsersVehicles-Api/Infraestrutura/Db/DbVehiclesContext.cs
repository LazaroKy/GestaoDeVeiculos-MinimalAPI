using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UsersVehicles_Api.Dominio.Entidades;

namespace UsersVehicles_Api.Infraestrutura.Db
{

    public class DbVehiclesContext : DbContext
    {
        public DbSet<Administrador> Administradores { get; set; } = default!;

        public DbSet<Veiculo> Veiculos { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Seeding databse with a admin
            modelBuilder.Entity<Administrador>().HasData(
                new Administrador
                {
                    Id = 1,
                    Email = "superadmin@gmail.com",
                    Senha = "1234567",
                    Perfil = "Admin"
                }
            );
        }
        
        public DbVehiclesContext(DbContextOptions builder) : base(builder)
        {
            //Detectar string de conexão a partir da injeção de serviço no program
        }
        
        //Poderia fazer o onConfiguring p redundância ou caso tivesse algum cenário que atenderia a tal condição, como executar local e prod
    }
}