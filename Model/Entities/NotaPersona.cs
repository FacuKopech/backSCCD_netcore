using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entities
{
    public class NotaPersona
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid NotaId { get; set; }
        public Nota Nota { get; set; }
        public Guid PersonaId { get; set; }
        public Persona Persona { get; set; }
        public bool Leida { get; set; } 
        public DateTime? FechaLectura { get; set; }
        public bool Firmada { get; set; } 
        public DateTime? FechaFirma { get; set; }
    }
}
