using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos
{
    public class PersonaAAgregar
    {
        public string TipoPersona { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string DNI { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Domicilio { get; set; }
        public Usuario? Usuario { get; set; }
        public Institucion? Institucion { get; set; }
    }
}
