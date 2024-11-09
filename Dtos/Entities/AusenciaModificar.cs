using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos
{
    public class AusenciaModificar
    {
        public DateTime FechaComienzo { get; set; }
        public DateTime FechaFin { get; set; }
        public string Motivo { get; set; }        
    }
}
