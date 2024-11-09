using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos.Entities.Reportes
{
    public class NotasAvg
    {
        public decimal DirectivosRecibidasAvg { get; set; }
        public decimal DirectivosEmitidasAvg { get; set; }
        public decimal DocentesRecibidasAvg { get; set; }
        public decimal DocentesEmitidasAvg { get; set; }
        public decimal PadresRecibidasAvg { get; set; }
        public decimal PadresEmitidasAvg { get; set; }
    }
}
