using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos
{
    public class AulaAAgregar
    {        
        public string NombreAula { get; set; }        
        public string GradoAula { get; set; }
        public string DivisionAula { get; set; }
        public Guid InstitucionId { get; set; }
        public IEnumerable<Guid> AlumnosSeleccionados { get; set; }
        public Guid DocenteId { get; set; }
    }
}
