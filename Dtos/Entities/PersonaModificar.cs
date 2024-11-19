using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos
{
    public class PersonaModificar
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int DNI { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Domicilio { get; set; }
        public Usuario? UsuarioSeleccionado { get; set; }
        public Institucion? InstitucionSeleccionada { get; set; }
        public IEnumerable<Guid>? HijosSeleccionados { get; set; }
    }
}
