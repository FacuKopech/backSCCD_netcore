using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos.Entities
{
    public class EventoAModificar
    {
        public DateTime Fecha { get; set; }

        public string Localidad { get; set; }

        public string Motivo { get; set; }

        public string Descripcion { get; set; }

        public Guid IdAulaDestinada { get; set; }
    }
}
