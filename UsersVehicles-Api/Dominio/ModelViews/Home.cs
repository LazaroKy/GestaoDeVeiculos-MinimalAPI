using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersVehicles_Api.Dominio.ModelViews
{
    public class Home
    {
        public string Mensagem { get => "Bem vindo à Minimal API de Veículos"; }
        public string Doc { get => "/swagger"; }
    }
}