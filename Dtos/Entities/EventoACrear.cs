using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos.Entities
{
    public class EventoACrear
    {
        public DateTime Fecha { get; set; }
        public string Localidad { get; set; }
        public string Motivo { get; set; }
        public string Descripcion { get; set; }
        public Guid IdAulaDestinada { get; set; }
        public Guid IdCreador { get; set; }
        public IEnumerable<Persona>? Asistiran { get; set; }
        public IEnumerable<Persona>? NoAsistiran { get; set; }
        public IEnumerable<Persona>? TalVezAsistan { get; set; }
    }
}
