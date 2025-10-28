using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using UsersVehicles_Api.Dominio.Enums;

namespace UsersVehicles_Api.Dominio.DTOs
{
    public class AdministradorDTO
    {
       
        public string Email { get; set; } = default!;
        public string Senha { get; set; } = default!;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Perfil? Perfil { get; set; } = default!;

    }
}