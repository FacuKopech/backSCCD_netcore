using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos
{
    public class AlumnoConEnumerables : Persona
    {
        public DateTime FechaNacimiento { get; set; }
        public IEnumerable<Historial> Historiales { get; set; }
        public IEnumerable<Ausencia> Ausencias { get; set; }     
        public decimal Asistencia { get; set; }
    }
}
