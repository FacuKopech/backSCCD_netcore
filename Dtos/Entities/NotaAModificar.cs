using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos
{
    public class NotaAModificar
    {
        public string Titulo { get; set; }
        public string Cuerpo { get; set; }
        public IEnumerable<Guid> AulasDestinadas { get; set; }

    }
}
