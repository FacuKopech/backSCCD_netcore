using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos.Entities.Reportes
{
    public class DataAuditHistorial
    {
        public int CantidadAltas { get; set; }
        public int CantidadBajas { get; set; }
        public int CantidadModificaciones { get; set; }
        public int CantidadFirmas { get; set; }
    }
}
