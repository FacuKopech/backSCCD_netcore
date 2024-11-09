using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos
{
    public class UsuarioACrear
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Clave { get; set; }
        public IEnumerable<string> RolesSeleccionados { get; set; }
    }
}
