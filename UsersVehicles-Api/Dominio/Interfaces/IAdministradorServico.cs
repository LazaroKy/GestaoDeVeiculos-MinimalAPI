using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersVehicles_Api.Dominio.DTOs;
using UsersVehicles_Api.Dominio.Entidades;

namespace UsersVehicles_Api.Dominio.Interfaces
{
    //Interface facilita os testes -- representa um contrato
    public interface IAdministradorServico
    {
        Administrador? Login(LoginDTO loginDTO);
        Administrador Incluir(Administrador admDto);

        List<Administrador> Todos(int? pagina);
        Administrador? BuscarPorId(int id);
    }
}