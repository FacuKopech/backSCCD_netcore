using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos
{
    public class NotaACrear
    {
        public string Tipo { get; set; }
        public bool ConAula { get; set; }
        public IEnumerable<Guid> AulasDestinadas { get; set; }
        public Guid IdAlumnoReferido { get; set; }
        public IEnumerable<Guid> Destinatarios { get; set; }
        public string Titulo { get; set; }
        public string Cuerpo { get; set; }
        public string EnviaNotaComo { get; set; }
    }
}
