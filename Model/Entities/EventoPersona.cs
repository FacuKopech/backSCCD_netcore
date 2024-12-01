using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entities
{
    public class EventoPersona
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid EventoId { get; set; }
        public Evento Evento { get; set; }
        public Guid PersonaId { get; set; }
        public Persona Persona { get; set; }
        public bool Asistira { get; set; }
        public bool NoAsistira { get; set; }
        public bool TalVezAsista { get; set; }
        public DateTime? FechaConfirmacion { get; set; }
    }
}
