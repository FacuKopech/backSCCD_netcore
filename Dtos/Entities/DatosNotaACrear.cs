using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos
{
    public class DatosNotaACrear
    {
        public IEnumerable<Aula> Aulas { get; set; }
        public IEnumerable<Persona> Alumnos { get; set; }
        public List<string> Destinatarios { get; set; }
    }
}
